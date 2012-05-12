using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdQuit : Command
    {
        public CmdQuit() : base("quit") { }

        public override void Use(SteamID Room, SteamID Sender, string[] args, bool isAdmin)
        {
            if (isAdmin)
            {
                Steam.Shutdown();
                Environment.Exit(20);
            }
            else
                Chat.Send(Room, "I can't let you do that " + Steam.Friends.GetFriendPersonaName(Sender) + "!");
        }
    }
}