using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace discord.plugins
{
    public interface IPlugin
    {
        string Name { get; }
        string Desc { get; }
        string Auth { get; }

        void Load();
        void Unload();
    }

    public static class PluginLoader
    {
        public static void Load(string file)
        {
            string filepath = Path.Combine(discord.core.Settings.Instance.PluginPath, file);
            string cachefilepath = Path.Combine(discord.core.Settings.Instance.PluginPath, "cache", file);
            string cachepath = Path.Combine(discord.core.Settings.Instance.PluginPath, "cache");
            if (!File.Exists(filepath)) {
                Console.WriteLine(string.Format("{0} - doesn't exist.", filepath));
                return;
            }

            if (!Directory.Exists(discord.core.Settings.Instance.PluginPath))
                Directory.CreateDirectory(discord.core.Settings.Instance.PluginPath);

            if (!Directory.Exists(cachepath))
                Directory.CreateDirectory(cachepath);

            if (File.Exists(cachefilepath)) {
                if (!FilesMatch(cachefilepath, filepath))
                    File.Copy(filepath, cachefilepath, true);
            } else
                File.Copy(filepath, cachefilepath);

            try {
                Assembly asm = Assembly.LoadFrom(cachefilepath);

                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsClass)
                    {
                        if (type.GetInterface("discord.plugins.IPlugin") == null)
                            continue;

                        object ibaseObject = Activator.CreateInstance(type);
                        IPlugin plugin = (IPlugin)ibaseObject;
                        plugin.Load();

                        Console.WriteLine(string.Format("Loaded Plugin '{0}' by {1}", plugin.Name, plugin.Auth));
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private static FileSystemWatcher watcher;
        public static void WatchDirectory(string path)
        {
            watcher = new FileSystemWatcher();
            watcher.Filter = "*.dll";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Path = path;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;
        }

        static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            try {
                watcher.EnableRaisingEvents = false;
                Console.WriteLine(string.Format("{0} {1} {2}", e.Name, e.FullPath, e.ChangeType));
                string cachefilepath = Path.Combine(discord.core.Settings.Instance.PluginPath, "cache", e.Name);

                if (!File.Exists(cachefilepath))
                    Console.WriteLine(string.Format("New Plugin {0}, will load!", e.Name));
                else
                {
                    if (!FilesMatch(e.FullPath, cachefilepath))
                        Console.WriteLine(string.Format("Plugin {0} has been replaced, will reload!", e.Name));
                }
            }

            finally { watcher.EnableRaisingEvents = true; }
        }

        private static bool FilesMatch(string file1, string file2)
        {
            if (!File.Exists(file1) || !File.Exists(file2))
                return false;

            FileInfo fileinfo1 = new FileInfo(file1);
            FileInfo fileinfo2 = new FileInfo(file2);

            if (fileinfo1.Length != fileinfo2.Length)
                return false;

            // gotta try wrap this, if anything messes up, it gets real mad.
            try
            {
                HashAlgorithm hash = HashAlgorithm.Create();
                byte[] fileHash1, fileHash2;
                using (FileStream fileStream1 = new FileStream(file1, FileMode.Open, FileAccess.Read),
                                  fileStream2 = new FileStream(file2, FileMode.Open, FileAccess.Read))
                {
                    fileHash1 = hash.ComputeHash(fileStream1);
                    fileHash2 = hash.ComputeHash(fileStream2);
                }

                return fileHash1.SequenceEqual(fileHash2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }
    }
}