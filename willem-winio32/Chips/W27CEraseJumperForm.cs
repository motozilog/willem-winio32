using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace willem_winio32
{
    public partial class W27CEraseJumperForm : Form
    {
        public W27CEraseJumperForm(string msg=null)
        {
            InitializeComponent();
            if (msg != null)
            {
                label1.Text = msg;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
