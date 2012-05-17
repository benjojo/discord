using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using IrcDotNet;
using SteamKit2;

namespace AgopBot.Commands
{
    class IRCMaster
    {
        private const string IRCServer = "irc.freenode.net";
        private const string IRCChannel = "#fpprogrammers";

        private static bool _started;
        private static IrcClient _client;
        private static SteamID _room;

        public static void Start(SteamID room)
        {
            _room = room;

            if (_started)
                return;

            _started = true;

            _client = new IrcClient();
           // _client.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
            _client.Connected += (sender, e) =>
                                     {
                                         Message("Connected.");

                                         var c = (IrcClient)sender;
                                         c.Channels.Join(IRCChannel);

                                         c.LocalUser.NoticeReceived += (sender2, e2) => Message(string.Format("{0}: {1}", e2.Source.Name, e2.Text));
                                         c.LocalUser.MessageReceived += (sender2, e2) => Message(string.Format("Received PM from {0}: {1}", e2.Source.Name, e2.Text));
                                         c.LocalUser.JoinedChannel += (sender2, e2) =>
                                                                          {
                                                                              Message(string.Format("Joined {0}. {1}", e2.Channel.Name, e2.Comment));
                                                                              Message("Topic: " + e2.Channel.Topic);

                                                                              e2.Channel.UserKicked += (sender3, e3) => Message(string.Format("{0} was kicked from {1}. {2}", e3.ChannelUser.User.NickName, e2.Channel.Name, e2.Comment));
                                                                              e2.Channel.UserJoined += (sender3, e3) => Message(string.Format("{0} joined {1}. {2}", e3.ChannelUser.User.NickName, e2.Channel.Name, e2.Comment));
                                                                              e2.Channel.UserLeft += (sender3, e3) => Message(string.Format("{0} left {1}. {2}", e3.ChannelUser.User.NickName, e2.Channel.Name, e2.Comment));
                                                                              e2.Channel.MessageReceived += SendIRC;
                                                                              e2.Channel.NoticeReceived += (sender3, e3) => Message(string.Format("{0}: {1}", e3.Source.Name, e3.Text));
                                                                          };

                                         c.LocalUser.LeftChannel += (sender2, e2) => Message(string.Format("Left {0}. ({1})", e2.Channel.Name, e2.Comment));

                                     };

            _client.Disconnected += (sender, e) => Message("Disconnected.");
            _client.Registered += (sender, e) => Message("Registered.");

            using (var connectedEvent = new ManualResetEventSlim(false))
            {
                _client.Connected += (sender2, e2) => connectedEvent.Set();

                Message("Connecting...");

                _client.Connect(IRCServer, false, new IrcUserRegistrationInfo
                                                      {
                                                          NickName = "AGOPBOT",
                                                          UserName = "AGOPBOAT",
                                                          Password = "AGOPVOMIT",
                                                          RealName = "Agopbot Shirinian"
                                                      });

                if (!connectedEvent.Wait(10000))
                {
                    _client.Dispose();
                    Message(string.Format("Connection to {0} timed out.", IRCServer));
                }
            }
        }

        private static void Message(string text)
        {
            Steam.Friends.SetPersonaName(String.IsNullOrWhiteSpace(Configurator.Config.BotName) ? "AgopBot" : Configurator.Config.BotName);

            Timer t = new Timer(s => Chat.Send(_room, "[IRC] " + text), null, 300, Timeout.Infinite); //Slight delay to allow for name change
        }

        private static void SendIRC(object sender, IrcMessageEventArgs e)
        {
            Steam.Friends.SetPersonaName("*" + e.Source.Name);
            Timer t = new Timer(s => Chat.Send(_room, e.Text), null, 300, Timeout.Infinite); //Slight delay to allow for name change
        }

        public static void Stop()
        {
            if (!_started)
                return;

            _started = false;

            string leaveMessage = "STEAM > IRC";

            _client.Channels.Leave(new List<string> { IRCChannel }, leaveMessage);
            _client.Quit(leaveMessage);

            _client.Dispose();
        }
    }

    public class CmdIRC : Command
    {
        public CmdIRC() : base("irc") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            string subcommand = args[0];
            switch (subcommand)
            {
                case "start":
                    IRCMaster.Start(room); break;
                case "stop":
                    if (Util.IsAdmin(sender)) IRCMaster.Stop(); break;
            }
        }
    }
}