using System;
using System.Collections;
using SteamKit2;

namespace discord.core
{
    public delegate void AccountInfoEventHandler(SteamUser.AccountInfoCallback msg);
    public delegate void LoggedOffEventHandler(SteamUser.LoggedOffCallback msg);
    public delegate void LoggedOnEventHandler(SteamUser.LoggedOnCallback msg);
    public delegate void LoginKeyEventHandler(SteamUser.LoginKeyCallback msg);
    public delegate void ChatActionResultEventHandler(SteamFriends.ChatActionResultCallback msg);
    public delegate void ChatEnterEventHandler(SteamFriends.ChatEnterCallback msg);
    public delegate void ChatInviteEventHandler(SteamFriends.ChatInviteCallback msg);
    public delegate void ChatMemberInfoEventHandler(SteamFriends.ChatMemberInfoCallback msg);
    public delegate void ChatMsgEventHandler(SteamFriends.ChatMsgCallback msg);
    public delegate void PersonaStateEventHandler(SteamFriends.PersonaStateCallback msg);

    public delegate void LogEventHandler(String msg);

    public static class Events
    {
        public static event AccountInfoEventHandler OnAccountInfoCallback;
        public static event LoggedOffEventHandler OnLoggedOffCallback;
        public static event LoggedOnEventHandler OnLoggedOnCallback;
        public static event LoginKeyEventHandler OnLoginKeyCallback;
        public static event ChatActionResultEventHandler OnChatActionResultCallback;
        public static event ChatEnterEventHandler OnChatEnterCallback;
        public static event ChatInviteEventHandler OnChatInviteCallback;
        public static event ChatMemberInfoEventHandler OnChatMemberInfoCallback;
        public static event ChatMsgEventHandler OnChatMsgCallback;
        public static event PersonaStateEventHandler OnPersonaStateCallback;

        public static event LogEventHandler OnLog;

        public static void HandleCallback(CallbackMsg msg)
        {
            if (msg.IsType<SteamUser.AccountInfoCallback>())
                if (OnAccountInfoCallback != null) { OnAccountInfoCallback((SteamUser.AccountInfoCallback)msg); }
            if (msg.IsType<SteamUser.LoggedOffCallback>())
                if (OnLoggedOffCallback != null) { OnLoggedOffCallback((SteamUser.LoggedOffCallback)msg); }
            if (msg.IsType<SteamUser.LoggedOnCallback>())
                if (OnLoggedOnCallback != null) { OnLoggedOnCallback((SteamUser.LoggedOnCallback)msg); }
            if (msg.IsType<SteamUser.LoginKeyCallback>())
                if (OnLoginKeyCallback != null) { OnLoginKeyCallback((SteamUser.LoginKeyCallback)msg); }
            if (msg.IsType<SteamFriends.ChatActionResultCallback>())
                if (OnChatActionResultCallback != null) { OnChatActionResultCallback((SteamFriends.ChatActionResultCallback)msg); }
            if (msg.IsType<SteamFriends.ChatEnterCallback>())
                if (OnChatEnterCallback != null) { OnChatEnterCallback((SteamFriends.ChatEnterCallback)msg); }
            if (msg.IsType<SteamFriends.ChatInviteCallback>())
                if (OnChatInviteCallback != null) { OnChatInviteCallback((SteamFriends.ChatInviteCallback)msg); }
            if (msg.IsType<SteamFriends.ChatMemberInfoCallback>())
                if (OnChatMemberInfoCallback != null) { OnChatMemberInfoCallback((SteamFriends.ChatMemberInfoCallback)msg); }
            if (msg.IsType<SteamFriends.ChatMsgCallback>())
                if (OnChatMsgCallback != null) { OnChatMsgCallback((SteamFriends.ChatMsgCallback)msg); }
            if (msg.IsType<SteamFriends.PersonaStateCallback>())
                if (OnPersonaStateCallback != null) { OnPersonaStateCallback((SteamFriends.PersonaStateCallback)msg); }
        }

        public static void CallEvent_Log(string log) { if(OnLog != null) OnLog(log); }
    }
}
