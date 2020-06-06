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
using OBS.WebSocket.NET.Types;
using System.Configuration;
using System.IO;
using System.Reflection;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using ObsBridgeCore;

namespace ObsBridge
{
    class TallyInstance
    {
        public string SourceType { get; set; }
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

    abstract class MainOptions
    {

        [Option('c', "password", Required = false, Default = "", HelpText = "Password for client connections")]
        public string ClientPassword { get; set; }
    }

    class MainClass
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            new Program().Run(args);

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e);
        }

        private static Type[] LoadVerbs()
        {
            return Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();
        }

        public class Program
        {
            private static string FirebaseApiKey = ConfigurationManager.AppSettings.Get("APIKEY");
            TallyInstance MainInstance = new TallyInstance();

            FirebaseClient FirebaseClient;

            MainOptions CurrentOptions;

            ISource CurrentSource;

            public void Run(string[] args)
            {

                var types = LoadVerbs();

                Logger.Info("Starting OBS Bridge");

                Parser.Default.ParseArguments(args,types)
                    .WithParsed<MainOptions>(async o =>
                    {
                        CurrentOptions = o;

                        await SetupFirebase();

                        if (!string.IsNullOrEmpty(o.ClientPassword))
                            MainInstance.Pwd = EncryptPwd(o.ClientPassword);

                        Type srcT = CurrentOptions.GetType();

                        var tt = Assembly.GetExecutingAssembly().GetTypes().First(t => t.FullName == srcT.FullName.Split("+")[0]);

                        CurrentSource = (ISource)Activator.CreateInstance(tt, Logger, CurrentOptions);
                        CurrentSource.OnSourcesChanged += CurrentSource_OnSourcesChanged;
                        CurrentSource.OnOnlineChanged += CurrentSource_OnOnlineChanged;
                        CurrentSource.OnNameChanged += CurrentSource_OnNameChanged;
                        CurrentSource.OnPreviewTallyChange += CurrentSource_OnPreviewTallyChange;
                        CurrentSource.OnRecordingStarted += CurrentSource_OnRecordingStarted;
                        CurrentSource.OnRecordingStopped += CurrentSource_OnRecordingStopped;
                        CurrentSource.OnStreamingStarted += CurrentSource_OnStreamingStarted;
                        CurrentSource.OnStreamingStopped += CurrentSource_OnStreamingStopped;
                        CurrentSource.OnTallyChange += CurrentSource_OnTallyChange;
                        MainInstance.SourceType = tt.Name;
                        UpdateInstance();
                        CurrentSource.Connect();
                    });
            }

            private void CurrentSource_OnTallyChange(List<string> obj)
            {
                MainInstance.Tally = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnStreamingStopped(DateTime obj)
            {
                MainInstance.Streaming = false;
                MainInstance.StoppedStreaming = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnStreamingStarted(DateTime obj)
            {
                MainInstance.Streaming = true;
                MainInstance.StartedStreaming = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnRecordingStopped(DateTime obj)
            {
                MainInstance.Recording = false;
                MainInstance.StoppedRecording = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnRecordingStarted(DateTime obj)
            {
                MainInstance.Recording = true;
                MainInstance.StartedRecording = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnPreviewTallyChange(List<string> obj)
            {
                MainInstance.PreviewTally = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnNameChanged(string obj)
            {
                MainInstance.Name = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnOnlineChanged(bool obj)
            {
                MainInstance.Online = obj;
                UpdateInstance();
            }

            private void CurrentSource_OnSourcesChanged(List<string> obj)
            {
                MainInstance.Sources = obj;
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
                var resourceName = "ObsBridgeCore.client_id.json";

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
                try
                {
                    await FirebaseClient.Child("instances/" + Environment.MachineName).PatchAsync(MainInstance);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Firebase Error");
                }
            }

            
        }
    }
}
