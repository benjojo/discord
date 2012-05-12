using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdKick : Command
    {
        public CmdKick() : base("kick") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            if (Util.IsAdmin(sender))
                Steam.Friends.KickChatMember(room, sender);
            else
                Chat.Send(room, "I can't let you do that " + Steam.Friends.GetFriendPersonaName(sender) + "!");
        }
    }
}