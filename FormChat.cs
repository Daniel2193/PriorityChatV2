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

namespace PriorityChatV2
{
    public partial class FormChat : Form
    {
        public static string version = "2.6.0-pre";
        private string[] changelog =
        {
            "Upcoming/Planned Features:",
            "",
            "Migrate to .NET 5 or 6",
            "Add Emote Menu",
            "",
            "",
            "Actual changes:",
            "",
            "v2.6.0: Added Username Color",
            "v2.6.0: Added Emote Manager",
            "v2.5.1: Fixed UI Scaling",
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
        public FormEmotes emotes = new FormEmotes();
        public static FormChat instance;
        BackgroundWorker bw = new BackgroundWorker();
        private bool expanded = false;
        private int expandedDiff = 238;
        public FormChat()
        {
            InitializeComponent();
            instance = this;
        }
        private void FormChat_Load(object sender, EventArgs e)
        {
            ConfigManager.setup();
            UserManager.setup();
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
            Color color = UserManager.userList.Find(x => x.Username == message.Sender).Color;
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Select(richTextBox1.TextLength, 0);
                richTextBox1.SelectionColor = Color.White;
                richTextBox1.AppendText(message.Time.ToString("HH:mm:ss") + " ");
                richTextBox1.SelectionColor = color == Color.Empty ? Color.Blue : color;
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
                    if (split[0].Equals("msg"))
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
                        UserManager.UpdateStatus(split[1], (Status)status);
                        updateUserList();
                    }
                    if(split[0].Equals("heartbeat"))
                    {
                        if (split.Length != 4)
                            continue;
                        string[] colorSplit = split[3].Split(',');
                        UserManager.UpdateUser(split[1], (Status)int.Parse(split[2]), Color.FromArgb(int.Parse(colorSplit[0]), int.Parse(colorSplit[1]), int.Parse(colorSplit[2])));
                        updateUserList();
                    }
                }
            }
        }
        private Color[] statusColors = new Color[] {Color.Green, Color.Yellow, Color.Black, Color.Red};
        private void updateUserList()
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox2.Clear();
                richTextBox2.Select(richTextBox2.TextLength, 0);
                foreach (User user in UserManager.userList)
                {
                    richTextBox2.SelectionColor = statusColors[(int)user.Status];
                    richTextBox2.AppendText(user.Username + "\n");
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
            int width = this.Width;
            if(expanded)
                width -= expandedDiff;
            richTextBox1.Size = new Size(width - (Consts.globalOffset * 3), this.Height - textBox1.Height - button3.Height - Consts.globalOffset * 6);
            richTextBox2.Location = new Point(width - Consts.globalOffset, richTextBox2.Location.Y);
            
            label1.Location = new Point(width + label1.Width, label1.Location.Y);
            button5.Location = new Point(width - Consts.globalOffset * 2 - button5.Width, this.Height - (int)(Consts.globalOffset * 3.7f) - button5.Height);
            textBox1.Location = new Point(textBox1.Location.X, this.Height - textBox1.Height - button3.Height - (int)(Consts.globalOffset * 4.3f));
            textBox1.Size = new Size(width - button2.Width - Consts.globalOffset * 4, textBox1.Height);
            button2.Location = new Point(width - button2.Width - Consts.globalOffset * 2, textBox1.Location.Y);
            richTextBox2.Size = new Size(richTextBox2.Width, button2.Bottom - richTextBox2.Top);
            button1.Location = new Point(button1.Location.X, button5.Location.Y);
            button3.Location = new Point(button3.Location.X, button5.Location.Y);
            button4.Location = new Point(button4.Location.X, button5.Location.Y);
            button6.Location = new Point(width - Consts.globalOffset, button5.Location.Y);
            button7.Location = new Point(width - (int)(Consts.globalOffset * 0.5f) + button6.Width, button5.Location.Y);
            button8.Location = new Point(button8.Location.X, button5.Location.Y);
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
            expanded = !expanded;
            if (expanded)
            {
                this.Width += expandedDiff;
                button5.Text = "<<";
            }
            else
            {
                this.Width -= expandedDiff;
                button5.Text = ">>";
            }
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

        private void button8_Click(object sender, EventArgs e)
        {
            emotes.Show();
        }
    }
}
