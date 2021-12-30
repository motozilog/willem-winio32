using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    //改部分写入、读取：ok

    public class MX26L6420 : IChip
    {
        private int chipsize = 0x800000;
        M59PW016 pw016 = new M59PW016();
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));
        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            //读的时候要8bit
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(1000);
            WillemOP.Write16BitCommand(0x555, 0, 0xF0);
            WillemOP.SetCE_H();

            WillemOP.SetVPP_L();
            Thread.Sleep(500);

            byte[] data = new byte[baseAddr + (Int64)length];

            //再用8bit读取
            for (Int64 i = baseAddr; i < (Int64)data.Length; i++)
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
            WillemOP.SetCE_H(); WillemOP.SetVCC_H(); Thread.Sleep(2000);
            WillemOP.SetVPP_H();
            Thread.Sleep(2000);

            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                //1 0x5555 0xAA
//            WAIT:
                WillemOP.Write16BitCommand(0x555, 0, 0xAA);

                //2 0x2AAA 0x55
                WillemOP.Write16BitCommand(0x2AA,0, 0x55);

                //3 0x5555 0xA0
                WillemOP.Write16BitCommand(0x555,0, 0xA0);

                WillemOP.SetCE_H();
                Console.WriteLine("Write ADDR:" + Tools.int2HexStr(i) + " DATA:" + data[i]);
                WillemOP.SetAddr(i + 1);
                WillemOP.SetData(data[i+1]);

                WillemOP.SetAddr(i);
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_L();
                WillemOP.SetCE_H();
                Tools.delayUs(0.2);
                if (i % 0x10000 == 0)
                {
                    Thread.Sleep(3);
                }
                //for (int s = 0; s < 5; s++)
                //{
                    //WillemOP.SetVPP_L();
                    //Tools.delayUs(0.2);
                    //byte reg = WillemOP.Read4021();
                    
                    //WillemOP.SetVPP_H();
                    //if (reg != data[i] && i > 0 && ((reg & 0x80) != (0x80)))
                    //{
                    //    Console.WriteLine("NO EQUAL:"+ Tools.int2HexStr(i)+":"+Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));
                    //    goto WAIT;
                    //}
                //    if ((reg & 0x80) == (0x80))
                //    {
                //        return;
                //    }

                //}

                Tools.ShowProgress(i, data, baseAddr, length);
            }
        }

        public void Erase(string args)
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            Thread.Sleep(2000);
            WillemOP.SetVPP_H();
            Thread.Sleep(2000); 

            //1 0x555 0xAA
            WillemOP.Write16BitCommand(0x555, 0, 0xAA);

            //2 0x2AA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0, 0x55);

            //3 0x555 0x80
            WillemOP.Write16BitCommand(0x555, 0, 0x80);

            //4 0x555 0xAA
            WillemOP.Write16BitCommand(0x555, 0, 0xAA);

            //5 0x2AA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0, 0x55);

            //6 0x555 0x10
            WillemOP.Write16BitCommand(0x555, 0, 0x10);

            WillemOP.SetCE_H();
            int waitTime = 2;
            Console.WriteLine("请等" + waitTime + "秒");
            //要设CE(即WE为H)
            Thread.Sleep(waitTime * 1000);
            Console.WriteLine("耗时:" + waitTime + "秒");

            WillemOP.SetVPP_L();
            Thread.Sleep(20);
            for (int i = 0; i < 500; i++)
            {
                Console.WriteLine("增长耗时:" + i * 1 + "秒");
                Thread.Sleep(1000);
                WillemOP.SetCE_H();
                //                WillemOP.SetAddr(i);
                //                WillemOP.SetCE_L();
                byte reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));

                if ((reg & 0x80) == (0x80))
                {
                    for (int l = 0; l < 5; l++)
                    {
                        Thread.Sleep(1000);
                        WillemOP.SetCE_H();
                        //                WillemOP.SetAddr(i);
                        //                WillemOP.SetCE_L();
                        byte reg2 = WillemOP.Read4021();
                        Console.WriteLine(Tools.byte2HexStr(reg2) + ":" + Tools.byte2Str(reg2));
                    }

                    return;
                }
                if (i > 300)
                {
                    Console.WriteLine("增长超过300秒，仍未检测到D7为1，可能芯片需要二次擦除");
                }
            }

        }

        public byte[] ReadId()
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(20); 

            //1 0x555 0xAA
            WillemOP.Write16BitCommand(0x555, 0, 0xAA);

            //2 0x2AA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0, 0x55);

            //3 0x555 0x70
            WillemOP.Write16BitCommand(0x555,0, 0x90);

            WillemOP.SetVPP_L();

            byte[] id = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                byte b = WillemOP.Read4021();
                id[i] = b;
            }

            Console.WriteLine("厂商代码(正确为0x00C2)：" + Tools.byte2HexStr(id[1]) + Tools.byte2HexStr(id[0]));
            Console.WriteLine("设备代码(正确为0x22FC)：" + Tools.byte2HexStr(id[3]) + Tools.byte2HexStr(id[2]));
            return id;
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
            config.EraseDelay = false;
            config.Note = "MX26L6420的写入失败率很高，建议采用M59PW064代替。MX26L6420写入后注意校验，若出现几个byte写错时，使用试写部分功能进行补写。擦除时，如果不跳码连续30秒以上，基本上要关掉重新擦除。擦除多次仍未成功时，芯片可能就寿命结束（MX26L6420只有100次写入）。MX26L6420不检查寄存器，因为寄存器超时时会变成内容读取模式。";

            config.ChipLength = chipsize;
            config.ChipModel = "MX26L6420";
            config.DipSw = willem_winio32.Properties.Resources.MX26L6420;
            config.Jumper = willem_winio32.Properties.Resources.MX26L6420_SOP_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_16Bit_Adapter;
            return config;
        }
    }
}
