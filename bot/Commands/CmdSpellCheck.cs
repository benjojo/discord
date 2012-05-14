using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdSpellCheck : Command
    {
        public CmdSpellCheck() : base("spell") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            new Thread(() =>
                           {
                               string arg = String.Join(" ", args);

                               var distances = new List<Tuple<int, string>>();
                               foreach (string s in File.ReadAllLines("enable1.txt"))
                               {
                                   int dist = Compute(arg, s);

                                   if (dist == 0)
                                   {
                                       Chat.Send(room, "This word isn't misspelled!");
                                       return;
                                   }

                                   distances.Add(new Tuple<int, string>(dist, s));
                               }

                               distances.Sort((t1, t2) => t1.Item1.CompareTo(t2.Item1));

                               Chat.Send(room, string.Format("Did you mean: {0}?", string.Join(", ", distances.Take(4))));

                           }).Start();
        }

        public static int Compute(string stringA, string stringB) //See http://www.dotnetperls.com/levenshtein, anyway this is some pretty horrible code!
        {
            int n = stringA.Length;
            int m = stringB.Length;
            int[,] matrix = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; matrix[i, 0] = i++) { }
            for (int j = 0; j <= m; matrix[0, j] = j++) { }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (stringB[j - 1] == stringA[i - 1]) ? 0 : 1;

                    // Step 6
                    matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), matrix[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return matrix[n, m];
        }
    }
}