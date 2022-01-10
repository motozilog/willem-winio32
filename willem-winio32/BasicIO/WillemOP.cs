using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    public class WillemOP
    {
        static bool debug = false;
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));

        public static void SetAddr(int addr)
        {
            //1.SET Auto = 0,地址输出模式
            LPT.Auto(0);

            //逐位输出

            //首8位
            byte A7_A0 = (byte)(addr & 0x0000FF);
            byte A15_A8 = (byte)(addr >> 8 & 0x0000FF);
            byte A23_A16 = (byte)(addr >> 16 & 0x0000FF);
            //Console.WriteLine(Tools.byte2Str(A23_A16) + " " + Tools.byte2Str(A15_A8) + " " + Tools.byte2Str(A7_A0));

            for (int i = 7; i >= 0; i--)
            {
                LPT.D0(0); //CLK
                int dataA7_A0 = (A7_A0 >> i) & 0x01;
                int dataA15_A8 = (A15_A8 >> i) & 0x01;
                int dataA23_A16 = (A23_A16 >> i) & 0x01;

                LPT.D1(dataA7_A0);      //A7_A0 ->D1
                LPT.D4(dataA15_A8);     //A15_A8 ->D4
                LPT.D5(dataA23_A16);    //A23_A16 ->D5
                LPT.D0(1);              //CLK

                //合在一条
                //byte value = 0x00;
                //value = Tools.setBit(value, 1, dataA7_A0);      //A7_A0 ->D1
                //value = Tools.setBit(value, 4, dataA15_A8);     //A15_A8 ->D4
                //value = Tools.setBit(value, 5, dataA23_A16);    //A23_A16 ->D5
                //value = Tools.setBit(value, 0, 1);              //CLK
                //LPT.Write378(value);
            }
        }
        public static void SetAddr(Int64 addr)
        {
            SetAddr((int)addr);
        }

        public static void SetData(byte data)
        {
            //1.SET Auto = 1,切换到数据模式
            LPT.Auto(1);    //此时OE自动变成H
            //写入数据
            LPT.Write378(data);
        }

        public static void SetCE_H() { LPT.SELin(0); }
        public static void SetCE_L() { LPT.SELin(1); }
        public static void SetVCC_H() { LPT.Init(1); }
        public static void SetVCC_L() { LPT.Init(0); }
        public static void SetVPP_H() { LPT.STB(0); }
        public static void SetVPP_L() { LPT.STB(1); }

        public static byte Read4021()
        {
            byte data = 0;

            //1.SET Auto = 0,地址输出模式
            LPT.Auto(0);

            //设置4021 PS为L, CLK为L
            LPT.D1(0);
            LPT.D2(0);
            //合并命令
            //byte psClk = 0x00;
            //psClk = Tools.setBit(psClk, 1, 0);
            //psClk = Tools.setBit(psClk, 2, 0);
            //LPT.Write378(psClk);

            //设置4021 PS为H，锁存数据
            LPT.D1(1);

            //读取最高位
            byte by = LPT.Read379();
            int d7 = ((by >> 6) & 0x01);
            data = Tools.setBit(data, 7, d7);

            //设置4021 PS为L, CLK为L
            LPT.D1(0);    //WinIO.Set378(1, 0);
            LPT.D2(1);    //WinIO.Set378(2, 1);
            //合并
            //byte readClkD7 = 0x00;
            //readClkD7 = Tools.setBit(readClkD7, 1, 0);
            //readClkD7 = Tools.setBit(readClkD7, 2, 1);
            //LPT.Write378(readClkD7);
          
            //读取剩余位：
            for (int i = 6; i >= 0; i--)
            {
                LPT.D2(0);
                byte b0 = LPT.Read379();
                int d0 = (b0 >> 6) & 0x01;
                data = Tools.setBit(data, i, d0);
                LPT.D2(1);
            }

            //取反
            data = (byte)~data;
            return data;
        }

        public static byte ReadSerialOut()
        {
            byte b  = (byte)~LPT.Read379();
            b = (byte)((b >> 7) & 0x01);
            return b;
        }

        public static void Write16BitCommandData(int addr, byte dataH, byte dataL)
        {
            int data = dataH;
            data = data << 8;
            data = data | dataL;
            if (debug)
            {
                Console.WriteLine("16bit DATA:" + Convert.ToString(data, 16).PadLeft(4, '0').ToUpper()
                    + " H:" + Convert.ToString(dataH, 16).PadLeft(2, '0').ToUpper()
                    + " L:" + Convert.ToString(dataL, 16).PadLeft(2, '0').ToUpper());
            }
            if (debug) { Console.WriteLine(Convert.ToString(addr, 16).PadLeft(6, '0').ToUpper()); }
            addr = addr << 1;

            //H
            addr = Tools.setBit(addr, 0, 1);
            if (debug) { Console.WriteLine(Convert.ToString(addr, 16).PadLeft(6, '0').ToUpper()); }
            WillemOP.SetAddr(addr);
            WillemOP.SetData(dataH);

            //L
            addr = Tools.setBit(addr, 0, 0);
            if (debug) { Console.WriteLine(Convert.ToString(addr, 16).PadLeft(6, '0').ToUpper()); }
            WillemOP.SetAddr(addr);
            WillemOP.SetData(dataL);

        }

        public static void Write16BitCommand(int addr, byte dataH, byte dataL)
        {
            WillemOP.SetCE_H();
            WillemOP.Write16BitCommandData(addr, dataH, dataL);
            WillemOP.SetCE_L();
            Tools.delayUs(0.02);
        }

        public static void Write16BitCommand(int addr, byte dataL)
        {
            WillemOP.SetCE_H();
            WillemOP.Write16BitCommandData(addr, dataL);
            WillemOP.SetCE_L();
            //Tools.delayUs(0.02);
        }

        public static void Write16BitCommandData(int addr, byte dataL)
        {
            addr = addr << 1;

            //L
            addr = Tools.setBit(addr, 0, 0);
            if (debug) { Console.WriteLine(Convert.ToString(addr, 16).PadLeft(6, '0').ToUpper()); }
            WillemOP.SetAddr(addr);
            WillemOP.SetData(dataL);
        }

        public static void Init(bool close=false)
        {
            if (close == true)
            {
                WillemOP.SetVPP_L();
                Thread.Sleep(10);
            }
            WillemOP.SetVCC_L();
            WillemOP.SetVPP_L();
            WillemOP.SetCE_H();
            Thread.Sleep(10);
            WillemOP.SetData(0);
            Thread.Sleep(10);
            WillemOP.SetAddr(0);
            Thread.Sleep(10);
        }


    }
}
