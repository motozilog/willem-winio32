using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace willem_winio32
{
    public class EpromOp
    {
        static bool debug = false;

        #region W27C
        public static void W27CWrite(byte[] data, Int64 baseAddr, int length)
        {
            //初始化
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();


            //主写入流程
            for (Int64 i = baseAddr; i < (baseAddr + length); i++)
            {
                WillemOP.SetCE_H();
                WillemOP.SetAddr(i);
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_L();
                Tools.delayUs(0.02);
                WillemOP.SetCE_H();
                Tools.delayUs(0.01);
                Tools.ShowProgress(i, data, baseAddr, length);
            }
        }

        public static byte[] W27CRead(Int64 baseAddr, int length)
        {
            byte[] data = new byte[baseAddr+length];

            //设置VCC
            WillemOP.SetVCC_H();

            //设置CE为低电平，读取使能
            WillemOP.SetCE_L();

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

        public static void W27CErase(int length)
        {
            W27CEraseJumperForm jf = new W27CEraseJumperForm();
            jf.ShowDialog();
            byte[] data = new byte[length];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)0xff;
            }


            //初始化
            WillemOP.SetVCC_H();
            WillemOP.SetVPP_H();

            //主写入流程
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 0x100 == 0)
                {
                    Console.WriteLine("ADDR:" + Convert.ToString(i, 16).PadLeft(6, '0').ToUpper() + " DATA:" + Convert.ToString(data[i], 16).PadLeft(2, '0').ToUpper());
                }

                WillemOP.SetCE_H();
                //Willem.SetAddr(0x000000);
                WillemOP.SetData(data[i]);
                WillemOP.SetCE_L();
                Tools.delayUs(0.02);
                WillemOP.SetCE_H();
                Tools.delayUs(0.01);
            }

            Thread.Sleep(20);
            WillemOP.Init();
            W27CEraseJumperForm jf2 = new W27CEraseJumperForm("请将Willem跳线从蓝色跳回黄色");
            jf2.ShowDialog();
        }
        #endregion

        #region MX29
        public static void MX29WriteCommand()
        {
            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55);

            //3 0x5555 0xA0
            WillemOP.Write16BitCommand(0x5555, 0xA0);
        }

        public static void MX29WriteCommand(Int64 addr, byte data)
        {
            MX29WriteCommand();
            WillemOP.SetCE_H();
            WillemOP.SetAddr(addr);
            WillemOP.SetData(data);
            WillemOP.SetCE_L();
        }

        public static void MX29WriteCommand(Int64 addr, byte dataH,byte dataL)
        {
            MX29WriteCommand();

            WillemOP.SetCE_H();
            WillemOP.SetAddr(addr+1);
            WillemOP.SetData(dataH);

            WillemOP.SetAddr(addr);
            WillemOP.SetData(dataL);

            WillemOP.SetCE_L();
            WillemOP.SetCE_H();
        }

        public static byte[] MX29ReadIdCommand()
        {
            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55);

            //3 0x5555 0x70
            WillemOP.Write16BitCommand(0x5555, 0x90);

            WillemOP.SetVPP_L();

            byte[] id = new byte[2];

            WillemOP.SetCE_H();
            WillemOP.SetAddr(0);
            WillemOP.SetCE_L();
            byte bL = WillemOP.Read4021();
            id[0] = bL;
            WillemOP.SetCE_L();

            WillemOP.SetCE_H();
            WillemOP.SetAddr(1 << 1);
            WillemOP.SetCE_L();

            byte bH = WillemOP.Read4021();
            id[1] = bH;
            return id;
        }

        public static void MX29ResetCommand()
        {
            //先用3个16位命令做复位
            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55, 0x55);

            //3 0x5555 0xF0
            WillemOP.Write16BitCommand(0x5555, 0xF0, 0xF0);
        }

        public static void MX29EraseCommand()
        {
            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55);

            //3 0x5555 0x80
            WillemOP.Write16BitCommand(0x5555, 0x80);

            //4 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA);

            //5 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55);

            //6 0x5555 0x10
            WillemOP.Write16BitCommand(0x5555, 0x10);
        }

        public static byte[] MX29Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            WillemOP.SetCE_H();
            WillemOP.SetVCC_H();
            Thread.Sleep(200);

            //先复位
            WillemOP.SetVPP_H();
            EpromOp.MX29ResetCommand();
            WillemOP.SetVPP_L();
            return GenRead(baseAddr,length,totalLength);
        }

        public static void MX29ReadRegCommandOnly()
        {
            //1 0x5555 0xAA
            WillemOP.Write16BitCommand(0x5555, 0xAA);

            //2 0x2AAA 0x55
            WillemOP.Write16BitCommand(0x2AAA, 0x55);

            //3 0x5555 0x70
            WillemOP.Write16BitCommand(0x5555, 0x70);
        }

        public static byte MX29ReadRegCommand()
        {
            MX29ReadRegCommandOnly();
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

        #endregion


        public static byte[] GenRead(Int64 baseAddr, int length, Int64 totalLength)
        {            
            byte[] data = new byte[baseAddr + length];

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


        #region AM29LV


        #endregion
    }
}
