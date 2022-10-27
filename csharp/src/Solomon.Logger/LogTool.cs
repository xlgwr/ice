using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.DataType;

namespace Solomon.Log
{
    public static class LogTool
    {
        /// <summary>
        /// 传入队列中处理日志
        /// 文件目录,主参数对象,标题或其它信息,类型
        /// </summary>
        public static Action<string, object, string, int> AllMessageToDoQueueAction { get; set; }
        public static void InitLog()
        {
            LogHelper.Init(System.Environment.CurrentDirectory + "/Log", 50);
        }
        /// <summary>
        /// 根据分割时间,来处理文件目录
        /// </summary>
        /// <param name="divTime"></param>
        /// <returns></returns>
        public static string GetDivLogPath(int divTime)
        {
            int nowTime = Convert.ToInt32(DateTime.Now.ToString("HHmm"));
            string date = DateTime.Now.ToString("yyyyMMdd");
            if (divTime > nowTime)
            {
                date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            }
            return System.Environment.CurrentDirectory + "\\Log\\" + date;
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="info"></param>
        /// <param name="addTime"></param>
        /// <param name="isLineFilePath">,默认为1w行</param>
        public static void WriteLog(string filename, string info, bool addTime = true, bool isLineFilePath = false)
        {
            LogHelper.WriteLog(filename, info, addTime, isLineFilePath);
        }
        /// <summary>
        /// 程序错误日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteErrorLog(string info)
        {
            LogHelper.WriteLog("ErrorLog", info);

        }
        /// <summary>
        /// 程序错误日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteErrorLog(object info)
        {
            LogHelper.WriteLog("ErrorLog", info.toJsonStr());

        }
        /// <summary>
        /// 程序错误日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="error"></param>
        public static void WriteErrorLog(string info, Exception error)
        {
            LogHelper.WriteLog("ErrorLog", info, error);
        }
        /// <summary>
        /// 程序错误日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="error"></param>
        public static void RECOVERYError(string info, Exception error = null)
        {
            LogHelper.WriteLog("RecoveryError", info, error);
        }

        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteSystemLog(string info)
        {
            LogHelper.WriteLog("SystemLog", info);

        }
        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="error"></param>
        public static void WriteSystemLog(string info, Exception error)
        {
            LogHelper.WriteLog("SystemLog", info, error);
        }

        /// <summary>
        /// 数据库日志
        /// </summary>
        /// <param name="info"></param>
        public static void WriteDBLog(string info)
        {
            LogHelper.WriteLog("DBLog", info);
        }

        public static void WriteOperationLog(int serNo, string info)
        {
            LogHelper.WriteLog("OperationLog", "[" + serNo + "][" + info + "]");
        }

        /// <summary>
        /// 数据库日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="error"></param>
        public static void WriteDBLog(string info, Exception error)
        {
            LogHelper.WriteLog("DBLog", info, error);
        }

        public static void Send(int serNo, string[] data)
        {
            string log = LogFormatHelper.GetInstance().FormatRequest(serNo, data);
            if (string.IsNullOrWhiteSpace(log))
            {
                log = data?.toJsonStr();
            }
            LogHelper.WriteLog("Send", log);
        }

        public static void Send(string serverLogEnum, int serverNo, string[] data)
        {
            var param = data.Length > 0 ? data.toJsonStr() : "";
            var log = $"server:【{serverLogEnum.ToString()}】,serverNo:【{serverNo}】\r\n\t param->【{param}】";
            LogHelper.WriteLog("Send", log);
        }

        public static void Receive(int serNo, string[][] data, int errorNo, string errorInfo)
        {
            string log = LogFormatHelper.GetInstance().FormatResponse(serNo, data, errorNo, errorInfo);
            if (string.IsNullOrWhiteSpace(log))
            {
                log = $"{serNo},Length:{data?.toJsonStr()},{errorInfo}";
            }
            LogHelper.WriteLog("Receive", log);
        }
    }
}
