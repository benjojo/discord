using System;
using System.Diagnostics;
using SteamKit2;

namespace AgopBot
{
    public class CmdAfk : Command
    {
        public CmdAfk(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args, bool isAdmin)
        {
            Chat.Send(Room, Steam.Friends.GetFriendPersonaName(Sender) + " is now AFK!");
        }

        /* We could possibly do with a method of hooking into the chat from here. */
        /* For this command probably check if the users name is mentioned. */
    }
}