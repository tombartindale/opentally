using System;
using System.Threading;
using LibAtem.Commands;
using LibAtem.Discovery;
using LibAtem.Net;
using 

namespace TallyTest
{
    class Program
    {
        static AtemDiscoveryService discoveryService = new AtemDiscoveryService();
        static AtemClient atem;

        static void Main(string[] args)
        {
            Console.WriteLine("Started");
            discoveryService.OnDeviceSeen += DiscoveryService_OnDeviceSeen;
            discoveryService.GetDevices();

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        private static void DiscoveryService_OnDeviceSeen(object sender, AtemDeviceInfo device)
        {
            discoveryService.Stop();
            Console.WriteLine($"Seen {device.Name} at {device.Address}");
            atem = new AtemClient(device.Address, true);
            atem.OnReceive += Atem_OnReceive;
        }

        private static void Atem_OnReceive(object sender, System.Collections.Generic.IReadOnlyList<LibAtem.Commands.ICommand> commands)
        {
            foreach (ICommand cmd in commands)
            {
                if (cmd is TallyBySourceCommand)
                {
                    var tally = ((TallyBySourceCommand)cmd).Tally
                        tally.First()

                    Console.WriteLine(((TallyBySourceCommand)cmd).Tally.Count);
                    Console.WriteLine(((TallyBySourceCommand)cmd).Tally.Keys);
                    Console.WriteLine(((TallyBySourceCommand)cmd).Tally);
                }
            }

        }
    }
}
