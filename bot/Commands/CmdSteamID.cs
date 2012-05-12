using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdSteamID : Command
    {
        public CmdSteamID() : base("id") { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            Chat.Send(Room, Steam.Friends.GetFriendPersonaName(Sender) + ": " + Sender.AccountID.ToString());
        }
    }
}