using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PriorityChatV2
{
    public partial class FormSettings : Form
    {
        public static FormSettings instance;
        public FormSettings()
        {
            InitializeComponent();
            instance = this;
        }
        private void FormSettings_Load(object sender, EventArgs e)
        {
            textBox1.Text = ConfigManager.getConfig().ip;
            textBox2.Text = ConfigManager.getConfig().port.ToString();
            textBox3.Text = ConfigManager.getConfig().username;
            checkBox1.Checked = ConfigManager.getConfig().showNotifications;
            checkBox2.Checked = ConfigManager.getConfig().sendOnEnter;
            numericUpDown1.Value = ConfigManager.getConfig().emoteScale;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(checkConfig()){
                ConfigManager.updateConfig(textBox1.Text, int.Parse(textBox2.Text), textBox3.Text, checkBox1.Checked, ConfigManager.getConfig().colorMessages, ConfigManager.getConfig().colorMessagesRead, checkBox2.Checked, (int)numericUpDown1.Value);
                this.Hide();
            }
        }
        private bool checkConfig(){
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Please fill in all fields");
                return false;
            }
            if (!Regex.IsMatch(textBox1.Text, @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$"))
            {
                MessageBox.Show("Please enter a valid IP");
                return false;
            }
            if (!Regex.IsMatch(textBox2.Text, @"^[0-9]{1,5}$"))
            {
                MessageBox.Show("Please enter a valid Port");
                return false;
            }
            if (textBox3.Text.Length > 30 || textBox3.Text.Length < 3 || !Regex.IsMatch(textBox3.Text, @"^[a-zA-Z0-9_]*$"))
            {
                MessageBox.Show("Please enter a valid Username");
                return false;
            }
            if(numericUpDown1.Value < 10 || numericUpDown1.Value > 400){
                MessageBox.Show("Please enter a valid Scale factor");
                return false;
            }
            return true;
        }
        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                ConfigManager.getConfig().colorMessages = cd.Color;
                ConfigManager.getConfig().colorMessagesR = cd.Color.R;
                ConfigManager.getConfig().colorMessagesG = cd.Color.G;
                ConfigManager.getConfig().colorMessagesB = cd.Color.B;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                ConfigManager.getConfig().colorMessagesRead = cd.Color;
                ConfigManager.getConfig().colorMessagesReadR = cd.Color.R;
                ConfigManager.getConfig().colorMessagesReadG = cd.Color.G;
                ConfigManager.getConfig().colorMessagesReadB = cd.Color.B;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EmoteManager.loadEmotes();
        }
    }
}
