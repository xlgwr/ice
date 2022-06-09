using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmCommunicator.Server
{

    /// <summary>
    /// 服务器端的通信帮助类
    /// </summary>
    public class CommunicatorServerHelper
    {
        #region 属性变量
        private static CommunicatorServerHelper _instance = null;
        private static object _lock = new object();
        #endregion

        #region 构造方法
        public CommunicatorServerHelper()
        {

        }

        public static CommunicatorServerHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new CommunicatorServerHelper();
                    }
                }
            }
            return _instance;
        }

        ~CommunicatorServerHelper()
        {

        }
        #endregion


        #region 私有方法
        private string[][] dataRowCol2StringArray(DataRowCollection drCol)
        {
            int outterCount = drCol.Count;
            string[][] rtn = new string[outterCount][];
            for (int num = 0; num < outterCount; num++)
            {
                string[] array = new string[drCol[num].ItemArray.Count()];
                object[] names = (object[])drCol[num].ItemArray;
                for (int count = 0; count < names.Count(); count++)
                {
                    array[count] = names[count].ToString();
                }
                rtn[num] = array;
            }
            return rtn;
        }

        private string[][] Object2StringArray(object objValue)
        {
            string[][] rtn = new string[][] { };
            if (objValue == null) return rtn;
            Type type = objValue.GetType();


            if (type.FullName.Equals("System.String[][]"))
                rtn = objValue as string[][];
            else
            {
                rtn = new string[1][];
                rtn[0] = new string[] { objValue.ToString() };
            }
            return rtn;
        }
        #endregion

        #region 公有方法
        /// <summary>
        /// 注册默认事件
        /// </summary>
        private void RegInitDefaultEven()
        {
            //注册接收数据事件
            EventServerHelper.GetInstance().RegReceivePackageEvent(SessionServerManager.GetInstance().ReceivePackage);
            EventServerHelper.GetInstance().RegReceivePackageListEvent(SessionServerManager.GetInstance().ReceivePackageList);
            EventServerHelper.GetInstance().RegReceivePackageListAsyncEvent(SessionServerManager.GetInstance().ReceivePackageListAsync);
            EventServerHelper.GetInstance().RegSendPackageByTypeEvent(SessionServerManager.GetInstance().ReceivePackageByType);
            EventServerHelper.GetInstance().RegReceivePackageCacheBaseAsyncEvent(SessionServerManager.GetInstance().ReceivePackageCacheBaseAsync);
            EventServerHelper.GetInstance().RegReceivePackageCacheTradeEvent(SessionServerManager.GetInstance().ReceivePackageCacheTrade);
            EventServerHelper.GetInstance().RegReceivePackageCacheTradeAsyncEvent(SessionServerManager.GetInstance().ReceivePackageCacheTradeAsync);
        }
        /// <summary>
        /// 初始化通信
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public int Init(string ip, string port, int ACM_TIME_OUT = 60, bool iszip = false)
        {
            //启动ice连接
            IceServerHelper.GetInstance().ACM_TIME_OUT = ACM_TIME_OUT;

            //注册
            RegInitDefaultEven();

            //init
            IceServerHelper.GetInstance().Init(ip, port, iszip);

            return 0;
        }


        /// <summary>
        /// 初始化SSL通信
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="sslPath">ssl路径</param>
        /// <param name="pemName">pem文件名</param>
        /// <param name="p12Name">p12文件名</param>
        /// <param name="caPass">ca密码</param>
        /// <returns>初始化结果 0成功</returns>
        public int SSLInit(string ip, string port, string sslPath, string pemName, string p12Name, string caPass)
        {
            //去掉路径最后的斜杠
            string tmp = sslPath;
            while (tmp.Substring(tmp.Length - 1, 1) == "\\" || tmp.Substring(tmp.Length - 1, 1) == "/")
            {
                tmp = tmp.Substring(0, tmp.Length - 1);
            }

            //检查ssl参数
            if (!System.IO.Directory.Exists(sslPath) || !System.IO.File.Exists(tmp + "\\" + pemName) || !System.IO.File.Exists(tmp + "\\" + p12Name))
            {
                return -1;
            }

            //注册
            RegInitDefaultEven();

            //启动ice连接
            IceServerHelper.GetInstance().SSLInit(ip, port, sslPath, pemName, p12Name, caPass);

            return 0;
        }

        public void ResetSendCallback()
        {
            EventServerHelper.GetInstance().ResetSendCallback();
        }

        public void Reset()
        {
            EventServerHelper.GetInstance().ResetAll();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            //断开客户端
            //SessionServerManager.GetInstance().Dispose();
            //关闭ice
            IceServerHelper.GetInstance().Dispose();
        }
        //public byte[] readByteSeq(SlmOrderRow requestPackageOrderLog)
        //{
        //    return IceServerHelper.GetInstance().readByteSeq(requestPackageOrderLog);
        //}
        //public SlmOrderRow GetSlmOrderRow(byte[] readByteSeq)
        //{
        //    return IceServerHelper.GetInstance().GetSlmOrderRow(readByteSeq);
        //}
        /// <summary>
        /// 注册接收消息回调事件
        /// </summary>
        /// <param name="call"></param>
        public void RegSendCommandHandle(SendPackageServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendPackageEvent(handler);
        }
        /// <summary>
        /// 注册接收消息回调事件
        /// </summary>
        /// <param name="call"></param>
        public void RegSendCommandListHandle(SendPackageListServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendPackageListEvent(handler);
        }
        #region cache Reg
        public void RegSendPackageCacheDBEvent(SendPackageCacheDBServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendPackageCacheDBEvent(handler);
        }
        public void RegSendPackageCacheTradeEvent(SendPackageCacheTradeServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendPackageCacheTradeEvent(handler);
        }
        /// <summary>
        /// 注册推送回调,回包处理
        /// </summary>
        /// <param name="handler"></param>
        public void RegReceivePackageCacheTradeAsyncCallBackEvent(ReceivePackageCacheTradeServerAsyncCallBackEventHandler handler)
        {
            EventServerHelper.GetInstance().RegReceivePackageCacheTradeAsyncCallBackEvent(handler);
        }
        #endregion
        /// <summary>
        /// 推送 异步消息
        /// </summary>
        /// <param name="package"></param>
        public bool SendPackage(ResponsePackage package, string connectId)
        {
            return EventServerHelper.GetInstance().OnReceivePackageEvent(package, connectId);
        }
        /// <summary>
        ///  推送 同步list消息
        /// </summary>
        /// <param name="package"></param>
        public bool SendPackageList(ResponsePackageOrder[] package, string connectId)
        {
            return EventServerHelper.GetInstance().OnReceivePackageListEvent(package, connectId);
        }
        /// <summary>
        ///  推送 异步list消息
        /// </summary>
        /// <param name="package"></param>
        public bool SendPackageListAsync(ResponsePackageOrder[] package, string connectId)
        {
            return EventServerHelper.GetInstance().OnReceivePackageListAsyncEvent(package, connectId);
        }
        #region cache
        /// <summary>
        ///  推送 异步 PackageCacheBaseAsync 消息
        /// </summary>
        /// <param name="package"></param>
        public bool SendPackageCacheBaseAsync(PackageCacheDB package, string connectId)
        {
            return EventServerHelper.GetInstance().OnReceivePackageCacheBaseAsyncEvent(package, connectId);
        }
        /// <summary>
        ///  推送 异步 ReceivePackageCacheTradeAsync 消息
        /// </summary>
        /// <param name="package"></param>
        public bool SendPackageCacheTradeAsync(CacheTradeDB package, string connectId)
        {
            return EventServerHelper.GetInstance().OnReceivePackageCacheTradeAsyncEvent(package, connectId);
        }
        /// <summary>
        ///  推送 同步 ReceivePackageCacheTrade 消息
        /// </summary>
        /// <param name="package"></param>
        public PackageCacheDB SendPackageCacheTrade(CacheTradeDB package, string connectId)
        {
            return EventServerHelper.GetInstance().OnReceivePackageCacheTradeEvent(package, connectId);
        }
        #endregion
        #region log
        /// <summary>
        /// 注册接收消息回调事件log
        /// </summary>
        /// <param name="call"></param>
        public void RegSendLogCommandHandle(SendPackageLogServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendPackageLogEvent(handler);
        }
        /// <summary>
        /// 注册接收消息回调事件log list
        /// </summary>
        /// <param name="call"></param>
        public void RegSendLogCommandListHandle(SendPackageLogListServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendLogPackageListEvent(handler);
        }
        /// <summary>
        /// 获取所有连接对象
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, SessionInfo> GetSessionDic()
        {
            return SessionServerManager.GetInstance().GetSessionDic();
        }
        /// <summary>
        /// 注册接收消息回调事件log list Request
        /// </summary>
        /// <param name="call"></param>
        public void RegSendLogRequestPackageOrderListServerEventHandle(SendLogRequestPackageOrderServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendLogRequestPackageOrderListServerEvent(handler);
        }
        /// <summary>
        /// 注册接收消息回调事件log list Response
        /// </summary>
        /// <param name="call"></param>
        public void RegSendLogResponsePackageOrderListServerEventHandle(SendLogResponsePackageOrderServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSendLogResponsePackageOrderListServerEvent(handler);
        }
        #endregion
        /// <summary>
        /// 给某类客户端发送消息
        /// </summary>
        /// <param name="package"></param>
        public bool SendPackage(ResponsePackage package, int type)
        {
            return EventServerHelper.GetInstance().OnSendPackageByTypeEvent(package, type);
        }

        /// <summary>
        /// 注册客户端连接断开状态
        /// </summary>
        /// <param name="handler"></param>
        public void RegSessionChangeEvent(SessionChangeServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegSessionChangeEvent(handler);
        }

        /// <summary>
        /// 获取所有连接对象
        /// </summary>
        /// <returns></returns>
        public List<SessionInfo> GetSessionList()
        {
            return SessionServerManager.GetInstance().GetSessionList();
        }

        /// <summary>
        /// 获取所有连接对象
        /// </summary>
        /// <returns></returns>
        public List<SessionInfo> GetSessionListByType(int type)
        {
            return SessionServerManager.GetInstance().GetSessionList(type);
        }
        /// <summary>
        /// 获取所有连接对象
        /// </summary>
        /// <returns></returns>
        public SessionInfo GetSessionListByIdAndType(string idandtype)
        {
            return SessionServerManager.GetInstance().GetSessionConnectIdAndType(idandtype);
        }
        /// <summary>
        /// 获取所有对象
        /// </summary>
        /// <returns></returns>
        public SessionInfo GetSessionConnectId(string connectId)
        {
            return SessionServerManager.GetInstance().GetSessionConnectId(connectId);
        }

        /// <summary>
        /// 注册服务状态
        /// </summary>
        /// <param name="handler"></param>
        public void RegConnectStateEvent(ConnectStateServerEventHandler handler)
        {
            EventServerHelper.GetInstance().RegConnectChangeEvent(handler);
        }

        /// <summary>
        /// dataTable对象打包方法
        /// </summary>
        /// <param name="serviceNo"></param>
        /// <param name="clientId"></param>
        /// <param name="errNo"></param>
        /// <param name="errInfo"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private ResponsePackage MakeRspPackageEx(int serviceNo, string clientId, int errNo, string errInfo, DataTable table)
        {
            ResponsePackage rsp = new ResponsePackage();
            rsp.ClientId = clientId;
            rsp.ServiceNo = serviceNo;
            rsp.ErrorNo = errNo;
            rsp.ErrorInfo = errInfo;
            if (table != null)
            {
                string[] head = new string[table.Columns.Count];
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    head[i] = table.Columns[i].ColumnName.ToLower();
                }
                rsp.Head = head;
                rsp.DataBody = dataRowCol2StringArray(table.Rows);
            }
            return rsp;
        }

        /// <summary>
        /// 通用对象打包方法
        /// </summary>
        /// <param name="serviceNo"></param>
        /// <param name="clientId"></param>
        /// <param name="errNo"></param>
        /// <param name="errInfo"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        public ResponsePackage MakeRspPackage(int serviceNo, string clientId, int errNo, string errInfo, object returnValue)
        {
            if (returnValue is DataTable)
            {
                return MakeRspPackageEx(serviceNo, clientId, errNo, errInfo, returnValue as DataTable);
            }

            ResponsePackage rsp = new ResponsePackage();
            rsp.ClientId = clientId;
            rsp.ServiceNo = serviceNo;
            rsp.ErrorNo = errNo;
            rsp.ErrorInfo = errInfo;
            rsp.Head = new string[1] { "value" };
            if (returnValue != null)
            {
                rsp.DataBody = Object2StringArray(returnValue);
            }
            return rsp;
        }

        /// <summary>
        /// 推送行情数据专用
        /// </summary>
        /// <param name="connectionId">数量</param>
        /// <param name="type">商品类型</param>
        /// <param name="nfArray">实时更新数据列表</param>
        /// <param name="dArray">深度行情列表</param>
        public void PushPackage(string connectionId, byte type, List<NewFlashData> nfArray, List<DeepData> dArray)
        {
            SessionServerManager.GetInstance().PushPackage(connectionId, type, nfArray, dArray);
        }

        /// <summary>
        /// 主动断开客户端连接
        /// </summary>
        /// <param name="id"></param>
        public void RemoveClientById(string id)
        {
            SessionServerManager.GetInstance().CloseClientById(id);
        }
        #endregion
    }
}
