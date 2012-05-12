using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdJoin : Command
    {
        public CmdJoin() : base("join") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            if (Util.IsAdmin(sender))
                switch (args[0])
                {
                    case "fpp":
                        Chat.Join(103582791430091926);
                        break;
                    case "bfpp":
                        Chat.Join(103582791433166824);
                        break;
                    default:
                        Chat.Join(new SteamID(args[0]));
                        break;
                }
            else
                Chat.Send(room, "I can't let you do that " + Steam.Friends.GetFriendPersonaName(sender) + "!");
        }
    }
}