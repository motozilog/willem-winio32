using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace willem_winio32
{
    //改部分写入、读取：ok
    public class W27X010 : IChip
    {
        private int chipsize = 0x20000;
        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ChipLength = chipsize;
            config.ChipModel = "W27X010";
            config.DipSw = willem_winio32.Properties.Resources.W27X010;
            config.Jumper = willem_winio32.Properties.Resources.W27X010_Erase;
            config.Note = "W27C010/W27E010，注意擦除时要跳线(A9=VPP)";

            return config;
        }


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
            EpromOp.W27CErase(chipsize);
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
