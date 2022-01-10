using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    public class S25XX : IChip
    {
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));
        double delayUs = 0;
        bool debug = false;
        private Int64 chipLength = 0x100;
        public void send25Byte(byte b, bool isCommand=false)
        {
            if (isCommand && debug) { Console.WriteLine("发送25命令:" + Tools.byte2HexStr(b)); }
            //数据在上升沿采样，下降沿发送
            //注意是MSB
            LPT.D1(0);
            for (int i = 7; i >= 0; i--)
            {
                LPT.D1(0);
                if (delayUs != 0) { Thread.Sleep(1); }
                int value = (b >> i)&0x01;
                value = value==1?0:1;   //要反相
                LPT.D0(value);
                if (delayUs != 0) { Thread.Sleep(1); }
                LPT.D1(1);
                if (delayUs != 0) { Thread.Sleep(1); }
                LPT.D1(0);
            }
            LPT.D1(0);
            if (delayUs != 0) { Thread.Sleep(1); }
        }

        public byte recv25Byte()
        {
            byte data = 0;
            LPT.D1(0);
            if (delayUs != 0) { Thread.Sleep(1); } 
            for (int i = 7; i >= 0; i--)
            {
                LPT.D1(1);
                if (delayUs != 0) { Thread.Sleep(1); }
                byte value = WillemOP.ReadSerialOut();
                data = Tools.setBit(data, i, value);
                LPT.D1(0);
                if (delayUs != 0) { Thread.Sleep(1); }
            }
            LPT.D1(0);
            //Console.WriteLine("data:" + Tools.byte2HexStr(data));
            if (delayUs != 0) { Thread.Sleep(1); } 
            return data;
        }

        public void send25Addr24bit(Int64 addr)
        {            
            //A23-A16
            send25Byte((byte)((addr >> 16)&0xFF));

            //A15-A8
            send25Byte((byte)(addr >> 8 & 0xFF));

            //A7-A0
            send25Byte((byte)(addr& 0xFF));
        }

        public void send25Addr32bit(Int64 addr)
        {
            //A31-A17
            send25Byte((byte)((addr >> 24) & 0xFF));

            //A23-A16
            send25Byte((byte)((addr >> 16) & 0xFF));

            //A15-A8
            send25Byte((byte)(addr >> 8 & 0xFF));

            //A7-A0
            send25Byte((byte)(addr & 0xFF));
        }

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            byte[] data = new byte[length];

            WillemOP.SetData(0x00);
            WillemOP.SetCE_H();
            Thread.Sleep(20);
            WillemOP.SetVCC_H();
            Thread.Sleep(20);

            WillemOP.SetCE_L();

            if (chipLength <= 0x1000000)
            {
                //启动命令
                send25Byte(0x03, true);

                //发送初始地址
                send25Addr24bit(baseAddr);
            }
            else
            {
                //启动命令
                send25Byte(0x13, true);

                //发送初始地址
                send25Addr32bit(baseAddr);
            }

            for (Int64 i = 0; i < length; i++)
            {
                byte b = recv25Byte();
                data[i] = b;
                Tools.ShowProgress(i, b, baseAddr, totalLength, 0x1000);
            }
            return data;
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {

            if (length % 0x100 != 0)
            {
                Console.WriteLine("读取长度必须为0x100的整倍数");
                return;
            }

            WillemOP.SetData(0x00);
            WillemOP.SetCE_H();
            Thread.Sleep(20);
            WillemOP.SetVCC_H();
            Thread.Sleep(20);


            for (Int64 i = 0; i < data.Length; i = i + 0x100)
            {
                //Write Enable
                WillemOP.SetCE_L();
                send25Byte(0x06, true);
                WillemOP.SetCE_H();

                WillemOP.SetCE_L();
                if (chipLength <= 0x1000000)
                {
                    //Page Program
                    send25Byte(0x02, true);
                    send25Addr24bit(baseAddr+i);
                }
                else
                {
                    //Page Program 4Byte
                    send25Byte(0x12, true);
                    send25Addr32bit(baseAddr + i);
                }

                for (Int64 d = i; d < i + 0x100; d++)
                {
                    send25Byte(data[d]);
                }

                WillemOP.SetCE_H();

                for (int s = 0; s < 300; s++)
                {
                    byte reg = ReadStatusRegister();
                    if ((reg & 0x01) == 0)
                    {
                        break;
                    }
                }
                Tools.ShowProgress(baseAddr + i, data[i], 0, totalLength, 0x1000);
            }
        }

        public void Erase(string args)
        {
            WillemOP.SetData(0x00);
            WillemOP.SetCE_H();
            Thread.Sleep(20);
            WillemOP.SetVCC_H();
            Thread.Sleep(20);

            WillemOP.SetCE_L();

            //Write Enable
            send25Byte(0x06, true);
            WillemOP.SetCE_H();

            //ChipErase
            WillemOP.SetCE_L();
            send25Byte(0x60, true);
            WillemOP.SetCE_H();

            for (int i = 0; i < 300; i++)
            {
                Thread.Sleep(1000);
                byte reg = ReadStatusRegister();
                Console.WriteLine("擦除延迟："+i+"秒，寄存器状态："+Tools.byte2Str(reg));
                if ((reg & 0x01) == 0)
                {
                    return;
                }
            }
        }

        private byte ReadStatusRegister()
        {
            WillemOP.SetCE_H();
            WillemOP.SetCE_L();
            send25Byte(0x05, true);
            byte b = recv25Byte();
            WillemOP.SetCE_H();
            return b;

        }


        public byte[] ReadId()
        {

            WillemOP.SetData(0x00);
            WillemOP.SetCE_H();
            Thread.Sleep(20);
            WillemOP.SetVCC_H();
            Thread.Sleep(20);

            WillemOP.SetCE_L();


            //0x90: Read Manufacturer/Device ID
            send25Byte(0x90);
            send25Addr24bit(0);

            byte ManufactureID = recv25Byte();
            Console.WriteLine("ManufactureID:" + Tools.byte2HexStr(ManufactureID));

            byte DeviceID = recv25Byte();
            Console.WriteLine("DeviceID:" + Tools.byte2HexStr(DeviceID));

            WillemOP.SetCE_H();

            //0x9F: Read Identification
            WillemOP.SetCE_L();
            send25Byte(0x9F);
            byte ManufactureID2 = recv25Byte();
            Console.WriteLine("ManufactureID2:" + Tools.byte2HexStr(ManufactureID2));

            byte MemoryTypeID = recv25Byte();
            Console.WriteLine("MemoryTypeID:" + Tools.byte2HexStr(MemoryTypeID));

            byte Capacity = recv25Byte();
            Console.WriteLine("Capacity:" + Tools.byte2HexStr(Capacity));

            return new byte[1];
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
            S25XXChipLengthForm s = new S25XXChipLengthForm();
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
            config.ChipLength = 0x100;
            try
            {
                int length = Convert.ToInt32(Ini.Read("S25XXLength"),16);
                if (length >= 0x100)
                {
                    config.ChipLength = length;
                    chipLength = length;
                }
            }
            catch { }
            config.ChipModel = "S25XX";
            config.Note = "擦写读25 NOR FLASH系列，最高4GByte，芯片需要支持24位地址或32位地址模式，旧芯片请直接用Willem程序进行，己测试EN25T80和MX25L51245。若芯片为3.3V请在VCC降压，若芯片为1.8V请使用电平转换模块";
            config.SpecialFunction = "设置25芯片容量";
            config.DipSw = willem_winio32.Properties.Resources.S25XX;
            config.Adapter = willem_winio32.Properties.Resources.SOP16_Adapter_S25XX;
            return config;
        }
    }
}
