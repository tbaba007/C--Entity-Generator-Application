using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityGenerator
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fm = new Form1();
            fm.Show();
            this.Hide();
        }
    }
}
