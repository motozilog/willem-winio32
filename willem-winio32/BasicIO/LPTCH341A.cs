using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CH341;
using System.Threading;

namespace willem_winio32
{
    public class LPTCH341A : ILPT
    {
        //单例模式
        private static LPTCH341A instance;
        private LPTCH341A()
        {
        }

        public static LPTCH341A GetInstance()
        {
            if (instance == null)
            {
                instance = new LPTCH341A();
                try
                {
                    string LPTParamConfig = Ini.Read("LPTParamConfig");
                    delay = Convert.ToInt32(LPTParamConfig);
                }
                catch { }
            }
            Console.WriteLine("DriverType:CH341A(需要安装CH341PAR驱动)");
            return instance;
        }
        //EOF单例模式

        private static int delay = 0;//指令延迟

        private static uint lastData = 0xFFFFFFFF;
        private static uint direction = 0xFFFFFFFF; //10:INT→ACK  13:WAIT→BUSY
//        private static uint direction = 0xFFFFFFFF; //10:INT→ACK  13:WAIT→BUSY
        CH341A ch = new CH341A();

        // Data valid flag:
        // Bit 0 is 1 indicating that bit 15-bit 8 of iSetDataOut is valid, otherwise it is ignored.
        // Bit 1 is 1 indicating that bit 15-bit 8 of iSetDirOut is valid, otherwise it is ignored
        // Bit 2 is 1 to indicate that 7-bit 0 of iSetDataOut is valid, otherwise it is ignored.
        // Bit 3 is 1 to indicate that bit 7-bit 0 of iSetDirOut is valid, otherwise ignore
        // Bit 4 is 1 indicating that bit 23-bit 16 of iSetDataOut is valid, otherwise it is ignored.

        // iSetDirOut and iSetDataOut
        // Bit 7-bit 0 corresponds to the D7-D0 pin of CH341
        // Bit 8 corresponds to the ERR# pin of CH341, 
        // bit 9 corresponds to the PEMP pin of CH341, 
        // bit 10 corresponds to the INT# pin of CH341, 
        // bit 11 corresponds to the SLCT pin of CH341
        // Bit 13 corresponds to the WAIT# pin of CH341, 
        // bit 14 corresponds to the DATAS#/READ# pin of CH341, 
        // bit 15 corresponds to the ADDRS#/ADDR/ALE pin of CH341

        // The following pins can only be output, regardless of I/O direction: 
        // Bit 16 corresponds to CH341 RESET# pin, 
        // Bit 17 corresponds to CH341 WRITE# pin, 
        // Bit 18 corresponds to CH341 SCL pin, 
        // Bit 29 corresponds to CH341 SDA pin
        private void writeBit(int bit, int value)
        {
            lastData = Tools.setBit(lastData, bit, value);
            uint enableBit = 0x0;
            enableBit = Tools.setBit(enableBit, bit, 1);
            writeToCH341(enableBit);
        }

        private void writeToCH341(uint enableBit)
        {
            try
            {
                if (delay != 0)
                {
                    Thread.Sleep(delay);
                }
                ch.SetOutput(direction, direction, lastData);
            }
            catch { Console.WriteLine("无CH341A设备，或CH341A设备跳线错误(需要EPP/MEM模式)"); }
        }

        private uint readFromCH341()
        {
            uint data = 0;
            try
            {
                if (delay != 0)
                {
                    Thread.Sleep(delay);
                }
                ch.GetStatus(out data);
            }
            catch { Console.WriteLine("无CH341A设备，或CH341A设备跳线错误(需要EPP/MEM模式)"); }
            return data;
        }

        public void D0(int value) { writeBit(0, value); }
        public void D1(int value) { writeBit(1, value); }
        public void D2(int value) { writeBit(2, value); }
        public void D3(int value) { writeBit(3, value); }
        public void D4(int value) { writeBit(4, value); }
        public void D5(int value) { writeBit(5, value); }
        public void D6(int value) { writeBit(6, value); }
        public void D7(int value) { writeBit(7, value); }

        public void SELin(int value)    //CE
        {
            writeBit(15, ~value);
        }

        public void Init(int value)     //VCC
        {
            writeBit(16, value);
        }

        public void Auto(int value)     //ADDR/DATA
        {
            writeBit(14, ~value);
        }

        public void STB(int value)      //VPP
        {
            writeBit(17, ~value);
        }

        //10:INT→ACK  13:WAIT→BUSY
        public byte Read379()
        {
            uint data = readFromCH341();
//            Console.WriteLine("Read341:0x" + Tools.uint2HexStr(data));

            byte by = 0;
            uint b10 = (data >> 9) & 0x01;
            uint b13 = (data >> 12) & 0x01;
            by = Tools.setBit(by, 6, (int)b10);
            by = Tools.setBit(by, 7, (int)b13);
//            Console.WriteLine("Read341Byte:" + Tools.byte2Str(by)+" b10:"+b10+" b13:"+b13);

            return by;
        }

        public void Write378(byte value)
        {
            //Console.WriteLine("写378前" + Tools.uint2HexStr(lastData));
            for (int i = 0; i < 8; i++)
            {
                int v2 = (value >> i) & 0x01;
                lastData = Tools.setBit(lastData,i,v2);

            }
            //Console.WriteLine("写378后" + Tools.uint2HexStr(lastData));
            uint enableBit = 0xFF;
            writeToCH341(enableBit);
        }

        public void Initialize()
        {
            Console.WriteLine("打开CH341A设备结果：" + ch.OpenDevice());
        }

        public void Close()
        {
            try
            {
                ch.CloseDevice();
            }
            catch { }
        }


        public LPTConfig GetConfig()
        {
            LPTConfig config = new LPTConfig();
            config.LPTNote = "*采用CH340A时，速度只有LPT的1/10，建议仅作为功能验证，不要进行写片";
            config.LPTNoteImage = willem_winio32.Properties.Resources.CH341A_TO_LPT;
            config.LPTParam = "输出延迟，单位ms：";
            return config;
        }
    }
}
