using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace willem_winio32
{
    //改部分写入：ok
    public class W27C4096 : IChip
    {
        private int chipsize = 0x80000;

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            byte[] data = EpromOp.W27CRead(baseAddr, length);
            return data;
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            EpromOp.W27CWrite(data,baseAddr, length);
        }

        public void Erase(string args)
        {
            W27C4096EraseForm ef = new W27C4096EraseForm();
            ef.ShowDialog();
            EpromOp.W27CErase(chipsize);
            MessageBox.Show("请更换回DIP42适配器，并将Willem跳线恢复");
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Write = true;
            config.Erase = true;
            config.Read = true;
            config.ChipLength = chipsize;
            config.ChipModel = "W27C4096";
            config.DipSw = willem_winio32.Properties.Resources.W27C4096;
            config.Jumper = willem_winio32.Properties.Resources.W27C4096AndMX29F1615Jumper;
            config.Adapter = willem_winio32.Properties.Resources.DIP42_Adapter_W27C4096;
            config.Note = "W27C4096擦除，请用W27C4096擦除适配器进行，注意擦除时要跳线(A9=VPP)";
            return config;
        }


        
        public byte[] ReadId()
        {
            return new byte[0];
        }


        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

    }
}
