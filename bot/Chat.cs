using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Linq;
using System.Timers;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace AgopBot
{
    internal class Chat : ICallbackHandler
    {
        class Request
        {
            public SteamID Sender { get; set; }
            public DateTime Time { get; set; }
            public int KickThreshold { get; set; }
        }

        private static List<Request> recentRequests = new List<Request>();

        public Chat()
        {
            ChatCommands.InitAll();
            Steam.AddHandler(this);
        }

        public static void Join(SteamID id)
        {
            id.AccountInstance = 0x80000;
            id.AccountType = EAccountType.Chat;
            Steam.Friends.JoinChat(id);
        }

        public static void Send(SteamID id, String message)
        {
            Steam.Friends.SendChatRoomMessage(id, EChatEntryType.ChatMsg, message);
        }

        public void HandleCallback(CallbackMsg msg)
        {
            if (msg.IsType<SteamUser.LoginKeyCallback>())
                HandleLogin((SteamUser.LoginKeyCallback)msg);
            if (msg.IsType<SteamUser.LoggedOffCallback>())
                HandleLogoff((SteamUser.LoggedOffCallback)msg);
            if (msg.IsType<SteamFriends.ChatEnterCallback>())
                HandleChatEnter((SteamFriends.ChatEnterCallback)msg);
            if (msg.IsType<SteamFriends.ChatMsgCallback>())
                HandleChatMessage((SteamFriends.ChatMsgCallback)msg);
            if (msg.IsType<SteamFriends.PersonaStateCallback>())
                HandlePersonaStateChange((SteamFriends.PersonaStateCallback)msg);
            if (msg.IsType<SteamFriends.ChatMemberInfoCallback>())
                HandleChatMemberInfoCallback((SteamFriends.ChatMemberInfoCallback)msg);
            if (msg.IsType<SteamFriends.ChatActionResultCallback>())
                HandleChatActionResultCallback((SteamFriends.ChatActionResultCallback)msg);
        }

        public static void HandleLogin(SteamUser.LoginKeyCallback msg)
        {
            Steam.Friends.SetPersonaName(String.IsNullOrWhiteSpace(Configurator.Config.BotName) ? "AgopBot" : Configurator.Config.BotName);
            Steam.Friends.SetPersonaState((EPersonaState)6); // Looking to Play - hehe.
            //Join(103582791430091926);
            Join(103582791433166824);
        }

        public static void HandleLogoff(SteamUser.LoggedOffCallback msg)
        {
        }

        public static void HandleChatEnter(SteamFriends.ChatEnterCallback msg)
        {
            if (msg.EnterResponse != EChatRoomEnterResponse.Success)
                Console.WriteLine("Steam > Unable to enter this chatroom (" + msg.EnterResponse.ToString() + ").");
        }

        public static void HandleChatMessage(SteamFriends.ChatMsgCallback msg)
        {
            SteamID sender = msg.ChatterID;
            SteamID room = msg.ChatRoomID;
            String message = msg.Message;

            Console.WriteLine(Steam.Friends.GetFriendPersonaName(sender) + ": " + message);

            if (Configurator.Config.EnableMySQL)
                SQL.DB.QueryNoReturn(string.Format(@"INSERT INTO chat (uid, message) VALUES({0}, '{1}');", sender.ConvertToUInt64(), SQL.DB.EscapeString(message)));

            recentRequests.RemoveAll(r => DateTime.Now.Subtract(r.Time).TotalSeconds > 2); //Remove recent requests if they were more than 2 seconds ago

            Request recent = recentRequests.FirstOrDefault(r => r.Sender == sender);
            if (recent != null) //This user has already sent a command in the last 2 seconds: Increase kick threshold!
            {
                recent.KickThreshold++;
                recent.Time = DateTime.Now;

                if (!Util.IsAdmin(recent.Sender) && recent.KickThreshold > 4) //When given enough warnings, and not an admin, KICK THE BASTARD
                {
                    Steam.Friends.KickChatMember(room, sender);
                    Send(room, string.Format("{0} has been kicked for spam.", Steam.Friends.GetFriendPersonaName(sender)));

                    recent.KickThreshold = 0; //Don't kick again!
                    return;
                }
            }
            else
                recentRequests.Add(new Request { Sender = sender, Time = DateTime.Now });

            int sudocmd = message.StartsWith("sudo") ? 4 : (message.StartsWith("su ") ? 2 : 0);

            if (sudocmd != 0)
            {
                string[] args = message.Substring(sudocmd).Trim().Split(' ');
                if ((args.Length == 0))
                    Send(room, "No command specified.");
                else
                    ChatCommands.HandleChatCommand(room, sender, args);

                return; // We don't want it parsing URLs and what not.
            }

            Thread T = new Thread(() =>
            {
                try {
                    Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");

                    Match m = r.Match(message);
                    if (m.Success)
                    {
                        if (m.Value.EndsWith(".mp3"))
                        {
                            /*
                            Uri uri = new Uri(m.Value);
                            using (var client = new System.Net.Sockets.TcpClient(uri.Host, uri.Port))
                            {
                                using (System.Net.Sockets.NetworkStream n = client.GetStream())
                                {
                                    uri.PathAndQuery
                                }
                            }

                            Send(room, "MP3 File: We'll finish this later.");
                             */
                        }
                        else if (m.Groups["Domain"].Value.EndsWith("youtube.com"))
                        {
                            Regex getvRegex = new Regex(@"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
                            Match YTMatch = getvRegex.Match(m.Value);

                            if (YTMatch.Success)
                            {
                                string vid = YTMatch.Groups[1].Value;
                                string api = "http://www.youtube.com/get_video_info?video_id=" + vid;

                                WebRequest request = WebRequest.Create(api);
                                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                                Stream dataStream = response.GetResponseStream();
                                StreamReader reader = new StreamReader(dataStream);
                                string responseFromServer = reader.ReadToEnd();

                                string name = "Unknown";
                                string creator = "Probably a copyrighted video.";

                                foreach (var item in responseFromServer.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var tokens = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tokens.Length < 2)
                                        continue;
                                    var paramName = tokens[0];
                                    var paramValue = tokens[1];

                                    if (paramName == "title")
                                        name = paramValue;
                                    if (paramName == "author")
                                        creator = paramValue;
                                }

                                name = Uri.UnescapeDataString(name.Replace('+', ' '));
                                creator = Uri.UnescapeDataString(creator.Replace('+', ' '));

                                Send(room, "YouTube: " + name + " - " + creator);
                            }
                        }
                        else if (m.Groups["Domain"].Value == "open.spotify.com")
                        {
                            string api = "http://ws.spotify.com/lookup/1/.json?uri=spotify:track:" + m.Value.Substring(30);
                            WebRequest request = WebRequest.Create(api);
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                            Stream dataStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();

                            JObject token = JObject.Parse(responseFromServer);
                            string name = token.SelectToken("track").SelectToken("name").ToObject<string>();
                            string artist = token.SelectToken("track").SelectToken("artists").First.SelectToken("name").ToObject<string>();

                            Send(room, "Spotify: '" + name + "' by " + artist);

                            reader.Close();
                            dataStream.Close();
                            response.Close();
                        }
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine("Something went wrong with the URL parsing!");
                    Console.WriteLine(ex.Message);
                }

            });

            T.Start();

            if (message == "make me a sandwich")
                Send(room, "What? Make it yourself.");
        }

        private static void HandleChatMemberInfoCallback(SteamFriends.ChatMemberInfoCallback msg)
        {
            SteamID Room = msg.ChatRoomID;

            Console.WriteLine("ChatMemberInfoCallback: " +
                   Steam.Friends.GetFriendPersonaName(msg.StateChangeInfo.ChatterActedBy) + " - " +
                   Steam.Friends.GetFriendPersonaName(msg.StateChangeInfo.ChatterActedOn)
                   );
        }

        public static void HandlePersonaStateChange(SteamFriends.PersonaStateCallback msg)
        {
            Console.WriteLine("PersonaStateChange: " + msg.Name + ", " + msg.State + ", " + msg.FriendID + ", " + msg.OnlineSessionInstances);
        }

        public static void HandleChatActionResultCallback(SteamFriends.ChatActionResultCallback msg)
        {
            Console.WriteLine("ChatActionResult: " + msg.ChatterID + ", " + msg.Result);
        }
    }
}