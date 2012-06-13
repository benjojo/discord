using System;
using System.Threading;

namespace discord
{
    static class Program
    {
        public static core.Discord discord;

        [STAThread]
        static void Main(string[] args)
        {
            discord = new core.Discord();

            Thread listenThread = new Thread(new ThreadStart(ConsoleListen));
            listenThread.IsBackground = true;
            listenThread.Start();

            while (!discord.StopLoop)
            {
                discord.Update();
                Thread.Sleep(1);
            }
        }

        static void ConsoleListen()
        {
            string text = "";
            while (true)
            {
                text = Console.ReadLine();
                discord.Say(text);
            }
        }
    }
}
