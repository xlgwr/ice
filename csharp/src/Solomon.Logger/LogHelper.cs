using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Solomon.Log
{
    public class MoinitorLog
    {

        public string errorinfo { get; set; }
        public string systeminfo { get; set; }

        public string servertype { get; set; } = "1";
    }

    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper
    {

        delegate int WriteEventHandle(string fileName, string msg, bool addTime);
        #region 内部数据类
        private class LogData
        {
            public string FileName = string.Empty;
            public string LogMsg = string.Empty;

            public LogData(string name, string msg)
            {
                FileName = name;
                LogMsg = msg;
            }
        }
        #endregion
        private static object _lock = new object();
        private static LogHelper _instance = null;
        private static object _writeLock = new object();

        private int _fileSize = 500; //默认文件大小50M
        private string _filePath = "Log";
        private List<string> _errLog = new List<string>();
        private bool _stop = false;
        private Queue<LogData> _dataQueue = new Queue<LogData>();
        ConcurrentDictionary<string, ConcurrentQueue<LogData>> logQueueDatas = new ConcurrentDictionary<string, ConcurrentQueue<LogData>>();

        private Dictionary<string, StreamWriter> _writeList = new Dictionary<string, StreamWriter>();
        private Dictionary<string, int> _filePart = new Dictionary<string, int>();
        /// <summary>
        /// 记录需要按行限制保存，又文件名为key
        /// 行记数
        /// </summary>
        private static Dictionary<string, int> _fileLinePart = new Dictionary<string, int>();
        /// <summary>
        /// 每个文件，最大行限制
        /// 默认为1w行
        /// </summary>
        private int _baseLineLimt { get; set; } = 10000;
        private WriteEventHandle _writeHandle;
        private long _mb = 1024 * 1024;
        public static bool IsSettle { get; set; } = false;

        private static object _fileNameQueueLock = new object();
        #region 每10毫秒更新一次时间变量 
        /// <summary>
        /// 每10毫秒更新一次时间变量
        /// </summary>
        public int timeSpanInterval { get; set; } = 10;
        /// <summary>
        /// 全局时间变量 每10毫秒更新一次
        /// yyyy-MM-dd HH:mm:ss fff
        /// </summary>
        public string DateTimeNowAll { get; private set; }
        /// <summary>
        /// 全局时间变量 每10毫秒更新一次
        /// yyyyMMddHHmmssfff
        /// </summary>
        public string DateTimeNowAll2 { get; private set; }
        /// <summary>
        /// 全局日期变量 每10毫秒更新一次
        /// yyyyMMdd
        /// </summary>
        public string DateTimeNowyyyyMMdd { get; private set; }
        /// <summary>
        /// yyyyMMdd
        /// </summary>
        public int DateTimeNowyyyyMMddInt { get; private set; }
        /// <summary>
        /// 全局时间 每10毫秒更新一次
        /// HHmmss
        /// </summary>
        public string DateTimeNowHHmmss { get; private set; }
        /// <summary>
        /// HHmmss
        /// </summary>
        public int DateTimeNowHHmmssInt { get; private set; }
        /// <summary>
        /// 全局时间 每10毫秒更新一次
        /// yyyyMMdd HHmmss
        /// </summary>
        public string DateTimeNowyyyyMMddHHmmss { get; private set; }
        #endregion

        void AddLogueueDatas(LogData logData)
        {
            if (!logQueueDatas.ContainsKey(logData.FileName))
            {
                var queue = new ConcurrentQueue<LogData> { };
                logQueueDatas.TryAdd(logData.FileName, queue);
            }
            logQueueDatas[logData.FileName].Enqueue(logData);
            // lock (logQueueDatas[logData.FileName])

        }

        void NewQueueWriteLog()
        {
            try
            {
                foreach (var item in logQueueDatas.Keys)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    //if (logQueueDatas[item].Count > 0)
                    //{
                    //    stringBuilder.Append($"logQueueDatas[item].Count:{logQueueDatas[item].Count}");
                    //    stringBuilder.Append(Environment.NewLine);
                    //}
                    for (int i = 0; i < 1000000; i++)
                    {

                        if (logQueueDatas[item].Count > 0)
                        {
                            LogData q = null;
                            //lock (logQueueDatas[item])
                            {
                                logQueueDatas[item].TryDequeue(out q);
                            }

                            if (q == null)
                            {
                                break;
                            }

                            stringBuilder.Append(q.LogMsg);
                            stringBuilder.Append(Environment.NewLine);
                        }
                        else
                        { break; }
                    }
                    var str = stringBuilder.ToString();
                    if (str.IsNotNullOrWhiteSpace())
                    {
                        writeData(item, str, false);
                    }
                }
            }
            catch (Exception)
            {


            }

        }

        public static LogHelper GetInstance()
        {

            return _instance;
        }
        string SystemLog = "SystemLog.log";
        string ErrorLog = "ErrorLog.log";
        int SystemLogRow;
        int ErrorLogRow;

        public MoinitorLog GetLog(string isAll)
        {
            if (isAll == "1")
            {
                SystemLogRow = 0;
                ErrorLogRow = 0;
            }
            string[] systemLogs = new string[0];
            string[] errorLogs = new string[0];
            var filePathSystem = LogHelper.GetInstance().FilePath + SystemLog;
            var filePathErrorlog = LogHelper.GetInstance().FilePath + ErrorLog;
            if (File.Exists(filePathSystem))
            {
                systemLogs = filePathSystem.FilePathGetAllLine(SystemLogRow, Encoding.UTF8).ToArray();
            }
            if (File.Exists(filePathErrorlog))
            {
                errorLogs = filePathErrorlog.FilePathGetAllLine(ErrorLogRow, Encoding.UTF8).ToArray(); ;
            }
            SystemLogRow = SystemLogRow + systemLogs.Count();
            ErrorLogRow = ErrorLogRow + errorLogs.Count();
            MoinitorLog moinitorLog = new MoinitorLog()
            {
                errorinfo = errorLogs.Count() > 0 ? errorLogs.Skip(errorLogs.Length - 100).Take(100).Aggregate((x, y) => { return x + Environment.NewLine + y; }) : "",
                systeminfo = systemLogs.Count() > 0 ? systemLogs.Skip(systemLogs.Length - 100).Take(100).Aggregate((x, y) => { return x + Environment.NewLine + y; }) : ""
            };
            return moinitorLog;
        }
        public string FilePath { get { return _filePath; } }

        public LogHelper(string filePath, int fileSize, int fileLine)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            _filePath = filePath + "\\";
            _fileSize = fileSize;
            _baseLineLimt = fileLine;

            startThread();
            _writeHandle += writeData;
        }
        ~LogHelper()
        {
            _stop = true;
        }

        #region 私有方法 
        private LogData getCommand()
        {
            lock (_dataQueue)
            {
                if (_dataQueue.Count > 0)
                {
                    return _dataQueue.Dequeue();
                }
                return null;
            }
        }
        private void workProcess2()
        {
            while (!_stop)
            {
                try
                {

                    var currTime = DateTime.Now;
                    this.DateTimeNowAll = currTime.ToString("yyyy-MM-dd HH:mm:ss fff");
                    this.DateTimeNowAll2 = currTime.ToString("yyyyMMddHHmmssfff");
                    this.DateTimeNowyyyyMMdd = currTime.ToString("yyyyMMdd");
                    this.DateTimeNowyyyyMMddInt = int.Parse(this.DateTimeNowyyyyMMdd);
                    this.DateTimeNowHHmmss = currTime.ToString("HHmmss");
                    this.DateTimeNowHHmmssInt = int.Parse(this.DateTimeNowHHmmss);
                    this.DateTimeNowyyyyMMddHHmmss = $"{this.DateTimeNowyyyyMMdd} {this.DateTimeNowHHmmss}";

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(timeSpanInterval);
            }

        }
        private void workProcess()
        {
            while (!_stop)
            {
                try
                {
                    //LogData comm = getCommand();
                    //if (comm == null)
                    //{
                    //    Thread.Sleep(1);
                    //    continue;
                    //}
                    ////处理消息
                    //processCommand(comm);
                    NewQueueWriteLog();

                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Thread.Sleep(1);
            }
        }

        private bool haveLog()
        {
            foreach (var item in logQueueDatas.Values)
            {
                if (item?.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 写文件Log
        /// </summary>
        /// <param name="comm"></param>
        private void processCommand(LogData comm)
        {
            //写日志
            writeData(comm.FileName, comm.LogMsg, false);
        }

        private void addCommand(string fileName, string msg)
        {
            lock (_dataQueue)
            {
                msg = string.Format("【{0}】{1}", this.DateTimeNowAll, msg);
                LogData data = new LogData(fileName, msg);
                _dataQueue.Enqueue(data);
            }
        }

        private void startThread()
        {
            Thread logThread = new Thread(workProcess);
            logThread.IsBackground = true;
            logThread.Name = "LogThread";
            logThread.Start();

            Thread TimeThread = new Thread(workProcess2);
            TimeThread.IsBackground = true;
            TimeThread.Name = "TimeThread";
            TimeThread.Start();
        }

        /// <summary>
        /// 初始化filePart
        /// </summary> 
        /// <param name="path">为空时，默认为_filePath</param>
        /// <returns></returns>
        public Dictionary<string, int> GetInitFilePart(string path, bool initFilePath = true)
        {
            if (path.IsNullOrWhiteSpace())
            {
                path = _filePath;
            }
            var result = new Dictionary<string, int>();
            try
            {
                DirectoryInfo fdir = new DirectoryInfo(path);//文件列表   
                FileInfo[] file = fdir.GetFiles();
                if (file.Length == 0) //当前目录文件或文件夹不为空                   
                {
                    return result;
                }
                var currname = string.Empty;
                foreach (FileInfo f in file) //显示当前目录所有文件   
                {
                    currname = Path.GetFileNameWithoutExtension(f.Name);
                    if (currname.IsNullOrWhiteSpace())
                    {
                        continue;
                    }
                    var spliteName = currname.Split('_');

                    string filename = spliteName[0];
                    int partNo = 0;
                    if (spliteName.Length > 1)
                    {
                        int.TryParse(spliteName[1], out partNo);
                    }
                    if (!result.ContainsKey(filename))
                    {
                        result[filename] = partNo;
                    }
                    else
                    {
                        if (partNo > result[filename])
                        {
                            result[filename] = partNo;
                        }
                    }
                }
                if (initFilePath)
                {
                    //初始化_filePart
                    _filePart = result;
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog($"GetDirFileName:{ex.Message}");
            }
            return result;
        }
        /// <summary>
        /// 返回_filePart
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> CurrFilePart()
        {
            return _filePart;
        }
        public string GetPath(string fileName)
        {
            string fName = fileName;
            int part = 0;
            if (_filePart.ContainsKey(fileName))
            {
                part = _filePart[fileName];
            }
            else
            {
                _filePart.Add(fileName, part);
            }
            //写日志

            if (part > 0)
            {
                fName = fName + "_" + part;
            }
            string path = string.Format("{0}{1}.log", _filePath, fName);
            //string logMsg = string.Format("【{0}】{1}", LogHelper.GetInstance().DateTimeNowyyyyMMddHHmmss, msg);

            //if (File.Exists(path))
            //{
            //    FileInfo fiInput = new FileInfo(path);
            //    long len = fiInput.Length;
            //    float mb = (float)len / _mb;

            //    //当文件大小超过设定值，则分块
            //    //if (mb >= _fileSize)
            //    //{
            //    //	part++;
            //    //	_filePart[fileName] = part; //更新
            //    //	fName = fileName + "_" + part;
            //    //	path = string.Format("{0}{1}.log", _filePath, fName);
            //    //}
            //}
            return path;
        }
        /// <summary>
        /// 获取所有当前名字下文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Dictionary<int, string> GetPathAll(string fileName, Dictionary<string, int> currFilePath = null, string filepath = "")
        {
            var result = new Dictionary<int, string>();
            string fName = fileName;
            int part = 0;

            var tmpFilePath = currFilePath != null ? currFilePath : _filePart;
            var tmpPath = filepath.IsNullOrWhiteSpace() ? _filePath : filepath;
            if (tmpFilePath.ContainsKey(fileName))
            {
                part = tmpFilePath[fileName];
            }
            else
            {
                tmpFilePath.Add(fileName, part);
            }
            //写日志
            string path = string.Format("{0}{1}.log", tmpPath, fileName);
            result[0] = path;

            if (part > 0)
            {
                for (int i = 1; i <= part; i++)
                {
                    fName = fileName + "_" + i;
                    path = string.Format("{0}{1}.log", tmpPath, fName);
                    result[i] = path;
                }
            }
            return result;
        }

        private int writeData(string fileName, string msg, bool addTime = true)
        {
            lock (_writeLock)
            {
                string fName = fileName;
                int part = 0;
                if (_filePart.ContainsKey(fileName))
                {
                    part = _filePart[fileName];
                }
                else
                {
                    _filePart.Add(fileName, part);
                }
                //写日志
                try
                {
                    if (part > 0)
                    {
                        fName = fName + "_" + part;
                    }
                    string path = string.Format("{0}{1}.log", _filePath, fName);
                    string logMsg = addTime ? string.Format("【{0}】{1}", this.DateTimeNowAll, msg) : msg;


                    if (_fileLinePart.ContainsKey(fileName))
                    {
                        //按行分文件
                        #region 按行分文件   

                        var allLineString = logMsg.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        int msgLine = allLineString.Length;

                        //总行+要存的行
                        long Alllines = _fileLinePart[fileName] + msgLine;
                        //已存文件数对应行数
                        long currSaveLine = part * _baseLineLimt;
                        //求差，是否大于限制行
                        int currModel = (int)(Alllines - currSaveLine - _baseLineLimt);
                        bool isNewPart = currModel >= 0;
                        if (isNewPart)
                        {
                            //补全当前文件为最大LineLimt
                            logMsg = string.Join(Environment.NewLine, allLineString.Take(msgLine - currModel));
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                            {
                                file.Write(logMsg);// 直接追加文件末尾，换行 
                                file.Flush();
                            }

                            //分文件加剩下的行
                            //分
                            part++;
                            _fileLinePart[fileName] += msgLine;//更新总行数
                            _filePart[fileName] = part; //更新
                            fName = fileName + "_" + part;
                            path = string.Format("{0}{1}.log", _filePath, fName);

                            logMsg = string.Join(Environment.NewLine, allLineString.Skip(msgLine - currModel));
                            if (!logMsg.IsNullOrWhiteSpace())
                            {
                                logMsg += Environment.NewLine;
                            }
                            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                            {
                                file.Write(logMsg);// 直接追加文件末尾，换行 
                                file.Flush();
                            }
                            return 0;
                        }
                        else
                        {
                            _fileLinePart[fileName] += msgLine;//更新总行数
                        }
                        #endregion
                    }
                    else
                    {
                        //按大小分文件
                        if (File.Exists(path))
                        {
                            FileInfo fiInput = new FileInfo(path);
                            #region 按大小分文件
                            long len = fiInput.Length;
                            float mb = (float)len / _mb;

                            //当文件大小超过设定值，则分块
                            if (mb >= _fileSize)
                            {
                                part++;
                                _filePart[fileName] = part; //更新
                                fName = fileName + "_" + part;
                                path = string.Format("{0}{1}.log", _filePath, fName);
                            }
                            #endregion
                        }

                    }

                    //判断writer
                    //if (!_writeList.ContainsKey(fName))
                    //{
                    //    System.IO.StreamWriter file = new System.IO.StreamWriter(path, true);
                    //    _writeList.Add(fName, file);
                    //} 

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                    {
                        file.Write(logMsg);// 直接追加文件末尾，换行 
                        file.Flush();
                    }
                }
                catch //(Exception ex)
                {
                    return -1;
                }
                return 0;
            }
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="info">日志信息</param>
        /// <param name="addTime"></param>
        /// <param name="isLineFilePath">是否按行分文件,默认为1w行</param>
        private static void writeLog(string fileName, string info, bool addTime = true, bool isLineFilePath = false)
        {
            try
            {

                if (IsSettle && fileName == "ErrorLog")
                {
                    if ("行情|网关|前置|认证|关闭|无法连接".Split('|').Where(t => info.Contains(t)).HasItem())
                    {
                        fileName = "SystemLog";
                    }
                }
                //判断实例是否初始化
                if (_instance != null && !string.IsNullOrWhiteSpace(info))
                {
                    if (addTime)
                    {
                        info = string.Format("【{0}】{1}", LogHelper.GetInstance().DateTimeNowAll, info);
                    }

                    //按行分文件
                    if (isLineFilePath && !_fileLinePart.ContainsKey(fileName))
                    {
                        int allLine = GetLineAllByLimit(fileName);
                        //记数初始化
                        _fileLinePart.Add(fileName, allLine);

                    }

                    //使用回调方式调用
                    //  _instance._writeHandle.BeginInvoke(fileName, info, addTime, null, null);

                    //使用线程方式
                    //_instance.addCommand(fileName, info);
                    //高效的写日志
                    _instance.AddLogueueDatas(new LogData(fileName, info) { });


                }

            }
            catch //(Exception ex)
            {

            }
        }
        /// <summary>
        /// 获取限制行数
        /// </summary>
        public int GetBaseLineLimt
        {
            get
            {
                return _baseLineLimt;
            }
        }
        /// <summary>
        /// 返回文件part最后一个文件及对应总行数
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static int GetLineAllByLimit(string filename)
        {
            string fName = filename;
            int part = 0;
            if (_instance == null)
            {
                return 0;
            }
            var filePath = _instance._filePath;
            var filePart = _instance._filePart;
            if (filePart.ContainsKey(filename))
            {
                part = filePart[filename];
            }
            else
            {
                filePart.Add(filename, part);
            }
            //获取最后的文件名
            if (part > 0)
            {
                fName = fName + "_" + part;
            }
            string path = string.Format("{0}{1}.log", filePath, fName);

            var allLine = path.FilePathGetAllLineNumber(Encoding.UTF8);

            allLine = part * _instance._baseLineLimt + allLine;

            return allLine;
        }
        private static string dataFormart(string[] data)
        {
            StringBuilder logStr = new StringBuilder();
            if (data != null && data.Length > 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    logStr.Append(data[i] + ",");
                }
            }
            return logStr.ToString();
        }

        private static string dataFormart(string[][] data)
        {
            StringBuilder logStr = new StringBuilder();
            if (data != null && data.Length > 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != null && data[i].Length > 0)
                    {
                        for (int j = 0; j < data[i].Length; j++)
                        {
                            logStr.Append(data[i][j] + ", ");
                        }
                        logStr.Append(" ; ");
                    }

                }
            }
            return logStr.ToString();
        }
        #endregion

        #region 对外接口方法
        /// <summary>
        /// 初始化日志帮助类
        /// </summary>
        /// <param name="filePath">日志路径</param>
        /// <param name="fileSize">文件大小，单位MB</param>
        /// <returns></returns>
        public static int Init(string filePath, int fileSize, int fileLine = 10000)

        {
            //实列化文件操作
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new LogHelper(filePath, fileSize, fileLine);
                    }
                }
            }

            if (_instance == null || !Directory.Exists(filePath))
            {
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="info">日志信息</param>
        /// <param name="addTime"></param>
        /// <param name="isLineFilePath">是否按行分文件</param>
        public static void WriteLog(string fileName, string info, bool addTime = true, bool isLineFilePath = false)
        {

            writeLog(fileName, info, addTime, isLineFilePath);
        }

        public static string[][] GetErrLog()
        {
            int count = _instance._errLog.Count;
            string[][] infoList = new string[count][];
            for (int i = 0; i < count; i++)
            {
                string[] log = new string[2];
                log[0] = _instance._errLog[i].Substring(0, 23);
                log[1] = _instance._errLog[i].Substring(23, _instance._errLog[i].Length - 23);
                infoList[i] = log;
            }
            return infoList;
        }

        public void Dispose()
        {
            while (true)
            {
                if (haveLog())
                {
                    Thread.Sleep(10);
                    continue;
                }
                break;
            }
            _stop = true;
        }

        #region 写日志方法
        #region 异常日志
        /// <summary>
        /// 异常信息写入
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="info">异常标签</param>
        /// <param name="ex">异常对象</param>
        public static void WriteLog(string fileName, string info, Exception ex)
        {
            try
            {
                WriteLog(fileName, string.Format("【{0}】 {1} \r\n", info, ex.ToString()));
            }
            catch //(Exception e)
            {
                WriteLog(fileName, $"[{info}] {ex.Message}\r\n {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 记录错误日志
        /// </summary>
        public static void WriteErrorLog(string info)
        {
            writeLog("ErrorLog", info);
        }

        public static void CoreJsonLog(string info)
        {
            writeLog("CoreJsonLog", info);
        }
        /// <summary>
        /// 记录错误日志-异常
        /// </summary>
        public static void WriteErrorLog(Exception ex)
        {
            writeLog("ErrorLog", ex.ToString());
        }

        public static void Warn(string message, string source = "", string remark = "", Exception ex = null)
        {
            string msg = message;
            if (source.IsNotNullOrWhiteSpace())
            {
                msg += "/r/n source:" + source;
            }
            if (remark.IsNotNullOrWhiteSpace())
            {
                msg += "/r/n remark:" + remark;
            }
            if (ex != null)
            {
                msg += Environment.NewLine;
                msg += ex.toJsonStr();
            }
            writeLog("Warn", msg);
        }

        public static void Error(Exception ex, string source = "", string remark = "")
        {
            writeLog("ErrorLog", "source:" + source + " remark:" + remark + " /r/nerror:" + JsonConvert.SerializeObject(ex));
        }

        public static void IceError(Exception ex, string source = "", string remark = "")
        {
            writeLog("IceErrorLog", " source: " + source + " remark:" + remark + " /r/nerror:" + JsonConvert.SerializeObject(ex));
        }
        public static void Error(string msg)
        {
            writeLog("ErrorLog", msg);
        }
        #endregion

        #region 系统日志
        /// <summary>
        /// 记录系统日志
        /// </summary>
        public static void WriteSystemLog(string info)
        {
            writeLog("SystemLog", info);
        }
        #endregion

        #region FrontReceive收到前置请求
        /// <summary>
        /// 记录收到前置日志
        /// </summary>
        public static void WriteFrontReceiveLog(string info)
        {
            writeLog("FrontReceive", info);
        }

        /// <summary>
        /// 记录收到前置日志
        /// </summary>
        public static void WriteFrontReceiveLog(string serviceNo, string[] info)
        {
            StringBuilder sb = new StringBuilder(string.Format("ServiceNo【{0}】:", serviceNo));
            for (int i = 0; i < info.Length; i++)
            {
                sb.Append(info[i] + ",");
            }
            writeLog("FrontReceive", sb.ToString());
        }

        public static void WriteFrontReceiveLog(string serviceName, string serviceNo, string[] log)
        {
            string str = string.Format("【{0}】【{1}】{2}", serviceName, serviceNo, dataFormart(log));
            writeLog("FrontReceive", str);
        }
        public static void WriteDBReceiveLog(string serviceName, string serviceNo, string[] log)
        {
            string str = string.Format("{0}|{1}", serviceNo, dataFormart(log));
            writeLog("news_recover_data", str, false);
        }
        public static void WriteDBSMSLog(string serviceName, string serviceNo, string[] log)
        {
            string str = string.Format("{0}|{1}", serviceNo, dataFormart(log));
            writeLog("sms_recover_data", str, false);
        }

        /// <summary>
        /// 请求上手日志
        /// </summary>
        /// <param name="info"></param>
        public static void ProxySendForTestTwo(int serverNo, string[] header, string[] dataBody, string type = "")
        {
            string log = LogFormatHelper.GetInstance().FormatRequestTest(serverNo, header, dataBody);
            if (!string.IsNullOrWhiteSpace(log))
            {
                LogHelper.WriteLog("ProxySendForTestTwo", $"{type}:{dataBody?.Length}:{log}");
            }
        }
        /// <summary>
        /// 将SqlParameter，转为字符值数组
        /// </summary>
        /// <param name="replaceChar"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static List<string> getSqlParameterList(List<SqlParameter> commandParameters, bool replaceChar = true, string charReplace = ",", string charTo = "&", string newline = "^")
        {
            var result = new List<string>();

            foreach (var item in commandParameters)
            {
                var toadd = item.Value.NullToStr();
                if (replaceChar && toadd.IsNotNullOrWhiteSpace())
                {
                    toadd = toadd.Replace(charReplace, charTo);
                    toadd = toadd.Replace("\r\n", newline);
                    toadd = toadd.Replace("\n", newline);
                }
                result.Add(toadd);
            }

            return result;
        }
        #endregion

        #region FrontSend发送给前置
        /// <summary>
        /// 记录发送到前置日志
        /// </summary>
        public static void WriteFrontSendLog(string info)
        {
            writeLog("FrontSend", info);
        }

        /// <summary>
        /// 记录发送到前置日志
        /// </summary>
        public static void WriteFrontSendLog(string serviceNo, string errNo, string errInfo, string[][] dataCount, string syncOrAsync, string[] header = null)
        {
            int count = 0;
            int col = header != null ? header.Length : 0;
            if (dataCount != null && dataCount.Length > 0)
            {
                count = dataCount.Length;
            }
            string str = string.Format("【{4}】【{0}】errNo:{1},errInfo:{2},dataCount:{3},col:{5},Header:{6}", serviceNo, errNo, errInfo, count, syncOrAsync, col, header.toJsonStr());
            writeLog("FrontSend", str);
        }
        #endregion

        #region ProxyReceive收到上手回复
        /// <summary>
        /// 记录收到上手日志
        /// </summary>
        public static void WriteProxyReceiveLog(string info)
        {
            writeLog("ProxyReceive", info);
        }
        #endregion

        #region 发送到上手
        /// <summary>
        /// 记录发送到上手日志
        /// </summary>
        public static void WriteProxySendLog(string info)
        {
            writeLog("ProxySend", info);
        }
        #endregion

        #endregion
        #endregion
    }
}
