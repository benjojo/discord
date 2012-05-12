using System;
using System.Diagnostics;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdInfo : Command
    {
        public CmdInfo() : base("info") { }

        public override void Use(SteamID room, SteamID Sender, string[] args, bool isAdmin)
        {
            TimeSpan tp = Process.GetCurrentProcess().TotalProcessorTime;
            TimeSpan up = (DateTime.Now - Process.GetCurrentProcess().StartTime);

            int megabytes = (int)Math.Round(Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024f);
            string MemoryUsage = "Memory Usage: " + megabytes.ToString() + " Megabytes";
            string Uptime = "Uptime: " + up.Days + " Days " + up.Hours + " Hours " + up.Minutes + " Minutes " + up.Seconds + " Seconds";

            Chat.Send(room, "~~ AgopBot ~~");
            Chat.Send(room, "Contribute here: https://github.com/iRzilla/AgopBot");
            Chat.Send(room, Uptime);
            Chat.Send(room, MemoryUsage);
        }
    }
}