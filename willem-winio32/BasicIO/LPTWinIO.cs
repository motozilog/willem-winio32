using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace willem_winio32
{
    public class LPTWinIO : ILPT
    {
        //单例模式
        private static LPTWinIO instance;
        private LPTWinIO()
        {
        }

        public static LPTWinIO GetInstance()
        {
            if (instance == null)
            {
                instance = new LPTWinIO();
            }
            Console.WriteLine("DriverType:WinIO(LPT打印口)");
            return instance;
        }
        //EOF单例模式


        public void D0(int value) { WinIO.Set378(0, value); }
        public void D1(int value) { WinIO.Set378(1, value); }
        public void D2(int value) { WinIO.Set378(2, value); }
        public void D3(int value) { WinIO.Set378(3, value); }
        public void D4(int value) { WinIO.Set378(4, value); }
        public void D5(int value) { WinIO.Set378(5, value); }
        public void D6(int value) { WinIO.Set378(6, value); }
        public void D7(int value) { WinIO.Set378(7, value); }

        public void SELin(int value) { WinIO.Set37A(3, value); }
        public void Init(int value) { WinIO.Set37A(2, value); }
        public void Auto(int value) { WinIO.Set37A(1, value); }
        public void STB(int value) { WinIO.Set37A(0, value); }

        public void Initialize() { WinIO.Initialize(); }


        public void Write378(byte value) {
            //Console.WriteLine(Tools.byte2Str(value)); 
            WinIO.Write378(value); }
        public byte Read379() { return WinIO.Read379(); }

        public void Close()
        {

        }

        public LPTConfig GetConfig()
        {
            LPTConfig config = new LPTConfig();
            config.LPTNote= "需要LPT打印口，默认地址为0x378。\r\n若为Win7-64/Win10-64，需要开启调试模式";
            config.LPTParam = "接口地址(默认：0x378)：";
            return config;
        }

    }
}
