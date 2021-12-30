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
        private double cmdDelay = 0.1;
        MX26L6420 L6420 = new MX26L6420();
        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return L6420.Read(baseAddr, length,totalLength);
        }

        public string procReg()
        {
            WillemOP.SetCE_H();
            WillemOP.SetData(0x70);
            WillemOP.SetCE_L();
            WillemOP.SetCE_H();

            byte b = WillemOP.Read4021();
            Console.WriteLine("Write Reg:" + Tools.byte2Str(b));

            if ((b & 0x80) == (0x80))
            {
                return "true";
            }
            return "continue";
        }

        //public void ClearReg(int i)
        //{
        //    WillemOP.SetCE_H();
        //    WillemOP.SetAddr(baseAddr + i);
        //    WillemOP.SetData(0x50);
        //    WillemOP.SetCE_L();
        //}

        public void WriteBlock(byte[] data, Int64 startAddr, Int64 endAddr)
        {
        REDO:
            WillemOP.SetVPP_H();
            WillemOP.SetCE_H();
            WillemOP.SetAddr(startAddr);
            WillemOP.SetData(0xE8);
            WillemOP.SetCE_L();
            WillemOP.SetCE_H();
            Tools.delayUs(0.02);
            //WillemOP.SetVPP_L();
            //Thread.Sleep(10);
            //byte ready = WillemOP.Read4021();
            //Console.WriteLine("AReady:" + Tools.byte2Str(ready));
            //if ((ready & 0x80) != 0x80)
            //{
            //    Console.WriteLine("NotReady:" + Tools.byte2Str(ready));
            //    Tools.delayUs(0.2);
            //    goto REDO;
            //}

            Int64 opAddr = (startAddr / blocksize) * blocksize;//无语要取块的首地址！
            //            Console.WriteLine("opAddr:" + Tools.int2HexStr(opAddr));
            WillemOP.SetAddr(opAddr);
            WillemOP.SetData(0xF);
            WillemOP.SetCE_L();
            WillemOP.SetCE_H();

            for (Int64 i = startAddr; i < endAddr; i = i + 2)
            {
                Console.WriteLine("写入地址：" + Tools.int2HexStr(i) + " 数据:" + Tools.byte2HexStr(data[i]));
                WillemOP.SetAddr(i + 1);
                WillemOP.SetData(data[i + 1]);
                WillemOP.SetAddr(i);
                WillemOP.SetData(data[i]);

                WillemOP.SetCE_L();
                WillemOP.SetCE_H();
            }
            WillemOP.SetCE_H();
            WillemOP.SetAddr(opAddr);
            WillemOP.SetData(0xD0);
            WillemOP.SetCE_L();
            WillemOP.SetCE_H();
        }

        public void WriteWord2(byte[] data, int baseAddr, int length)
        {
            WillemOP.SetCE_H(); WillemOP.SetVCC_H(); Thread.Sleep(1000);
            WillemOP.SetVPP_H();
            Thread.Sleep(100);
            DateTime startTime = System.DateTime.Now;

            for (int i = baseAddr; i < (baseAddr + length); i = i + 2)
            {
                WillemOP.SetVPP_H();
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetData(0x40);
                WillemOP.SetCE_L();
                WillemOP.SetCE_H();
                Tools.delayUs(0.02);

                WillemOP.SetAddr(i + 1);
                //                WillemOP.SetData(0xFF);
                WillemOP.SetData(data[i + 1]);

                WillemOP.SetAddr(i);
                //                WillemOP.SetData(0xFF);
                WillemOP.SetData(data[i]);


                WillemOP.SetCE_L();
                Tools.delayUs(1);
                WillemOP.SetCE_H();
                Tools.delayUs(1);

                //byte reg = WillemOP.Read4021();
                //Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));

                if (i % 0x20 == 0)
                {
                    //WillemOP.SetVPP_L();
                    //Thread.Sleep(2000);
                }
                if (data[i] == 0 && data[i + 1] == 0)
                {
                    //Thread.Sleep(2000);
                }
                procReg();

                Tools.ShowProgress(i, data, baseAddr, length);
            }
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            DateTime startTime = System.DateTime.Now;

            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(1000);

            for (Int64 i = baseAddr; i < baseAddr + (Int64)length; i = i + 0x20)
            {
                WillemOP.SetCE_H();
                WillemOP.SetVCC_H();
                WillemOP.SetVPP_H();
                Thread.Sleep(40);
                //Console.WriteLine("buffer:" + Tools.int2HexStr(i));
                WriteBlock(data, i, i + 0x20);

                Tools.ShowProgress(i, data, baseAddr, length);
            }

            //WriteWord2(data, baseAddr,length);
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
                int blockAddr = Convert.ToInt32(args,16);
                EraseBlock(blockAddr);
            }
            catch
            {
                for (int i = 0; i < chipsize; i = i + blocksize)
                {
                    EraseBlock(i);
                }
            }
        }

        private void EraseBlock(int blockAddr)
        {
            //擦除块
            WillemOP.Write16BitCommand(blockAddr/2, 0xFF, 0x20);
            WillemOP.Write16BitCommand(blockAddr/2, 0xFF, 0xD0);

            WillemOP.SetCE_H();
            //                Thread.Sleep(10000);

            for (int s = 0; s < 100; s++)
            {
            //RE_ERASE:
                Thread.Sleep(1000);
                //WillemOP.SetCE_H();
                //WillemOP.SetData(0x70);
                //WillemOP.SetCE_L();
                //WillemOP.SetCE_H();
                byte b = WillemOP.Read4021();
                Console.WriteLine("擦除块地址：" + Tools.int2HexStr(blockAddr) + " 16位地址：" + Tools.int2HexStr(blockAddr / 2) + ":" + Tools.byte2Str(b));
                if (b == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(1000);
                        byte b2 = WillemOP.Read4021();
                        Console.WriteLine("擦除块地址：" + Tools.int2HexStr(blockAddr) + " 16位地址：" + Tools.int2HexStr(blockAddr / 2) + ":" + Tools.byte2Str(b));
                        if (b2 != 0)
                        {
                            //goto RE_ERASE;
                        }
                    }
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
