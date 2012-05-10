using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SteamKit2;

namespace AgopBot
{
    interface ICallbackHandler
    {
        void HandleCallback(CallbackMsg msg);
    }

    internal static class Steam
    {
        public static SteamClient Client { get; private set; }

        public static SteamFriends Friends { get; private set; }

        public static SteamUser User { get; private set; }

        public static String Username { get; set; }

        public static String Password { get; set; }

        public static String AuthCode { get; set; }

        public static Chat Chat { get; private set; }

        static List<ICallbackHandler> Callbacks = new List<ICallbackHandler>();

        public static void Connect()
        {
            Client = new SteamClient(ProtocolType.Udp);
            Friends = Client.GetHandler<SteamFriends>();
            User = Client.GetHandler<SteamUser>();
            Chat = new Chat();

            Client.Connect();
        }

        public static void AddHandler(ICallbackHandler handler)
        {
            Callbacks.Add(handler);
        }

        public static void RemoveHandler(ICallbackHandler handler)
        {
            Callbacks.Remove(handler);
        }

        public static void Update()
        {
            CallbackMsg Message = Client.GetCallback();
            if (Message == null)
                return;

            Client.FreeLastCallback();

            if (Message.IsType<SteamClient.ConnectedCallback>())
            {
                Console.WriteLine("Steam >> Logging in.");
                User.LogOn(new SteamUser.LogOnDetails()
                {
                    Username = Username,
                    Password = Password,
                    AuthCode = AuthCode
                });
            }

            List<ICallbackHandler> Handlers = new List<ICallbackHandler>(Callbacks);

            foreach (ICallbackHandler i in Handlers)
                i.HandleCallback(Message);
        }

        public static void Shutdown()
        {
            User.LogOff();
            Client.Disconnect();
        }
    }
}