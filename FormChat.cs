using System.Net;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace PriorityChatV2
{
    public partial class FormChat : Form
    {
        public static string version = "2.5.0";
        private string[] changelog =
        {
            "Upcoming/Planned Features:",
            "",
            "Add emote manager",
            "",
            "",
            "Actual changes:",
            "",
            "v2.5.0: Added Userlist/Userstatus",
            "v2.5.0: Code cleanup",
            "v2.5.0: Added external auto/self updateing with version history",
            "v2.5.0: Changed message transmition protocol",
            "v2.5.0: Added Message colors to settings",
            "v2.5.0: Fixed Colored Messages",
            "v2.5.0: Changed the way the chat is displayed",
            "v2.5.0: Added support for emotes",
            "v2.5.0: Moved \"Send on Enter\" to the settings window",
            "v2.4.1: Fixed \"Send on Enter\" scaling",
            "v2.4.0: slight changelog upgrade (will be changed)",
            "v2.4.0: Added \"Send message on ENTER\" feature",
            "v2.4.0: Settings window will close automatically after clicking on \"apply\"",
            "v2.4.0: Added message timestamps",
            "v2.4.0: Added adaptive UI scaling",
            "v2.3.7: Removed DLL dependency by shipping it inside the exe file",
            "v2.3.6: Quit button works again",
            "v2.3.6: Fixed usernames not showing up in chat",
            "v2.3.5: Updated Changelog",
            "v2.3.4: Added auto-scroll to chat",
            "v2.3.3: Fixed a bug where messages could not be displayed",
            "v2.3.2: (temporary) removed colored messages",
            "v2.3.1: Flash now includes Windows internal flash system",
            "v2.3.0: Added Changelog",
            "v2.3.0: Added Colors to Messages",
            "v2.3.0: Structural Rework",
            "v2.3.0: Flash works again",
        };
        public FormSettings settings = new FormSettings();
        public static FormChat instance;
        BackgroundWorker bw = new BackgroundWorker();
        public FormChat()
        {
            InitializeComponent();
            instance = this;
        }
        private void FormChat_Load(object sender, EventArgs e)
        {
            ConfigManager.setup();
            NetworkManager.setup();
            EmoteManager.loadEmotes();
            this.Text = "PriorityChatV" + version;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try { bw.CancelAsync(); } catch { }
            ConfigManager.saveConfig();
            NetworkManager.sendStatus(Status.OFFLINE);
            Application.Exit();
            Environment.Exit(0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            ChatMessage message = new ChatMessage(textBox1.Text, ConfigManager.getConfig().username);
            if (!NetworkManager.sendMessage(message))
            {
                MessageBox.Show("Message could not be send");
            }
            textBox1.Text = "";
            button2.Enabled = true;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            settings.Show();
        }
        public void writeMessage(ChatMessage message)
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Select(richTextBox1.TextLength, 0);
                richTextBox1.SelectionColor = Color.White;
                richTextBox1.AppendText(message.Time.ToString("HH:mm:ss") + " ");
                richTextBox1.SelectionColor = Color.Blue;
                richTextBox1.AppendText(message.Sender);
                richTextBox1.SelectionColor = Color.White;
                richTextBox1.AppendText(": ");
                richTextBox1.SelectionColor = ConfigManager.getConfig().colorMessages;
                richTextBox1.AppendText(message.Message + "\n");
                richTextBox1.Rtf = processRTF(richTextBox1.Rtf);
                richTextBox1.ScrollToCaret();
                if (ConfigManager.getConfig().showNotifications && !message.Sender.Equals(ConfigManager.getConfig().username))
                    FlashWindowEx(instance);
            });
        }
        private string processRTF(string msg)
        {
            string[] emotes = EmoteManager.GetEmotes();
            foreach (string emote in emotes)
            {
                Bitmap bmp = EmoteManager.GetEmote(emote);
                bmp.MakeTransparent(bmp.GetPixel(0, 0));
                var stream = new MemoryStream();
                byte[] data = File.ReadAllBytes(EmoteManager.GetEmotePath(emote));
                stream.Write(data, 0, data.Length);
                msg = msg.Replace(emote, @"{\pict\pngblip\picw" + bmp.Width * 15 + "\\pich" + bmp.Height * 15 + "\\picwgoal" + 3 * ConfigManager.getConfig().emoteScale + "\\pichgoal" + 3 * ConfigManager.getConfig().emoteScale + " " + BitConverter.ToString(stream.ToArray()).Replace("-", "") + "}");
            }
            return msg;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string changelog = "";
            foreach (string s in this.changelog)
            {
                changelog += s + "\n";
            }
            MessageBox.Show(changelog);
        }
        private void bw_DoWork(object o, DoWorkEventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = false;
            socket.Bind(new IPEndPoint(IPAddress.Any, ConfigManager.getConfig().port));
            while (true)
            {
                byte[] buffer = new byte[4096];
                int recv = socket.Receive(buffer);
                string rawMsg = Encoding.ASCII.GetString(buffer, 0, recv);
                string[] split = Regex.Split(rawMsg, Regex.Escape(Consts.msgSeperator));
                if (split.Length >= 2)
                {
                    if (split[0].Equals("json"))
                    {
                        ChatMessage message = JsonSerializer.Deserialize<ChatMessage>(split[1]);
                        writeMessage(message);
                    }
                    if (split[0].Equals("status"))
                    {
                        if (split.Length != 3)
                            continue;
                        int status = 2;
                        int.TryParse(split[2], out status);
                        UserManager.UpdateUser(split[1], (Status)status);
                        updateUserList();
                    }
                }
            }
        }
        private void updateUserList(){
            Invoke((MethodInvoker)delegate
            {
                richTextBox2.Text = "";
                foreach (KeyValuePair<string, Status> user in UserManager.users)
                {
                    richTextBox2.Select(richTextBox2.TextLength, 0);
                    if(user.Value == Status.ONLINE)
                        richTextBox2.SelectionColor = Color.Green;
                    else if(user.Value == Status.OFFLINE)
                        richTextBox2.SelectionColor = Color.Black;
                    else if(user.Value == Status.AFK)
                        richTextBox2.SelectionColor = Color.Yellow;
                    richTextBox2.AppendText(user.Key + "\n");
                }
            });
        }
        #region flashWindow
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        public const UInt32 FLASHW_ALL = 3;
        public const UInt32 FLASHW_TIMERNOFG = 12;
        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;
            return FlashWindowEx(ref fInfo);
        }
        #endregion
        private void FormChat_Resize(object sender, EventArgs e)
        {
            textBox1.Size = new Size(this.Size.Width - button2.Size.Width - 2 * Consts.globalOffset, textBox1.Size.Height);
            textBox1.Location = new Point(textBox1.Location.X, this.Size.Height - textBox1.Size.Height - 3 * Consts.globalOffset - button3.Size.Height);
            button2.Location = new Point(this.Size.Width - button2.Size.Width - Consts.globalOffset, textBox1.Location.Y);
            button1.Location = new Point(button1.Location.X, this.Size.Height - 70);
            button3.Location = new Point(button3.Location.X, this.Size.Height - 70);
            button4.Location = new Point(button4.Location.X, this.Size.Height - 70);
            richTextBox1.Size = new Size(this.Size.Width - 2 * Consts.globalOffset, this.Size.Height - textBox1.Size.Height - 4 * Consts.globalOffset - button3.Size.Height - button4.Size.Height);
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ConfigManager.getConfig().sendOnEnter && e.KeyCode == Keys.Enter && !e.Shift)
            {
                button2_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.saveConfig();
        }
        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            button1_Click(null, null);
            e.Cancel = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Userlist
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            NetworkManager.sendStatus(Status.AFK);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            NetworkManager.sendStatus(Status.ONLINE);
        }
    }
}
