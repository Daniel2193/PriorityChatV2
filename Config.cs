using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PriorityChatV2
{
    public class Config
    {
        public string ip { get; set; } = "192.168.191.248";
        public int port { get; set; } = 21930;
        public string username { get; set; } = "user";
        public bool showNotifications { get; set; } = false;
        //Not used yet
        public Color colorMessages { get; set; } = Color.FromArgb(255, 255, 255);
        //Not used yet
        public Color colorMessagesRead { get; set; } = Color.FromArgb(150, 150, 150);
        public bool sendOnEnter = false;
    }
}
