using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Solomon.Log
{

    public class CvsLogHelper
    {
        private static CvsLogHelper _instance = null;
        private static object _lock = new object();
        private StreamWriter swProxySend = null;
        private StreamWriter swProxyReceive = null;
        private StreamWriter swFrontSend = null;
        private StreamWriter swFrontReceive = null;
        private bool _start = false;
        public static bool IsRuning = false;
        private Queue<string[]> _commandQueue = new Queue<string[]>();

        public CvsLogHelper()
        {

        }

        public static CvsLogHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CvsLogHelper();
                        IsRuning = true;
                    }
                }
            }
            return _instance;
        }

        ~CvsLogHelper()
        {
            Dispose();
        }

        #region 私有方法
        private string[] getCommand()
        {
            lock (_commandQueue)
            {
                if (_commandQueue.Count > 0)
                {
                    return _commandQueue.Dequeue();
                }
                return null;
            }
        }

        private void stopThread()
        {
            _start = false;
        }

        private void workProcess()
        {
            _start = true;
            while (_start)
            {
                try
                {

                    string[] comm = getCommand();
                    if (comm == null)
                    {
                        Thread.Sleep(1);
                        continue;
                    }
                    //处理消息
                    processCommand(comm);
                    //Thread.Sleep(1);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        /// <summary>
        /// 处理易盛回调消息
        /// </summary>
        /// <param name="comm"></param>
        private void processCommand(string[] comm)
        {
            //记录日志
            try
            {
                if (comm[0] == "1")
                {
                    swProxySend.WriteLine(comm[1]);
                    swProxySend.Flush();
                }
                else if (comm[0] == "2")
                {
                    swProxyReceive.WriteLine(comm[1]);
                    swProxyReceive.Flush();
                }
                else if (comm[0] == "3")
                {
                    swFrontSend.WriteLine(comm[1]);
                    swFrontSend.Flush();
                }
                else if (comm[0] == "4")
                {
                    swFrontReceive.WriteLine(comm[1]);
                    swFrontReceive.Flush();
                }
            }
            catch
            {

            }
        }

        private void AddCommand(string[] comm)
        {
            lock (_commandQueue)
            {
                _commandQueue.Enqueue(comm);
            }
        }

        private void startThread()
        {
            Thread checkThread = new Thread(workProcess);
            checkThread.IsBackground = true;
            checkThread.Name = "CsvLogThread";
            checkThread.Start();
        }
        #endregion

        public int Init(string tradeDate)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "CSVLog";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = AppDomain.CurrentDomain.BaseDirectory + "CSVLog\\" + tradeDate;

            try
            {
                swProxySend = new StreamWriter(filePath + " ProxySend.csv", true, Encoding.UTF8);
                swProxyReceive = new StreamWriter(filePath + " ProxyReceive.csv", true, Encoding.UTF8);
                swFrontSend = new StreamWriter(filePath + " FrontSend.csv", true, Encoding.UTF8);
                swFrontReceive = new StreamWriter(filePath + " FrontReceive.csv", true, Encoding.UTF8);
            }
            catch
            {
                Dispose();
                return -1;
            }

            startThread();
            return 0;
        }

        public void Dispose()
        {
            stopThread();

            if (swProxySend != null)
            {
                try
                {
                    swProxySend.Close();
                    swProxySend.Dispose();
                    swProxySend = null;
                }
                catch
                {

                }
            }

            if (swProxyReceive != null)
            {
                try
                {
                    swProxyReceive.Close();
                    swProxyReceive.Dispose();
                    swProxyReceive = null;
                }
                catch
                {

                }
            }

            if (swFrontSend != null)
            {
                try
                {
                    swFrontSend.Close();
                    swFrontSend.Dispose();
                    swFrontSend = null;
                }
                catch
                {

                }
            }

            if (swFrontReceive != null)
            {
                try
                {
                    swFrontReceive.Close();
                    swFrontReceive.Dispose();
                    swFrontReceive = null;
                }
                catch
                {

                }
            }
        }

        #region 发送到上手
        public void WriteProxySendLog(string msg, string type)
        {
            string[] comm = new string[2];
            comm[0] = "1";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + msg;
            AddCommand(comm);
        }

        public void WriteProxySendLog(string[] msg, string type)
        {
            string log = "";
            for (int i = 0; i < msg.Length; i++)
            {
                log += msg[i] + ",";
            }

            string[] comm = new string[2];
            comm[0] = "1";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            AddCommand(comm);
        }

        public void WriteProxySendLog<T>(T obj, string type)
        {
            string log = "";
            Type type1 = typeof(T);

            PropertyInfo[] propertys = type1.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                log += p.GetValue(obj, null) + ",";
            }

            string[] comm = new string[2];
            comm[0] = "1";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            AddCommand(comm);
        }
        #endregion

        #region 接收上手消息
        public void WriteProxyReceiveLog(string msg, string type)
        {
            string[] comm = new string[2];
            comm[0] = "2";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + msg;
            AddCommand(comm);
        }

        public void WriteProxyReceiveLog(string[] msg, string type)
        {
            string log = "";
            for (int i = 0; i < msg.Length; i++)
            {
                log += msg[i] + ",";
            }
            string[] comm = new string[2];
            comm[0] = "2";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            AddCommand(comm);
        }

        public void WriteProxyReceiveLog<T>(T obj, string type)
        {
            string log = "";
            Type type1 = typeof(T);

            PropertyInfo[] propertys = type1.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                log += p.GetValue(obj, null) + ",";
            }

            string[] comm = new string[2];
            comm[0] = "2";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            AddCommand(comm);
        }
        #endregion

        #region 发送到前置
        public void WriteFrontSendLog(string msg, string type)
        {
            string[] comm = new string[2];
            comm[0] = "3";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + msg;
            AddCommand(comm);
        }

        public void WriteFrontSendLog(string[] msg, string type)
        {
            string log = "";
            for (int i = 0; i < msg.Length; i++)
            {
                log += msg[i] + ",";
            }
            string[] comm = new string[2];
            comm[0] = "3";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            AddCommand(comm);
        }

        public void WriteFrontSendLog<T>(T obj, string type)
        {
            //string log = "";
            //Type type1 = typeof(T);

            //PropertyInfo[] propertys = type1.GetProperties();
            //foreach (PropertyInfo p in propertys)
            //{
            //    log += p.GetValue(obj, null) + ",";
            //}

            //string[] comm = new string[2];
            //comm[0] = "3";
            //comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            //AddCommand(comm);
        }
        #endregion

        #region 接收前置消息
        public void WriteFrontReceiveLog(string msg, string type)
        {
            string[] comm = new string[2];
            comm[0] = "4";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + msg;
            AddCommand(comm);
        }

        public void WriteFrontReceiveLog(string[] msg, string type)
        {
            //string log = "";
            //for (int i = 0; i < msg.Length; i++)
            //{
            //    log += msg[i] + ",";
            //}
            //string[] comm = new string[2];
            //comm[0] = "4";
            //comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            //AddCommand(comm);
        }

        public void WriteFrontReceiveLog<T>(T obj, string type)
        {
            string log = "";
            Type type1 = typeof(T);

            PropertyInfo[] propertys = type1.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                log += p.GetValue(obj, null) + ",";
            }

            string[] comm = new string[2];
            comm[0] = "4";
            comm[1] = DateTime.Now.ToString("yyyyMMdd HHmmss,") + type + "," + log;
            AddCommand(comm);
        }
        #endregion
    }
}
