using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace willem_winio32
{
    //改部分写入：ok
    public class W27C4096 : IChip
    {
        private int chipsize = 0x80000;

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return new byte[0];
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            EpromOp.W27CWrite(data,baseAddr, length);
        }

        public void Erase(string args)
        {
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Write = true;
            config.ChipLength = chipsize;
            config.ChipModel = "W27C4096";
            config.DipSw = willem_winio32.Properties.Resources.W27C4096;
            config.Jumper = willem_winio32.Properties.Resources.W27C4096AndMX29F1615Jumper;
            config.Adapter = willem_winio32.Properties.Resources.DIP42_Adapter_W27C4096;
            config.Note = "W27C4096擦除，请用W27C4096擦除适配器进行";
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
