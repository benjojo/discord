using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    internal static class ChatCommands
    {
        private static List<Command> commands = new List<Command>();
        private static System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

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

            if (command != null)
            {
                stopwatch.Start();
                command.Use(room, sender, args.Skip(1).ToArray());
                Console.WriteLine("Command \"" + command.Name + "\" took " + stopwatch.ElapsedMilliseconds + " ms to execute.");
                stopwatch.Reset();
            }
        }

        public static void InitAll()
        {
            //SCREW THIS SHIT LETS USE REFLECTION (http://stackoverflow.com/questions/79693/getting-all-types-in-a-namespace-via-reflection)

            string cmdNamespace = "AgopBot.Commands";

            var q = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == cmdNamespace
                    select t;

            List<Type> commandTypes = q.ToList(); //This list contains every class in the AgopBot.Commands namespace

            foreach (Type t in commandTypes)
            {
                if (t.BaseType == typeof(Command)) //Might include some rubbish classes
                {
                    Add((Command)Activator.CreateInstance(t));
                    Console.WriteLine(string.Format("Added command {0}", t));
                }
            }
        }
    }
}