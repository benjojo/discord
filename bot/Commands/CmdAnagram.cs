using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdAnagram : Command
    {
        public CmdAnagram() : base("anagram")
        {
        }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            new Thread(() =>
                           {
                               string arg = String.Join(" ", args);
                               var anagrams = Show(Read(), arg);

                               Chat.Send(room, string.Format("Anagrams for {0}: {1}", arg, anagrams));

                           }).Start();
        }

        private static Dictionary<string, string> Read() //Literally ripped from http://www.dotnetperls.com/anagram
        {
            var d = new Dictionary<string, string>();
            // Read each line
            using (StreamReader r = new StreamReader("enable1.txt"))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    // Alphabetize the line for the key
                    // Then add to the value string
                    string a = Alphabetize(line);
                    string v;
                    if (d.TryGetValue(a, out v))
                        d[a] = v + ", " + line;
                    else
                        d.Add(a, line);
                }
            }
            return d;
        }

        private static string Alphabetize(string s)
        {
            // Convert to char array, then sort and return
            char[] a = s.ToCharArray();
            Array.Sort(a);
            return new string(a);
        }

        private static string Show(Dictionary<string, string> d, string w)
        {
            // Write value for alphabetized word
            string v;
            return d.TryGetValue(Alphabetize(w), out v) ? v : null;
        }
    }
}