using EliteAPI.Abstractions;
using Matric.Integration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.IO;

namespace EDAPITEST
{
    // Core class of our application
    public class Core
    {
        private readonly ILogger<Core> _log;
        private readonly IEliteDangerousAPI _api;
        static string AppName = "EDMatricBridge";
        public static string DECK_ID = "72b9fc1a-2386-45b2-abbb-70533cdacb1b";
        static string ConfigName = "EDConfig.ini";
        public static string PIN = "";
        public static string CLIENT_ID;
        static Matric.Integration.Matric matric;
        static IniData InitData;

        static class DeckPages
        {
            public static string Flight = "2a3a2b7a-cf33-46c7-8fce-0082f959434e";
            public static string Docking = "2fbb99f9-ec4e-438a-b67f-b563340ebbb1";


        }
        // Button id's in our demo deck that we want to control
        static class Buttons
        {
            public static string LandindPad = "54b6ee84-079f-4d4d-957e-22f81b40a947";
        }


        public Core(ILogger<Core> log, IEliteDangerousAPI api)
        {
            // Get our dependencies through dependency injection
            _log = log;
            _api = api;
            Console.Title = AppName;
            _log.LogInformation("Authorize connection in MATRIC, then enter PIN:");
            var parser = new FileIniDataParser();
            if (!File.Exists(ConfigName))
            {
                File.Create(ConfigName).Close();
                
            };
            InitData = parser.ReadFile(ConfigName);
            PIN = InitData["DECK"]["PIN"];
            matric = new Matric.Integration.Matric(AppName);
            if (String.IsNullOrEmpty(PIN)){ 
                matric.RequestAuthorizePrompt();
                PIN = Console.ReadLine();
            }
            matric.PIN = PIN;
            matric.OnConnectedClientsReceived += Matric_OnConnectedClientsReceived;
            matric.GetConnectedClients();
            InitData["DECK"]["PIN"] = PIN;
            parser.WriteFile(ConfigName, InitData);
        }
        private static void Matric_OnConnectedClientsReceived(object source, List<ClientInfo> clients)
        {
            UpdateClientsList(clients);
        }

        public static void UpdateClientsList(List<ClientInfo> connectedClients)
        {
            if (connectedClients.Count == 0)
            {
                Console.WriteLine("No connected devices found, make sure your smartphone/tablet is connected\nPress any key to exit");
                Console.ReadKey();
                Environment.Exit(0);
            }
            Console.WriteLine("Found devices:");
            foreach (ClientInfo client in connectedClients)
            {
                Console.WriteLine($@"{client.Id} {client.Name}");
            }
            CLIENT_ID = connectedClients[0].Id;
            Console.WriteLine("Starting on First device found.");
           }
        public async Task Run()
        {
            
            List<SetButtonsVisualStateArgs> LandingOn = new List<SetButtonsVisualStateArgs>();
            LandingOn.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_LANDING"));
            List<SetButtonsVisualStateArgs> LandingOff = new List<SetButtonsVisualStateArgs>();
            LandingOff.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_LANDING"));
            _api.Status.Gear.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {
                    

                   matric.SetButtonsVisualState(CLIENT_ID, LandingOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LandingOff);
                }
            };

            List<SetButtonsVisualStateArgs> CargoOn = new List<SetButtonsVisualStateArgs>();
            CargoOn.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_CARGO"));
            List<SetButtonsVisualStateArgs> CargoOff = new List<SetButtonsVisualStateArgs>();
            CargoOff.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_CARGO"));
            _api.Status.CargoScoop.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {             
                    matric.SetButtonsVisualState(CLIENT_ID, CargoOn);
                } else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, CargoOff);
                }
            };


            List<SetButtonsVisualStateArgs> SilentOn = new List<SetButtonsVisualStateArgs>();
            SilentOn.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_SILENT"));
            List<SetButtonsVisualStateArgs> SilentOff = new List<SetButtonsVisualStateArgs>();
            SilentOff.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_SILENT"));
            _api.Status.SilentRunning.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, SilentOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, SilentOff);
                }
            };
            List<SetButtonsVisualStateArgs> FlightAssistOn = new List<SetButtonsVisualStateArgs>();
            FlightAssistOn.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_FLIGHTASSIST"));
            List<SetButtonsVisualStateArgs> FlightAssistOff = new List<SetButtonsVisualStateArgs>();
            FlightAssistOff.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_FLIGHTASSIST"));
            _api.Status.FlightAssist.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, FlightAssistOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, FlightAssistOff);
                }
            };

            List<SetButtonsVisualStateArgs> HardPointsOut = new List<SetButtonsVisualStateArgs>();
            HardPointsOut.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_HARDPOINTS"));
            List<SetButtonsVisualStateArgs> HardPointsIn = new List<SetButtonsVisualStateArgs>();
            HardPointsIn.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_HARDPOINTS"));
            _api.Status.Hardpoints.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, HardPointsOut);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, HardPointsIn);
                }
            };
            List<SetButtonsVisualStateArgs> NightVisionOn = new List<SetButtonsVisualStateArgs>();
            NightVisionOn.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_NIGHTVISION"));
            List<SetButtonsVisualStateArgs> NightVisionOff = new List<SetButtonsVisualStateArgs>();
            NightVisionOff.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_NIGHTVISION"));
            _api.Status.NightVision.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, NightVisionOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, NightVisionOff);
                }
            };
            List<SetButtonsVisualStateArgs> LightsOn = new List<SetButtonsVisualStateArgs>();
            LightsOn.Add(new SetButtonsVisualStateArgs(null, "on", buttonName: "BTN_LIGHTS"));
            List<SetButtonsVisualStateArgs> LightsOff = new List<SetButtonsVisualStateArgs>();
            LightsOff.Add(new SetButtonsVisualStateArgs(null, "off", buttonName: "BTN_LIGHTS"));
            _api.Status.Lights.OnChange += (sender, isDeployed) =>
            {
                if (isDeployed)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LightsOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LightsOff);
                }
            };
            //Taking Damage orange
            _api.Events.HeatDamageEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: "#FF6500", buttonName: "BTN_HEATSINK");
            };
            //Close to taking damage yellow
            _api.Events.HeatWarningEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: "yellow", buttonName:"BTN_HEATSINK");
            };
            //On Docking event start make icon yellow
            _api.Events.DockingRequestedEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: "yellow",buttonName:"BTN_DOCKING");
            };
            //If event was denied make it Traffic Cone Orange
            _api.Events.DockingDeniedEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: "orange", buttonName: "BTN_DOCKING");
            };
            //If Granted Go Green
            _api.Events.DockingGrantedEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: "green", buttonName: "BTN_DOCKING");
            };
            //If we cancel it go back to null
            _api.Events.DockingCancelledEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: null, buttonName: "BTN_DOCKING");
            };
            //If timeout go back to clear, Would like to go error color for a moment then clear but another day
            _api.Events.DockingTimeoutEvent += (sender, e) =>
            {
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: null, buttonName: "BTN_DOCKING");
            };

            _api.Events.DockedEvent += (sender, e) =>
            {
                matric.SetActivePage(CLIENT_ID, DeckPages.Docking);
                matric.SetButtonProperties(CLIENT_ID, Buttons.LandindPad, text: "Docked: " + e.StationName, backgroundcolorOff: "#df1616",
                    backgroundcolorOn: "white");
                matric.SetButtonProperties(CLIENT_ID, null, backgroundcolorOff: null, buttonName: "BTN_DOCKING");

                if (_api.Status.Gear.Value)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LandingOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LandingOff);
                }
                if (_api.Status.FlightAssist.Value)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, FlightAssistOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, FlightAssistOff);
                }
                if (_api.Status.SilentRunning.Value)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, SilentOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, SilentOff);
                }
            };

            _api.Events.UndockedEvent += (sender, e) =>
            {
                matric.SetActivePage(CLIENT_ID, DeckPages.Flight);
                if (_api.Status.Gear.Value)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LandingOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, LandingOff);
                }
                if (_api.Status.FlightAssist.Value)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, FlightAssistOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, FlightAssistOff);
                }
                if (_api.Status.SilentRunning.Value)
                {
                    matric.SetButtonsVisualState(CLIENT_ID, SilentOn);
                }
                else
                {
                    matric.SetButtonsVisualState(CLIENT_ID, SilentOff);
                }

            };
            

                // Start EliteAPI
            await _api.StartAsync();
        }
    }
}