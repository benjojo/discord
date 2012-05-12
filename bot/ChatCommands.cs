using System;
using System.Collections.Generic;
using System.Linq;
using AgopBot.Commands;
using SteamKit2;

namespace AgopBot
{
    public class Command
    {
        public string Name { get; private set; }

        public bool isAdmin; //someone encapsulate this im shit at that

        public Command(string name)
        {
            Name = name;
        }

        public virtual void Use(SteamID room, SteamID sender, string[] args, bool isAdmin)
        {
            Chat.Send(room, "Sorry '" + Steam.Friends.GetFriendPersonaName(sender) + "'! The command '" + args[1] + "' has no use.");
        }
    }

    internal static class ChatCommands
    {
        static List<Command> commands = new List<Command>();
        static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public static void Add(Command cmd)
        {
            commands.Add(cmd);
        }

        public static Command Find(string name)
        {
            name = name.ToLower();
            return commands.FirstOrDefault(cmd => cmd.Name == name);
        }

        public static void HandleChatCommand(SteamID room, SteamID sender, string[] args, bool isAdmin)
        {
            Command command = Find(args[0]);

            if (command == null)
                Chat.Send(room, "Sorry '" + Steam.Friends.GetFriendPersonaName(sender) + "'! The command '" + args[0] + "' is unrecognized.");
            else
            {
                stopwatch.Start();
                command.Use(room, sender, args.Skip(1).ToArray(), isAdmin);
                Console.WriteLine("Command \"" + command.Name + "\" took " + stopwatch.ElapsedMilliseconds + " ms to execute.");
                stopwatch.Reset();
            }
        }

        public static void InitAll()
        {
            Add(new CmdAfk());
            Add(new CmdInfo());
            Add(new CmdTime());
            Add(new CmdQuit());
            Add(new CmdPing());
            Add(new CmdKick());
            Add(new CmdBan());
            Add(new CmdJoin());
            Add(new CmdSteamID("id"));
            Add(new CmdCleverbot());
            Add(new CmdMakeMeASandwich());
        }
    }
}