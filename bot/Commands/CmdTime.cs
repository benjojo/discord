using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdTime : Command
    {
        public CmdTime() : base("time") { }

        public override void Use(SteamID room, SteamID Sender, string[] args)
        {
            Chat.Send(room, "The time is: " + DateTime.UtcNow.ToShortTimeString() + " (UTC)");
        }
    }
}