using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SteamKit2;

namespace AgopBot
{
    internal class Chat : ICallbackHandler
    {
        public static List<SteamID> Admins = new List<SteamID>
        {
            new SteamID("STEAM_0:0:32616431"), //Naarkie
            new SteamID("STEAM_0:1:18296695"), //Matt
            new SteamID("STEAM_0:0:15033805") //Dlaor
        };

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
        }

        public static void HandleLogin(SteamUser.LoginKeyCallback msg)
        {
            Steam.Friends.SetPersonaName("AgopBot");
            Steam.Friends.SetPersonaState(EPersonaState.Online);
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
            if (Message.StartsWith("sudo"))
            {
                string[] args = Message.Substring(4).Trim().Split(' ');
                if ((args.Length == 0))
                    Send(Room, "No command specified.");
                else
                    if (Admins.Contains(Sender))
                        ChatCommands.HandleChatCommand(Room, Sender, args, true);
                    else
                        ChatCommands.HandleChatCommand(Room, Sender, args, false);
            }
            if (Message == "make me a sandwich")
            {
                Send(Room, "What? Make it yourself.");
            }

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
        }
    }
}