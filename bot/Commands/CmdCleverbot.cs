using System;
using System.Threading;
using ChatterBotAPI;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdCleverbot : Command
    {
        public static ChatterBotFactory botFactory = new ChatterBotFactory();
        public static ChatterBot cleverBot = botFactory.Create(ChatterBotType.CLEVERBOT);
        public static ChatterBotSession cleverBotSession = cleverBot.CreateSession();

        public CmdCleverbot()
            : base("cb")
        {
        }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            Thread T = new Thread(() =>
                                      {
                                          string response = cleverBotSession.Think(string.Join(" ", args));
                                          Chat.Send(room, response);
                                      });

            T.Start();

            Chat.Send(room, "Awaiting CleverBot response...");
        }
    }
}