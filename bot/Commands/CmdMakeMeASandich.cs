using System;
using SteamKit2;

namespace AgopBot
{
    public class CmdMakeMeASandwich : Command
    {
        public CmdMakeMeASandwich() : base("make") { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            Chat.Send(Room, "Okay.");
        }
    }
}