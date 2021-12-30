using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    //改部分写入、读取：ok
    public class MX29F1615 : IChip
    {
        private int chipsize = 0x200000;
        private bool debug = false;
        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ReadId = true;
            config.Register = true;
            config.Note = "MX29F1615写入时均不检查寄存器，请注意校验和补写。";

            config.ChipLength = chipsize;
            config.ChipModel = "MX29F1615";
            config.DipSw = willem_winio32.Properties.Resources.MX29F1615;
            config.Jumper = willem_winio32.Properties.Resources.W27C4096AndMX29F1615Jumper;
            config.Adapter = willem_winio32.Properties.Resources.DIP42_Adapter_MX29F1615;
            return config;
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            //读的时候要8bit
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_L();
            Thread.Sleep(2000);

            //先复位
            //EpromOp.MX29Reset();
            byte[] data = new byte[baseAddr+length];

            //再用8bit读取
            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                byte b = WillemOP.Read4021();
                data[i] = b;
                WillemOP.SetCE_H();

                Tools.ShowProgress(i, data, baseAddr, length);
            }
            return data;
        }


        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            //写的时候要16bit
            WillemOP.SetCE_H();

            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(2000);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {

                EpromOp.MX29Write(i, data[i]);

                Tools.ShowProgress(i, data, baseAddr, length);
                //延迟
                Tools.delayUs(0.02);               
            }
        }

        public void Erase(string args)
        {
            //初始化
            WillemOP.SetCE_H();

            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(2000);


            EpromOp.MX29Erase();

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("耗时:" + i * 3 + "秒");
                Thread.Sleep(3000);
                byte reg = ReadReg();
                Console.WriteLine(Tools.byte2Str(reg));
                if ((reg & 0x80) == (0x80))
                {
                    //成功
                    return;
                }
                if (i > 20)
                {
                    Console.WriteLine("超过60秒，仍未检测到D7为1，可能芯片是坏的");
                }
            }
            
        }

        public byte ReadReg()
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            //MX29F1615读寄存器要将VPP置VIL，而不是datasheet写的VHH
            WillemOP.SetVPP_L();
            //Thread.Sleep(2000);

            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55);

            //3 0x5555 0x70
            WillemOP.Write16BitCommand(0x5555, 0x70);

            WillemOP.SetCE_H();
            WillemOP.SetAddr(0);
            WillemOP.SetCE_L();
            byte bL = WillemOP.Read4021();
            if (debug)
            {
                Console.WriteLine(Tools.byte2Str(bL));
            }
            WillemOP.SetCE_H();
            return bL;
        }

        public byte[] ReadId()
        {
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();

            byte[] id = EpromOp.MX29ReadId();
            Console.WriteLine("厂商代码(正确为0xC2)：" + Tools.byte2HexStr(id[0]));
            Console.WriteLine("设备代码(正确为0x6B)：" + Tools.byte2HexStr(id[1]));
            return id;           
        }

       
    }
}
