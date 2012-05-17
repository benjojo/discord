using System;
using System.Collections.Generic;
using System.Linq;
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
                                         Message("Connected. Waiting for registration...");

                                         var c = (IrcClient)sender;
                                         c.Channels.Join(IRCChannel);

                                         c.LocalUser.NoticeReceived += (sender2, e2) => Console.WriteLine(string.Format("Notice from {0}: {1}", e2.Source.Name, e2.Text));
                                         c.LocalUser.MessageReceived += (sender2, e2) => Message(string.Format("Received PM from {0}: {1}", e2.Source.Name, e2.Text));
                                         c.LocalUser.JoinedChannel += (sender2, e2) =>
                                                                          {
                                                                              Message(string.Format("Joined {0} - IRC Spy Mode engaged. {1}", e2.Channel.Name, e2.Comment));

                                                                              //Message("Topic: " + e2.Channel.Topic); //Nothing happens!
                                                                              //e2.Channel.GetTopic(); //This causes TopicChanged to be called twice!

                                                                              e2.Channel.UsersListReceived += (sender3, e3) => Message("Users: " + string.Join(", ", e2.Channel.Users.OrderBy(u => u.User.NickName).Select(u => u.User.IsOperator ? "@" : "" + u.User.NickName))); //OH GOD MY EYES
                                                                              e2.Channel.TopicChanged += (sender3, e3) => Message("Topic: " + e2.Channel.Topic);
                                                                              e2.Channel.UserKicked += (sender3, e3) => Message(string.Format("{0} was kicked from {1}. {2}", e3.ChannelUser.User.NickName, e2.Channel.Name, e2.Comment));
                                                                              e2.Channel.UserJoined += (sender3, e3) => Message(string.Format("{0} joined {1}. {2}", e3.ChannelUser.User.NickName, e2.Channel.Name, e2.Comment));
                                                                              e2.Channel.UserLeft += (sender3, e3) => Message(string.Format("{0} left {1}. {2}", e3.ChannelUser.User.NickName, e2.Channel.Name, e2.Comment));
                                                                              e2.Channel.MessageReceived += SendIRC;
                                                                              e2.Channel.NoticeReceived += (sender3, e3) => Console.WriteLine(string.Format("Notice from {0}: {1}", e3.Source.Name, e3.Text));
                                                                          };

                                         c.LocalUser.LeftChannel += (sender2, e2) => Message(string.Format("Left {0}. ({1})", e2.Channel.Name, e2.Comment));

                                     };

            _client.Disconnected += (sender, e) => Message("Disconnected.");
            _client.Registered += (sender, e) => Message(string.Format("Registered. Joining {0}...", IRCChannel));

            using (var connectedEvent = new ManualResetEventSlim(false))
            {
                _client.Connected += (sender2, e2) => connectedEvent.Set();

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

        public static void Send(string message)
        {
            _client.LocalUser.SendMessage(IRCChannel, message);
        }
    }

    public class CmdIRC : Command
    {
        public CmdIRC() : base("irc") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            string subcommand = args[0];
            string arg = string.Join(" ", args.Skip(1));
            switch (subcommand)
            {
                case "start":
                    if (Util.IsAdmin(sender)) IRCMaster.Start(room); break;
                case "stop":
                    if (Util.IsAdmin(sender)) IRCMaster.Stop(); break;
                case "send":
                    if (Util.IsAdmin(sender)) IRCMaster.Send(arg); break;
            }
        }
    }
}