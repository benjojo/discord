using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using AgopBot;
using SteamKit2;

namespace bot.Commands
{
    class CmdPing : Command
    {
        public CmdPing(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(string.Join(" ", args));

                if (reply != null)
                {
                    if (reply.Status == IPStatus.Success)
                        Chat.Send(Room, string.Format("Address [{0}] responded in {1} ms!", reply.Address, reply.RoundtripTime));
                    else
                        Chat.Send(Room, string.Format("Error pinging [{0}]: {1}", reply.Address, reply.Status));
                }
            }
            catch
            {
                Chat.Send(Room, "Messed up!");
            }
        }
    }
}
