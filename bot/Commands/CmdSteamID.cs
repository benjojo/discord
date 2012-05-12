using System;
using SteamKit2;

namespace AgopBot
{
    public class CmdSteamID : Command
    {
        public CmdSteamID(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args, bool isAdmin)
        {
            Chat.Send(Room, Steam.Friends.GetFriendPersonaName(Sender) + ": " + Sender.AccountID.ToString());
        }
    }
}