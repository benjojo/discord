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
            Steam.Friends.SetPersonaName("AgopBot");
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
            SteamID Sender = msg.ChatterID;
            SteamID Room = msg.ChatRoomID;
            String Message = msg.Message;

            Console.WriteLine(Steam.Friends.GetFriendPersonaName(Sender) + ": " + Message);

            recentRequests.RemoveAll(r => DateTime.Now.Subtract(r.Time).TotalSeconds > 2); //Remove recent requests if they were more than 3 seconds ago

            Request recent = recentRequests.FirstOrDefault(r => r.Sender == Sender);
            if (recent != null) //This user has already sent a command in the last 2 seconds: Increase kick threshold!
            {
                recent.KickThreshold++;
                recent.Time = DateTime.Now;

                if (recent.KickThreshold > 4) //If warned enough, kick!
                {
                    //TODO: Actually kick!
                    Chat.Send(Room, string.Format("{0} has been kicked for spam.", Steam.Friends.GetFriendPersonaName(Sender)));
                    return;
                }
            }
            else
                recentRequests.Add(new Request { Sender = Sender, Time = DateTime.Now });

            int sudocmd = Message.StartsWith("sudo") ? 4 : (Message.StartsWith("su") ? 2 : 0);

            if (sudocmd != 0)
            {
                string[] args = Message.Substring(sudocmd).Trim().Split(' ');
                if ((args.Length == 0))
                    Send(Room, "No command specified.");
                else
                    ChatCommands.HandleChatCommand(Room, Sender, args);

                return; // We don't want it parsing URLs and what not.
            }

            Thread T = new Thread(() =>
            {
                try {
                    Regex r = new Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");

                    Match m = r.Match(Message);
                    if (m.Success)
                    {
                        if (m.Groups["Domain"].Value.EndsWith("youtube.com"))
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

                                Send(Room, "YouTube: " + name + " - " + creator);
                            }
                        }
                        if (m.Groups["Domain"].Value == "open.spotify.com")
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

                            Send(Room, "Spotify: '" + name + "' by " + artist);

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

            if (Message == "make me a sandwich")
                Send(Room, "What? Make it yourself.");
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