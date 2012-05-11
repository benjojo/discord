using System;
using SteamKit2;

namespace AgopBot
{
    public class CmdQuit : Command
    {
        public CmdQuit(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args, bool isAdmin)
        {
            if (isAdmin)
                Chat.Send(Room, "I can't let you do that " + Steam.Friends.GetFriendPersonaName(Sender) + "!");
            else
            {
                Steam.Shutdown();
                Environment.Exit(20);
            }
        }
    }
}