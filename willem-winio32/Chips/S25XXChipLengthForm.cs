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
    public partial class S25XXChipLengthForm : Form
    {
        public S25XXChipLengthForm()
        {
            InitializeComponent();
            try
            {
                int length = Convert.ToInt32(Ini.Read("S25XXLength"), 16);
                if (length >= 0x100)
                {
                    comboBoxLength.Text = "0x" + Convert.ToString(length,16).PadLeft(6, '0');
                }
                else
                {
                    comboBoxLength.Text = "0x100";
                }
            }
            catch { }
            
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            Ini.Write("S25XXLength", comboBoxLength.Text);
            MessageBox.Show("设置成功，程序将自动退出后生效");
            System.Environment.Exit(0);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxLength_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                string s = "";
                long length = Convert.ToInt64(comboBoxLength.Text, 16);
                s = length.ToString();
                if (length >= 0x100000)
                {
                    s = s + "(" + length / 0x100000 + "M)";
                }
                else if (length > 0x400)
                {
                    s = s + "(" + length / 0x400 + "K)";
                }
                labelLength.Text = s;
            }
            catch { }
        }
    }
}
