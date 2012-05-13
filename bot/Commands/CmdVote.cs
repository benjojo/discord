using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SteamKit2;
using System.Linq;
namespace AgopBot.Commands
{
    public class VoteMaster
    {
        public static bool InVote { get; set; }
        public static Dictionary<SteamID, int> Votes = new Dictionary<SteamID, int>(); 
        public static string[] Answers; 

        public static void EndVote(object state)
        {
            InVote = false;

            Chat.Send((SteamID)state, "Vote has ended!");
        }

        public static void StartVote(SteamID room, SteamID sender, string question, string[] answers)
        {
            if (InVote)
            {
                Chat.Send(room, "Already in vote!");
                return;
            }

            InVote = true;
            Chat.Send(room, string.Format("A vote has been started by {0}!", Steam.Friends.GetFriendPersonaName(sender)));
            Chat.Send(room, string.Format("Vote: {0}", question));

            int i = 0;
            string answersNice = string.Join(", ", answers.Select(s => string.Format("[{0}] {1}", i++, s)));
            Chat.Send(room, string.Format("Possible answers: {0}", answersNice));

            int time = 90;
            Chat.Send(room, string.Format("Vote ends in {0} seconds. Use sudo vote to vote. Get going!", time));

            new Timer(EndVote, room, time * 1000, Timeout.Infinite);

            Answers = answers;
        }
    }
    public class CmdStartVote : Command
    {
        public CmdStartVote() : base("startvote") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            string arg = String.Join(" ", args);
            string[] argSplit = arg.Split('|');
            if (argSplit.Length < 3)
            {
                Chat.Send(room, "You did it wrong! Usage: sudo startvote Does this work?|Yes|No");
                return;
            }

            argSplit = argSplit.Select(s => s.Trim()).ToArray();
            string question = argSplit[0];
            string[] answers = argSplit.Skip(1).ToArray();

            VoteMaster.StartVote(room, sender, question, answers);
        }
    }

    public class CmdVote : Command
    {
        public CmdVote() : base("vote") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            if (VoteMaster.Votes.ContainsKey(sender))
            {
                Chat.Send(room, "You already voted!");
                return;
            }

            int vote;
            if (int.TryParse(string.Join(" ", args), out vote))
                VoteMaster.Votes[sender] = vote;
            else
                Chat.Send(room, "Invalid number!");

        }
    }
}