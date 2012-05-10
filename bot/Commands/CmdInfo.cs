using System;
using System.Diagnostics;
using SteamKit2;

namespace AgopBot
{
    public class CmdInfo : Command
    {
        public CmdInfo(string name) : base(name) { }

        public override void Use(SteamID Room, SteamID Sender, string[] args)
        {
            TimeSpan tp = Process.GetCurrentProcess().TotalProcessorTime;
            TimeSpan up = (DateTime.Now - Process.GetCurrentProcess().StartTime);

            string MemoryUsage = "Memory Usage: " + Math.Round((double)Process.GetCurrentProcess().PrivateMemorySize64 / 1048576).ToString() + " Megabytes";
            string Uptime = "Uptime: " + up.Days + " Days " + up.Hours + " Hours " + up.Minutes + " Minutes " + up.Seconds + " Seconds";

            Chat.Send(Room, "~~ AgopBot ~~");
            Chat.Send(Room, "Contribute here: https://github.com/iRzilla/AgopBot");
            Chat.Send(Room, Uptime);
            Chat.Send(Room, MemoryUsage);
        }
    }
}
