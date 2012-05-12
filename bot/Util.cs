using System;
using System.Collections.Generic;
using SteamKit2;

namespace AgopBot
{
    static class Util
    {
        public static List<SteamID> Admins = new List<SteamID>
        {
            new SteamID("STEAM_0:0:32616431"), //Naarkie
            new SteamID("STEAM_0:1:18296695"), //Matt
            new SteamID("STEAM_0:0:15033805") //Dlaor
            //Blk, Benjojo and Austech, fill yourselves in or send me your SteamIDs ("sudo id" in chat)
        };

        public static bool IsAdmin(SteamID user)
        {
            return Admins.Contains(user);
        }

        public static SteamID GetSteamIDFromName(SteamID room, string name)
        {
            return null;
        }
    }
}
