using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;

namespace PriorityChatV2
{
    public partial class FormEmotes : Form
    {
        private FormDownload downloadProgress = new FormDownload();
        public FormEmotes()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private async void button2_Click(object sender, EventArgs e)
        {
            string path = ConfigManager.path + "EmoteDownloader.exe";
            if (!File.Exists(path))
            {
                DialogResult result = MessageBox.Show("EmoteDownloader.exe not found.\nDo you want to download it now?", "EmoteDownloader.exe not found", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    using (var client = new WebClient())
                    {
                        string url = await getDownloadURL();
                        if(url == ""){
                            DialogResult res2 = MessageBox.Show("Could not find download link.\nDo you want to download it manually?", "Error", MessageBoxButtons.YesNo);
                            if(res2 == DialogResult.Yes)
                                Process.Start("https://github.com/Daniel2193/EmoteDownloader/releases/latest");
                            return;
                        }
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                            downloadProgress.Show();
                            await wc.DownloadFileTaskAsync(new Uri(url), ConfigManager.path + "EmoteDownloader.exe");
                            downloadProgress.Hide();
                        }
                    }
                }
                else
                    return;
            }
            string args = "";
            args += "-p " + comboBox1.SelectedText + " ";
            if(radioButton1.Checked)
                args += "--channel_names " + textBox1.Text + " ";
            if(radioButton2.Checked)
                args += "--channel_ids " + textBox2.Text + " ";
            if(panel1.Visible){
                args += "--client_id " + textBox3.Text + " ";
                if(radioButton3.Checked){
                    args += "--client_secret " + textBox4.Text + " ";
                }else if(radioButton4.Checked){
                    args += "-t " + textBox5.Text + " ";
                }
            }
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = @"cmd.exe",
                Arguments = "/c " + path + " " + args + "",
                UseShellExecute = false
            };
            p.EnableRaisingEvents = true;
            p.Exited += p_Exited;
            p.Start();
        }
        private void p_Exited(object sender, EventArgs e)
        {
            int exitCode = ((Process)sender).ExitCode;
            if(exitCode == 0)
                MessageBox.Show("Emotes successfully downloaded.");
            else
                MessageBox.Show("Error while downloading emotes.\nError code: " + exitCode);
        }
        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            downloadProgress.setProgress(e.ProgressPercentage);
        }
        private void FormEmotes_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            textBox6.Lines = EmoteManager.GetEmotes();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            EmoteManager.loadEmotes();
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.Enabled = radioButton3.Checked;
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.Enabled = radioButton4.Checked;
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = radioButton1.Checked;
            panel1.Visible = (radioButton1.Checked || comboBox1.SelectedIndex == 0);
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = radioButton2.Checked;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Visible = (radioButton1.Checked || comboBox1.SelectedIndex == 0);
        }
        private static async Task<string> getDownloadURL()
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/Daniel2193/EmoteDownloader/releases/latest");
                request.Headers.Add("User-Agent", "Update-Tool");
                var response = await client.SendAsync(request);
                string json = await response.Content.ReadAsStringAsync();
                string[] split = json.Split(',');
                foreach(string s in split)
                {
                    if(s.Contains("browser_download_url"))
                    {
                        return s.Split('\"')[3];
                    }
                }
            }
            return "";
        }
    }
}
