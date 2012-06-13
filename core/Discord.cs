using System;
using System.IO;

namespace discord.core
{
    public class Discord
    {
        public bool StopLoop { get; private set; }

        public Discord()
        {
            if (!Settings.LoadXML())
                Settings.SaveXML();

            plugins.PluginLoader.Load("test.dll");
            plugins.PluginLoader.WatchDirectory(Settings.Instance.PluginPath);

            SteamInterface.Username = Settings.Instance.Username;
            SteamInterface.Password = Settings.Instance.Password;
            SteamInterface.Nick     = Settings.Instance.Nick;
            SteamInterface.Connect();

            Events.OnChatMsgCallback += new ChatMsgEventHandler(Events_OnChatMsgCallback);
            Events.OnLoginKeyCallback += new LoginKeyEventHandler(Events_OnLoginKeyCallback);
        }

        ~Discord()
        {
            SteamInterface.Disconnect();
        }

        public void Log(string data)
        {
            DateTime date = DateTime.UtcNow;
            string time = date.ToString("[HH:mm:ss]");
            string formatted = string.Format("{0} {1}", time, data);
            Events.CallEvent_Log(formatted);

            string filePath = Path.Combine(Settings.Instance.LogPath,
                string.Format("{0}{1}{2}{3}{4}.txt", date.ToString("yyyy"), Path.DirectorySeparatorChar,
                date.ToString("MMMM"), Path.DirectorySeparatorChar, date.ToString("dd")));
            Logger.Write(formatted, filePath.ToLowerInvariant());
            Console.WriteLine(formatted);
        }

        public void Say(string message)
        {
            Log(message);
        }

        public void Update()
        {
            SteamInterface.Update();
        }

        void Events_OnChatMsgCallback(SteamKit2.SteamFriends.ChatMsgCallback msg)
        {
            if (msg.ChatMsgType == SteamKit2.EChatEntryType.ChatMsg) {
                Log("<" + SteamInterface.Friends.GetFriendPersonaName(msg.ChatterID) + "> [" + msg.ChatterID + "]" + " - " + msg.Message);
            }
        }

        void Events_OnLoginKeyCallback(SteamKit2.SteamUser.LoginKeyCallback msg)
        {
            SteamKit2.SteamID id = new SteamKit2.SteamID(103582791433166824);
            id.AccountInstance = 0x80000;
            id.AccountType = SteamKit2.EAccountType.Chat;
            SteamInterface.Friends.JoinChat(id);
        }
    }
}
