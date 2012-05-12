using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdDefine : Command
    {
        public CmdDefine() : base("define") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            string word = String.Join(" ", args);
            Chat.Send(room, string.Format("Looking up '{0}', one second...", word));

            Thread T = new Thread(() => Chat.Send(room, Lookup(word)));
            T.Start();
        }

        public static string Lookup(string s)
        {
            string safeString = MakeSafeString(s);
            string url = string.Format("http://www.google.com/dictionary/json?callback=dict_api.callbacks.id100&q={0}&sl=en&tl=en&restrict=pr%2Cde&client=te", safeString);
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);

            string json = reader.ReadToEnd();
            json = json.Replace("dict_api.callbacks.id100(", "");
            json = json.Replace(",200,null)", "");

            json = json.Replace(@"\x3c", "<");
            json = json.Replace(@"\x3e", ">");
            json = json.Replace(@"\x3d", "=");
            json = json.Replace(@"\x22", "\""); //Actually " but whatever
            json = json.Replace(@"\x26", "&");
            json = json.Replace(@"\x27", "'");
            json = json.Replace(@"\xad", "");
            json = json.Replace(@"&quot;", "\""); //Actually " but whatever
            json = json.Replace(@"&#39;", "\""); //Actually " but whatever

            if (!json.Contains("Invalid query:"))
            {
                var sr = new StringReader(json);
                var jsonReader = new JsonTextReader(sr);

                bool b = jsonReader.Read();
                while (b)
                {
                    var currentValue = jsonReader.Value;

                    if (currentValue != null)
                    {
                        string currentVal = currentValue.ToString();
                        if (jsonReader.TokenType == JsonToken.String && currentVal[currentVal.Length - 1] == '.')
                            return string.Format("Defintion of the word '{0}': ", Uri.UnescapeDataString(s)) + currentValue;
                    }

                    b = jsonReader.Read();
                }
            }

            return "Failed dictionary lookup!";
        }

        private static string MakeSafeString(string s)
        {
            string safeChars = "abcdefghijklmnopqrstuvwxyz0123456789'-";
            return new string(s.Where(safeChars.Contains).ToArray());
        }
    }
}