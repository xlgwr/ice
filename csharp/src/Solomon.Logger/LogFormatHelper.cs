using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Log
{
    public class LogFormatHelper
    {

        private static LogFormatHelper mLogFormatHelper;

        private Dictionary<int, ServerLogClass> servers = new Dictionary<int, ServerLogClass>();
        private LogFormatHelper()
        {
            List<ServerLogClass> ss = new FrontLogFormat().GetAllServerClass();

            foreach (var pa in ss)
            {
                if (!servers.ContainsKey(pa.ServerNo))
                {
                    servers.Add(pa.ServerNo, pa);
                }
            }

            ss = new LogFormat().GetAllServerClass();

            foreach (var pa in ss)
            {
                if (!servers.ContainsKey(pa.ServerNo))
                {
                    servers.Add(pa.ServerNo, pa);
                }
            }

            ss = new TradeServerLogFormat().GetAllServerClass();

            foreach (var pa in ss)
            {
                if (!servers.ContainsKey(pa.ServerNo))
                {
                    servers.Add(pa.ServerNo, pa);
                }
            }

        }


        private static object _lock = new object();
        public static LogFormatHelper GetInstance()
        {
            if (mLogFormatHelper == null)
            {
                lock (_lock)
                {
                    mLogFormatHelper = new LogFormatHelper();
                }
            }
            return mLogFormatHelper;
        }

        public string FormatRequest(int serverNo, string[] par)
        {
            if (servers.ContainsKey(serverNo))
            {
                ServerLogClass slc = servers[serverNo];
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append(serverNo);
                sb.Append("]");
                sb.Append("[");
                sb.Append(slc.ServerName);
                sb.Append("]");
                sb.Append("[");
                sb.Append(LogHelper.GetInstance().DateTimeNowAll);
                sb.Append("]");
                sb.AppendLine();


                if (par != null)
                {
                    sb.Append("[");
                    for (int i = 0; i < par.Length && i < slc.Keys.Length; i++)
                    {
                        sb.Append(slc.Keys[i] + "=" + par[i]).Append(",");
                    }
                    sb.Append("]");
                }

                return sb.ToString();
            }
            return $"{serverNo}:{par.toJsonStr()}";
        }
        /// <summary>
        /// 转为测试用json
        /// </summary>
        /// <param name="serverNo"></param>
        /// <param name="header"></param>
        /// <param name="databody"></param>
        /// <returns></returns>
        public string FormatRequestTest(int serverNo, string[] header, string[] databody)
        {
            try
            {

                ServerLogClass slc = new ServerLogClass();
                if (servers.ContainsKey(serverNo))
                {
                    slc = servers[serverNo];
                }
                PackageRequestDTO requestDTO = new PackageRequestDTO()
                {
                    ServiceNo = serverNo,
                    ServiceName = slc?.ServerName,
                    Head = header?.ToList(),
                    DataBody = databody?.ToList()
                };
                return requestDTO.ToJsonString();

            }
            catch //(Exception ex)
            {
                return "";
            }
        }
        public string GetServerName(int serverNo)
        {
            string serverName = string.Empty;
            if (servers.ContainsKey(serverNo))
            {
                serverName = servers[serverNo]?.ServerName;
            }
            return serverName;
        }
        public string FormatResponse(int serverNo, string[][] par, int errorCode, string errorInfo)
        {
            //if (servers.ContainsKey(serverNo))
            //{
            //    ServerLogClass slc = servers[serverNo];
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append("[");
            //    sb.Append(serverNo);
            //    sb.Append("]");
            //    sb.Append("[");
            //    sb.Append(slc.ServerName);
            //    sb.Append("]");
            //    sb.Append("[");
            //    sb.Append(LogHelper.GetInstance().DateTimeNowAll);
            //    sb.Append("]");

            //    sb.Append("[");
            //    sb.Append("ErrorCode=" + errorCode);
            //    sb.Append("]");
            //    if (!string.IsNullOrWhiteSpace(errorInfo))
            //    {
            //        sb.Append("[");
            //        sb.Append("ErrorInfo=" + errorInfo);
            //        sb.Append("]");
            //    }


            //    sb.AppendLine();

            //    if (par != null)
            //    {
            //        foreach (string[] param in par)
            //        {
            //            sb.Append("[");
            //            for (int i = 0; i < param.Length && i < slc.Keys.Length; i++)
            //            {
            //                sb.Append(slc.Keys[i] + "=" + param[i]).Append(",");
            //            }
            //            sb.Append("]");
            //            sb.AppendLine();
            //        }

            //    }

            //    return sb.ToString();
            //}
            return $"{serverNo}:{par?.Length}";
        }
        public string FormatResponseTest(int serverNo, string[][] par, int errorCode, string errorInfo)
        {
            if (servers.ContainsKey(serverNo))
            {
                ServerLogClass slc = servers[serverNo];
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append(serverNo);
                sb.Append("]");
                sb.Append("[");
                sb.Append(slc.ServerName);
                sb.Append("]");
                sb.Append("[");
                //sb.Append(LogHelper.GetInstance().DateTimeNowAll);     
                sb.Append(LogHelper.GetInstance().DateTimeNowAll);
                sb.Append("]");

                sb.Append("[");
                sb.Append("ErrorCode=" + errorCode);
                sb.Append("]");
                if (!string.IsNullOrWhiteSpace(errorInfo))
                {
                    sb.Append("[");
                    sb.Append("ErrorInfo=" + errorInfo);
                    sb.Append("]");
                }


                sb.AppendLine();

                if (par != null)
                {
                    foreach (string[] param in par)
                    {
                        sb.Append("[");
                        for (int i = 0; i < param.Length && i < slc.Keys.Length; i++)
                        {
                            sb.Append(slc.Keys[i] + "=" + param[i]).Append(",");
                        }
                        sb.Append("]");
                        sb.AppendLine();
                    }

                }

                return sb.ToString();
            }
            return "0";
        }
    }


}
