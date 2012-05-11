using System;
using System.Threading;

namespace AgopBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Configurator.Load();
            
            SQL.DB.Initialize("localhost", "agop", "root", "root");
          
            Steam.Username = Configurator.Config.Username;
            Steam.Password = Configurator.Config.Password;
            Steam.AuthCode = Configurator.Config.AuthCode;
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