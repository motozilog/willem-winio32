using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    //改部分写入、读取：ok
    public class MX29L3211_16Bit : IChip
    {
        private int chipsize = 0x400000;

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return EpromOp.MX29Read(baseAddr, length, totalLength);
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            //写的时候要16bit
            WillemOP.SetCE_L();
            WillemOP.SetVCC_H();
            Thread.Sleep(500);
            WillemOP.SetVPP_L();
            Thread.Sleep(200);

            DateTime startTime = System.DateTime.Now;

            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                if (i == baseAddr)
                {
                    for (int w = 0; w < 100; w++)
                    {
                        byte reg = ReadReg();
                        if ((reg & 0x80) != (0x80))
                        {
                            Tools.delayUs(0.02);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                WillemOP.SetVPP_H();

                EpromOp.MX29WriteCommand(i, data[i + 1], data[i]);

                for (int w = 0; w < 100; w++)
                {
                    byte reg = ReadReg();
                    //Console.WriteLine(Tools.byte2Str(reg));
                    if ((reg & 0x80) != (0x80))
                    {
                        Tools.delayUs(0.02);
                    }
                    else
                    {
                        break;
                    }
                }

                Tools.ShowProgress(i, data, baseAddr, length);
            }
        }

        public void Erase(string args)
        {
            int waitTime = Convert.ToInt32(args);

            //初始化
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(200);

            EpromOp.MX29EraseCommand();
            WillemOP.SetVPP_L();

            Console.WriteLine("请等" + waitTime + "秒");
            //要设CE(即WE为H)
            WillemOP.SetCE_H();
            Thread.Sleep(waitTime * 1000);
            Console.WriteLine("耗时:" + waitTime + "秒");

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("增长耗时:" + i * 1 + "秒");
                Thread.Sleep(1000);
                byte reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2Str(reg));

                if ((reg & 0x80) == (0x80))
                {
                    return;
                }
                if (i > 60)
                {
                    Console.WriteLine("增长超过60秒，仍未检测到D7为1，可能芯片是坏的");
                }
            }
        }

        bool debug = false;
        public byte ReadReg()
        {
            WillemOP.SetCE_H(); //WE=H
            EpromOp.MX29ReadRegCommandOnly();
            WillemOP.SetVPP_L();    //OE=L
            byte bL = WillemOP.Read4021();
            if (debug)
            {
                Console.WriteLine(Tools.byte2Str(bL));
            }
            return bL;
        }

        public byte[] ReadId()
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(200);

            byte[] id = EpromOp.MX29ReadIdCommand();
            Console.WriteLine("厂商代码(正确为0xC2)：" + Tools.byte2HexStr(id[0]));
            Console.WriteLine("设备代码(正确为0xFA(1610A)或FB(1610A)或F1(1610)或F9(3211))：" + Tools.byte2HexStr(id[1]));
            return id;
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

        public ChipConfig GetConfig()
        {
            MX29F1610 mx29f1610 = new MX29F1610();
            ChipConfig config = mx29f1610.GetConfig();
            config.ChipLength = chipsize;
            config.ChipModel = "MX29L3211";
            config.DipSw = willem_winio32.Properties.Resources.MX26L6420;
            config.Jumper = willem_winio32.Properties.Resources.MX29L3211_SOP16_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_16Bit_Adapter_MX29L3211;
            return config;
        }
    }
}
