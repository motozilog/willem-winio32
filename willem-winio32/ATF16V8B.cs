//partical source from ATFBlast.exe - based on GALBLAST by Manfred Winterhoff
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
            config.erasetime = 1000;
            config.progtime = 2;

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
            return readATFGALByS25XX(config);
        }

        private void init()
        {
            WillemOP.SetVPP_L();
            LPT.Auto(1);
            LPT.D2(0);  //P/V- low
            WillemOP.SetAddr(0x3F);
            LPT.Auto(1);
            LPT.D0(1);  //SDIN H
            WillemOP.SetCE_H(); //SCLK H
            LPT.Auto(1);
            LPT.D1(1);  //STB H
            WillemOP.SetVCC_H();
            Thread.Sleep(200);

            WillemOP.SetCE_L(); //SCLK L

            WillemOP.SetVPP_H();
            Thread.Sleep(200);
        }

        private byte[] readATFGALByS25XX(GALConfig config)
        {
            init();

            byte[] fuses = new byte[config.fuses];

            Console.Write("BITS:");
            for (int d = 0; d < 64; d++)
            {
                int s = (d * 32) % 10;
                Console.Write(s);
            }
            Console.WriteLine();

            // read fuse rows ATF16V8
            for (int row = 0; row < config.rows; row++)
            {
                Console.Write("R:" + row.ToString().PadLeft(2, '0') + ":");
                setAddr(row);
                for (int bit = 0; bit < config.bits; bit++)
                {
                    fuses[config.rows * bit + row] = readBit();
                }
                Console.WriteLine();
            }

            // read UES ATF16V8
            setAddr(config.uesrow);
            Console.WriteLine("UES fuse");
            for (int bit = 0; bit < 64; bit++)
            {
                fuses[config.uesfuse + bit] = readBit();
            }
            Console.WriteLine();

            //read CFG ATF16V8
            setAddr(config.cfgrow);
            Console.WriteLine("CFG fuse");
            for (int bit = 0; bit < config.cfg.Length; bit++)
            {
                fuses[config.cfg[bit]] = readBit();
            }

            showFuses(fuses);
            shutdown();

            //保存
            byte[] datas =writeMAMEFusesBin(fuses);
            return datas ;
        }

        private void showFuses(byte[] fuses)
        {
            Console.WriteLine();
            for (int row = 0; row < fuses.Length; row = row + 32)
            {
                Console.Write("L" + row.ToString().PadLeft(5, '0') + " ");
                for (int c = 0; c < 32; c++)
                {
                    if ((row + c) < fuses.Length)
                    {
                        Console.Write(fuses[row + c]);
                    }
                }
                Console.WriteLine("*");
            }
        }

        private void shutdown()
        {
            WillemOP.SetVPP_L();
            Thread.Sleep(10);
            WillemOP.SetVCC_L();
            WillemOP.SetData(0);
            Console.WriteLine();
        }

        private void setAddr(int addr)
        {
            //写地址前的这个动作少不了，否则会写入时写飞
            LPT.Auto(1);
            LPT.D0(1);
            LPT.D1(1);
            LPT.D2(0);  //读写的PV，设地址时必须为0(Read)
            Thread.Sleep(10);

            WillemOP.SetAddr(addr);

            Thread.Sleep(10);
            LPT.Auto(1);
            LPT.D0(1);
            LPT.D1(1);
            LPT.D2(0);  //读写的PV，设地址时必须为0(Read)

            Thread.Sleep(1);
            LPT.D1(0);
            Thread.Sleep(10);
            LPT.D1(1);
            Thread.Sleep(10);
        }

        private byte readBit()
        {
            byte value = WillemOP.ReadSerialOut();
//            byte value = (byte)((WillemOP.Read4021()>>7)&0x01);
            Console.Write(Tools.byte2HexStr(value).Substring(1, 1));
            WillemOP.SetCE_H();
            WillemOP.SetCE_L();
            return value;
        }

        private byte[] readMAMEFusesBin(GALConfig config, byte[] data)
        {
            byte[] fuses = new byte[config.fuses];
            //TODO: MAME bin读入过程
            int fusesCount = Tools.FourByteToIntMSB(data[0], data[1], data[2], data[3]);
            Console.WriteLine("fusesCount:" + fusesCount);
            if (fusesCount != config.fuses)
            {
                Console.WriteLine("熔丝位数量不对，若为jed格式请先用jedutil.exe转成bin后再导入");
                return null;
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
            return fuses;
        }

        private byte[] writeMAMEFusesBin(byte[] fuses)
        {
            //先转换长度
            byte[] fusesLength = Tools.int2ByteMSB(fuses.Length);

            for (int i = 0; i < fusesLength.Length; i++)
            {
                Console.Write(Tools.byte2HexStr(fusesLength[i]));                
            }

            //转换熔丝位
            List<byte> fuseList = new List<byte>();
            for (int f = 0; f < fuses.Length; f = f + 8)
            {
                byte[] bs = new byte[8];
                int index = 8;
                //最后一byte
                if ((f + 8) > fuses.Length)
                {
                    index = fuses.Length - f;
                    Console.WriteLine(index);
                }
                for (int i = 0; i < index; i++)
                {
                    bs[i] = fuses[f + i];
                }
                byte b = Tools.bit2ByteLSB(bs);
                fuseList.Add(b);
            }

            //组装数据
            List<byte> dataList = new List<byte>();
            dataList.AddRange(fusesLength);
            dataList.AddRange(fuseList);

            byte[] data = dataList.ToArray();
            string s = Tools.file2HexStr(data);
            Console.WriteLine(s);
            return data;
        }

        public void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength)
        {
            GALConfig config = makeGALConfig();

            byte[] fuses = readMAMEFusesBin(config, data);
            if (fuses == null) { return; }
            //显示出来
            //showFuses(fuses);

            //开始
            init();
            LPT.Auto(1);
            LPT.D2(0);
            //write fuse array 32*64 = 2048
            for (int row = 0; row < config.rows; row++)
            {
                setAddr(row);
                LPT.Auto(1);
                LPT.D2(1);
                Console.Write("R"+row.ToString().PadLeft(2,'0')+" ");
                for (int bit = 0; bit < config.bits; bit++)
                {
                    byte writeByte = fuses[config.rows * bit + row];
                    Console.Write(writeByte);
                    sendBit(writeByte);
                }
                Console.WriteLine();
                strobe(config.progtime);
            }

            // write UES 64
            Console.WriteLine("UES fuse");
            setAddr(config.uesrow);
            LPT.Auto(1);
            LPT.D2(1);
            for (int bit = 0; bit < 64; bit++)
            {
                byte writeByte = fuses[config.uesfuse + bit];
                Console.Write(writeByte);
                sendBit(writeByte);
            }
            strobe(config.progtime);

            // write CFG
            Console.WriteLine("CFG fuse");
            setAddr(config.cfgrow);
            LPT.Auto(1);
            LPT.D2(1);
            for (int bit = 0; bit < config.cfg.Length; bit++)
            {
                byte writeByte = fuses[config.cfg[bit]];
                Console.Write(writeByte);
                sendBit(writeByte);
            }
            strobe(config.progtime);

            shutdown();                
        }

        private void sendBit(byte b)
        {
            LPT.D0(b);
            WillemOP.SetCE_H();
            WillemOP.SetCE_L();
        }

        public void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength)
        {
            GALConfig config = makeGALConfig();
            eraseATF(config.eraseallrow, config.erasetime);
            Console.WriteLine("擦除签名位结束");
        }

        public void Erase(string args)
        {
            GALConfig config = makeGALConfig();
            eraseATF(config.eraserow, config.erasetime);
        }

        private void eraseATF(int addr, int erasetime)
        {
            init();

            setAddr(addr);
            LPT.Auto(1);
            LPT.D2(1);  // P/V-

            sendBit(1);  //SDIN H
            strobe(erasetime);

            shutdown();

        }

        private void strobe(int ms)
        {
            LPT.D1(0);
            Thread.Sleep(ms);
            LPT.D1(1);
            Thread.Sleep(ms);
        }

        public byte[] ReadId() { return null; }



        public ChipConfig GetConfig()
        {
            ChipConfig config = new ChipConfig();
            config.Erase = true;
            config.Read = true;
            config.Write = true;
            config.ChipLength = 0x117;
            config.ChipModel = "ATF16V8B";
            config.Note = "注意结尾是B。其它结尾没有测试";
            config.SpecialFunction = "擦除签名位";
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
        public int erasetime { get; set; }      //擦除等待时间
        public int progtime { get; set; }   //写入等待时间
    }
}
