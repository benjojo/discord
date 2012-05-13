using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using SteamKit2;

namespace AgopBot.Commands
{
    public class CmdUpdate : Command
    {
        public CmdUpdate() : base("update") { }

        private volatile bool _busy;
        public override void Use(SteamID room, SteamID sender, string[] args)
        {
            Thread T = new Thread(() =>
                                      {
                                          if (_busy || !Util.IsAdmin(sender))
                                              return;

                                          _busy = true;
                                          Chat.Send(room, "Pulling... (skipping this for now)");

                                          //TODO: Run update.bat to pull changes!
                                          //Process.Start("update.bat");

                                          Chat.Send(room, "Recompiling...");

                                          ProcessStartInfo info = new ProcessStartInfo(string.Format(@"{0}\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe", Environment.GetEnvironmentVariable("SystemRoot"))); //That's the Windows folder
                                          info.RedirectStandardOutput = true;
                                          info.RedirectStandardError = true;
                                          info.UseShellExecute = false;
                                          info.Arguments = Configurator.Config.SolutionFile ?? @"..\..\..\bot.sln";

                                          Process p = Process.Start(info); //Start MSBuild.exe, which compiles the project file
                                          p.BeginOutputReadLine();
                                          p.BeginErrorReadLine();

                                          bool shouldOutput = false;
                                          List<string> outputs = new List<string>(); //List of every single output line from MSBuild.exe
                                          Action<string> output = message =>
                                                                      {
                                                                          if (String.IsNullOrWhiteSpace(message)) return;

                                                                          if (message.StartsWith("Done Building Project"))
                                                                              shouldOutput = true; //Only output stuff that comes AFTER "Done Building Project"

                                                                          if (shouldOutput)
                                                                          {
                                                                              Chat.Send(room, message); //This might be spammy
                                                                              outputs.Add(message);
                                                                          }
                                                                      };

                                          p.OutputDataReceived += (obj, e) => output(e.Data);
                                          p.ErrorDataReceived += (obj, e) => output(e.Data);
                                          p.WaitForExit(); //The magic of blocking threads which are not main threads...

                                          if (outputs.Any(s => s.Contains("Build FAILED.")))
                                          {
                                              Chat.Send(room, "Failure! Not restarting!");
                                              return;
                                          }

                                          Chat.Send(room, "Done! Be right back!");

                                          string fileName = AppDomain.CurrentDomain.FriendlyName.Replace(".vshost", ""); //Turns bot.vshost.exe into bot.exe
                                          Process.Start(fileName);

                                          _busy = false;
                                          Steam.Shutdown();
                                      });
            T.Start();
        }
    }
}