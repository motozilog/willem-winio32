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
    public partial class LPTParamConfigForm : Form
    {
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));

        public LPTParamConfigForm()
        {
            InitializeComponent();
            textBoxParam.Text = Ini.Read("LPTParamConfig");
            this.labelParam.Text = LPT.GetConfig().LPTParam;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ini.Write("LPTParamConfig", textBoxParam.Text.Trim());
            MessageBox.Show("保存参数成功，将关闭程序重新运行");
            System.Environment.Exit(0);
        }
    }
}
