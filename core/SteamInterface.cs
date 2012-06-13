using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SteamKit2;

namespace discord.core
{   
    internal static class SteamInterface {
        public static SteamClient Client { get; private set; }
        public static SteamFriends Friends { get; private set; }
        public static SteamUser User { get; private set; }

        public static String Username { get; set; }
        public static String Password { private get; set; }

        private static String nick;
        public static String Nick { get { return nick; } set { nick = value; if (Friends!=null) Friends.SetPersonaName(nick); } } 

        public static void Connect()
        {
            Client = new SteamClient(ProtocolType.Udp);
            Friends = Client.GetHandler<SteamFriends>();
            User = Client.GetHandler<SteamUser>();
            Client.Connect();
        }

        public static void Update()
        {
            CallbackMsg Message = Client.GetCallback();
            if (Message == null)
                return;

            Client.FreeLastCallback();

            if (Message.IsType<SteamClient.ConnectedCallback>())
            {
                SteamClient.ConnectedCallback Callback = (SteamClient.ConnectedCallback)Message;

                if (Callback.Result != EResult.OK)
                {
                    Console.WriteLine("Failed to connect to Steam Network - " + Callback.Result.ToString());
                    return;
                }

                User.LogOn(new SteamUser.LogOnDetails()
                {
                    Username = Username,
                    Password = Password,
                    AuthCode = "" // who the fuck cares
                });
            }
            else if (Message.IsType<SteamClient.DisconnectedCallback>())
            {
                SteamClient.DisconnectedCallback Callback = (SteamClient.DisconnectedCallback)Message;
                Console.WriteLine("Disconnected from the Steam Network!");
            }
            else if (Message.IsType<SteamUser.LoggedOnCallback>())
            {
                SteamUser.LoggedOnCallback Callback = (SteamUser.LoggedOnCallback)Message;
                Console.WriteLine("Logged On - " + Callback.Result);

                Friends.SetPersonaName(Nick);
                Friends.SetPersonaState((EPersonaState)6);
            }

            Events.HandleCallback(Message);
        }

        public static void Disconnect()
        {
            User.LogOff();
            Client.Disconnect();
        }
    }
}
