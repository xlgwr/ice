using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tools.OpenExe.Exts
{
    public static class DateTimeExts
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMiliseconds;
        }

        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        /// <summary>
        /// 修改当前电脑时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool SetLocalTimeByDateTime(this DateTime dt)
        {
            bool flag = false;
            SystemTime sysTime = new SystemTime();
            //DateTime dt = Convert.ToDateTime(timestr);
            sysTime.wYear = Convert.ToUInt16(dt.Year);
            sysTime.wMonth = Convert.ToUInt16(dt.Month);
            sysTime.wDay = Convert.ToUInt16(dt.Day);
            sysTime.wHour = Convert.ToUInt16(dt.Hour);
            sysTime.wMinute = Convert.ToUInt16(dt.Minute);
            sysTime.wSecond = Convert.ToUInt16(dt.Second);
            try
            {
                flag = SetLocalTime(ref sysTime);
            }
            catch (Exception e)
            {
                Console.WriteLine("SetSystemDateTime函数执行异常" + e.Message);
            }
            return flag;
        }
    }
}
