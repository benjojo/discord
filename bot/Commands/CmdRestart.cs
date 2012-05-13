using System;
using System.Diagnostics;
using System.Net.Mime;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdRestart : Command
    {
        public CmdRestart() : base("restart") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            Steam.Shutdown();

            string fileName = AppDomain.CurrentDomain.FriendlyName.Replace(".vshost", ""); //Turns bot.vshost.exe into bot.exe
            Process.Start(fileName);
        }
    }
}