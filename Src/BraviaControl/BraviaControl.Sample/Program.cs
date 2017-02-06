using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraviaControl.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            var authKey = File.Exists("BraviaClientAuth.key") ? File.ReadAllText("BraviaClientAuth.key") : String.Empty;
            var hostName = ConfigurationManager.AppSettings["TvHostname"];
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var client = new BraviaControlClient(hostName, clientId, "SampleClient (.NET)", authKey);

            // Register or Update AuthKey
            if (String.IsNullOrWhiteSpace(authKey))
            {
                // Register a client and acquire AuthKey (at first time)
                await client.RequestPinAsync();
                Console.Write("Enter PIN: ");
                var pinCode = Console.ReadLine();
                authKey = await client.RegisterAsync(pinCode);
                File.WriteAllText("BraviaClientAuth.key", authKey);
            }
            else
            {
                // Update Registration Authkey
                authKey = await client.RenewAuthKeyAsync();
                File.WriteAllText("BraviaClientAuth.key", authKey);
            }

            // Get power status
            // Console.WriteLine((await client.System.GetPowerStatusAsync()).Status);

            // Power on/off
            // await client.SendIrccAsync(RemoteControllerKeys.TvPower);

            // Get available ISBT Channels list on TV
            Console.WriteLine("Channels:");
            var contentList = await client.AvContent.GetContentListAsync("tv:isdbt", 0, 100, "");
            foreach (var content in contentList)
            {
                Console.WriteLine($"{content.DirectRemoteNum}: {content.Title} ({content.DispNum})");
            }

            Console.ReadLine();
        }
    }
}
