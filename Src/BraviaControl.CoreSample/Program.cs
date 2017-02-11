using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BraviaControl.CoreSample
{
    public class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            // Get the pre generated Key if exists
            var authKey = File.Exists("BraviaClientAuth.key") ? 
                            File.ReadAllText("BraviaClientAuth.key") : 
                            String.Empty;

            // Host name or IP Address of the TV
            var hostName = "tv.home";

            // Random unique string identifier for each client
            var clientId = "8d76d476-4e75-4891-8888-22ffe33a3ef8";

            // Name of the client displayed on the TV
            var clientName = ".net Core Sony Bravia remote control Sample";

            // Client used to control the TV
            IBraviaControlClient client = new BraviaControlClient(hostName, clientId, clientName, authKey);

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

            // Get power status and show it
            Console.WriteLine((await client.System.GetPowerStatusAsync()).Status);

            // Example of command to Power on / off the TV
            // await client.SendIrccAsync(RemoteControllerKeys.TvPower);

            //// Get available ISBT Channels list on TV
            Console.WriteLine("Digital Channels:");
            BraviaControlClient.GetContentListResponse[] digitalChannels = null;
            BraviaControlClient.GetContentListResponse[] satelliteChannels = null;

            var stringBuilder = new StringBuilder("[");

            var initial = 0;
            // Read all Digital Television channels in blocks of 50
            while (initial == 0 || digitalChannels.Length > 0)
            {                
                digitalChannels = await client.AvContent.GetContentListAsync("tv:dvbt", initial, 50, "");
                foreach (var content in digitalChannels)
                {
                    stringBuilder.Append("{");
                    stringBuilder.AppendLine($"uri='{content.Uri}',name='{content.Title}',channel='{content.DispNum}'");
                    stringBuilder.Append("},");
                }
                initial += 50;
            }

            initial = 0;

            // Read all Satellite Television channels in blocks of 50
            while (initial == 0 || satelliteChannels.Length > 0)
            {
                satelliteChannels = await client.AvContent.GetContentListAsync("tv:dvbs", initial, 50, "");
                foreach (var content in satelliteChannels)
                {
                    stringBuilder.Append("{");
                    stringBuilder.AppendLine($"uri='{content.Uri}',name='{content.Title}',channel='{content.DispNum}'");
                    stringBuilder.Append("},");
                }
                initial += 50;
            }
            stringBuilder.AppendLine("}]");
            Console.WriteLine(stringBuilder);

            // Set channel using the URI
            //await client.AvContent.SetPlayContentAsync("tv:dvbs?trip=272.6000.2&srvName=Canale 5");
            //await client.AvContent.SetPlayContentAsync("tv:dvbt?trip=9018.20544.22272&srvName=Dave");

            Console.WriteLine("Apps:");
            // Get the list of installed apps
            var appList = await client.AppControl.GetApplicationListAsync();
            foreach (var app in appList)
            {
                Console.WriteLine($"{app.Title} - {app.Data} - {app.Icon} - {app.Uri}");
            }
            
            // Launch the Amazon App
            //await client.AppControl.SetActiveAppAsync("com.sony.dtv.com.amazon.aiv.eu.com.amazon.ignition.IgnitionActivity", "");

            // Lunch Plex
            //await client.AppControl.SetActiveAppAsync("com.sony.dtv.com.plexapp.android.com.plexapp.plex.activities.mobile.PickUserActivity", "");
                        
            Console.ReadLine();
        }
    }
}
