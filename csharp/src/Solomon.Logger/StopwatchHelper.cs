using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Logger
{
    public class StopwatchHelper
    {
        private static Stopwatch stopwatch = new Stopwatch();
        private static List<string> watchInfos = new List<string>();

        public static void Record(string methodName)
        {
            watchInfos.Add($"【{DateTime.Now}】{methodName}->{stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public static void Restart()
        {
            stopwatch.Restart();
        }

        public static void Print(string path)
        {
            System.IO.File.AppendAllLines(path, watchInfos);
            watchInfos.Clear(); 
            stopwatch.Restart();
        }

        public static void PrintDesktop(string fileName)
        {
            var allPath = $@"C:\Users\53021\Desktop\{fileName}";
            Print(allPath);
        }



    }
}
