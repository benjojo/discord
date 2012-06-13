using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace discord.core
{
    [Serializable()]
    public class Settings
    {
        #region Singleton rubbish
        static Settings instance = null;
        static readonly object padlock = new object();

        Settings()
        {
            string path = Path.GetDirectoryName(DataPath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static Settings Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new Settings();
                    return instance;
                }
            }
        }

        public static void Assign(Settings settings)
        {
            lock (padlock)
                instance = settings;
        }
        #endregion Singleton rubbish

        public static bool LoadXML()
        {
            if (!File.Exists("settings.xml"))
                return false;

            XmlSerializer deserializer = new XmlSerializer(Settings.Instance.GetType());
            StreamReader streamReader = new StreamReader("settings.xml");
            Settings.Assign((Settings)deserializer.Deserialize(streamReader));
            streamReader.Close();

            return true;
        }

        public static void SaveXML()
        {
            XmlSerializer serializer = new XmlSerializer(Settings.Instance.GetType());
            StreamWriter streamWriter = new StreamWriter("settings.xml");
            serializer.Serialize(streamWriter, Settings.Instance);
            streamWriter.Close();
        }

        public string Username = "username";
        public string Password = "password";
        public string Nick     = "Discord";
        public List<string> IgnoreList = new List<string>();

        [XmlIgnore]
        public static string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        [XmlIgnore]
        public string PluginPath = Path.Combine(AppPath, "plugins");
        [XmlIgnore]
        public string DataPath = Path.Combine(AppPath, "data");
        [XmlIgnore]
        public string LogPath = Path.Combine(AppPath, "logs");
        [XmlIgnore]
        public string ErrorFile = Path.Combine(AppPath, string.Format("logs{0}errors.txt", Path.DirectorySeparatorChar));
    }
}