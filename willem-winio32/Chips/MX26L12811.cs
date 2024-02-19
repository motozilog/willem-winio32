using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    public class MX26L12811 : IChip
    {
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));
        private int chipsize = 0x1000000;
        private int blocksize = 0x20000;

        MX26L6420 L6420 = new MX26L6420();
        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            //读的时候要8bit
            WillemOP.SetVCC_L();
            WillemOP.SetVPP_L();
            Thread.Sleep(5000);

            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(3000);
            WillemOP.SetCE_L();

            WillemOP.SetVPP_L();
            Thread.Sleep(3000);

            byte[] data = new byte[baseAddr + (Int64)length];

            WillemOP.SetCE_H();
            WillemOP.SetAddr(2);
            WillemOP.SetCE_L();
            WillemOP.Read4021();

            //再用8bit读取
            for (Int64 i = baseAddr; i < (Int64)data.Length; i++)
            {
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                byte b = WillemOP.Read4021();
                data[i] = b;
                //WillemOP.SetCE_H();

                Tools.ShowProgress(i, data, baseAddr, length);
            }
            return data;
        }

        public string procReg()
        {
            WillemOP.SetCE_H();
            WillemOP.SetData(0x70);
            WillemOP.SetCE_L();
            WillemOP.SetCE_H();
            //read status
            WillemOP.SetDataMode();
            byte b = WillemOP.Read4021();
            Console.WriteLine("Write Reg:" + Tools.byte2Str(b));

            if ((b & 0x80) == (0x80))
            {
                return "true";
            }
            return "continue";
        }


        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            DateTime startTime = System.DateTime.Now;

            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(3000);
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();

            Thread.Sleep(3000);
            WillemOP.SetVPP_H();
            Thread.Sleep(100);

            for (int i = (int)baseAddr; i < (baseAddr + length); i = i + 2)
            {
                WillemOP.SetVPP_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                WillemOP.SetData(0x40);
                WillemOP.SetCE_H();

                WillemOP.SetAddr(i + 1);
                WillemOP.SetData(data[i + 1]);
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_H();

                for (int s = 0; s < 100; s++)
                {
                    int delay = 1;
                    WillemOP.SetCE_H();
                    WillemOP.SetVPP_L();
                    Tools.delayUs(delay);
                    byte b = WillemOP.Read4021();
                    //Console.WriteLine("擦除块地址：" + Tools.byte2Str(b));
                    WillemOP.SetVPP_H();
                    Tools.delayUs(delay);
                    WillemOP.Write16BitCommand(0, 0x00, 0x70);
                    WillemOP.SetCE_H();
                    WillemOP.SetVPP_L();
                    Tools.delayUs(delay);
                    WillemOP.SetDataMode();
                    byte be = WillemOP.Read4021();
                    //Console.WriteLine("e擦除块地址：" +  Tools.byte2Str(be));
                    WillemOP.SetVPP_H();
                    Tools.delayUs(delay);
                    WillemOP.SetCE_L();
                    if (be == 0x80 && b == 0x80)
                    {
                        break;
                    }
                }

                Tools.ShowProgress(i, data, baseAddr, length);
            }
        }

        public void Erase(string args)
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            Thread.Sleep(1000);
            WillemOP.SetVPP_H();
            Thread.Sleep(1000);
            try
            {
                int blockAddr = Convert.ToInt32(args, 16);
                EraseBlock(blockAddr);
            }
            catch
            {
                for (int i = 0; i < chipsize; i = i + blocksize)
                {
                    EraseBlock(i);
                }
            }

            WillemOP.SetCE_H();
            WillemOP.SetVCC_L();
            Thread.Sleep(1000);
            WillemOP.SetVPP_L();
            Thread.Sleep(1000);
        }

        private void EraseBlock(int blockAddr)
        {
            //擦除块
            WillemOP.Write16BitCommand(blockAddr / 2, 0xFF, 0x20);
            WillemOP.Write16BitCommand(blockAddr / 2, 0xFF, 0xD0);

            for (int s = 0; s < 100; s++)
            {
                WillemOP.SetCE_H();
                WillemOP.SetVPP_L();
                Thread.Sleep(200);
                //WillemOP.SetCE_L();
                //WillemOP.SetDataMode();
                byte b = WillemOP.Read4021();
                Console.WriteLine("擦除块地址：" + Tools.int2HexStr(blockAddr) + " 16位地址：" + Tools.int2HexStr(blockAddr / 2) + ":" + Tools.byte2Str(b));
                WillemOP.SetVPP_H();
                Thread.Sleep(200);
                WillemOP.Write16BitCommand(0, 0x00, 0x70);
                WillemOP.SetCE_H();
                WillemOP.SetVPP_L();
                Thread.Sleep(200);
                WillemOP.SetDataMode();
                byte be = WillemOP.Read4021();
                Console.WriteLine("e擦除块地址：" + Tools.int2HexStr(blockAddr) + " 16位地址：" + Tools.int2HexStr(blockAddr / 2) + ":" + Tools.byte2Str(be));
                WillemOP.SetVPP_H();
                Thread.Sleep(200);
                WillemOP.SetCE_L();
                if (be == 0x80 && b==0x80)
                {
                    break;
                }
            }
        }

        public byte[] ReadId()
        {
            return L6420.ReadId();
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ReadId = true;
            config.Register = false;
            config.EraseDelay = true;
            config.EraseDelayTime = "FULL";
            config.Note = "MX26L12811只有10次的擦除/写入次数，很容易出现坏byte。建议仅进行读取，写入建议采用M59PW1282代替。";

            config.ChipLength = chipsize;
            config.ChipModel = "MX26L12811";
            config.DipSw = willem_winio32.Properties.Resources.MX26L6420;
            config.Jumper = willem_winio32.Properties.Resources.MX26L12811_SOP_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_16Bit_Adapter;
            return config;
        }
    }
}
