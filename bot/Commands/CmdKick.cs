using System;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdKick : Command
    {
        public CmdKick() : base("kick") { }

        public override void Use(SteamID room, SteamID sender, string[] args, bool isAdmin)
        {
            if (isAdmin)
            {
                //Steam.Friends.KickChatMember(room, sender);
                //For this to work, we need to keep a hashtable of all the people who have talked in the room/ joined the room and their SteamIDs,
                //so that we can kick/ban people by their names. Otherwise, we'll have to pass the persons SteamID.
            }
            else
                Chat.Send(room, "nope.avi");
        }
    }
}