using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace willem_winio32
{
    public class EmptyChip : IChip
    {
        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return new byte[1];
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
        }

        public void Erase(string args)
        {
        }


        public byte[] ReadId()
        {
            return new byte[1];
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

        public ChipConfig GetConfig()
        {
            return new ChipConfig();
        }
    }
}
