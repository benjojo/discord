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
            SQL.DB.Initialize("localhost", "agop", "root", "root");
          
            Steam.Username = "TF2GUY1001";//args[0];
            Steam.Password = "TF2GUY1996";//args[1];
            Steam.AuthCode = "";
            Steam.Connect();

            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += (object s, ConsoleCancelEventArgs e) =>
            {
                Steam.Shutdown();
            };

            while (!Steam.ShouldQuit)
            {
                Steam.Update();
                Thread.Sleep(1);
            }
        }
    }
}