using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;
using System.IO;

namespace willem_winio32
{
    public class Tools
    {

        //从左到右是76543210
        public static byte setBit(byte data, int index, int bitValue)
        {
            if ((bitValue&0x01) == 1)
            {
                return (byte)(data | 1 << index);
            }
            else
            {
                return (byte)(data & (byte.MaxValue - (1 << index)));
            }
        }

        public static int setBit(int data, int index, int bitValue)
        {
            if ((bitValue & 0x01) == 1)
            {
                return (data | 1 << index);
            }
            else
            {
                return (data & (Int32.MaxValue - (1 << index)));
            }
        }

        public static uint setBit(uint data, int index, int bitValue)
        {
            if ((bitValue & 0x01) == 1)
            {
                return (uint)(data | 1L << index);
            }
            else
            {
                return (uint)(data & (UInt32.MaxValue - (1 << index)));
            }
        }

        public static string byte2Str(byte data)
        {
            return Convert.ToString(data, 2).PadLeft(8, '0');
        }

        public static string byte2HexStr(byte data)
        {
            return Convert.ToString(data, 16).PadLeft(2, '0').ToUpper();
        }

        public static string int2HexStr(int addr)
        {
            return Convert.ToString(addr, 16).PadLeft(6, '0').ToUpper();
        }

        public static string int2HexStr(Int64 addr)
        {
            return Convert.ToString(addr, 16).PadLeft(6, '0').ToUpper();
        }

        public static string uint2HexStr(uint addr)
        {
            return Convert.ToString(addr, 16).PadLeft(8, '0').ToUpper();
        }

        public static int FourByteToIntMSB(byte b4, byte b3, byte b2, byte b1)
        {
            int value = 0;
            value = b1;
            value = value + b2 * 0x100;
            value = value + b3 * 0x10000;
            value = value + b4 * 0x1000000;
            return value;
        }

        public static byte[] int2ByteMSB(int i)
        {
            byte[] b = new byte[4];
            b[0] = (byte)(i >> 24);
            b[1] = (byte)(i >> 16);
            b[2] = (byte)(i >> 8);
            b[3] = (byte)(i);

            return b;
        }

        public static byte[] byte2BitLSB(byte input)
        {
            byte[] result = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                byte value = (byte)((input >> i) & 0x01);
                result[i] = value;
            }
            return result;
        }

        public static byte bit2ByteLSB(byte[] bs)
        {
            byte b = 0;
            for (int i = 0; i < bs.Length; i++)
            {
                b = setBit(b, i, bs[i]);
            }
            return b;
        }

        public static string file2HexStr(byte[] bytes)
        {
            string s = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                if (i % 0x10 == 0)
                {
                    if ((i % 0x10) == 0 && i > 0)
                    {
                        s = s + "\r\n";
                    }
                    s = s + int2HexStr(i) + ":";

                }
                s = s + byte2HexStr(bytes[i]) + " ";
            }
            return s;
        }

        //微秒级延迟
        //1:1ms 0.05:500us
        public static double delayUs(double time)
        {
            Stopwatch stopTime = new Stopwatch();
            stopTime.Start();
            while (stopTime.Elapsed.TotalMilliseconds < time) { }
            stopTime.Stop();
            return stopTime.Elapsed.TotalMilliseconds;
        }

        public static string NowStr(string format)
        {
            string s = DateTime.Now.ToString(format);
            return s;
        }

        public static string CalcMD5(byte[] bytes)
        {
            var Md5 = new MD5CryptoServiceProvider().ComputeHash(bytes);//求哈希值
            return Convert.ToBase64String(Md5);
        }

        public static string CalcMD5(FileStream file)
        {
            BinaryReader r = new BinaryReader(file);

            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开

            byte[] bytes = r.ReadBytes((int)r.BaseStream.Length);
            var Md5 = new MD5CryptoServiceProvider().ComputeHash(bytes);//求哈希值
            file.Close();
            file.Dispose();
            return Convert.ToBase64String(Md5);//将Byte[]数组转为净荷明文(其实就是字符串) 
        }

        //显示进度
        static DateTime startTime = System.DateTime.Now;
        public static void ShowProgress(Int64 i, byte[] data, Int64 baseAddr, int length, int interval = 0x100)
        {
            if (i == baseAddr)
            {
                startTime = System.DateTime.Now;
            }
            if (i % interval == 0)
            {
                DateTime endTime = System.DateTime.Now;
                TimeSpan ts = endTime.Subtract(startTime);
                if ((i - baseAddr) > 0)
                {
                    int totalTime = (int)(ts.TotalSeconds * length / (i - baseAddr));
                    
                    Console.WriteLine("ADDR:" + Convert.ToString(i, 16).PadLeft(6, '0').ToUpper() + " DATA:" + Convert.ToString(data[i], 16).PadLeft(2, '0').ToUpper() + " 用时:" + Math.Round(ts.TotalSeconds, 2).ToString("0.00") + " 预估用时:" + TimePretty(totalTime));
                }
            }
        }

        public static void ShowProgress(Int64 i, byte data, Int64 baseAddr, Int64 length, int interval = 0x100)
        {
            if (i == baseAddr)
            {
                startTime = System.DateTime.Now;
            }
            if (i % interval == 0)
            {
                DateTime endTime = System.DateTime.Now;
                TimeSpan ts = endTime.Subtract(startTime);
                if ((baseAddr+i) > 0)
                {
                    int totalTime = (int)(ts.TotalSeconds * length / (baseAddr + i));

                    Console.WriteLine("ADDR:" +
                        Convert.ToString(baseAddr+i, 16).PadLeft(6, '0').ToUpper() +
                        " DATA:" + Tools.byte2HexStr(data) +
                        " 用时:" + Math.Round(ts.TotalSeconds, 2).ToString("0.00") +
                        " 预估用时:" + TimePretty(totalTime));
                }
            }
        }

        public static string TimePretty(int totalTime)
        {
            string totalTimeStr = Convert.ToString(totalTime);
            if (totalTime > 3600)
            {
                totalTimeStr = (int)(totalTime / 3600) + "小时" + (int)((totalTime % 3600) / 60) + "分";
            }
            else if (totalTime > 60)
            {
                totalTimeStr = (int)(totalTime / 60) + "分";
            }
            else
            {
                totalTimeStr = totalTime + "秒";
            }
            return totalTimeStr;
        }

        public static string Compare(byte[] ori, byte[] current, Int64 baseAddr)
        {
            StringBuilder sb = new StringBuilder();
            if (ori.Length != current.Length)
            {
                return sb.ToString();
            }
            for (int i = 0; i < ori.Length; i++)
            {
                if (ori[i] != current[i])
                {
                    sb.AppendLine(Tools.int2HexStr(i) + ": " + Tools.byte2HexStr(ori[i]) + " " + Tools.byte2HexStr(current[i]));
                }
            }
            return sb.ToString();

        }

        public static string Compare(string ori, string current, Int64 baseAddr)
        {
            Int64 splitLength = G.splitLength;
            StringBuilder sb = new StringBuilder();
            FileStream fsOri = new FileStream(ori, FileMode.Open);
            FileStream fsCurrent = new FileStream(current, FileMode.Open);
            if (fsOri.Length != fsCurrent.Length)
            {
                return sb.ToString();
            }
            for (Int64 addr = 0; addr < ori.Length; addr+=splitLength)
            {
                byte[] byteOri = new byte[splitLength];
                byte[] byteCurrent = new byte[splitLength];

                fsOri.Seek(addr, SeekOrigin.Begin);
                BinaryReader br = new BinaryReader(fsOri);
                br.Read(byteOri, 0, byteOri.Length);
                br.Close();


                fsCurrent.Seek(addr, SeekOrigin.Begin);
                BinaryReader brCurrent = new BinaryReader(fsCurrent);
                brCurrent.Read(byteCurrent, 0, byteCurrent.Length);
                brCurrent.Close();


                for (int p = 0; p < byteOri.Length; p++)
                {
                    if (byteOri[addr] != byteCurrent[addr])
                    {
                        sb.AppendLine(Tools.int2HexStr(addr+p) + ": " + Tools.byte2HexStr(byteOri[addr]) + " " + Tools.byte2HexStr(byteCurrent[addr]));
                    }
                }
            }
            fsOri.Close();
            fsOri.Dispose();
            fsCurrent.Close();
            fsCurrent.Dispose();

            return sb.ToString();

        }


        public static void CopyFile(String oldPath, String newPath)
        {
            FileStream input = null;
            FileStream output = null;
            input = new FileStream(oldPath, FileMode.Open);
            output = new FileStream(newPath, FileMode.Create, FileAccess.ReadWrite);

            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
            output.Close();
            output.Dispose();
            GC.WaitForPendingFinalizers();

            input.Close();
            input.Dispose();
            GC.WaitForPendingFinalizers();
        }
    }
}
