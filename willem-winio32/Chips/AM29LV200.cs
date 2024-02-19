using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{

    public class AM29VPP : IChip
    {
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
            WillemOP.SetData(0);
            WillemOP.SetVPP_L();
            WillemOP.SetData(0);
            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xF0);
            Thread.Sleep(2000);
        }

        private byte[] readVpp(long baseAddr, int length, long totalLength)
        {
            byte[] data = new byte[baseAddr + length];

            int addressLength = 23;
            try
            {
                long chipLength = Convert.ToInt64(Ini.Read(G.AM29VPPLengthIniKey), 16);
                if (chipLength >= 0x4000000)
                {
                    //S29GL512
                    addressLength = 24;
                }

                if (chipLength >= 0x8000000)
                {
                    //S29GL01G
                    addressLength = 25;
                }

                if (chipLength >= 0x10000000)
                {
                    //S70GL02G
                    addressLength = 26;
                }
            }
            catch { }

            //再用8bit读取
            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                WillemOP.SetCE_H();

                if (addressLength <= 23)
                {
                    //24位地址
                    WillemOP.SetAddr(i / 2);
                }
                else
                {
                    //32位地址
                    WillemOP.SetAddr(i / 2, addressLength);
                }

                WillemOP.SetVPP_L();
                byte bl = WillemOP.Read4021();
                data[i] = bl;

                WillemOP.SetVPP_H();
                byte bh = WillemOP.Read4021();
                data[i + 1] = bh;

                WillemOP.SetCE_H();

                Tools.ShowProgress(i, data, baseAddr, length);
            }
            return data;

        }


        public void Write(byte[] data, long baseAddr, int length, long totalLength)
        {
            init();

            int addressLength = 23;
            try
            {
                long chipLength = Convert.ToInt64(Ini.Read(G.AM29VPPLengthIniKey), 16);
                if (chipLength >= 0x4000000)
                {
                    //S29GL512
                    addressLength = 24;
                }

                if (chipLength >= 0x8000000)
                {
                    //S29GL01G
                    addressLength = 25;
                }

                if (chipLength >= 0x10000000)
                {
                    //S70GL02G
                    addressLength = 26;
                }
            }
            catch { }



            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xAA);

                WillemOP.Write16BitCommandDataVPP(0x2AA, 0x00, 0x55);

                WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xA0);


                if (addressLength <= 23)
                {
                    //24位地址
                    WillemOP.Write16BitCommandDataVPP((int)i / 2, data[i + 1], data[i]);
                }
                else
                {
                    //32位地址
                    WillemOP.Write16BitCommandDataVPP32((Int64)i / 2, addressLength, data[i + 1], data[i]);
                }

                WillemOP.SetCE_H();

                //read status
                Tools.delayUs(0.01);
                WillemOP.SetDataMode();
                byte reg0 = WillemOP.Read4021();
                byte lastData6 = (byte)((reg0 & 0x40) >> 6);

                for (int w = 0; w < 100; w++)
                {
                    Tools.delayUs(0.01);
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
        
        public void Erase(string args)
        {
            init();
            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xAA);

            WillemOP.Write16BitCommandDataVPP(0x2AA, 0x00, 0x55);

            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0x80);

            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xAA);

            WillemOP.Write16BitCommandDataVPP(0x2AA, 0x00, 0x55);

            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0x10);

            byte lastReg = 0x0;

            int maxTime = 100;
            int warningTime = 100;

            try
            {
                long length = Convert.ToInt64(Ini.Read(G.AM29VPPLengthIniKey), 16);
                if (length >= 0x1000000)
                {
                    //S29GL128
                    warningTime = 400;
                    maxTime = 800;
                }

                if (length >= 0x2000000)
                {
                    //S29GL256
                    warningTime = 800;
                    maxTime = 1600;
                }

                if (length >= 0x4000000)
                {
                    //S29GL512
                    warningTime = 1600;
                    maxTime = 3200;
                }

                if (length >= 0x8000000)
                {
                    //S29GL01G
                    warningTime = 3200;
                    maxTime = 6400;
                }

                if (length >= 0x10000000)
                {
                    //S70GL02G
                    warningTime = 6400;
                    maxTime = 12800;
                }
            }
            catch { }

            for (int i = 0; i < maxTime; i++)
            {
                Console.WriteLine("增长耗时:" + i * 1 + "秒");
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

        public void EraseSector(string args)
        {
            init();
            long length = Convert.ToInt64(Ini.Read(G.AM29VPPLengthIniKey), 16);

            for (uint sector = 0; sector < length; sector = sector + 0x10000)
            {
                Console.WriteLine(Tools.uint2HexStr(sector));
                WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xAA);

                WillemOP.Write16BitCommandDataVPP(0x2AA, 0x00, 0x55);

                WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0x80);

                WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xAA);

                WillemOP.Write16BitCommandDataVPP(0x2AA, 0x00, 0x55);

                //WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0x10);
                WillemOP.Write16BitCommandDataVPP((int)sector, 0x00, 0x30);

                byte lastReg = 0x0;

                int maxTime = 10;
                int warningTime = 3;

                for (int i = 0; i < maxTime; i++)
                {
                    Thread.Sleep(1000);
                    WillemOP.SetCE_H();

                    //read status
                    WillemOP.SetDataMode();
                    byte reg = WillemOP.Read4021();

                    if (reg == 0xFF && lastReg == 0xFF)
                    {
                        break;
                    }
                    if (i > warningTime)
                    {
                        Console.WriteLine("增长超过60秒，仍未检测到D7为1，可能芯片是坏的");
                    }
                    lastReg = reg;
                }
            }
        }


        public byte[] ReadId()
        {
            init();
            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0xAA);

            WillemOP.Write16BitCommandDataVPP(0x2AA, 0x00, 0x55);

            WillemOP.Write16BitCommandDataVPP(0x555, 0x00, 0x90);

            WillemOP.SetCE_H();
            byte[] id = readVpp(0x00, 04, 0x04);
            Console.WriteLine("厂商代码(AM29LV200B正确为0x0001，MX29LV320为0x00C2其它厂商请参考手册)：" + Tools.byte2HexStr(id[1]) + Tools.byte2HexStr(id[0]));
            Console.WriteLine("设备代码(AM29LV200B正确为0x22BF或0x223B，MX29LV320为0x22A7，其它厂商请参考手册)：" + Tools.byte2HexStr(id[3]) + Tools.byte2HexStr(id[2]));
            return id;
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
            AM29ChipLengthForm s = new AM29ChipLengthForm(G.AM29VPPLengthIniKey);
            s.ShowDialog();
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ReadId = true;
            config.Register = false;
            //config.EraseDelay = true;
            //config.EraseDelayTime = "FULL";

            config.ChipLength = 0x200000;
            try
            {
                long length = Convert.ToInt64(Ini.Read(G.AM29VPPLengthIniKey), 16);
                if (length >= 0x100)
                {
                    config.ChipLength = length;
                }
            }
            catch { }

            config.ChipModel = "29X";
            config.DipSw = willem_winio32.Properties.Resources.AM29;
            config.Jumper = willem_winio32.Properties.Resources.AM29_Jumper;
            config.Adapter = willem_winio32.Properties.Resources.AM29_Adapter;
            config.Note = "不仅支持29系列，并且支持兼容29写入的16位芯片，如：MSP55系列";
            config.SpecialFunction = "设置芯片容量";

            return config;
        }


    }
}
