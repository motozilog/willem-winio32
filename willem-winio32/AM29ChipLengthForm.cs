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
    public partial class AM29ChipLengthForm : Form
    {
        string iniSaveKey = null;
        public AM29ChipLengthForm(string iniKey = G.AM29VPPLengthIniKey, List<string> lengthList=null)
        {
            iniSaveKey = iniKey;
            InitializeComponent();
            try
            {
                int length = Convert.ToInt32(Ini.Read(iniKey), 16);
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

            if (lengthList != null)
            {
            }
        }


        private void buttonOk_Click(object sender, EventArgs e)
        {
            Ini.Write(iniSaveKey, comboBoxLength.Text);
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

                if (length == 0x40000)
                {
                    labelNote.Text = "实测通过：AM29LV200BB";
                }
                else if (length == 0x400000)
                {
                    labelNote.Text = "实测通过：MX29LV320、S29GL032";
                }
                else if (length == 0x800000)
                {
                    labelNote.Text = "实测通过：MX29LV640、S29GL064";
                }
                else if (length == 0x1000000)
                {
                    labelNote.Text = "实测通过：MX29LV128、S29GL128";
                }
                else if (length == 0x2000000)
                {
                    labelNote.Text = "实测通过：MX29GL256、S29GL256";
                }
                else if (length == 0x4000000)
                {
                    labelNote.Text = "实测通过：MX29GL512、S29GL512";
                }
                else if (length == 0x8000000)
                {
                    labelNote.Text = "实测通过：S29GL01G";
                }
                else
                {
                    labelNote.Text = "该容量未经测试";
                }

            }
            catch { }
        }
    }
}
