using System.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.Json;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PriorityChatV2
{
    public partial class FormChat : Form
    {
        public string version = "";
        private string[] changelog =
        {
            "Upcoming/Planned Features:",
            "",
            
            "[v2.5.0: Self-updating]",
            "[v2.5.0: Userlist/Userstatus]",
            "[v2.4.1: Fix colored messages]",
            "[v2.4.1: change changelog POG]",
            "",
            "",
            "",
            "Actual changes:",
            "",
            "v2.4.0: UPDATED   slight changelog upgrade (will be changed)",
            "v2.4.0: ADDED      \"Send message on ENTER\" feature",
            "v2.4.0: ADDED      Settings window closed automatically after clicking on \"apply\"",
            "v2.4.0: ADDED      message timestamps",
            "v2.4.0: ADDED      adaptive UI scaling",
            "v2.3.7: FIXED         DLL dependency by shipping it inside the exe file",
            "v2.3.6: FIXED         quit button",
            "v2.3.6: FIXED         usernames not showing up in chat",
            "v2.3.5: UPDATED   Changelog",
            "v2.3.4: ADDED      auto-scroll to chat",
            "v2.3.3: FIXED         bug where messages could be rendered invisible",
            "v2.3.2: REMOVED (temporary) colored messages",
            "v2.3.1: UPDATED   Flash now includes Windows internal flash",
            "v2.3.0: ADDED      Changelog",
            "v2.3.0: ADDED      Colors to Messages",
            "v2.3.0: UPDATED   Structural Rework",
            "v2.3.0: FIXED         flash"
        };
        public FormSettings settings = new FormSettings();
        public static FormChat instance;
        BackgroundWorker bw = new BackgroundWorker();
        public FormChat(string version)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            this.version = version;
            InitializeComponent();
            instance = this;
        }
        private void FormChat_Load(object sender, EventArgs e)
        {
            ConfigManager.readConfig();
            NetworkManager.setup();
            ChangelogManager.setup();
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            this.Text = "PriorityChatV" + version;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.RunWorkerAsync();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try{bw.CancelAsync();}catch{}
            ConfigManager.saveConfig();
            Application.Exit();
            Environment.Exit(0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            if (!NetworkManager.sendMessage(textBox1.Text))
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
        public void writeMessage(string user, string message)
        {
            Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Add(new ChatMessage(message, user));
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                if (ConfigManager.getConfig().showNotifications && !listBox1.Items[listBox1.Items.Count - 1].ToString().Contains(ConfigManager.getConfig().username))
                    FlashWindowEx(instance);
            });
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string cl = "";
            foreach (string s in changelog)
            {
                cl += s + "\n";
            }
            MessageBox.Show(cl);
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                ChatMessage item = listBox1.Items[e.Index] as ChatMessage;
                if (item != null)
                {
                    e.Graphics.DrawString(item.Time.ToString("HH:mm:ss") + " | <<" + item.Sender + ">>: " + item.Message, e.Font, new SolidBrush(Color.White), e.Bounds);
                }
            }catch(Exception){}
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
                string message = Encoding.ASCII.GetString(buffer, 0, recv);
                if(message.Contains("|")){
                    string[] split = message.Split('|');
                    if(split.Length >= 3){
                        if(!split[0].Equals(ConfigManager.getConfig().username)){
                            if(split[1].Equals("status")){
                                if(split[2].Equals("online")){
                                    writeMessage(split[0], split[0] + " is online");
                                }else{
                                    writeMessage(split[0], split[0] + " is offline");
                                }
                            }else if(split[1].Equals("message")){
                                writeMessage(split[0], split[2]);
                            }
                            else if(split[1].Equals("eval")){
                                processMessage(split[0], split[2]);
                            }
                        }
                    }
                    if(split.Length == 2)
                    {
                        writeMessage(split[0], split[1]);
                    }
                }
            }
        }
        private void processMessage(string user, string rawMessage)
        {
            
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
            listBox1.Size = new Size((int)(this.Size.Width - 40), this.Size.Height - 165);
            textBox1.Size = new Size(this.Size.Width - button2.Size.Width - 2 * Consts.globalOffset, textBox1.Size.Height);
            textBox1.Location = new Point(textBox1.Location.X, this.Size.Height - textBox1.Size.Height - 3*Consts.globalOffset - button3.Size.Height);
            button2.Location = new Point(this.Size.Width - button2.Size.Width - Consts.globalOffset, textBox1.Location.Y);
            button1.Location = new Point(button1.Location.X, this.Size.Height - 70);
            button3.Location = new Point(button3.Location.X, this.Size.Height - 70);
            button4.Location = new Point(button4.Location.X, this.Size.Height - 70);
            checkBox1.Location = new Point(checkBox1.Location.X, this.Size.Height - 70 - (int)(checkBox1.Height/2) + (int)(button1.Size.Height/2));
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(ConfigManager.getConfig().sendOnEnter && e.KeyCode == Keys.Enter && !e.Shift)
            {
                button2_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ConfigManager.getConfig().sendOnEnter = checkBox1.Checked;
            ConfigManager.saveConfig();
        }
        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            button1_Click(null, null);
            e.Cancel = true;
        }
    }
}
