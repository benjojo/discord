using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdQuit : Command
    {
        public CmdQuit() : base("quit") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            if (Util.IsAdmin(sender))
            {
                Steam.Shutdown();
                Environment.Exit(20);
            }
            else
                Chat.Send(room, "I can't let you do that " + Steam.Friends.GetFriendPersonaName(sender) + "!");
        }
    }
}