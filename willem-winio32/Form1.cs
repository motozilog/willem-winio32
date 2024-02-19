using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace willem_winio32
{
    public partial class Form1 : Form
    {
        bool isDevelopMode = false;//是否为开发者模式

        IChip chip = null;
        ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));
        Int64 largeChipLength = G.largeChipLength;
        Int64 splitLength = G.splitLength;
        private string tempFilename = System.AppDomain.CurrentDomain.BaseDirectory + "tempWillem.bin";
        public Form1()
        {
            InitializeComponent();
            //this.Icon = willem_winio32.Properties.Resources.icon;

#if DEBUG
            buttonTest.Visible = true;
#endif

            //加载配置
            textBoxFilename.Text = Ini.Read("Filename");
            comboBoxChipSelect.Text = Ini.Read("ChipSelect");
            textBoxTryLength.Text = Ini.Read("TryLength");
            comboBoxLPTDeviceType.Text = Ini.Read("LPTDeviceType");
            textBoxBaseAddr.Text = Ini.Read("BaseAddr");

            if (!string.IsNullOrEmpty(textBoxFilename.Text))
            {
                try
                {
                    loadFile(textBoxFilename.Text);
                }
                catch
                {
                    textBoxFilename.Text = "";
                }
            }

            if (string.IsNullOrEmpty(textBoxBaseAddr.Text))
            {
                textBoxBaseAddr.Text = "0x0";
            }
            else
            {
                try
                {
                    int length = Convert.ToInt32(textBoxBaseAddr.Text, 16);
                }
                catch { textBoxBaseAddr.Text = "0x0"; }
            }

            if (string.IsNullOrEmpty(textBoxTryLength.Text))
            {
                textBoxTryLength.Text = "0x20";
            }
            else
            {
                try
                {
                    int length = Convert.ToInt32(textBoxTryLength.Text, 16);
                }
                catch { textBoxTryLength.Text = "0x20"; }
            }

            if (isDevelopMode)
            {
                this.comboBoxChipSelect.Items.Add("MX26L6420");
                this.comboBoxChipSelect.Items.Add("MX26L12811");
                this.comboBoxChipSelect.Items.Add("SST29EE512");
            }

            //IO初始化
            LPT.Initialize();

            //Willem初始化
            WillemOP.Init();
        }



        #region TestButton



        private void buttonRead379_Click(object sender, EventArgs e) { Console.WriteLine(Tools.byte2Str(LPT.Read379())); }

        private void buttonSetADDR_Click(object sender, EventArgs e)
        {
            int addr = Convert.ToInt32(textBoxADDR.Text, 16);
            WillemOP.SetAddr(addr);
        }

        private void buttonSetData_Click(object sender, EventArgs e)
        {
            //数据
            int data = Convert.ToInt32(textBoxData.Text, 16);
            WillemOP.SetData((byte)(data & 0xFF));
        }

        private void buttonRead4021_Click(object sender, EventArgs e)
        {
            byte data = WillemOP.Read4021();
            Console.WriteLine(Tools.byte2Str(data));
        }


        private void buttonSetVCC_Click(object sender, EventArgs e) { WillemOP.SetVCC_H(); }

        private void buttonCleanVCC_Click(object sender, EventArgs e) { WillemOP.SetVCC_L(); }

        private void buttonSetCE_Click(object sender, EventArgs e) { WillemOP.SetCE_H(); }

        private void buttonClearCE_Click(object sender, EventArgs e) { WillemOP.SetCE_L(); }

        private void buttonSetVPP_Click(object sender, EventArgs e) { WillemOP.SetVPP_H(); }

        private void buttonCleanVPP_Click(object sender, EventArgs e) { WillemOP.SetVPP_L(); }

        #endregion TestButton


        #region CONTROL
        private void buttonSElin0_Click(object sender, EventArgs e) { LPT.SELin(0); }

        private void buttonSELin1_Click(object sender, EventArgs e) { LPT.SELin(1); }

        private void buttonInit0_Click(object sender, EventArgs e) { LPT.Init(0); }

        private void buttonInit1_Click(object sender, EventArgs e) { LPT.Init(1); }

        private void buttonAUTO0_Click(object sender, EventArgs e) { LPT.Auto(0); }

        private void buttonAUTO1_Click(object sender, EventArgs e) { LPT.Auto(1); }

        private void buttonSTB0_Click(object sender, EventArgs e) { LPT.STB(0); }

        private void buttonSTB1_Click(object sender, EventArgs e) { LPT.STB(1); }
        #endregion  CONTROL

        #region DATA
        private void buttonD0_0_Click(object sender, EventArgs e) { LPT.D0(0); }

        private void buttonD0_1_Click(object sender, EventArgs e) { LPT.D0(1); }

        private void buttonD1_0_Click(object sender, EventArgs e) { LPT.D1(0); }

        private void buttonD1_1_Click(object sender, EventArgs e) { LPT.D1(1); }

        private void buttonD2_0_Click(object sender, EventArgs e) { LPT.D2(0); }

        private void buttonD2_1_Click(object sender, EventArgs e) { LPT.D2(1); }

        private void buttonD3_0_Click(object sender, EventArgs e) { LPT.D3(0); }

        private void buttonD3_1_Click(object sender, EventArgs e) { LPT.D3(1); }

        private void buttonD4_0_Click(object sender, EventArgs e) { LPT.D4(0); }

        private void buttonD4_1_Click(object sender, EventArgs e) { LPT.D4(1); }

        private void buttonD5_0_Click(object sender, EventArgs e) { LPT.D5(0); }

        private void buttonD5_1_Click(object sender, EventArgs e) { LPT.D5(1); }

        private void buttonD6_0_Click(object sender, EventArgs e) { LPT.D6(0); }

        private void buttonD6_1_Click(object sender, EventArgs e) { LPT.D6(1); }

        private void buttonD7_0_Click(object sender, EventArgs e) { LPT.D7(0); }

        private void buttonD7_1_Click(object sender, EventArgs e) { LPT.D7(1); }
        #endregion DATA




        private void buttonLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == DialogResult.OK)
            {
                textBoxFilename.Text = of.FileName;
                loadFile(of.FileName);
            }
        }

        private void loadFile(string fileName)
        {

            FileStream fs = new FileStream(fileName, FileMode.Open);            
            BinaryReader r = new BinaryReader(fs);
            //显示前0x100
            if (r.BaseStream.Length >= 0x100)
            {
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开            
                byte[] bytes = r.ReadBytes(0x100);
                string s = Tools.file2HexStr(bytes);
                textBoxFileContent.Text = s;
            }


            //文件长度
            textBoxFileLength.Text = "0x" + Convert.ToString(r.BaseStream.Length, 16).ToUpper() + "(" + r.BaseStream.Length + ")";
            r.Close();
            fs.Close();

            fs = new FileStream(fileName, FileMode.Open);
            textBoxFileMd5.Text = Tools.CalcMD5(fs);
            fs.Close();


        }




        private void comboBoxChipSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            chip = ChipFactory.create(comboBoxChipSelect.Text);
            if (chip != null)
            {
                ChipConfig config = chip.GetConfig();
                if (config != null)
                {
                    buttonErase.Visible = config.Erase;
                    labelEraseDelay.Visible = config.Erase;
                    textBoxEraseDelay.Visible = config.Erase;

                    buttonWrite.Visible = config.Write;
                    buttonWriteLength.Visible = config.Write;

                    buttonRead.Visible = config.Read;
                    buttonReadLength.Visible = config.Read;

                    buttonReadId.Visible = config.ReadId;

                    pictureBoxDipSw.Image = config.DipSw;
                    pictureBoxJumper.Image = config.Jumper;
                    pictureBoxAdapter.Image = config.Adapter;

                    textBoxChipLength.Text = "0x" + Convert.ToString(config.ChipLength, 16).ToUpper() + "(" + config.ChipLength + ")";

                    if (config.EraseDelay)
                    {
                        labelEraseDelay.Visible = true;
                        textBoxEraseDelay.Visible = true;
                        textBoxEraseDelay.Text = config.EraseDelayTime;
                    }
                    else
                    {
                        labelEraseDelay.Visible = false;
                        textBoxEraseDelay.Visible = false;
                        textBoxEraseDelay.Text = "0";
                    }

                    toolStripStatusLabel1.Text = config.Note;

                    if (string.IsNullOrEmpty(config.SpecialFunction))
                    {
                        buttonSpecialFunction.Visible = false;
                    }
                    else
                    {
                        buttonSpecialFunction.Visible = true;
                        buttonSpecialFunction.Text = config.SpecialFunction;
                    }


                    this.Text = "Willem WinIO32 byLiangyx 当前芯片：" + config.ChipModel;
                }
            }
        }

        private void buttonErase_Click(object sender, EventArgs e)
        {
            WillemOP.Init();
            if (chip != null)
            {
                chip.Erase(textBoxEraseDelay.Text);
            }
            Thread.Sleep(20);
            WillemOP.Init();
            Console.WriteLine("擦除结束");
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            write(0);
        }

        private void buttonWrite20_Click(object sender, EventArgs e)
        {
            Int64 baseAddr = 0;
            int length = 0;
            if(checkBaseAddrAndLength(out baseAddr, out length)==true)
            {
                write(baseAddr, length);
            }
        }

        //检查尝试地址和尝试长度
        private bool checkBaseAddrAndLength(out Int64 baseAddr, out int length)
        {
            try
            {
                baseAddr = Convert.ToInt64(textBoxBaseAddr.Text, 16);
                length = Convert.ToInt32(textBoxTryLength.Text, 16);

                Int64 chipLength = chip.GetConfig().ChipLength;

                if (baseAddr < 0)
                {
                    MessageBox.Show("尝试地址不能为负数");
                    return false;
                }

                if (baseAddr + length > chipLength)
                {
                    MessageBox.Show("尝试地址+尝试长度不能超过芯片容量");
                    return false;
                }

                if (baseAddr % 2 != 0)
                {
                    MessageBox.Show("尝试地址必须为2的倍数(即只能为偶数)");
                    return false;
                }

                if (length % 2 != 0)
                {
                    MessageBox.Show("尝试长度必须为2的倍数(即只能为偶数)");
                    return false;
                }

                if (length < 2)
                {
                    MessageBox.Show("尝试长度需要>=2");
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("尝试地址及尝试长度需要以16进制输入且不留空格，如：0x00");
                baseAddr = 0; length = 0;

                return false;
            }

            return true;

        }

        private void writeSmall(Int64 baseAddr, int length = 0)
        {
            byte[] data = new byte[chip.GetConfig().ChipLength];
            string file = textBoxFilename.Text;
            if (String.IsNullOrEmpty(file) && length == 0)
            {
                MessageBox.Show("先加载文件");
                return;
            }
            else if (!String.IsNullOrEmpty(file))
            {
                //有文件就优先加载文件
                FileStream fs = new FileStream(file, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                br.Read(data, 0, data.Length);
                br.Close();
                fs.Close();
            }
            else if (String.IsNullOrEmpty(file) && length != 0)
            {
                //没有文件生成顺序数据来测试
                for (int i = 0; i < data.Length; i++)
                {
                    if (i < data.Length)
                    {
                        data[i] = (byte)i;
                    }
                }
            }

            WillemOP.Init();
            DateTime startTime = System.DateTime.Now;
            if (chip != null)
            {
                if (length != 0)
                {
                    chip.Write(data, baseAddr, length, length);
                }
                else
                {
                    chip.Write(data, 0, data.Length, data.Length);
                }
            }
            DateTime endTime = System.DateTime.Now;
            TimeSpan ts = endTime.Subtract(startTime);
            Console.WriteLine();
            Console.WriteLine("写入用时：" + ts.TotalSeconds);

            Thread.Sleep(20);
            WillemOP.Init(true);
            Console.WriteLine("写入结束");
        }

        //大容量芯片分次加载
        private void writeLarge(Int64 baseAddr, int length = 0)
        {
            string file = textBoxFilename.Text;
            if (String.IsNullOrEmpty(file) && length == 0)
            {
                MessageBox.Show("先加载文件");
                return;
            }
            Int64 length64 = 0;
            if (length == 0)
            {
                length64 = chip.GetConfig().ChipLength;
            }
            else
            {
                length64 = length;
            }


            WillemOP.Init();
            DateTime startTime = System.DateTime.Now;

            for (Int64 addr = baseAddr; addr < baseAddr + length64; addr += splitLength)
            {
                byte[] data = new byte[splitLength];
                
                if (!String.IsNullOrEmpty(file))
                {
                    //有文件就优先加载文件
                    FileStream fs = new FileStream(file, FileMode.Open);
                    fs.Seek(addr, SeekOrigin.Begin);
                    //Console.WriteLine("读取文件，位置：" + Tools.int2HexStr(addr) + " 长度：" + data.Length);
                    BinaryReader br = new BinaryReader(fs);
                    br.Read(data, 0, data.Length);
                    br.Close();
                    fs.Close();
                }
                else if (String.IsNullOrEmpty(file) && length != 0)
                {
                    //没有文件生成顺序数据来测试
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (i < data.Length)
                        {
                            data[i] = (byte)i;
                        }
                    }
                }

                if (chip != null)
                {
                    if (length == 0)
                    {
                        chip.Write(data, addr, (int)splitLength, chip.GetConfig().ChipLength);
                    }
                    else
                    {
                        chip.Write(data, addr, (int)length64, (int)length64);
                    }
                }
            }

            
            
            DateTime endTime = System.DateTime.Now;
            TimeSpan ts = endTime.Subtract(startTime);
            Console.WriteLine();
            Console.WriteLine("写入用时：" + ts.TotalSeconds);

            Thread.Sleep(20);
            WillemOP.Init(true);
            Console.WriteLine("写入结束");
        }

        private void write(Int64 baseAddr, int length = 0)
        {
            if (chip.GetConfig().ChipLength <= largeChipLength)
            {
                writeSmall(baseAddr, length);
            }
            else
            {
                writeLarge(baseAddr, length);
            }
        }

        private void buttonRead_Click(object sender, EventArgs e)
        {
            read(0, 0);
        }

        private void readSmall(Int64 baseAddr, int length, bool save=true)
        {
            WillemOP.Init();
            if (chip != null)
            {
                byte[] dataRead = null;
                if (length != 0)
                {
                    //部分读取
                    dataRead = chip.Read(baseAddr, length, length);
                    //显示出来
                    for (Int64 i = baseAddr; i < baseAddr + length; i++)
                    {
                        if (i % 0x10 == 0)
                        {
                            Console.WriteLine();
                            Console.Write(Tools.int2HexStr(i) + ":");
                        }
                        Console.Write(Tools.byte2HexStr(dataRead[i]) + " ");
                    }
                }
                else
                {
                    //完全读取
                    dataRead = chip.Read(0, (int)chip.GetConfig().ChipLength, (int)chip.GetConfig().ChipLength);
                }

                Thread.Sleep(20);
                WillemOP.Init(true);

                Console.WriteLine("完成读取");
                if (length == 0)
                {
                    Console.WriteLine("MD5:" + Tools.CalcMD5(dataRead));
                    if (save == true)
                    {
                        SaveFileDialog sf = new SaveFileDialog();
                        sf.FileName = "save_" + chip.GetConfig().ChipModel + "_" + Tools.NowStr("yyyyMMdd_HHmmss") + ".bin";
                        if (sf.ShowDialog() == DialogResult.OK)
                        {
                            string file = sf.FileName;
                            FileStream fs = new FileStream(file, FileMode.Create);
                            BinaryWriter bw = new BinaryWriter(fs);
                            bw.Write(dataRead);
                            bw.Close();

                            //有文件就比较
                            string fileWrite = textBoxFilename.Text;
                            if (!String.IsNullOrEmpty(fileWrite))
                            {
                                FileStream fsWrite = new FileStream(fileWrite, FileMode.Open);
                                BinaryReader br = new BinaryReader(fsWrite);
                                byte[] dataWrite = new byte[chip.GetConfig().ChipLength];
                                br.Read(dataWrite, 0, dataWrite.Length);
                                br.Close();
                                fs.Close();

                                string comp = Tools.Compare(dataWrite, dataRead, 0);
                                if (comp.Length > 0)
                                {
                                    FileStream fsTxt = new FileStream(file + "_diff.txt", FileMode.Create);
                                    fsTxt.Write(Encoding.UTF8.GetBytes(comp), 0, Encoding.UTF8.GetBytes(comp).Length);
                                    fsTxt.Close();
                                }


                            }
                            MessageBox.Show("保存到" + sf.FileName + "\r\nMD5:" + Tools.CalcMD5(dataRead));
                        }
                    }
                }
            }
        }

        private void readLarge(Int64 baseAddr, int length, bool save = true)
        {
            WillemOP.Init();
            if (chip == null) { return; }

            if (length != 0)
            {
                byte[] data = null;
                //部分读取
                data = chip.Read(baseAddr, length, length);
                //显示出来
                for (Int64 i = baseAddr; i < baseAddr + length; i++)
                {
                    if (i % 0x10 == 0)
                    {
                        Console.WriteLine();
                        Console.Write(Tools.int2HexStr(i) + ":");
                    }
                    Console.Write(Tools.byte2HexStr(data[i]) + " ");
                }
            }
            else
            {
                
                Int64 length64 = 0;
                if (length == 0)
                {
                    length64 = chip.GetConfig().ChipLength;
                }
                else
                {
                    length64 = length;
                }
                for (Int64 addr = baseAddr; addr < baseAddr + length64; addr += splitLength)
                {
                    byte[] data = chip.Read(addr, (int)splitLength, chip.GetConfig().ChipLength);
                    FileStream fs = null;
                    if (addr == baseAddr)
                    {
                        fs = new FileStream(tempFilename, FileMode.Create);
                    }
                    else
                    {
                        fs = new FileStream(tempFilename, FileMode.Append);
                    }
                    //Console.WriteLine("写入文件，位置：" + Tools.int2HexStr(addr) + " 长度：" + data.Length);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(data);
                    bw.Close();
                    bw.Dispose();
                    fs.Close();
                    fs.Dispose();
                }
                Thread.Sleep(20);
                WillemOP.Init(true);
                //对文件进行保存
                FileStream fsTemp = new FileStream(tempFilename, FileMode.Open);
                string md5= Tools.CalcMD5(fsTemp);
                Console.WriteLine("MD5:" + md5);
                if (save == true)
                {
                    SaveFileDialog sf = new SaveFileDialog();
                    sf.FileName = "save_" + chip.GetConfig().ChipModel + "_" + Tools.NowStr("yyyyMMdd_HHmmss") + ".bin";
                    if (sf.ShowDialog() == DialogResult.OK)
                    {
                        Tools.CopyFile(tempFilename, sf.FileName);
                        
                        string fileWrite = textBoxFilename.Text;
                        if (!String.IsNullOrEmpty(fileWrite))
                        {
                            string comp = Tools.Compare(fileWrite, tempFilename, 0);
                            if (comp.Length > 0)
                            {
                                FileStream fsTxt = new FileStream(sf.FileName + "_diff.txt", FileMode.Create);
                                fsTxt.Write(Encoding.UTF8.GetBytes(comp), 0, Encoding.UTF8.GetBytes(comp).Length);
                                fsTxt.Close();
                            }
                        }

                        MessageBox.Show("保存到" + sf.FileName + "\r\nMD5:" + md5);
                    }
                }
            }

        }

        private void read(Int64 baseAddr, int length, bool save=true)
        {
            if (chip.GetConfig().ChipLength <= largeChipLength)
            {
                readSmall(baseAddr, length, save);
            }
            else
            {
                readLarge(baseAddr, length,save);
            }
        }

        private void buttonReadId_Click(object sender, EventArgs e)
        {
            WillemOP.Init();
            if (chip != null)
            {
                chip.ReadId();
            }
            WillemOP.Init();
            Console.WriteLine("读取ID结束");
        }

        private void buttonReadLength_Click(object sender, EventArgs e)
        {
            Int64 baseAddr = 0;
            int length = 0;
            if (checkBaseAddrAndLength(out baseAddr, out length) == true)
            {
                read(baseAddr, length);
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ini.Write("Filename", textBoxFilename.Text);
            Ini.Write("ChipSelect", comboBoxChipSelect.Text);
            Ini.Write("TryLength", textBoxTryLength.Text);
            Ini.Write("LPTDeviceType", comboBoxLPTDeviceType.Text);
            Ini.Write("BaseAddr", textBoxBaseAddr.Text);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonOpAuto_Click(object sender, EventArgs e)
        {
            //先加载文件
            byte[] dataWrite = new byte[chip.GetConfig().ChipLength];
            string file = textBoxFilename.Text;
            if (String.IsNullOrEmpty(file))
            {
                MessageBox.Show("先加载文件");
                return;
            }
            else if (!String.IsNullOrEmpty(file))
            {
                //有文件就优先加载文件
                FileStream fs = new FileStream(file, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                br.Read(dataWrite, 0, dataWrite.Length);
                br.Close();
                fs.Close();
            }

            if (checkBoxAutoOpErase.Checked == true)
            {
                
                //擦除
                WillemOP.Init();
                if (chip != null)
                {
                    chip.Erase(chip.GetConfig().EraseDelayTime);
                }
                Thread.Sleep(20);
                WillemOP.Init(true);
            }

            if (checkBoxOpEmptyCheck.Checked == true)
            {
                //查空
                string blank = blankCheck(chip, largeChipLength);
                if (blank.Length > 0)
                {
                    return;
                }
            }

            if (checkBoxOpWrite.Checked == true)
            {
                Thread.Sleep(20);
                WillemOP.Init();
                write(0);
                Thread.Sleep(20);
                WillemOP.Init(true);
            }

            if (checkBoxOpReadAndVerify.Checked == true)
            {
                Thread.Sleep(20);
                WillemOP.Init();
                read(0, 0);
                Thread.Sleep(20);
                WillemOP.Init(true);
            }
        }

        private void buttonBlankCheck_Click(object sender, EventArgs e)
        {
            blankCheck(chip,largeChipLength);
        }

        public string blankCheck(IChip chip, Int64 largeChipLength)
        {
            StringBuilder sb = new StringBuilder();
            WillemOP.Init();
            if (chip != null)
            {
                if (chip.GetConfig().ChipLength <= largeChipLength)
                {
                    byte[] data = null;
                    //完全读取
                    data = chip.Read(0, (int)chip.GetConfig().ChipLength, chip.GetConfig().ChipLength);

                    Thread.Sleep(20);
                    WillemOP.Init();

                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != 0xFF)
                        {
                            Console.WriteLine("NotBlank:\t" + Tools.int2HexStr(i) + " DATA:" + Tools.byte2HexStr(data[i]));
                            sb.AppendLine("NotBlank:\t" + Tools.int2HexStr(i) + " DATA:" + Tools.byte2HexStr(data[i]));
                            if (sb.Capacity > 0x100000) { return sb.ToString(); }   //避免刷爆内存
                        }
                    }
                }
                else
                {
                    this.read(0, 0, false);
                    FileStream fsLength = new FileStream(tempFilename,FileMode.Open);
                    Int64 fileLength = fsLength.Length;
                    fsLength.Close();
                    for (Int64 addr = 0; addr < fileLength; addr += splitLength)
                    {
                        byte[] data = new byte[splitLength];
                        FileStream fs = new FileStream(tempFilename, FileMode.Open);
                        fs.Seek(addr, SeekOrigin.Begin);
                        BinaryReader br = new BinaryReader(fs);
                        br.Read(data, 0, data.Length);
                        br.Close();
                        fs.Close();

                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i] != 0xFF)
                            {
                                Console.WriteLine("NotBlank:\t" + Tools.int2HexStr(addr+i) + " DATA:" + Tools.byte2HexStr(data[i]));
                                sb.AppendLine("NotBlank:\t" + Tools.int2HexStr(addr + i) + " DATA:" + Tools.byte2HexStr(data[i]));
                                if (sb.Capacity > 0x100000) { return sb.ToString(); }  //避免刷爆内存
                            }
                        }
                    }
                }
            }

            Console.WriteLine("空白检测完成");
            return sb.ToString();
        }


        private void buttonBaseAddrIncrease_Click(object sender, EventArgs e)
        {
            Int64 baseAddr = 0;
            Int64 chipLength = chip.GetConfig().ChipLength;
            try
            {
                baseAddr = Convert.ToInt64(textBoxBaseAddr.Text, 16);
                if (baseAddr + 0x100 < chipLength)
                {
                    baseAddr = baseAddr + 0x100;
                    textBoxBaseAddr.Text = "0x" + Convert.ToString(baseAddr, 16).ToUpper().PadLeft(6, '0');
                }
                else
                {
                    textBoxBaseAddr.Text = "0x000000";
                }
            }
            catch
            {
                textBoxBaseAddr.Text = "0x000000";
            }
        }

        private void buttonBaseAddrDecrease_Click(object sender, EventArgs e)
        {
            Int64 baseAddr = 0;
            Int64 chipLength = chip.GetConfig().ChipLength;
            try
            {
                baseAddr = Convert.ToInt64(textBoxBaseAddr.Text, 16);
                if (baseAddr - 0x100 >= 0)
                {
                    baseAddr = baseAddr - 0x100;
                }
                else
                {
                    baseAddr = chipLength - 0x100;
                }
                textBoxBaseAddr.Text = "0x" + Convert.ToString(baseAddr, 16).ToUpper().PadLeft(6, '0');
            }
            catch
            {
                textBoxBaseAddr.Text = "0x000000";
            }
        }

        private void statusStrip1_Click(object sender, EventArgs e)
        {
            if (toolStripStatusLabel1.Text!=null && toolStripStatusLabel1.Text.Length > 0)
            {
                MessageBox.Show(toolStripStatusLabel1.Text);
            }
        }

        private void comboBoxLPTDeviceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LPT = LPTFactory.create(comboBoxLPTDeviceType.Text);
            LPTConfig config = LPT.GetConfig();
            labelLPTNote.Text = config.LPTNote;
            pictureBoxLPTNoteImage.Image = config.LPTNoteImage;
            buttonLPTParamConfig.Visible = string.IsNullOrEmpty(LPT.GetConfig().LPTParam)?false:true;
            Ini.Write("LPTDeviceType", comboBoxLPTDeviceType.Text);
        }

        private void buttonSpecialFunction_Click(object sender, EventArgs e)
        {
            chip.SpecialFunction(textBoxFilename.Text, textBoxEraseDelay.Text, textBoxBaseAddr.Text, textBoxTryLength.Text);
        }

        private void textBoxFilename_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)                textBoxFilename.SelectAll();
        }

        private void buttonLPTParamConfig_Click(object sender, EventArgs e)
        {
            LPTParamConfigForm lpcf = new LPTParamConfigForm();
            lpcf.ShowDialog();
        }

        

    }
}
