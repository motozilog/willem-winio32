using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    class ATFBlast
    {
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));
        byte lastValue = 0;

        void ATFBlastRead()
        {
            LPT.SELin(1);// P/V- low Pin:19
            SetRow(0x3F);// RA0-5 high D1~D6
            SetSDIN(1);// SDIN high Pin:9 D0
            SetSCLK(1);// SCLK high Pin:8 D7
            LPT.STB(0);// STB high Pin:11
            Thread.Sleep(100);
            SetSCLK(0);   // SCLK low Pin:8

            //读
            Console.Write("BITS:");
            for (int d = 0; d < 64; d++)
            {
                int s = (d * 32) % 10;
                Console.Write(s);
            }
            Console.WriteLine();
            for (int r = 0; r < 32; r++)
            {
                Console.Write("R:" + r.ToString().PadLeft(2, '0') + ":");
                StrobeRow(r);
                for (int i = 0; i < 64; i++)
                {
                    byte b = ReceiveBit();
                }
                Console.WriteLine();
            }
        }

        byte ReceiveBit()
        {
            byte b = (byte)((LPT.Read379() & 0x40) >> 6);
            Console.Write(Tools.byte2HexStr(b).Substring(1, 1));
            SetSCLK(1);// SCLK high Pin:8 D7
            Thread.Sleep(1);

            //byte b2 = (byte)((LPT.Read379() & 0x40) >> 6);
            //Console.Write(Tools.byte2HexStr(b2));

            SetSCLK(0);// SCLK high Pin:8 D7
            //byte b3 = (byte)((LPT.Read379() & 0x40) >> 6);
            //Console.Write(Tools.byte2HexStr(b3) + " ");

            Thread.Sleep(1);

            return b;
        }


        void StrobeRow(int row)
        {
            SetRow((byte)row);
            Strobe(5);
        }

        void Strobe(int msec)
        {
            LPT.STB(1); //Pin 11
            Thread.Sleep(msec);
            LPT.STB(0); //Pin 11
        }



        void SetSDIN(byte on)
        {
            if (on == 1)
            {
                lastValue |= 0x01;
            }
            else
            {
                lastValue &= 0xFE;
            }
            LPT.Write378(lastValue);
        }

        void SetSCLK(byte on)
        {
            if (on == 1)
            {
                lastValue |= 0x80;
            }
            else
            {
                lastValue &= 0x7F;
            }
            LPT.Write378(lastValue);
        }

        void SetRow(byte row)
        {
            lastValue = (byte)((lastValue & 0x81) | (row << 1));
            LPT.Write378(lastValue);
        }



    }
}
