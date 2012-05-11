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

        public Command(string name)
        {
            Name = name;
        }

        public virtual void Use(SteamID room, SteamID sender, string[] args)
        {
            Chat.Send(room, "Sorry '" + Steam.Friends.GetFriendPersonaName(sender) + "'! The command '" + args[1] + "' has no use.");
        }
    }

    static class ChatCommands
    {
        static List<Command> commands = new List<Command>();

        public static void Add(Command cmd)
        {
            commands.Add(cmd);
        }

        public static Command Find(string name)
        {
            name = name.ToLower();
            return commands.FirstOrDefault(cmd => cmd.Name == name);
        }

        public static void HandleChatCommand(SteamID room, SteamID sender, string[] args)
        {
            Command command = Find(args[0]);

            if (command == null)
                Chat.Send(room, "Sorry '" + Steam.Friends.GetFriendPersonaName(sender) + "'! The command '" + args[0] + "' is unrecognized.");
            else
                command.Use(room, sender, args.Skip(1).ToArray());
        }

        public static void InitAll()
        {
            Add(new CmdAfk());
            Add(new CmdInfo());
            Add(new CmdTime());
            Add(new CmdQuit());
            Add(new CmdPing());
        }
    }
}
