using System.IO;

namespace discord.core
{
    public static class Logger
    {
        private static readonly object logSync = new object();

        private static void CreateDirIfNotExists(string filePath)
        {
            string path = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public static void Write(string data, string fileName)
        {
            lock (logSync)
            {
                CreateDirIfNotExists(fileName);

                bool fileInUse = true;
                while (fileInUse)
                    try
                    {
                        using (StreamWriter log = File.AppendText(fileName))
                            log.WriteLine(data);
                        fileInUse = false;
                    } catch (System.IO.IOException) { fileInUse = true; }
            }
        }

        public static string ReadFileToString(string fileName)
        {
            string logText = "";

            lock (logSync)
            {
                bool fileInUse = true;
                while (fileInUse)
                    try {
                        using (StreamReader reader = new StreamReader(fileName))
                            logText = reader.ReadToEnd();

                        fileInUse = false; 
                    } catch (System.IO.IOException ex) { fileInUse = true; }
            }

            return logText;
        }
    }
}