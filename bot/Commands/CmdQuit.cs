using System;
using SteamKit2;

namespace AgopBot
{
    public class CmdQuit : Command
    {
        public CmdQuit(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            Steam.Shutdown();
        }
    }
}
