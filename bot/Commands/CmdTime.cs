using System;
using SteamKit2;

namespace AgopBot
{
    public class CmdTime : Command
    {
        public CmdTime(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            Chat.Send(Room, "The time is: " + DateTime.UtcNow.ToShortTimeString() + " (UTC/GMT)");
        }
    }
}
