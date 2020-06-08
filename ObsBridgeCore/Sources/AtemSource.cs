using System;
using System.Collections.Generic;
using CommandLine;
using LibAtem.Commands;
using LibAtem.Discovery;
using LibAtem.Net;
using NLog;
using ObsBridge;
using System.Linq;
using LibAtem.Commands.DeviceProfile;

namespace ObsBridgeCore.Sources
{
    internal class AtemSource : ISource
    {
        [Verb("atem", HelpText = "Take Tally events from Atem Switchers")]
        public class AtemOptions : MainOptions
        {
            [Option("name",Default = "ATEM Mini Pro",HelpText = "Name to identify your device")]
            public string Name { get; set; }
        }

        AtemDiscoveryService discoveryService = new AtemDiscoveryService();
        AtemClient atem;
        AtemOptions CurrentOptions;
        List<string> Sources = new List<string>();
        List<string> Tally = new List<string>();
        List<string> Preview = new List<string>();

        public AtemSource(Logger log, MainOptions options) : base(log,options)
        {
            CurrentOptions = options as AtemOptions;
        }

        public override event Action<List<string>> OnTallyChange;
        public override event Action<List<string>> OnPreviewTallyChange;
        public override event Action<List<string>> OnSourcesChanged;
        public override event Action<bool> OnOnlineChanged;
        public override event Action<DateTime> OnRecordingStarted;
        public override event Action<DateTime> OnRecordingStopped;
        public override event Action<DateTime> OnStreamingStarted;
        public override event Action<DateTime> OnStreamingStopped;
        public override event Action<string> OnNameChanged;

        public override void Connect()
        {
            OnNameChanged?.Invoke(CurrentOptions.Name);
            discoveryService.OnDeviceSeen += DiscoveryService_OnDeviceSeen;
            discoveryService.GetDevices();

            Logger.Info("Searching for devices...");
        }

        private void DiscoveryService_OnDeviceSeen(object sender, AtemDeviceInfo device)
        {
            discoveryService.Stop();
            Logger.Info($"Connected to {device.Name} at {device.Address}");
            atem = new AtemClient(device.Address, true);
            atem.OnReceive += Atem_OnReceive;
            atem.OnConnection += Atem_OnConnection;
            atem.OnDisconnect += Atem_OnDisconnect;
            
        }

        private void Atem_OnDisconnect(object sender)
        {
            OnOnlineChanged?.Invoke(false);
            discoveryService.GetDevices();
            Logger.Info("Searching for devices...");
        }

        private void Atem_OnConnection(object sender)
        {
            OnOnlineChanged?.Invoke(true);
            OnNameChanged?.Invoke("ATEM Switcher");

            Sources.Add("Input 1");
            Sources.Add("Input 2");
            Sources.Add("Input 3");
            Sources.Add("Input 4");

            OnSourcesChanged?.Invoke(Sources);

            atem.SendCommand(new TopologyCommand());
        }

        private void Atem_OnReceive(object sender, IReadOnlyList<LibAtem.Commands.ICommand> commands)
        {
            foreach (ICommand cmd in commands)
            {
                if (cmd is TopologyCommand)
                {
                    var top = cmd as TopologyCommand;
                    var count = top.VideoSources;
                    Sources.Clear();
                    for (int i = 0; i < Sources.Count; i++)
                    {
                        Sources.Add($"Input {i}");
                    }
                    OnSourcesChanged?.Invoke(Sources);
                }

                if (cmd is TallyBySourceCommand)
                {
                    //var tally = ((TallyBySourceCommand)cmd).Tally

                    Console.WriteLine(((TallyBySourceCommand)cmd).Tally.Count);
                    Console.WriteLine(((TallyBySourceCommand)cmd).Tally.Keys);
                    Console.WriteLine(((TallyBySourceCommand)cmd).Tally);
                }

                if (cmd is TallyByInputCommand)
                {
                    //first bool is program, second is preview?
                    var tally = ((TallyByInputCommand)cmd).Tally;
                    Console.WriteLine(((TallyByInputCommand)cmd).Tally.Count);
                    for (int i=0;i<Sources.Count;i++)
                    {
                        if (tally[i].Item1)
                            Tally.Add(Sources[i]);
                        else
                            Tally.Remove(Sources[i]);
                        if (tally[i].Item2)
                            Preview.Add(Sources[i]);
                        else
                            Preview.Remove(Sources[i]);
                    }

                    OnTallyChange?.Invoke(Tally);
                    OnPreviewTallyChange?.Invoke(Preview);
                }
            }
        }
    }
}
