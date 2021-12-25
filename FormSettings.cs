using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(checkConfig()){
                ConfigManager.updateConfig(textBox1.Text, int.Parse(textBox2.Text), textBox3.Text, checkBox1.Checked, ConfigManager.getConfig().colorMessages, ConfigManager.getConfig().colorMessagesRead);
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
            return true;
        }
        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
