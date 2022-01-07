using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Drawing;
using Timer = System.Windows.Forms.Timer;

namespace PriorityChatV2
{
    class NetworkManager
    {
        private static IPEndPoint serverEP;
        private static bool canSend = false;
        public static void setup()
        {
            try
            {
                serverEP = new IPEndPoint(IPAddress.Parse(ConfigManager.getConfig().ip), ConfigManager.getConfig().port);
                canSend = true;
                sendStatus(Status.ONLINE);
                Timer timer = new Timer{Interval = 5000};
                timer.Tick += new EventHandler(sendHearbeat);
                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        public static bool sendMessage(ChatMessage message)
        {
            if (canSend && message.Message.Length >= 1)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ExclusiveAddressUse = false;
                string msgJson = JsonSerializer.Serialize<ChatMessage>(message);
                byte[] data = Encoding.ASCII.GetBytes("msg" + Consts.msgSeperator + msgJson);
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
                return true;
            }
            return false;
        }
        public static bool sendStatus(Status status)
        {
            if (canSend)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ExclusiveAddressUse = false;
                byte[] data = Encoding.ASCII.GetBytes("status" + Consts.msgSeperator + ConfigManager.getConfig().username + Consts.msgSeperator + (int)status);
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
                return true;
            }
            return false;
        }
        public static bool sendColor(Color color)
        {
            if (canSend)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ExclusiveAddressUse = false;
                byte[] data = Encoding.ASCII.GetBytes("color" + Consts.msgSeperator + ConfigManager.getConfig().username + Consts.msgSeperator + color.R + "," + color.G + "," + color.B);
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
                return true;
            }
            return false;
        }
        public static void sendHearbeat(object sender, EventArgs e)
        {
            if (canSend)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ExclusiveAddressUse = false;
                byte[] data = Encoding.ASCII.GetBytes("heartbeat" + Consts.msgSeperator + ConfigManager.getConfig().username + Consts.msgSeperator + (int)UserManager.userList.Find(x => x.Username == ConfigManager.getConfig().username).Status + Consts.msgSeperator + ConfigManager.getConfig().colorUsername.R + "," + ConfigManager.getConfig().colorUsername.G + "," + ConfigManager.getConfig().colorUsername.B);
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
            }
        }
    }
}
