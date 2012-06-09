using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdYoutube : Command
    {
        public CmdYoutube() : base("yt") { }

        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            string subcommand = args[0].ToLower();
            string arg = String.Join(" ", args.Skip(1));

            switch (subcommand)
            {
                case "search":
                    Search(room, arg); break;
            }
        }

        private void Search(SteamID room, string text)
        {
            if (String.IsNullOrWhiteSpace(text))
                return;

            string query = string.Format("https://gdata.youtube.com/feeds/api/videos?q={0}", text);
            string response = new WebClient().DownloadString(query);

            //Use Regex to find youtube url
            Regex r = new Regex(@"https://www\.youtube\.com/watch\?v=.+?&");
            var match = r.Match(response);
            Chat.Send(room, "Video: " + match.Value);

            /*XmlDocument x = new XmlDocument();
            x.Load(query);

            XmlNodeList entries = x.SelectNodes("feed");
            foreach (XmlNode node in entries)
            {
                Chat.Send(room, string.Format("Video: '{0}' by {1} - {2}", node.SelectSingleNode("title").InnerText, node.SelectSingleNode("author").InnerText, node.SelectSingleNode("link").InnerText));
            }*/

        }
    }
}