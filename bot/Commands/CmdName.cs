using System;
using System.Diagnostics;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdName : Command
    {
        public CmdName() : base("name") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            string arg = String.Join("", args);

            Steam.Friends.SetPersonaName(arg);
            Chat.Send(room, string.Format("Name is now {0}.", arg));
        }
    }
}