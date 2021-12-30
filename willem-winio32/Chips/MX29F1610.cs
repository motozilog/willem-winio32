using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    //改部分写入、读取：ok
    public class MX29F1610 : IChip
    {
        private int chipsize = 0x200000;
        private bool debug = false;
        MX29F1615 mx29f1615 = new MX29F1615();

        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ReadId = true;
            config.Register = true;
            config.EraseDelay = true;
            config.EraseDelayTime = "60";
            config.Note = "MX29F1610/MX29L3211会有30us超时自动写入的问题，因为要检查寄存位，所以写入速度会很慢。推荐采用M59PW016代替";

            config.ChipLength = chipsize;
            config.ChipModel = "MX29F1610";
            config.DipSw = willem_winio32.Properties.Resources.MX29F1615;
            config.Jumper = willem_winio32.Properties.Resources.MX29F1610_SOP_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_8Bit_Adapter_MX29F1610;
            return config;
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
        }

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            return mx29f1615.Read(baseAddr, length, totalLength);
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            //写的时候要16bit
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_L();
            Thread.Sleep(2000);

            DateTime startTime = System.DateTime.Now;

            for (Int64 i = baseAddr; i < baseAddr + (Int64)length; i++)
            {
                if (i == baseAddr)
                {
                    for (int w = 0; w < 10; w++)
                    {
                        //Console.Write("ADDR:"+i);
                        byte reg = mx29f1615.ReadReg();
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

                EpromOp.MX29Write(i, data[i]);

                

                WillemOP.SetCE_H();
                //Tools.delayUs(0.05);


                for (int w = 0; w < 10; w++)
                {
                    //Console.Write("ADDR:"+i);
                    //byte reg = WillemOP.Read4021();
                    byte reg = mx29f1615.ReadReg();
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
            WillemOP.SetVPP_L();
            Thread.Sleep(2000);

            EpromOp.MX29Erase();

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

        public byte ReadReg()
        {
            return mx29f1615.ReadReg();
        }

        public byte[] ReadId()
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_L();
            Thread.Sleep(2000);


            byte[] id = EpromOp.MX29ReadId();
            Console.WriteLine("厂商代码(正确为0xC2)：" + Tools.byte2HexStr(id[0]));
            Console.WriteLine("设备代码(正确为0xFA(1610A)或FB(1610A)或F1(1610)或F9(3211))：" + Tools.byte2HexStr(id[1]));
            return id;
        }

       
    }
}
