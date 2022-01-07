using System;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;

namespace PriorityChatV2
{
    class ConfigManager
    {
        private static Config config = null;
        private static JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        public static string pathConfig = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Daniel2193\\PriorityChat\\configChat.json";
        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Daniel2193\\PriorityChat\\";
        public static Config getConfig()
        {
            return config;
        }
        public static void saveConfig()
        {
            string jsonConfig = JsonSerializer.Serialize(config, options);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllText(pathConfig, jsonConfig);
        }
        public static void setup()
        {
            try
            {
                if (!Directory.Exists(path.Replace("PriorityChat\\", "")))
                {
                    Directory.CreateDirectory(path.Replace("PriorityChat\\", ""));
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllText(path + ".version.bin", FormChat.version);
                if (File.Exists(pathConfig))
                {
                    string jsonConfig = File.ReadAllText(pathConfig);
                    config = JsonSerializer.Deserialize<Config>(jsonConfig, options);
                    config.colorMessages = Color.FromArgb(config.colorMessagesR, config.colorMessagesG, config.colorMessagesB);
                    config.colorMessagesRead = Color.FromArgb(config.colorMessagesReadR, config.colorMessagesReadG, config.colorMessagesReadB);
                    config.colorUsername = Color.FromArgb(config.colorUsernameR, config.colorUsernameG, config.colorUsernameB);
                }
                else
                {
                    config = new Config();
                    saveConfig();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in setup: \n" + e.Message);
            }
        }
        public static void updateConfig(string ip, int port, string username, bool showNotifications, Color colorMessages, Color colorMessagesRead, Color colorUsername, bool sendOnEnter, int emoteScale)
        {
            config.ip = ip;
            config.port = port;
            config.username = username;
            config.showNotifications = showNotifications;
            config.colorMessages = colorMessages;
            config.colorMessagesRead = colorMessagesRead;
            config.colorUsername = colorUsername;
            config.sendOnEnter = sendOnEnter;
            config.emoteScale = emoteScale;
            config.colorMessagesR = colorMessages.R;
            config.colorMessagesG = colorMessages.G;
            config.colorMessagesB = colorMessages.B;
            config.colorMessagesReadR = colorMessagesRead.R;
            config.colorMessagesReadG = colorMessagesRead.G;
            config.colorMessagesReadB = colorMessagesRead.B;
            config.colorUsernameR = colorUsername.R;
            config.colorUsernameG = colorUsername.G;
            config.colorUsernameB = colorUsername.B;
            UserManager.UpdateColor(username, colorUsername);
            saveConfig();
        }
    }
}
