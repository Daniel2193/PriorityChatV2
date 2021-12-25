using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityChatV2
{
    class ChatMessage
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public DateTime Time { get; set; }
        public bool IsRead { get; set; }
        public ChatMessage(string message, string sender)
        {
            Message = message;
            Sender = sender;
            Time = DateTime.Now;
            IsRead = false;
        }
        public override string ToString()
        {
            return "<<" + Sender + ">>: " + Message;
        }
    }
}
