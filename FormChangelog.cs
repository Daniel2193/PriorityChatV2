using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PriorityChatV2
{
    public partial class FormChangelog : Form
    {
        private int amountBoxes = Enum.GetValues(typeof(ChangelogType)).Length;
        private ListBox[] categories;
        public FormChangelog()
        {
            categories = new ListBox[amountBoxes];
            InitializeComponent();
            for(int i = 0; i < amountBoxes; i++){
                categories[i] = new ListBox();
                categories[i].SetBounds(10, 10 + (i * (categories[i].Height + 10)), this.Width - 20, categories[i].Height);
                this.Controls.Add(categories[i]);
            }
        }
        public void addEntry(string version, string entry, ChangelogType type)
        {
            categories[(int)type].Items.Add(version + ": " + entry);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void FormChangelog_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}
