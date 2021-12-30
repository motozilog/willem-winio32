using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace willem_winio32
{
    public class ChipConfig
    {
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Erase { get; set; }
        public bool Register { get; set; }
        public bool ReadId { get; set; }
        public Int64 ChipLength { get; set; }
        public string EraseDelayTime { get; set; }
        public bool EraseDelay { get; set; }
        public string ChipModel { get; set; }
        public string Note { get; set; }
        public string SpecialFunction { get; set; }

        public Image DipSw { get; set;}
        public Image Jumper { get; set; }
        public Image Adapter { get; set; }
        

    }
}
