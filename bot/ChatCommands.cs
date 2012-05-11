using System;
using System.Collections.Generic;
using SteamKit2;

namespace AgopBot
{
    public class Command
    {
        public string Name { get { return strName; } }

        protected string strName;

        public bool isAdmin; //someone encapsulate this im shit at that

        public Command(string name)
        {
            strName = name;
        }

        public virtual void Use(SteamID Room, SteamID Sender, string[] args, bool isAdmin)
        {
            Chat.Send(Room, "Sorry '" + Steam.Friends.GetFriendPersonaName(Sender) + "'! The command '" + args[1] + "' has no use.");
        }
    }

    internal static class ChatCommands
    {
        static List<Command> commands = new List<Command>();

        public static void Add(Command cmd)
        {
            commands.Add(cmd);
        }

        public static Command Find(string name)
        {
            name = name.ToLower();
            foreach (Command cmd in commands)
                if (cmd.Name == name)
                    return cmd;
            return null;
        }

        public static void HandleChatCommand(SteamID Room, SteamID Sender, string[] args, bool isAdmin)
        {
            Command command = Find(args[1]);

            if (command == null)
                Chat.Send(Room, "Sorry '" + Steam.Friends.GetFriendPersonaName(Sender) + "'! The command '" + args[1] + "' is unrecognized.");
            else
                command.Use(Room, Sender, args, isAdmin);
        }

        public static void InitAll()
        {
            Add(new CmdAfk("afk"));
            Add(new CmdInfo("info"));
            Add(new CmdTime("time"));
            Add(new CmdQuit("quit"));
            Add(new CmdQuit("rm -rf /"));
            Add(new CmdSteamID("id"));
        }
    }
}