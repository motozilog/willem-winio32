using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace willem_winio32
{
    //改部分写入、读取：ok
    public class M59PW016 : M59PW, IChip
    {
    }

    public class M59PW032 : M59PW, IChip
    {
        private int chipsize = 0x400000;
        M59PW pw016 = new M59PW016();
        public ChipConfig GetConfig()
        {
            ChipConfig config = pw016.GetConfig();
            config.ChipLength = chipsize;
            config.ChipModel = "M59PW032";
            config.Note = "M59PW032未经芯片验证。" + config.Note;
            return config;
        }
    }

    public class M59PW064 : M59PW, IChip
    {
        private int chipsize = 0x800000;
        M59PW pw016 = new M59PW016();
        
        public ChipConfig GetConfig()
        {
            ChipConfig config = pw016.GetConfig();
            config.ChipLength = chipsize;
            config.ChipModel = "M59PW064";
            return config;
        }
    }



    public abstract class M59PW : IChip
    {
        private int chipsize = 0x200000;

        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            //初始化
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_L();
            Thread.Sleep(200);

            return EpromOp.GenRead(baseAddr, length, totalLength);
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            //写的时候要16bit
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(1000);

            for (Int64 i = baseAddr; i < baseAddr + length; i = i + 2)
            {
                //1 0x5555 0xAA
                WillemOP.Write16BitCommand(0x555, 0xAA);

                //2 0x2AAA 0x55
                WillemOP.Write16BitCommand(0x2AA, 0x55);

                //3 0x5555 0xA0
                WillemOP.Write16BitCommand(0x555, 0xA0);


                WillemOP.SetCE_H();
                WillemOP.SetAddr(i + 1);
                WillemOP.SetData(0xFF);
                WillemOP.SetData(data[i + 1]);
                WillemOP.SetAddr(i);
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_L();
                WillemOP.SetCE_H();

                Tools.ShowProgress(i, data, baseAddr, length);
            }
        }

        public void Erase(string args)
        {
            try
            {
                int blockAddr = Convert.ToInt32(args, 16);
                EraseBlock(args);
            }
            catch
            {
                FullErase(args);
            }
        }

        public void EraseBlock(string args)
        {
            int block = Convert.ToInt32(args);
            Console.WriteLine("擦除块：0x" + Tools.int2HexStr(block));

            //初始化
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(100);

            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x555, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0x55);

            //3 0x5555 0x80
            WillemOP.Write16BitCommand(0x555, 0x80);

            //4 0x5555 0xAA
            WillemOP.Write16BitCommand(0x555, 0xAA);

            //5 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0x55);

            //6 0x5555 0x10
            WillemOP.Write16BitCommand(block, 0x30);

            byte lastReg = 0x0;
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("增长耗时:" + i * 1 + "秒");
                Thread.Sleep(1000);
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                byte reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));

                if (reg == 0xFF && lastReg == 0xFF)
                {
                    return;
                }
                if (i > 60)
                {
                    Console.WriteLine("增长超过60秒，仍未检测到D7为1，可能芯片是坏的");
                }
                lastReg = reg;
            }
        }


        public void FullErase(string args)
        {
            Console.WriteLine("完整擦除");
            //初始化
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();
            Thread.Sleep(300);
            //WillemOP.SetCE_L();

            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x555, 0, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0, 0x55);

            //3 0x5555 0x80
            WillemOP.Write16BitCommand(0x555, 0, 0x80);

            //4 0x5555 0xAA
            WillemOP.Write16BitCommand(0x555, 0, 0xAA);

            //5 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AA, 0, 0x55);

            //6 0x5555 0x10
            WillemOP.Write16BitCommand(0x555, 0, 0x10);

            byte lastReg = 0x0;
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("增长耗时:" + i * 1 + "秒");
                Thread.Sleep(1000);
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                byte reg = WillemOP.Read4021();
                Console.WriteLine(Tools.byte2HexStr(reg) + ":" + Tools.byte2Str(reg));

                if (reg == 0xFF && lastReg == 0xFF)
                {
                    return;
                }
                if (i > 60)
                {
                    Console.WriteLine("增长超过60秒，仍未检测到D7为1，可能芯片是坏的");
                }
                lastReg = reg;
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
            WillemOP.Write16BitCommand(0x555, 0, 0x90);

            byte[] id = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetCE_L();
                byte b = WillemOP.Read4021();
                id[i] = b;
            }
            WillemOP.SetVPP_L();
            Thread.Sleep(20);
            WillemOP.SetVCC_L();

            Console.WriteLine("厂商代码(正确为0x0020)：" + Tools.byte2HexStr(id[1]) + Tools.byte2HexStr(id[0]));
            Console.WriteLine("设备代码(正确为016:0x88AD 064:0x88AA 1282: 0x88A8)：" + Tools.byte2HexStr(id[3]) + Tools.byte2HexStr(id[2]));
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
            config.EraseDelay = true;
            config.EraseDelayTime = "FULL";
            config.Note = "M59PW系列，写入时均不检查寄存器，请注意校验。擦除参数若填块地址，将只进行对该块的擦除";

            config.ChipLength = chipsize;
            config.ChipModel = "M59PW016";
            config.DipSw = willem_winio32.Properties.Resources.MX29F1615;
            config.Jumper = willem_winio32.Properties.Resources.M59PW016_Sub_SW;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_16Bit_Adapter;
            return config;
        }

    }
}
