using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using OBS.WebSocket.NET;
using System.Linq;
using CommandLine;
using Newtonsoft.Json.Linq;
using OBS.WebSocket.NET.Types;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;

namespace ObsBridge
{
    class ObsInstance
    {
        //public string MachineId { get; set; }
        public string Name { get; set; }
        public bool Online { get; set; }
        public string Pwd { get; set; }
        public string uid { get; set; }
        public bool Recording { get; set; }
        public bool Streaming { get; set; }
        public List<string> Tally { get; set; }
        public List<string> PreviewTally { get; set; }
        public List<string> Sources { get; set; }
        public DateTime StartedRecording { get; set; }
        public DateTime StartedStreaming { get; set; }
        public DateTime StoppedRecording { get; set; }
        public DateTime StoppedStreaming { get; set; }
    }

    class Options
    {
        [Option('c', "password", Required = false, Default = "", HelpText = "Password for client connections")]
        public string ClientPassword { get; set; }
        [Option('p', "obspassword", Required = false, Default = "", HelpText = "Password for local OBS")]
        public string ObsPassword { get; set; }
        [Option('h', "host", Required = false, Default = "127.0.0.1", HelpText = "Host for local OBS")]
        public string Host { get; set; }
        [Option('t', "port", Required = false, Default = "4444", HelpText = "Port for local OBS")]
        public string Port { get; set; }
    }

    class MainClass
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            new Program().Run(args);

            while(true)
            {
                Thread.Sleep(100);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e);
        }

        public class Program
        {
            private static string FirebaseApiKey = ConfigurationManager.AppSettings.Get("APIKEY");
            private ObsWebSocket _obs;
            ObsInstance MainInstance = new ObsInstance();
            FirebaseClient FirebaseClient;

            public async void Run(string[] args)
            {
                Logger.Info("Starting OBS Bridge");
               
                    Parser.Default.ParseArguments<Options>(args)
                       .WithParsed<Options>(async o =>
                       {

                           await SetupFirebase();
                           _obs = new ObsWebSocket();
                           _obs.Connected += _obs_Connected;
                           _obs.Disconnected += _obs_Disconnected;
                           _obs.RecordingStateChanged += _obs_RecordingStateChanged;
                           _obs.StreamingStateChanged += _obs_StreamingStateChanged;
                           _obs.PreviewSceneChanged += _obs_PreviewSceneChanged;
                           _obs.SceneChanged += _obs_SceneChanged;
                           _obs.TransitionBegin += _obs_TransitionBegin;
                           MainInstance.Pwd = EncryptPwd(o.ClientPassword);
                           Logger.Info($"Starting with {o.ObsPassword}@{o.Host}:{o.Port}");
                           _obs.Connect($"ws://{o.Host}:{o.Port}", o.ObsPassword);
                       });
                //}
              
            }

            private void _obs_Disconnected(object sender, EventArgs e)
            {
                MainInstance.Online = false;
                UpdateInstance();
            }

            private async Task<T> CallObsFunc<T>(Func<T> f)
            {
                var result = await Task<T>.Factory.StartNew(() =>
                {
                    return (T)f.DynamicInvoke();
                });

                return result;
            }

            private void _obs_TransitionBegin(object sender, EventArgs e)
            {
                //need to change tally to current + upcoming
                if (MainInstance.Tally == null)
                    MainInstance.Tally = MainInstance.PreviewTally;
                else
                    MainInstance.Tally.AddRange(MainInstance.PreviewTally);

                UpdateInstance();
            }

            private async void _obs_SceneChanged(ObsWebSocket sender, string newSceneName)
            {
                var scene = await CallObsFunc(_obs.Api.GetCurrentScene);
                MainInstance.Tally = (from n in scene.Items select n.SourceName).ToList();
                UpdateInstance();
            }

            private async void _obs_PreviewSceneChanged(ObsWebSocket sender, string newSceneName)
            {
                var scenes = await CallObsFunc<List<OBSScene>>(_obs.Api.ListScenes);
                var scene = (from n in scenes where n.Name == newSceneName select n).First();
                MainInstance.PreviewTally = (from n in scene.Items select n.SourceName).ToList();
                UpdateInstance();
            }

            private void _obs_StreamingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
            {
                switch (type)
                {
                    case OutputState.Started:
                        MainInstance.StartedStreaming = DateTime.Now;
                        break;

                    case OutputState.Stopped:
                        MainInstance.StoppedStreaming = DateTime.Now;
                        break;
                }

                MainInstance.Streaming = (type != OutputState.Stopped)? true : false;
                UpdateInstance();
            }

            private void _obs_RecordingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
            {
                switch (type)
                {
                    case OutputState.Started:
                        MainInstance.StartedRecording = DateTime.Now;
                        break;

                    case OutputState.Stopped:
                        MainInstance.StoppedRecording = DateTime.Now;
                        break;
                }

                MainInstance.Recording = (type != OutputState.Stopped) ? true : false;
                UpdateInstance();
            }

            string EncryptPwd(string pwd)
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(pwd);
                data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
                String hash = System.Text.Encoding.ASCII.GetString(data);
                return hash;
            }

            public async void Logout()
            {
                await new FileDataStore("Obs.Credentials").ClearAsync();
            }

            UserCredential credential;

            private async Task LoginToGoogle()
            {

                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "ObsBridge.client_id.json";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] {
                          "https://www.googleapis.com/auth/userinfo.email"
                        },
                        "user",
                        CancellationToken.None,
                        new FileDataStore("Obs.Credentials")
                    );
                }
            }


            async Task SetupFirebase()
            {
                Logger.Info("Logging in");

                await LoginToGoogle();

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));

                await credential.RefreshTokenAsync(new CancellationToken());


                var auth = await authProvider.SignInWithGoogleIdTokenAsync(credential.Token.IdToken);

                FirebaseClient = new FirebaseClient(
                  "https://obstally.firebaseio.com/",
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                  });

                MainInstance.Name = $"[{Environment.MachineName}] Loading...";

                MainInstance.uid = auth.User.LocalId;

                UpdateInstance();
            }

            async void UpdateInstance()
            {
                try { 
                    await FirebaseClient.Child("instances/"+ Environment.MachineName).PatchAsync(MainInstance);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Firebase Error");
                }
    }

            void _obs_Connected(object sender, EventArgs e)
            {
                Logger.Info("Connected");
                var profilename = _obs.Api.GetCurrentProfile();
                MainInstance.Name = $"[{Environment.MachineName}] {profilename}";
                MainInstance.Online = true;
                MainInstance.Sources = (from n in _obs.Api.GetSourcesList() select n.Name).ToList();
                UpdateInstance();
            }
        }
    }
}
