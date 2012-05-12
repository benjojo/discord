using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdLeave : Command
    {
        public CmdLeave() : base("leave") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            if (Util.IsAdmin(sender))
                Steam.Friends.LeaveChat(room);
            else
                Chat.Send(room, "I can't let you do that " + Steam.Friends.GetFriendPersonaName(sender) + "!");
        }
    }
}