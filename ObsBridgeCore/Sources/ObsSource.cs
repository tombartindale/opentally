using System;
using System.Threading.Tasks;
using CommandLine;
using OBS.WebSocket.NET;
using System.Linq;
using System.Collections.Generic;
using OBS.WebSocket.NET.Types;
using NLog;
using ObsBridge;
using System.Threading;

namespace ObsBridgeCore
{


    class ObsSource : ISource
    {
        //public override Type OptionsClass => typeof(ObsOptions);

        [Verb("obs", HelpText = "Take Tally events from OBS")]
        public class ObsOptions:MainOptions
        {
            [Option('p', "obspassword", Required = false, Default = "", HelpText = "Password for local OBS")]
            public string ObsPassword { get; set; }
            [Option('h', "host", Required = false, Default = "127.0.0.1", HelpText = "Host for local OBS")]
            public string Host { get; set; }
            [Option('t', "port", Required = false, Default = "4444", HelpText = "Port for local OBS")]
            public string Port { get; set; }
        }


        public override event Action<List<string>> OnTallyChange;
        public override event Action<List<string>> OnSourcesChanged;
        public override event Action<List<string>> OnPreviewTallyChange;
        public override event Action<bool> OnOnlineChanged;
        public override event Action<string> OnNameChanged;
        public override event Action<DateTime> OnRecordingStarted;
        public override event Action<DateTime> OnRecordingStopped;
        public override event Action<DateTime> OnStreamingStarted;
        public override event Action<DateTime> OnStreamingStopped;

        ObsWebSocket _obs;
        System.Timers.Timer heartbeatCheck;
        DateTime _lastHeatbeat = DateTime.MinValue;
        private bool attemptingreconnect = false;
        ObsOptions CurrentOptions;
        List<string> Tally = new List<string>();
        List<string> PreviewTally = new List<string>();

        public ObsSource(Logger log,MainOptions options):base(log)
        {
            var o = options as ObsOptions;
            CurrentOptions = o;
        }


        public override void Connect()
        {
            heartbeatCheck = new System.Timers.Timer(500);
            heartbeatCheck.Elapsed += HeatbeatCheck_Elapsed;
            heartbeatCheck.Enabled = false;
            heartbeatCheck.Start();

            _obs = new ObsWebSocket();
            _obs.Connected += _obs_Connected;
            _obs.Disconnected += _obs_Disconnected;
            _obs.RecordingStateChanged += _obs_RecordingStateChanged;
            _obs.StreamingStateChanged += _obs_StreamingStateChanged;
            _obs.PreviewSceneChanged += _obs_PreviewSceneChanged;
            _obs.SceneChanged += _obs_SceneChanged;
            _obs.TransitionBegin += _obs_TransitionBegin;
            _obs.SourceCreated += _obs_SourceCreated;
            _obs.SourceDestroyed += _obs_SourceDestroyed;
            _obs.SourceRenamed += _obs_SourceRenamed;
            _obs.Heartbeat += _obs_Heartbeat;

            Logger.Info($"Starting with {CurrentOptions.ObsPassword}@{CurrentOptions.Host}:{CurrentOptions.Port}");
            _obs.Connect($"ws://{CurrentOptions.Host}:{CurrentOptions.Port}", CurrentOptions.ObsPassword);
        }

        private async void UpdateSources()
        {
            await Task.Delay(1000);
            if (_obs.IsConnected)
            {
                var sources=  (from n in _obs.Api.GetSourcesList() select n.Name).ToList();
                OnSourcesChanged?.Invoke(sources);
            }
        }

        private void _obs_SourceRenamed(ObsWebSocket sender, string newName, string previousName)
        {
            UpdateSources();
        }

        private void _obs_SourceDestroyed(ObsWebSocket sender, string sourceName, string sourceType, string sourceKind)
        {
            UpdateSources();
        }

        private void _obs_SourceCreated(ObsWebSocket sender, SourceSettings settings)
        {
            UpdateSources();
        }

        private void HeatbeatCheck_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_lastHeatbeat != DateTime.MinValue)
            {
                if (e.SignalTime > _lastHeatbeat.Add(TimeSpan.FromSeconds(2.1)))
                {
                    Logger.Info("Lost Heartbeat");
                    heartbeatCheck.Enabled = false;
                    _obs.Disconnect();
                }
            }
        }

        private void _obs_Heartbeat(ObsWebSocket sender, Heartbeat heatbeat)
        {
            _lastHeatbeat = DateTime.Now;
        }

       
        private void _obs_Disconnected(object sender, EventArgs e)
        {
            if (!attemptingreconnect)
            {
                attemptingreconnect = true;
                Logger.Info("Lost OBS");
                //MainInstance.Online = false;
                OnOnlineChanged?.Invoke(false);
                //UpdateInstance();
                new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(5000);
                    if (attemptingreconnect)
                    {
                        attemptingreconnect = false;
                        Logger.Info("Attempting Re-connect");
                        _obs.Connect($"ws://{CurrentOptions.Host}:{CurrentOptions.Port}", CurrentOptions.ObsPassword);
                    }


                })).Start();
            }
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
            if (Tally == null)
                Tally = PreviewTally;
            else
                Tally.AddRange(PreviewTally);

            OnPreviewTallyChange?.Invoke(PreviewTally);
        }

        private async void _obs_SceneChanged(ObsWebSocket sender, string newSceneName)
        {
            var scene = await CallObsFunc(_obs.Api.GetCurrentScene);
            Tally = (from n in scene.Items select n.SourceName).ToList();
            //UpdateInstance();
            OnTallyChange?.Invoke(Tally);
        }

        private async void _obs_PreviewSceneChanged(ObsWebSocket sender, string newSceneName)
        {
            var scenes = await CallObsFunc<List<OBSScene>>(_obs.Api.ListScenes);
            var scene = (from n in scenes where n.Name == newSceneName select n).First();
            PreviewTally = (from n in scene.Items select n.SourceName).ToList();
            //UpdateInstance();
            OnPreviewTallyChange?.Invoke(PreviewTally);
        }

        void _obs_Connected(object sender, EventArgs e)
        {
            _lastHeatbeat = DateTime.Now;
            attemptingreconnect = false;
            Logger.Info("Connected");
            try
            {
                var profilename = _obs.Api.GetCurrentProfile();
                OnNameChanged?.Invoke($"[{Environment.MachineName}] {profilename}");
                OnOnlineChanged?.Invoke(true);
                OnSourcesChanged?.Invoke((from n in _obs.Api.GetSourcesList() select n.Name).ToList());
                _obs.Api.SetHeartbeat(true);
                heartbeatCheck.Enabled = true;
            }
            catch (Exception ex)
            {

            }
        }


        private void _obs_StreamingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
        {
            switch (type)
            {
                case OutputState.Started:
                    //MainInstance.StartedStreaming = DateTime.Now;
                    OnStreamingStarted?.Invoke(DateTime.Now);
                    break;

                case OutputState.Stopped:
                    //MainInstance.StoppedStreaming = DateTime.Now;
                    OnStreamingStopped?.Invoke(DateTime.Now);

                    break;
            }



            //UpdateInstance();
        }

        private void _obs_RecordingStateChanged(ObsWebSocket sender, OBS.WebSocket.NET.Types.OutputState type)
        {
            switch (type)
            {
                case OutputState.Started:
                    OnRecordingStarted?.Invoke(DateTime.Now);
                    break;

                case OutputState.Stopped:
                    OnRecordingStopped?.Invoke(DateTime.Now);
                    break;
            }
        }
    }
}
