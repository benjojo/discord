using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdQuit : Command
    {
        public CmdQuit() : base("quit") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            if (sender.AccountID != 36593391)
                Chat.Send(room, "I can't let you do that " + sender.AccountID + "!");
            else
                Steam.Shutdown();
        }
    }
}
