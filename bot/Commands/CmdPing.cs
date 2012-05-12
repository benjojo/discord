using System.Net.NetworkInformation;
using SteamKit2;

namespace AgopBot.Commands
{
    internal class CmdPing : Command
    {
        public CmdPing() : base("ping") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            try
            {
                Ping pingSender = new Ping();
                string constring = string.Join(" ", args);
                System.Console.WriteLine(constring);
                PingReply reply = pingSender.Send(constring);

                if (reply != null)
                {
                    if (reply.Status == IPStatus.Success)
                        Chat.Send(room, string.Format("Address [{0}] responded in {1} ms!", reply.Address, reply.RoundtripTime));
                    else
                        Chat.Send(room, string.Format("Error pinging [{0}]: {1}", reply.Address, reply.Status));
                }
            }
            catch (System.Exception ex)
            {
                Chat.Send(room, "Messed up! [ " + ex.Message + " ]");
            }
        }
    }
}