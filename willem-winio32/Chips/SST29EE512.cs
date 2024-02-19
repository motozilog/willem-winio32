using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    
    public class SST29EE512 : IChip
    {
        private int chipsize = 0x10000;
        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ReadId = true;
            config.ChipLength = chipsize;
            config.ChipModel = "SST29EE512";
            config.DipSw = willem_winio32.Properties.Resources.W27X010;
            //config.Jumper = willem_winio32.Properties.Resources.W27X010_Erase;
            config.Note = "请使用AT29C512来写入，本代码的写入是有问题的，暂时不知道怎样修";

            return config;
        }

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            byte[] data = new byte[baseAddr + length];

            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(1000);
            //设置WE为高电平，读取使能
            WillemOP.SetCE_H();

            //设置读取地址
            for (Int64 i = baseAddr; i < (baseAddr + length); i++)
            {
                WillemOP.SetAddr(i);
                byte b = WillemOP.Read4021();
                data[i] = b;
                Tools.ShowProgress(i, data, baseAddr, length);
            }
            return data;
        }


        //S6:near success
        public void Write(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            Thread.Sleep(100);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                if ((i % 128 == 0))
                {
                    if (i > 0)
                    {
                        //WillemOP.SetAddr(i - 1);
                        Thread.Sleep(100);
                        //for (int r = 0; r < 8; r++)
                        //{
                        //Tools.delayUs(100);
                        //byte lastReg = WillemOP.Read4021();
                        //Tools.delayUs(100);
                        //Console.WriteLine(Tools.int2HexStr(i) + " lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));
                        //}
                    }


                    WillemOP.SetCE_H();
                    WillemOP.Write8BitCommandData(0x5555, 0xAA);
                    WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                    WillemOP.Write8BitCommandData(0x5555, 0xA0);
                    WillemOP.Write8BitCommandData(i, data[i]);
                    Tools.delayUs(0.1);
                    WillemOP.SetCE_H();
                    Tools.delayUs(0.1);
                }
                else
                {
                    WillemOP.Write8BitCommandData(i, data[i]);
                    Tools.delayUs(0.1);
                    WillemOP.SetCE_H();
                    Tools.delayUs(0.1);
                }
            }

            //退出
            Thread.Sleep(500);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0xF0);
            //Thread.Sleep(500);
            WillemOP.SetVCC_L();
        }

        //S6:near success
        public void WriteS6_2(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetDataMode();
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            //Erase(null);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {

                if ((i % 128 == 0))
                {
                    Thread.Sleep(35);
                    //for (int r = 0; r < 4; r++)
                    //{
                    //    WillemOP.SetCE_L();
                    ////    WillemOP.SetAddressMode();
                    //    Tools.delayUs(1);
                    //    WillemOP.SetDataMode();
                    //    byte lastReg = WillemOP.Read4021();
                    //    byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                    //    Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));
                    //}

                    WillemOP.Write8BitCommandData(0x5555, 0xAA);
                    WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                    WillemOP.Write8BitCommandData(0x5555, 0xA0);
                    WillemOP.Write8BitCommandData(i, data[i]);
                    WillemOP.SetCE_H();
                }
                else
                {
                    WillemOP.Write8BitCommandData(i, data[i]);
                    //WillemOP.SetCE_H();
                    //WillemOP.SetAddr(i);
                    //WillemOP.SetData(data[i]);
                    //WillemOP.SetCE_L();
                    ////WillemOP.SetCE_H();
                }
                //Thread.Sleep(20);

                //byte lastReg = WillemOP.Read4021();
                //byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                //Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }

        //S6:near success
        public void WriteS6(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            //Erase(null);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                if ((i % 128 == 0))
                {
                    Thread.Sleep(15);
                    WillemOP.Write8BitCommandData(0x5555, 0xAA);
                    WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                    WillemOP.Write8BitCommandData(0x5555, 0xA0);
                    WillemOP.Write8BitCommandData(i, data[i]);
                    WillemOP.SetCE_H();
                }
                else
                {
                    WillemOP.SetCE_H();
                    WillemOP.SetAddr(i);
                    WillemOP.SetData(data[i]);
                    WillemOP.SetCE_L();
                }
                //Thread.Sleep(20);

                //byte lastReg = WillemOP.Read4021();
                //byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                //Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }

        //S5: don't check reg, write command at page start
        //write data by CE and OE, not ADDR
        //result: write first byte,but error
        public void WriteS5(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            Erase(null);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                if (i % 128 == 0)
                {
                    Thread.Sleep(20);
                    WillemOP.Write8BitCommandData(0x5555, 0xAA);
                    WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                    WillemOP.Write8BitCommandData(0x5555, 0xA0);
                    //WillemOP.Write8BitCommandData(i, data[i]);
                    WillemOP.SetCE_H();
                    WillemOP.SetAddr(i);
                    WillemOP.SetCE_L();
                }
                WillemOP.SetCE_H();
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_L();
                WillemOP.SetAddressMode();
                //Thread.Sleep(20);

                //byte lastReg = WillemOP.Read4021();
                //byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                //Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }


        //S4: don't check reg, write command at page start
        //write data by CE and OE, not ADDR
        //result: write first byte,but error
        public void WriteS4(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            Erase(null);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                if (i % 128 == 0)
                {
                    Thread.Sleep(20);
                    WillemOP.Write8BitCommandData(0x5555, 0xAA);
                    WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                    WillemOP.Write8BitCommandData(0x5555, 0xA0);
                    WillemOP.Write8BitCommandData(i, data[i]);
                    WillemOP.SetCE_H();
                }
                WillemOP.SetCE_H();
                WillemOP.SetDataMode();
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_L();
                WillemOP.SetAddressMode();
                //Thread.Sleep(20);

                //byte lastReg = WillemOP.Read4021();
                //byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                //Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }

        //S3: don't check reg, write command at page start
        //write data by each byte
        //result: page first 2 byte
        public void WriteS3(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            Erase(null);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                if (i % 128 == 0)
                {
                    Thread.Sleep(20);
                    WillemOP.Write8BitCommandData(0x5555, 0xAA);
                    WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                    WillemOP.Write8BitCommandData(0x5555, 0xA0);
                    WillemOP.SetCE_H();
                }
                WillemOP.Write8BitCommandData(i, data[i]);
                WillemOP.SetCE_H();
                //Thread.Sleep(20);

                //byte lastReg = WillemOP.Read4021();
                //byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                //Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }


        //S2: Disable SDP, write by each byte
        //result: No any write
        public void WriteS2(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            Erase(null);

            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0x80);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0x20);
            Thread.Sleep(20);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                WillemOP.Write8BitCommandData(i, data[i]);
                WillemOP.SetCE_H();
                Thread.Sleep(20);
            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }


        //S1: don't check reg, write by each byte
        //result: page last+write last
        public void WriteS1(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            Erase(null);

            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                WillemOP.Write8BitCommandData(0x5555, 0xAA);
                WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                WillemOP.Write8BitCommandData(0x5555, 0xA0);
                WillemOP.Write8BitCommandData(i, data[i]);
                WillemOP.SetCE_H();
                Thread.Sleep(20);

                byte lastReg = WillemOP.Read4021();
                byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }


        public void WriteUnchange(byte[] data, long baseAddr, int length, long totalLength)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(1000);
            for (Int64 i = baseAddr; i < baseAddr + length; i++)
            {
                WillemOP.Write8BitCommandData(0x5555, 0xAA);
                WillemOP.Write8BitCommandData(0x2AAA, 0x55);
                WillemOP.Write8BitCommandData(0x5555, 0xA0);
                if (i % 128 == 0)
                {
                }
                WillemOP.Write8BitCommandData(i, data[i]);
                WillemOP.SetCE_H();
                Thread.Sleep(1);
                Thread.Sleep(20);
                if (i % 128 == 0)
                {
                }
                //WillemOP.SetCE_L();


                WillemOP.SetDataMode();
                WillemOP.SetAddressMode();
                byte lastReg = WillemOP.Read4021();
                byte lastData6 = (byte)((lastReg & 0x40) >> 6);
                Console.WriteLine("lastReg:" + Tools.byte2HexStr(lastReg) + ":" + Tools.byte2Str(lastReg));

                for (Int64 r = 0; r < 10; r++)
                {
                    Tools.delayUs(1);
                    WillemOP.SetDataMode();
                    byte reg = WillemOP.Read4021();
                    Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));
                    byte data6 = (byte)((reg & 0x40) >> 6);
                    if (r > 1 && lastData6 == data6)
                    {
                        break;
                    }
                    else
                    {
                        lastReg = reg;
                    }
                }

            }

            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x80);
            //WillemOP.Write8BitCommandData(0x5555, 0xAA);
            //WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            //WillemOP.Write8BitCommandData(0x5555, 0x20);

            //退出
            Thread.Sleep(500);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0xF0);
            Thread.Sleep(500);
            WillemOP.SetVCC_L();

        }

        public void Erase(string args)
        {
            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            //设置WE为高电平，读取使能
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0x80);
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0x10);
            WillemOP.SetCE_H();
            //设置读取地址
            for (Int64 i = 0; i < 10; i++)
            {
                Thread.Sleep(50);
                byte reg = 0;
                WillemOP.SetDataMode();
                reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));
                if (i > 2 && reg == 0xFF)
                {
                    Console.WriteLine("擦除完成（一般只要几秒）");
                    break;
                }
            }

        }

        public byte[] ReadId()
        {
            byte[] data = new byte[0x10];

            //设置VCC
            WillemOP.SetVCC_H();
            Thread.Sleep(20);
            //设置WE为高电平，读取使能
            WillemOP.Write8BitCommandData(0x5555, 0xAA);
            WillemOP.Write8BitCommandData(0x2AAA, 0x55);
            WillemOP.Write8BitCommandData(0x5555, 0x90);

            //设置读取地址
            for (Int64 i = 0; i < 4; i++)
            {
                WillemOP.SetAddr(i);
                WillemOP.SetCE_H();
                byte b = WillemOP.Read4021();
                data[i] = b;
                WillemOP.SetCE_L();
            }

            Console.WriteLine("厂商代码(正确为0xBF)：" + Tools.byte2HexStr(data[0]));
            Console.WriteLine("设备代码(正确为0x5D或0x3D)：" + Tools.byte2HexStr(data[1]));

            return null;

        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
            throw new NotImplementedException();
        }

    }
}
