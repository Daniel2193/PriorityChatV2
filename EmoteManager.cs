using System.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PriorityChatV2
{
    class EmoteManager
    {
        //emoteName, filePath
        private static Dictionary<string, string> emotes = new Dictionary<string, string>();
        public static void AddEmote(string emoteName, string filePath)
        {
            if(!emotes.ContainsKey(emoteName))
            {
                emotes.Add(emoteName, filePath);
            }
        }
        public static void loadEmotes(){
            emotes.Clear();
            if(!Directory.Exists(ConfigManager.path + "\\emotes\\"))
            {
                return;
            }
            string[] files = Directory.GetFiles(ConfigManager.path + "\\emotes\\");
            foreach(string file in files)
            {
                string[] fileSplit = file.Split('\\');
                string fileName = fileSplit[fileSplit.Length - 1];
                string[] fileNameSplit = fileName.Split('.');
                string emoteName = fileNameSplit[0];
                AddEmote(emoteName, file);
            }
        }
        public static Bitmap GetEmote(string emoteName)
        {
            if(emotes.ContainsKey(emoteName))
            {
                foreach(string path in Directory.GetFiles(ConfigManager.path + "\\emotes\\"))
                {
                    if(path.Contains(emoteName))
                    {
                        return new Bitmap(path);
                    }
                }
            }
            return null;
        }
        public static string GetEmoteAsHexString(string emoteName)
        {
            if(emotes.ContainsKey(emoteName))
            {
                MemoryStream ms = new MemoryStream();
                Image img = Image.FromFile(emotes[emoteName]);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                return BitConverter.ToString(imageBytes).Replace("-", "");
            }
            return emoteName;
        }
        public static string[] GetEmotes()
        {
            return emotes.Keys.ToArray();
        }
        public static string GetEmotePath(string emoteName)
        {
            if(emotes.ContainsKey(emoteName))
            {
                return emotes[emoteName];
            }
            return null;
        }
    }
}
