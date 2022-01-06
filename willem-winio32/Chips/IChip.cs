using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace willem_winio32
{
    public interface IChip
    {
        byte[] Read(Int64 baseAddr, int length, Int64 totalLength);
        void Write(byte[] data, Int64 baseAddr, int length, Int64 totalLength);
        void Erase(string args);
        byte[] ReadId();
        void SpecialFunction(string Filename, string EraseDelay, string BaseAddr, string TryLength);
        ChipConfig GetConfig();        
    }

    public class ChipFactory
    {
        public static IChip create(string chip)
        {
            IChip ci = new EmptyChip();
            switch (chip)
            {
                case "W27X010":
                    ci = new W27X010();
                    break;
                case "W27C4096":
                    ci = new W27C4096();
                    break;
                case "MX29F1615":
                    ci = new MX29F1615();
                    break;
                case "MX29F1610":
                    ci = new MX29F1610();
                    break;
                case "MX29L3211_8bit":
                    ci = new MX29L3211_8bit();
                    break;
                case "MX29L3211":
                    ci = new MX29L3211_16Bit();
                    break;
                case "M59PW016":
                    ci = new M59PW016();
                    break;
                case "M59PW064":
                    ci = new M59PW064();
                    break;
                case "M59PW032":
                    ci = new M59PW032();
                    break;
                case "M59PW1282":
                    ci = new M59PW1282();
                    break;
                case "MX26L6420":
                    ci = new MX26L6420();
                    break;
                case "MX26L12811":
                    ci = new MX26L12811();
                    break;
                case "S25XX":
                    ci = new S25XX();
                    break;
                case "ATF16V8B":
                    ci = new ATF16V8B();
                    break;

            }
            return ci;
        }
    }
}
