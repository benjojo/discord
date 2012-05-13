﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace AgopBot
{
    class ConfigDefinition
    {
        public string BotName { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthCode { get; set; }

        public bool EnableMySQL { get; set; }
        public string SolutionFile { get; set; }
    }

    class Configurator
    {
        public const string FileName = "config.json";
        public static ConfigDefinition Config { get; private set; }

        public static void Load()
        {
            try
            {
                Config = JsonConvert.DeserializeObject<ConfigDefinition>(File.ReadAllText(FileName));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("There was an error loading the config file:\n{0}: {1}\nA new configfile will be generated.", e.GetType(), e.Message));

                Config = new ConfigDefinition(); //Create blank config file

                using (StreamWriter sw = File.CreateText(FileName))
                    sw.Write(JsonConvert.SerializeObject(Config, Formatting.Indented)); //Write to config.json
            }
        }
    }
}
