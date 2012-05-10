using System;
using System.Collections.Generic;
using System.Timers;
using SteamKit2;

namespace AgopBot
{
    class Chat : ICallbackHandler
    {
        public Chat()
        {
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
        }

        public static void HandleLogin(SteamUser.LoginKeyCallback msg)
        {
            Steam.Friends.SetPersonaName("AgopBot");
            Steam.Friends.SetPersonaState(EPersonaState.Online);
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
            String SenderName = Steam.Friends.GetFriendPersonaName(Sender);
            SteamID Room = msg.ChatRoomID;
        }

        public static void HandlePersonaStateChange(SteamFriends.PersonaStateCallback msg)
        {
            
        }
    }
}