using System;
using System.Diagnostics;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdAfk : Command
    {
        public CmdAfk() : base("afk") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            Chat.Send(room, Steam.Friends.GetFriendPersonaName(sender) + " is now AFK!");
        }

        /* We could possibly do with a method of hooking into the chat from here. */
        /* For this command probably check if the users name is mentioned. */
    }
}