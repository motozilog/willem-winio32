using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace willem_winio32
{
    class Ini
    {
        static string EXE = "willem-winio32";//带上程序运行的路径，避免出错
        static string Path = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + EXE + ".ini").FullName.ToString();

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        //public Ini(string IniPath = null)
        //{
        //    Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        //}

        public static string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public static void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public static void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public static void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public static bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }
    }
}
