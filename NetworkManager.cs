using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;

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
                byte[] data = Encoding.ASCII.GetBytes("json" + Consts.msgSeperator + msgJson);
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
    }
}
