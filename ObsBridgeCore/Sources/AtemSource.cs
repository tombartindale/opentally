using System;
using System.Collections.Generic;
using CommandLine;
using LibAtem.Commands;
using LibAtem.Net;
using NLog;
using ObsBridge;
using System.Linq;
using Zeroconf;
using System.Threading.Tasks;
using LibAtem.Commands.Settings;

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

        AtemClient atem;
        AtemOptions CurrentOptions;
        List<string> Sources = new List<string>();
        HashSet<string> Tally = new HashSet<string>();
        HashSet<string> Preview = new HashSet<string>();

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

        public async override void Connect()
        {

            OnNameChanged?.Invoke(CurrentOptions.Name);

            Logger.Info("Searching for devices...");

            //ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync();

            await Discover();
        }

        async Task Discover()
        {
            IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("_blackmagic._tcp.local.");
            if (results.Count() > 0)
                DiscoveryService_OnDeviceSeen(null, results[0]);
            else
                await Discover();
        }



        private void DiscoveryService_OnDeviceSeen(object sender, IZeroconfHost device)
        {
            //discoveryService.Stop();
            //var dd = await device.DiscoveredDevice.GetDeviceInfo();
            Logger.Info($"Connected to {device.DisplayName} at {device.IPAddress}");
            atem = new AtemClient(device.IPAddress, true);
            atem.OnReceive += Atem_OnReceive;
            atem.OnConnection += Atem_OnConnection;
            atem.OnDisconnect += Atem_OnDisconnect;

        }

        private async void Atem_OnDisconnect(object sender)
        {
            OnOnlineChanged?.Invoke(false);
            //discoveryService.GetDevices();
            Logger.Info("Searching for devices...");
            await Discover();
        }

        private void Atem_OnConnection(object sender)
        {
            OnOnlineChanged?.Invoke(true);
            OnNameChanged?.Invoke("ATEM Switcher");

            if (Sources.Count == 0)
            {

                Sources.Add("Input 1");
                Sources.Add("Input 2");
                Sources.Add("Input 3");
                Sources.Add("Input 4");

                OnSourcesChanged?.Invoke(Sources);
                //atem.SendCommand(new TopologyCommand());
            }

        }

        DateTime? recordingstarted;
        uint lastseentimecode;

        private void Atem_OnReceive(object sender, IReadOnlyList<LibAtem.Commands.ICommand> commands)
        {
            foreach (ICommand cmd in commands)
            {
                //Console.WriteLine(cmd);

                if (cmd is OnAirStateCommand)
                {
                    //Console.WriteLine("on air");
                }

                if (cmd is StreamingStateCommand)
                {
                    //Console.WriteLine("Streaming state changed " + ((StreamingStateCommand)cmd).State);
                    if (((StreamingStateCommand)cmd).State == StreamState.Starting)
                    {
                        OnStreamingStarted?.Invoke(DateTime.Now);
                    }

                    if (((StreamingStateCommand)cmd).State == StreamState.Stopped)
                    {
                        OnStreamingStopped?.Invoke(DateTime.Now);
                    }
                }

                if (cmd is RecordingStateCommand)
                {
                    Console.WriteLine("Rec state cmd");
                }

                if (cmd is InputPropertiesGetCommand)
                {
                    //var top = cmd as TopologyV811Command;
                    //var count = top.VideoSources;

                    if (((InputPropertiesGetCommand)cmd).Id == LibAtem.Common.VideoSource.Input1)
                        Sources[0] = ((InputPropertiesGetCommand)cmd).LongName;

                    if (((InputPropertiesGetCommand)cmd).Id == LibAtem.Common.VideoSource.Input2)
                        Sources[1] = ((InputPropertiesGetCommand)cmd).LongName;

                    if (((InputPropertiesGetCommand)cmd).Id == LibAtem.Common.VideoSource.Input3)
                        Sources[2] = ((InputPropertiesGetCommand)cmd).LongName;

                    if (((InputPropertiesGetCommand)cmd).Id == LibAtem.Common.VideoSource.Input4)
                        Sources[3] = ((InputPropertiesGetCommand)cmd).LongName;

                    OnSourcesChanged?.Invoke(Sources);
                }

                if (cmd is TallyByInputCommand)
                {
                    //first bool is program, second is preview?
                    var tally = ((TallyByInputCommand)cmd).Tally;
                    
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

                    OnTallyChange?.Invoke(Tally.ToList());
                    OnPreviewTallyChange?.Invoke(Preview.ToList());
                }
            }
        }
    }
}
