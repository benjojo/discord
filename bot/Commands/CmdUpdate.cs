using System;
using System.Diagnostics;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdUpdate : Command
    {
        public CmdUpdate() : base("update") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            Process.Start("update.bat");

            Chat.Send(room, "Updating, be right back!");
            Steam.Shutdown();
        }
    }
}