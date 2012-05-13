using System;
using System.Text;
using System.Threading;

namespace AgopBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Configurator.Load();
            
            if (Configurator.Config.EnableMySQL)
                SQL.DB.Initialize("localhost", "agop", "root", "root");
          
            Steam.Username = Configurator.Config.Username;
            Steam.Password = Encoding.UTF8.GetString(Convert.FromBase64String(Configurator.Config.Password)); //Password is stored in Base64
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