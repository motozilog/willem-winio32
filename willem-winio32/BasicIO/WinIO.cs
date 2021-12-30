using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace willem_winio32
{
    public class WinIO
    {
        static uint ADDR_378 = 0x00000378;
        static uint ADDR_379 = 0x00000379;
        static uint ADDR_37A = 0x0000037A;

        [DllImport("WinIo32.dll")]
        public static extern bool InitializeWinIo();

        [DllImport("WinIo32.dll")]
        public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);

        [DllImport("WinIo32.dll")]
        public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);

        [DllImport("WinIo32.dll")]
        public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);

        [DllImport("WinIo32.dll")]
        public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);

        [DllImport("WinIo32.dll")]
        public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);

        [DllImport("WinIo32.dll")]
        public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);

        [DllImport("WinIo32.dll")]
        public static extern void ShutdownWinIo();

        public static void Write378(byte by)
        {
            WinIO.SetPortVal(ADDR_378, (IntPtr)by, 1);
        }

        public static void Write37A(byte by)
        {
            WinIO.SetPortVal(ADDR_37A, (IntPtr)by, 1);
        }

        public static byte Read378()
        {
            int value = 0;
            WinIO.GetPortVal((IntPtr)ADDR_378, out value, 1);
            return (byte)value;
        }

        public static byte Read379()
        {
            int value = 0;
            WinIO.GetPortVal((IntPtr)ADDR_379, out value, 1);
            return (byte)value;
        }

        public static byte Read37A()
        {
            int value = 0;
            WinIO.GetPortVal((IntPtr)ADDR_37A, out value, 1);
            return (byte)value;
        }

        public static void Set37A(int index, int value)
        {
            byte data = WinIO.Read37A();
//            Console.WriteLine(Tools.byte2Str(data));
            data = Tools.setBit(data, index, value);
//            Console.WriteLine(Tools.byte2Str(data));
            WinIO.Write37A(data);
        }

        public static void Set378(int index, int value)
        {
            byte data = WinIO.Read378();
//            Console.WriteLine(Tools.byte2Str(data));
            data = Tools.setBit(data, index, value);
//            Console.WriteLine(Tools.byte2Str(data));
//            Thread.Sleep(1);
            WinIO.Write378(data);
//            Thread.Sleep(1);
        }


        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint Ucode, uint uMapType);


        private WinIO()
        {
            IsInitialize = true;
        }
        public static bool Initialize()
        {
            try
            {
                if (InitializeWinIo())
                {
                    IsInitialize = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void Shutdown()
        {
            if (IsInitialize)
                ShutdownWinIo();
            IsInitialize = false;
        }

        private static bool IsInitialize { get; set; }
    }
}
