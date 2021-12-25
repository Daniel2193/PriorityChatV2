using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        public static bool sendMessage(string msg)
        {
            if (canSend && msg.Length >= 1)
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.ExclusiveAddressUse = false;
                byte[] data = Encoding.ASCII.GetBytes(ConfigManager.getConfig().username + "|" + msg);
                socket.SendTo(data, data.Length, SocketFlags.None, serverEP);
                return true;
            }
            return false;
        }
    }
}
