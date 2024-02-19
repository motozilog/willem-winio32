using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    public class S70GL02 : IChip
    {
        int addressLength = 26;
        Int64 chipLength = 0x10000000;
        Int64 dieLength = 0x8000000;//内核长度
        public S70GL02(string partNoIn)
        {
        }

        public byte[] Read(long baseAddr, int length, long totalLength)
        {
            init();
            byte[] result = readVpp(baseAddr, length, totalLength);
            return result;
        }

        private void init()
        {
            //初始化
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(200);
            WillemOP.SetData(0xff);
            WillemOP.SetVPP_L();
            WillemOP.SetData(0xff);
            Thread.Sleep(2000);
        }

        private byte[] readVpp(long baseAddr, int length, long totalLength)
        {
            WillemOP.SetCE_H();
            byte[] data = new byte[baseAddr + length];

            //再用8bit读取
            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                //32位地址
                WillemOP.SetAddr(i / 2, addressLength);

                WillemOP.SetVPP_H();
                byte bh = WillemOP.Read4021();
                data[i + 1] = bh;

                WillemOP.SetVPP_L();
                //WillemOP.SetDataMode();
                byte bl = WillemOP.Read4021();
                data[i] = bl;


                Tools.ShowProgress(i, data, baseAddr, length);
            }
            return data;
        }

        public void Write(byte[] data, long baseAddr, int length, long totalLength)
        {
            init();

            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                if (i < 0x8000000)
                {
                    Write16BitCommandDataVPP32(0x000555, addressLength, 0x00, 0xAA);
                    Write16BitCommandDataVPP32(0x0002AA, addressLength, 0x00, 0x55);
                    Write16BitCommandDataVPP32(0x000555, addressLength, 0x00, 0xA0);

                }
                else
                {
                    Write16BitCommandDataVPP32(0x4000555, addressLength, 0x00, 0xAA);
                    Write16BitCommandDataVPP32(0x40002AA, addressLength, 0x00, 0x55);
                    Write16BitCommandDataVPP32(0x4000555, addressLength, 0x00, 0xA0);
                }
                //32位地址
                WillemOP.Write16BitCommandDataVPP32((Int64)i / 2, addressLength, data[i + 1], data[i]);

                WillemOP.SetCE_H();

                //read status
                WillemOP.SetDataMode();
                byte reg0 = WillemOP.Read4021();
                byte lastData6 = (byte)((reg0 & 0x40) >> 6);

                for (int w = 0; w < 100; w++)
                {
                    WillemOP.SetDataMode();
                    byte reg = WillemOP.Read4021();
                    byte data6 = (byte)((reg & 0x40) >> 6);
                    if (lastData6 == data6)
                    {
                        break;
                    }
                    else
                    {
                        lastData6 = data6;
                        //Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg) + " " + Tools.byte2HexStr(reg0) + ":" + Tools.byte2Str(reg0));
                    }
                }

                Tools.ShowProgress(i, data[i], baseAddr, length);
            }
        }

        private Int64 commandWithDieId(Int64 command, Int64 dieId)
        {
            Int64 command555 = (dieId * (dieLength)) + command;
            return command555;
        }

        public void Erase(string args)
        {
            init();
            eraseDie1();
            eraseDie2();            
        }

        public void eraseDie1()
        {
            WillemOP.Write16BitCommandDataVPP32(0x555, addressLength, 0x00, 0xAA);
            WillemOP.Write16BitCommandDataVPP32(0x2AA, addressLength, 0x00, 0x55);
            WillemOP.Write16BitCommandDataVPP32(0x555, addressLength, 0x00, 0x80);
            WillemOP.Write16BitCommandDataVPP32(0x555, addressLength, 0x00, 0xAA);
            WillemOP.Write16BitCommandDataVPP32(0x2AA, addressLength, 0x00, 0x55);
            WillemOP.Write16BitCommandDataVPP32(0x555, addressLength, 0x00, 0x10);

            byte lastReg = 0x0;

            //S70GL02G
            int warningTime = 6400;
            int maxTime = 12800;

            for (int i = 0; i < maxTime; i++)
            {
                Console.WriteLine("正在擦除第1 die，增长耗时:" + i * 1 + "秒");
                Thread.Sleep(1000);
                WillemOP.SetCE_H();

                //read status
                WillemOP.SetDataMode();
                byte reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));

                if (reg == 0xFF && lastReg == 0xFF)
                {
                    return;
                }
                lastReg = reg;
            }
        }

        public void eraseDie2()
        {
            WillemOP.Write16BitCommandDataVPP32(0x4000555, addressLength, 0x00, 0xAA);
            WillemOP.Write16BitCommandDataVPP32(0x40002AA, addressLength, 0x00, 0x55);
            WillemOP.Write16BitCommandDataVPP32(0x4000555, addressLength, 0x00, 0x80);
            WillemOP.Write16BitCommandDataVPP32(0x4000555, addressLength, 0x00, 0xAA);
            WillemOP.Write16BitCommandDataVPP32(0x40002AA, addressLength, 0x00, 0x55);
            WillemOP.Write16BitCommandDataVPP32(0x4000555, addressLength, 0x00, 0x10);

            byte lastReg = 0x0;

            //S70GL02G
            int warningTime = 6400;
            int maxTime = 12800;

            for (int i = 0; i < maxTime; i++)
            {
                Console.WriteLine("正在擦除第2 die，增长耗时:" + i * 1 + "秒");
                Thread.Sleep(1000);
                WillemOP.SetCE_H();

                //read status
                WillemOP.SetDataMode();
                byte reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));

                if (reg == 0xFF && lastReg == 0xFF)
                {
                    return;
                }
                lastReg = reg;
            }
        }

        public static void Write16BitCommandDataVPP32(Int64 addr, int addressLength, byte dataH, byte dataL)
        {
            int data = dataH;
            data = data << 8;
            data = data | dataL;
            
            WillemOP.SetAddr(addr, addressLength);
            WillemOP.SetCE_L();
            WillemOP.SetCE_H();
            //H
            WillemOP.SetVPP_H();
            WillemOP.SetData(dataH);

            //L
            WillemOP.SetVPP_L();
            WillemOP.SetData(dataL);
            WillemOP.SetCE_L();
            Tools.delayUs(0.01);
            WillemOP.SetCE_H();
            Tools.delayUs(0.01);
        }



        public byte[] ReadId()
        {
            init();
            WillemOP.Write16BitCommandDataVPP32(0x000555, addressLength, 0x00, 0xAA);
            WillemOP.Write16BitCommandDataVPP32(0x0002AA, addressLength, 0x00, 0x55);
            WillemOP.Write16BitCommandDataVPP32(0x000555, addressLength, 0x00, 0x90);

            WillemOP.SetCE_H();
            int length = 0x20;
            byte[] id = readVpp(0x00, length, length);
            for (int i = 0; i < length; i = i + 2)
            {
                Console.WriteLine(Tools.byte2HexStr((byte)(i/2))+"：" + Tools.byte2HexStr(id[i+1]) + Tools.byte2HexStr(id[i]));
            }
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

            config.ChipLength = chipLength;

            config.ChipModel = "S70GL";
            config.DipSw = willem_winio32.Properties.Resources.AM29;
            config.Jumper = willem_winio32.Properties.Resources.S70GL02_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.S70GL02_Adapter;
            config.Note = "S70GL02写入约需50小时，仅测试S70GL02GS11FHI010";

            return config;
        }


    }
}
