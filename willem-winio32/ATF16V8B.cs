using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace willem_winio32
{
    public class ATF16V8B : IChip
    {
        static ILPT LPT = LPTFactory.create(Ini.Read("LPTDeviceType"));
        public GALConfig makeGALConfig()
        {
            GALConfig config = new GALConfig();
            config.fuses = 2194;
            config.pins = 20;
            config.rows = 32;
            config.bits = 64;
            config.uesrow = 32;
            config.uesfuse = 2056;
            config.uesbytes = 8;
            config.eraserow = 63;
            config.eraseallrow = 54;
            config.pesrow = 58;
            config.pesbytes = 8;
            config.cfgrow = 60;

            int[] cfg16V8AB = new int[] { 
                2048, 2049, 2050, 2051, 2193, 2120, 2121, 2122, 
                2123, 2128, 2129, 2130, 2131, 2132, 2133, 2134, 
                2135, 2136, 2137, 2138, 2139, 2140, 2141, 2142, 
                2143, 2144, 2145, 2146, 2147, 2148, 2149, 2150, 
                2151, 2152, 2153, 2154, 2155, 2156, 2157, 2158, 
                2159, 2160, 2161, 2162, 2163, 2164, 2165, 2166, 
                2167, 2168, 2169, 2170, 2171, 2172, 2173, 2174, 
                2175, 2176, 2177, 2178, 2179, 2180, 2181, 2182, 
                2183, 2184, 2185, 2186, 2187, 2188, 2189, 2190, 
                2191, 2124, 2125, 2126, 2127, 2192, 2052, 2053, 2054, 2055 };
            config.cfg = cfg16V8AB;

            config.cfgbits = 82;
            return config;
        }




        public byte[] Read(Int64 baseAddr, int length, Int64 totalLength)
        {
            GALConfig config = makeGALConfig();
            byte[] fuses = new byte[config.fuses];

            byte[] data = new byte[config.fuses+4];
            return data;
        }

        private void PowerOn()
        {
            WillemOP.SetVCC_H();
            Thread.Sleep(100);
            WillemOP.SetVPP_H();
            Thread.Sleep(100);

            //CE作为CLK
            WillemOP.SetCE_L();
            WillemOP.SetAddr(0);

            //SetVPP(0);    // VPP off
            //SetPV(0);     // P/V- low
            //SetRow(0x3F); // RA0-5 high
            //SetSDIN(1);   // SDIN high
            //SetSCLK(1);   // SCLK high
            //SetSTB(1);    // STB high
            //SetVCC(1);    // turn on VCC (if controlled)
            //Delay(100);
            //SetSCLK(0);   // SCLK low
            //if (writeorerase)
            //{
            //    SetVPP(1); // VPP = programming voltage
            //}
            //else
            //{
            //    SetVPP(0); // VPP = +12V
            //}
            //Delay(20);

        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            PowerOn();

            GALConfig config = makeGALConfig();
            byte[] fuses = new byte[config.fuses];
            //TODO: MAME bin读入过程
            int fusesCount = Tools.FourByteToIntMSB(data[0], data[1], data[2], data[3]);
            Console.WriteLine("fusesCount:" + fusesCount);
            if (fusesCount != config.fuses)
            {
                Console.WriteLine("熔丝位数量不对，若为jed格式请先用jedutil.exe转成bin后再导入");
                return;
            }

            List<byte> fusesList = new List<byte>();
            for (int i = 4; i < data.Length; i++)
            {
                byte[] bits = Tools.byte2BitLSB(data[i]);
                fusesList.AddRange(bits);
            }

            //List转Array
            for (int i = 0; i < config.fuses; i++)
            {
                fuses[i] = fusesList[i];
            }

            //显示出来
            for (int i = 0; i < fuses.Length; i++)
            {
                if (i % 32 == 0) { Console.WriteLine(); Console.Write(i.ToString().PadLeft(5, '0') + ":"); }
                if (i % 8 == 0) { Console.Write(" "); }
                Console.Write(fuses[i]);
            }
            

                
            //SetPV(1); //TODO
            //write fuse array 32*64 = 2048
            for (int row = 0; row < config.rows; row++)
            {
                WillemOP.SetAddr(row);
                for (int bit = 0; bit < config.bits; bit++)
                {
                    SendBit(fuses[config.rows * bit + row]);
                }
                //Strobe(progtime); //TODO
            }

            // write UES 64
            WillemOP.SetAddr(config.uesrow);
            for (int bit = 0; bit < 64; bit++)
            {
                SendBit(fuses[config.uesfuse + bit]);
            }
            //Strobe(progtime);

            // write CFG
            WillemOP.SetAddr(config.cfgrow);
            for (int bit = 0; bit < 82; bit++)
            {
                SendBit(fuses[config.cfg[bit]]);
            }
            //Strobe(progtime);
            //SetPV(0);
        }

        private void SendBit(byte b)
        {
            LPT.D0(b);
            WillemOP.SetCE_H();
            WillemOP.SetCE_L();
        }

        public void Erase(string args)
        {
            GALConfig config =  makeGALConfig();
            PowerOn();

            //EraseGAL
            //SetPV(1);
            WillemOP.SetAddr(config.eraserow);

            //if (gal == GAL16V8 || gal == ATF16V8B || gal == GAL20V8)
            //{
            SendBit(1);
            //}
            //Strobe(erasetime);
            //SetPV(0);


            //EraseWholeGAL
            WillemOP.SetAddr(config.eraseallrow);
            //SetPV(1);
            //if (gal == GAL16V8 || gal == ATF16V8B || gal == GAL20V8)
            //{
                SendBit(1);
            //}
            //Strobe(erasetime);
            //SetPV(0);

        }

        public byte[] ReadId()
        {
            GALConfig config = makeGALConfig();
            PowerOn();
            StrobeRow(0x3B);
            byte[] pes = new byte[config.pesbytes];
            for (int by = 0; by < config.pesbytes; by++)
            {
                byte value = 0;
                for (int i = 0; i < 8;i++ )
                {
                    byte recValue = ReceiveBit();
                    //Console.WriteLine("by:" + Tools.byte2Str(recValue) + ":" + Tools.byte2HexStr(recValue));
                }               
            }

            //    for(byte=0;byte<galinfo[gal].pesbytes;byte++)
            //        {
            //        pes[byte]=0;
            //        for(bitmask=0x1;bitmask<=0x80;bitmask<<=1)
            //            {
            //            if(ReceiveBit()) pes[byte]|=bitmask;
            //        }
            //    }

            // F=0x46 1=0x31 6=0x36 V=0x56(01010110) 8=0x38(00111000)
            //if (pes[6]=='F' && pes[5]=='1' && pes[4]=='6' && pes[3]=='V' && pes[2]=='8')
            //{
            //   type = ATF16V8B;
            //}

            return new byte[1];
        }

        public byte ReceiveBit()
        {
            byte b = 0;
            Thread.Sleep(5);
            WillemOP.SetCE_L();
            Thread.Sleep(5);
            b = WillemOP.Read4021();
            Console.WriteLine("ReceiveBit:" + Tools.byte2Str(b));
            WillemOP.SetCE_H();
            Thread.Sleep(5);
            b = WillemOP.Read4021();
            Console.WriteLine("ReceiveBit:" + Tools.byte2Str(b));
            WillemOP.SetCE_L();
            Thread.Sleep(5);
            //BOOL bit;
            //bit=GetSDOUT();
            //SetSCLK(1);
            //SetSCLK(0);
            //return bit;
            b = (byte)((b & 0x80) >> 7);
            return b;
        }

        


        public void StrobeRow(int row)
        {
            //Strobe(2);
            //1.SET Auto = 0,地址输出模式
            WillemOP.SetAddr(row);
            Thread.Sleep(20);
            LPT.Auto(1);
            Thread.Sleep(20);
            LPT.Auto(0);



            //switch (gal)
            //{
            //    case GAL16V8:
            //    case GAL20V8:
            //    case ATF16V8B:
            //        SetRow(row);         // set RA0-5 to row number
            //        Strobe(2);           // pulse /STB for 2ms
            //        break;
            //    case GAL22V10:
            //    case ATF22V10B:
            //    case ATF22V10C:
            //        SetRow(0);           // set RA0-5 low
            //        SendAddress(6, row);  // send row number (6 bits)
            //        SetSTB(0);
            //        SetSTB(1);           // pulse /STB
            //        SetSDIN(0);          // SDIN low
            //}




        }



        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
            WillemOP.SetAddr(0);
            WillemOP.SetVCC_H();
            Thread.Sleep(1000);
            WillemOP.SetVPP_H();
            Thread.Sleep(1000);

            WillemOP.SetAddr(0x3B);
            LPT.Auto(1);
            WillemOP.SetData(0);
            LPT.D1(1);
            Thread.Sleep(1000);
            WillemOP.SetData(0xFF);
            LPT.D1(0);
            Thread.Sleep(1000);
            WillemOP.SetData(0);
            LPT.Auto(0);
            Thread.Sleep(1000);
            LPT.Auto(0);

            byte b = 0;
            for (int i = 0; i < 64; i++)
            {
                b = WillemOP.Read4021();
                Console.Write(i+":"+Tools.byte2HexStr(b)+" ");
                WillemOP.SetCE_H();
                Thread.Sleep(10);
                b = WillemOP.Read4021();
                Console.Write(i + ":" + Tools.byte2HexStr(b) + " ");

                WillemOP.SetCE_L();
                Thread.Sleep(10);
                b = WillemOP.Read4021();
                Console.WriteLine(i + ":" + Tools.byte2HexStr(b));

            }
        }

        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ReadId = true;
            config.ChipLength = 0x117;
            config.ChipModel = "ATF16V8B";
            config.Note = "注意结尾是B。其它结尾没有测试";
            config.SpecialFunction = "测试";
            config.DipSw = willem_winio32.Properties.Resources.MX29F1615;
            config.Adapter = willem_winio32.Properties.Resources.SOP44_16Bit_Adapter;
            return config;
        }
    }

    public class GALConfig
    {
        public int fuses { get; set; }        /* total number of fuses              */
        public int pins { get; set; }         /* number of pins on chip             */
        public int rows { get; set; }         /* number of fuse rows                */
        public int bits { get; set; }         /* number of fuses per row            */
        public int uesrow { get; set; }       /* UES row number                     */
        public int uesfuse { get; set; }      /* first UES fuse number              */
        public int uesbytes { get; set; }     /* number of UES bytes                */
        public int eraserow { get; set; }     /* row adddeess for erase             */
        public int eraseallrow { get; set; }  /* row address for erase all          */
        public int pesrow { get; set; }       /* row address for PES read/write     */
        public int pesbytes { get; set; }     /* number of PES bytes                */
        public int cfgrow { get; set; }       /* row address of config bits         */
        public int[] cfg { get; set; }         /* pointer to config bit numbers      */
        public int cfgbits { get; set; }      /* number of config bits              */
    }
}
