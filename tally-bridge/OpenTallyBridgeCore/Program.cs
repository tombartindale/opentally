using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using CommandLine;
using System.Configuration;
using System.IO;
using System.Reflection;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2;
using System.Net.Sockets;
using System.Net;

namespace OpenTallyBridge
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
        [Option('o', "offline", Required = false, Default = false, HelpText = "Only connect offline (without firebase)")]
        public bool Offline { get; set; }
    }

    [Verb("logout", HelpText = "Remove Google credentials for this instance")]
    class LogoutOptions : MainOptions
    {
        
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

            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse("224.0.0.0");

            IPEndPoint remoteep;
            

            public void Run(string[] args)
            {

                var types = LoadVerbs();

                Logger.Info("Starting OBS Bridge");
                
                Parser.Default.ParseArguments(args, types)
                    .WithParsed<MainOptions>(async o =>
                    {
                        udpclient.JoinMulticastGroup(multicastaddress);
                        remoteep = new IPEndPoint(multicastaddress, 8854);

                        CurrentOptions = o;

                        if (CurrentOptions is LogoutOptions)
                        {
                            Logger.Info($"Logging Out...");
                            Logout();
                            Environment.Exit(0);
                        }

                        if (!CurrentOptions.Offline)
                        {
                            Logger.Info($"Connecting to Firebase...");
                            await SetupFirebase();
                            MainInstance.uid = auth.User.LocalId;
                        }

                        MainInstance.Name = $"[{Environment.MachineName}] Loading...";
                        MainInstance.Sources = new List<string>();
                        MainInstance.Tally = new List<string>();
                        MainInstance.PreviewTally = new List<string>();


                        UpdateInstance();

                        if (!string.IsNullOrEmpty(o.ClientPassword))
                            MainInstance.Pwd = EncryptPwd(o.ClientPassword);

                        Type srcT = CurrentOptions.GetType();

                        var tt = Assembly.GetExecutingAssembly().GetTypes().First(t => t.FullName == srcT.FullName.Split("+")[0]);

                        Logger.Info($"Using {tt}");

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
                        CurrentSource.Connect();
                    })
                    .WithNotParsed((o) =>
                    {
                        Environment.Exit(0);
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
                MainInstance.Name = $"[{Environment.MachineName}] {obj}";
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

            FirebaseAuthLink auth;

            async Task SetupFirebase()
            {
                Logger.Info("Logging in...");

                await LoginToGoogle();

                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(FirebaseApiKey));

                await credential.RefreshTokenAsync(new CancellationToken());

                auth = await authProvider.SignInWithGoogleIdTokenAsync(credential.Token.IdToken);

                FirebaseClient = new FirebaseClient(
                  "https://obstally.firebaseio.com/",
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                  });
            }

            async void UpdateInstance()
            {
                SendUDP();

                if (!CurrentOptions.Offline)
                {
                    try
                    {
                        await FirebaseClient.Child("instances/" + Environment.MachineName).PatchAsync(MainInstance);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Firebase Error");
                        await SetupFirebase();
                    }
                }
            }

            void SendUDP()
            {

                byte[] data = { 0, 0, 0};
                byte mask = 0b_0000_0001;

                for (int i=0;i<=Math.Min(MainInstance.Sources.Count-1,7);i++)
                {
                    if (MainInstance.PreviewTally.Contains(MainInstance.Sources[i]))
                    {
                        data[0] |= mask;
                    }
                    mask = (byte)(mask << 1);
                }

                mask = 0b_0000_0001;

                // if (MainInstance.Streaming)
                // {
                for (int i = 0; i <= Math.Min(MainInstance.Sources.Count - 1, 7); i++)
                {
                    if (MainInstance.Tally.Contains(MainInstance.Sources[i]))
                    {
                        data[1] |= mask;
                    }
                    mask = (byte)(mask << 1);
                }
                // }

                //data[0] = 0;
                //data[1] = 0;

                //add is live...
                if (MainInstance.Recording || MainInstance.Streaming)
                    data[2] = 1;

                //data[2] = 0;

                udpclient.Send(data, data.Length, remoteep);
            }
        }
    }
}
