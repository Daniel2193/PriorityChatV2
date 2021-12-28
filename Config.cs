using System.Drawing;

namespace PriorityChatV2
{
    public class Config
    {
        public string ip { get; set; } = "192.168.191.248";
        public int port { get; set; } = 21930;
        public string username { get; set; } = "user";
        public bool showNotifications { get; set; } = false;
        public int colorMessagesR { get; set; } = 255;
        public int colorMessagesG { get; set; } = 255;
        public int colorMessagesB { get; set; } = 255;
        public Color colorMessages { get; set; } = Color.FromArgb(255, 255, 255);
        public int colorMessagesReadR { get; set; } = 150;
        public int colorMessagesReadG { get; set; } = 150;
        public int colorMessagesReadB { get; set; } = 150;
        public Color colorMessagesRead { get; set; } = Color.FromArgb(150, 150, 150);
        public bool sendOnEnter { get; set; } = false;
        public int emoteScale { get; set; } = 100;
    }
}
