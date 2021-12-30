using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace willem_winio32
{
    //改部分写入、读取：ok
    public class MX29L3211 : IChip
    {
        private int chipsize = 0x400000;

        MX29F1610 mx29f1610 = new MX29F1610();
        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return mx29f1610.Read(baseAddr,length, totalLength);
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            mx29f1610.Write(data, baseAddr,length, totalLength);
        }

        public void Erase(string args)
        {
            mx29f1610.Erase(args);
        }

        public byte ReadReg()
        {
            return mx29f1610.ReadReg();
        }

        public byte[] ReadId()
        {
            return mx29f1610.ReadId();
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = mx29f1610.GetConfig();
            config.ChipLength = chipsize;
            config.ChipModel = "MX29L3211";
            config.Jumper = willem_winio32.Properties.Resources.MX29L3211_SOP_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_8Bit_Adapter_MX29L3211;
            return config;
        }
    }
}
