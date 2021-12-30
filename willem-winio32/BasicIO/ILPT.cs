using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace willem_winio32
{
    public interface ILPT
    {
       void D0(int value);
       void D1(int value);
       void D2(int value);
       void D3(int value);
       void D4(int value);
       void D5(int value);
       void D6(int value);
       void D7(int value);

       void SELin(int value);
       void Init(int value);
       void Auto(int value);
       void STB(int value);

       byte Read379();
       void Write378(byte value);
       void Initialize();
       void Close();
       LPTConfig GetConfig();
    }

    public class LPTFactory
    {
        public static ILPT create(string LPTType)
        {
            ILPT ci = null;
            switch (LPTType)
            {
                case "WinIO":
                    ci = LPTWinIO.GetInstance();
                    break;
                case "CH341A":
                    ci = LPTCH341A.GetInstance();
                    break;
                default:
                    ci = LPTWinIO.GetInstance();
                    break;
            }
            return ci;
        }
    }

}
