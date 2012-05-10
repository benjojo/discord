using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SteamKit2;

namespace AgopBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Steam.Username = args[0];
            Steam.Password = args[1];
            Steam.AuthCode = "";
            Steam.Connect();

            bool isComplete = false;

            Console.CancelKeyPress += (object s, ConsoleCancelEventArgs e) =>
            {
                isComplete = true;
            };

            while (!isComplete)
            {
                Steam.Update();
                Thread.Sleep(1);
            }

            Steam.Shutdown();
        }
    }
}