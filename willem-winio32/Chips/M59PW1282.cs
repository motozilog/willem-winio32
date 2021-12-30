using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace willem_winio32
{
    public class M59PW1282 : IChip
    {
        //private int chipsize = 0x400000;
        private int chipsize = 0x1000000;
        M59PW016 pw016 = new M59PW016();
        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return pw016.Read(baseAddr, length, totalLength);
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            if (length < chipsize)
            {
                Console.WriteLine("试写不做die切换，同时写到2die上，即0x000000 与 0x800000的内容相同");
                MessageBox.Show("请将适配板拨码8置OFF(此时8与9均为OFF状态)");
                MessageBox.Show("请将适配板拨码9置ON");
                pw016.Write(data, baseAddr, length, totalLength);
                MessageBox.Show("请将适配板拨码9置OFF(此时8与9均为OFF状态)");
                MessageBox.Show("请将适配板拨码8置ON");
                return;
            }

            //完整写入
            if (length == chipsize)
            {
                Console.WriteLine("完整写入");
                switchDieL();
                pw016.Write(data, 0, 0x800000, totalLength);
                WillemOP.SetVPP_L();    //完成后立即关断VPP，避免80小时的VPP

                switchDieH();
                pw016.Write(data, 0x800000, 0x800000,totalLength);
                eraseOrWriteFinish();
            }
        }

        public void Erase(string args)
        {
            switchDieL();
            pw016.Erase("FULL");
            WillemOP.SetVPP_L();    //完成后立即关断VPP，避免80小时的VPP

            switchDieH();
            pw016.Erase("FULL");
            eraseOrWriteFinish();
        }

        private void switchDieL()
        {
            MessageBox.Show("M59PW1282的擦除/写入较为复杂，请严格按照指引进行。请将适配板拨码9置OFF，并将拨码8置ON");
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            Thread.Sleep(1000);
            WillemOP.SetVPP_H();
            Thread.Sleep(1000);
            int addr = 0;
            addr = Tools.setBit(addr, 10, 1);
            addr = Tools.setBit(addr, 23, 0);
            WillemOP.SetAddr(addr);
            WillemOP.SetData(0xFF);
            WillemOP.SetCE_L();
            MessageBox.Show("当前die区域0x000000~0x7FFFFF，请按一下适配板上的触发开关(按一下即可，勿长按，否则烧芯片)");
            MessageBox.Show("请将适配板拨码8置OFF(此时8与9均为OFF状态)");
            MessageBox.Show("请将适配板拨码9置ON");
        }

        private void switchDieH()
        {

            MessageBox.Show("即将切换到高8M die(0x800000~0xFFFFFF)，请将适配板拨码9置OFF(此时8与9均为OFF状态)");
            MessageBox.Show("即将切换到高8M die(0x800000~0xFFFFFF)，请将适配板拨码8置ON");
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            int addr = 0;
            addr = Tools.setBit(addr, 10, 1);
            addr = Tools.setBit(addr, 23, 1);
            WillemOP.SetAddr(addr);
            WillemOP.SetData(0xFF);
            WillemOP.SetCE_L();
            MessageBox.Show("当前die区域0x800000~0xFFFFFF，请按一下适配板上的触发开关(按一下即可，勿长按，否则烧芯片)");
            MessageBox.Show("请将适配板拨码9置ON(此时8与9均为ON状态，对！是ON，不是OFF)");
            MessageBox.Show("请将适配板拨码8置OFF");
        }

        private void eraseOrWriteFinish()
        {
            Thread.Sleep(1000);
            WillemOP.SetVPP_L();
            Thread.Sleep(1000);
            WillemOP.SetVCC_L();
            MessageBox.Show("请将适配板拨码9置OFF(此时8与9均为OFF状态)");
            MessageBox.Show("请将适配板拨码8置ON");
        }

        public byte[] ReadId()
        {
            MessageBox.Show("请将适配板拨码8置OFF(此时8与9均为OFF状态)");
            MessageBox.Show("请将适配板拨码9置ON");
            byte[] ids = pw016.ReadId();
            MessageBox.Show("请将适配板拨码9置OFF(此时8与9均为OFF状态)");
            MessageBox.Show("请将适配板拨码8置ON");
            return ids;
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
            //先加载文件
            byte[] data = new byte[chipsize];
            if (String.IsNullOrEmpty(Filename))
            {
                MessageBox.Show("先加载文件");
                return;
            }
            else if (!String.IsNullOrEmpty(Filename))
            {
                //有文件就优先加载文件
                FileStream fs = new FileStream(Filename, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                br.Read(data, 0, data.Length);
                br.Close();
                fs.Close();
            }

            switchDieH();
            pw016.Write(data, 0x800000, 0x800000,chipsize);
            eraseOrWriteFinish();
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = pw016.GetConfig();
            config.ChipLength = chipsize;
            config.ChipModel = "M59PW1282";
            config.Note = "M59PW1282擦除和写入操作较为复杂，且拨动顺序不能有错。否则只能用其中的8Mbyte，而不用了16Mbyte。" + config.Note+"(M59PW1282暂时只支持整die擦除，不支持部分block擦除)";
            config.EraseDelay = false;
            config.Jumper = willem_winio32.Properties.Resources.M59PW1282_Sub_SW;
            config.SpecialFunction = "仅写高8M die";
            return config;
        }

    }
}
