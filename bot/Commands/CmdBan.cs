using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdBan : Command
    {
        public CmdBan() : base("ban") { }

        public override void Use(SteamID room, SteamID sender, string[] args, bool isAdmin)
        {
            if (isAdmin)
            {
                //Steam.Friends.BanChatMember(room, sender);
                //See Line15 CmdKick.cs
            }
            else
                Chat.Send(room, "nope.avi");
        }
    }
}