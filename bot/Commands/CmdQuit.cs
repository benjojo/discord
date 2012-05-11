using System;
using SteamKit2;

namespace AgopBot
{
    public class CmdQuit : Command
    {
        public CmdQuit(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            if (Sender.AccountID != 18296695)
                Chat.Send(Room, "I can't let you do that " + Sender.AccountID + "!");
            else
                Steam.Shutdown();
        }
    }
}
