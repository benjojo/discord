using System;
using System.Threading;

namespace AgopBot
{
    class Program
    {
        static void Main(string[] args)
        {
            SQL.DB.Initialize("localhost", "agop", "root", "root");
          
            Steam.Username = args[0];
            Steam.Password = args[1];
            Steam.AuthCode = "";
            Steam.Connect();

            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += (s, e) => Steam.Shutdown();

            while (!Steam.ShouldQuit)
            {
                Steam.Update();
                Thread.Sleep(1);
            }
        }
    }
}