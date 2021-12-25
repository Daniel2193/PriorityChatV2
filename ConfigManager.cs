using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;

namespace PriorityChatV2
{
    class ConfigManager
    {
        private static Config config = null;
        private static JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        public static string pathConfig = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Daniel2193\\PriorityChat\\configChat.json";

        public static Config getConfig()
        {
            return config;
        }

        public static void saveConfig()
        {
            string jsonConfig = JsonSerializer.Serialize(config, options);
            File.WriteAllText(pathConfig, jsonConfig);
        }

        public static void readConfig()
        {
            if (File.Exists(pathConfig))
            {
                string jsonConfig = File.ReadAllText(pathConfig);
                config = JsonSerializer.Deserialize<Config>(jsonConfig, options);
            }
            else
            {
                config = new Config();
                saveConfig();
            }
        }

        public static void updateConfig(string ip, int port, string username, bool showNotifications, Color colorMessages, Color colorMessagesRead, bool sendOnEnter = false)
        {
            config.ip = ip;
            config.port = port;
            config.username = username;
            config.showNotifications = showNotifications;
            config.colorMessages = colorMessages;
            config.colorMessagesRead = colorMessagesRead;
            config.sendOnEnter = sendOnEnter;
            saveConfig();
        }
    }
}
