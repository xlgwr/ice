using SlmCommunicator;
using SlmCommunicator.Server;
using Solomon.Log;
using Solomon.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Test.PackageServer.EventHandler;
using CommunicatorHelper = SlmCommunicator.Server.CommunicatorServerHelper;
using ConnectState = SlmCommunicator.Server.ConnectStateServer;

namespace Test.PackageServer
{
    public class TestServerHelper
    {
        private static TestServerHelper _instance = null;
        private static object _lock = new object();
        private CommunicatorHelper _communicator;
        private List<string> _connectionList = new List<string>();
        private bool _isStart = false;
        public int MaxCountClient { get; set; } = 0;
        public Action<string> actionMsg { get; set; }
        public Action<string, bool> AddClientType { get; set; }
        public bool isShowAllMsg { get; set; }
        public bool isPushAsync { get; set; }
        public bool isPushOver { get; set; }
        public List<int> ClientTypeList { get; set; }
        /// <summary>
        /// 主列表
        /// </summary>
        public List<string> ActiveList { get; set; }
        public List<string> ActiveListTrade { get; set; }
        #region 构造方法
        public TestServerHelper()
        {

        }

        public static TestServerHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TestServerHelper();
                    }
                }
            }
            return _instance;
        }
        #endregion

        #region ice通信
        private ResponsePackage getResponsePackage(int serviceNo, string clientId, int err, string errInfo, string data)
        {
            ResponsePackage rp = new ResponsePackage();
            rp.ClientId = clientId;
            if (!string.IsNullOrEmpty(data))
            {
                rp.DataBody = data.MakeStringArray();//TestHelper.GetInstance().GetDataBody(param.ServiceNo);
            }
            rp.ErrorNo = err;
            rp.ErrorInfo = errInfo;
            rp.ServiceNo = serviceNo;
            return rp;
        }

        private ResponsePackage getResponsePackage(int serviceNo, string clientId, int err, string errInfo, string[][] data)
        {
            ResponsePackage rp = new ResponsePackage();
            rp.ClientId = clientId;
            rp.ErrorNo = err;
            rp.ErrorInfo = errInfo;
            rp.ServiceNo = serviceNo;
            rp.DataBody = data;
            return rp;
        }

        private ResponsePackage getErrResponsePackage(int serviceNo, string clientId, int err, string errInfo)
        {
            ResponsePackage rp = new ResponsePackage();
            rp.ClientId = clientId;
            rp.ErrorNo = err;
            rp.ErrorInfo = errInfo;
            rp.ServiceNo = serviceNo;
            rp.DataBody = null;
            return rp;
        }
        private PackageCacheDB getErrPackageCacheDB(int serviceNo, string clientId, int err, string errInfo)
        {
            var rp = new PackageCacheDB();
            rp.ClientId = clientId;
            rp.ErrorNo = err;
            rp.ErrorInfo = errInfo;
            rp.ServiceNo = serviceNo;
            rp.DataBody = null;
            return rp;
        }
        public Dictionary<string, CommunicationClientInfo> allClientInfo = new Dictionary<string, CommunicationClientInfo>();
        /// <summary>
        /// 客户端连接变化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isAdd"></param>
        private void onSessionChange(SessionInfo info, bool isAdd)
        {
            CommunicationClientInfo cInfo = new CommunicationClientInfo();
            cInfo.ClientId = info.ClientId;
            cInfo.IPInfo = info.IP;
            cInfo.LoginDateTime = info.LoginDateTime;
            cInfo.Port = info.Port;
            cInfo.ConnectionId = info.ConnectionId;
            cInfo.ClientType = EnumHelper.GetValue<ClientType>(info.ClientType);
            //发送消息
            App._ActionAll.ClientConnectedStateEvent?.Invoke(isAdd, cInfo);
            //修改列表
            sessionChange(isAdd, info.ConnectionId);

            //if (isAdd)
            //{
            //    //通知前置发送客户信息
            //    ClientManager.GetInstance().QryClientsInfo();
            //}
            //else
            //{
            //    ClientManager.GetInstance().ClearFrontClientList(info.ClientId);
            //} 
            if (isAdd)
            {
                allClientInfo[cInfo.KeyIDAndType] = cInfo;
            }
            else
            {
                allClientInfo.Remove(cInfo.KeyIDAndType);
            }

            var msg = $"{isAdd}:{info.ClientType}:" + cInfo.toJsonStr();
            actionMsg?.Invoke(msg);
            AddClientType?.Invoke(info.ClientType, isAdd);
        }

        private void sessionChange(bool isAdd, string connectionId)
        {
            if (isAdd && !_connectionList.Exists(p => p != null && p.Equals(connectionId)))
            {
                _connectionList.Add(connectionId);
            }
            else
            {
                if (!_connectionList.Exists(p => p != null && p.Equals(connectionId)))
                {
                    return;
                }
                _connectionList.Remove(connectionId);
            }
            GetMaxCount();
            var msg = $"已连接数:{_connectionList.Count},最大数：{MaxCountClient}";
            actionMsg?.Invoke(msg);
        }
        public int GetMaxCount()
        {
            if (_connectionList.Count > MaxCountClient)
            {
                MaxCountClient = _connectionList.Count;
            }
            return MaxCountClient;
        }
        #endregion


        public void Init(string ip, string port, bool iszip = false)
        {
            if (_isStart)
            {
                return;
            }
            if (_communicator == null)
            {
                _communicator = CommunicatorHelper.GetInstance();
            }
            //防止多次启动，多次注册回掉方法
            _communicator.Reset();
            //注册ice连接回调
            _communicator.RegSessionChangeEvent(onSessionChange);
            _communicator.RegConnectStateEvent(onConnectionState);
            _communicator.RegSendCommandHandle(onReceivePackage);
            _communicator.RegSendCommandListHandle(onReceivePackageList);

            _communicator.RegSendPackageCacheDBEvent(onSendPackageCacheDB);
            _communicator.RegSendPackageCacheTradeEvent(onSendPackageCacheTrade);

            _communicator.Init(ip, port, 60, iszip);
        }



        public void Dispose()
        {
            if (_communicator != null)
            {
                _communicator.Dispose();
                _communicator = null;
            }

            _connectionList.Clear();

            GC.Collect();
        }


        private void onConnectionState(ConnectState state)
        {
            string msg = "关闭交易服务...";
            short type = 1;
            if (state == ConnectState.Connect_Fail)
            {
                App._ActionAll.OnFrontServerStateEvent?.Invoke(false);
                type = 2;
                _isStart = false;
            }
            else
            {
                _isStart = true;
                App._ActionAll.OnFrontServerStateEvent?.Invoke(true);
                msg = "交易服务已开启...";
            }
            Console.WriteLine(msg);
            actionMsg?.Invoke(msg);
            LogHelper.WriteSystemLog(msg);
            App._ActionAll.OnSystemMessageEvent(msg, type);
        }
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="serviceNo"></param>
        /// <param name="errNo"></param>
        /// <param name="errInfo"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int SendResponsePackage(string clientId, int serviceNo, int errNo, string errInfo, string[][] data, int type)
        {
            ResponsePackage rp = new ResponsePackage();
            rp.DataBody = data;
            rp.ErrorNo = errNo;
            rp.ErrorInfo = errInfo;
            rp.ServiceNo = serviceNo;

            try
            {
                //根据客户端类型推送
                List<SessionInfo> sessionList = _communicator.GetSessionListByType(type);
                for (int i = 0; i < sessionList.Count; i++)
                {
                    var sendflag = _communicator.SendPackage(rp, sessionList[i].ConnectionId);
                    if (!sendflag)
                    {
                        LogHelper.WriteLog("SendMsgError", rp.toJsonStr());
                    }
                }
            }
            catch (Exception ex)
            {
                App._ActionAll.OnSystemMessageEvent(string.Format("发送推送消息失败！ServiceNo{0},Client{1},ErrorNo{2}", serviceNo, clientId, errNo), 2);
                LogHelper.WriteLog("ErrorLog", "[FrontCommunicationHelper.SendResponsePackage]", ex);
                LogHelper.WriteLog("ErrorLog", string.Format("Send message faild！ServiceNo{0},Client{1},ErrorNo{2}", serviceNo, clientId, errNo));
                return -1;
            }

            return 0;
        }
        /// <summary>
        /// 推送list
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="serviceNo"></param>
        /// <param name="errNo"></param>
        /// <param name="errInfo"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int SendResponsePackageList(ResponsePackageOrder[] rps, int type)
        {
            try
            {
                //根据客户端类型推送
                List<SessionInfo> sessionList = _communicator.GetSessionListByType(type);
                for (int i = 0; i < sessionList.Count; i++)
                {
                    var sendflag = _communicator.SendPackageListAsync(rps, sessionList[i].ConnectionId);
                    if (!sendflag)
                    {
                        actionMsg?.Invoke($"SendMsgError:{sendflag},Async:{isPushAsync}\n{rps.toJsonStr()}");
                        LogHelper.WriteLog("SendMsgError", rps.toJsonStr());
                    }
                }
            }
            catch (Exception ex)
            {
                App._ActionAll.OnSystemMessageEvent($"发送推送list消息失败！条数:{rps.Length}", 2);
                LogHelper.WriteLog("ErrorLog", "[FrontCommunicationHelper.SendResponsePackageList]", ex);
                LogHelper.WriteLog("ErrorLog", string.Format("Send batch message faild！ServiceNo{0}", rps.toJsonStr()));
                return -1;
            }

            return 0;
        }
        #region Cache 推送

        /// <summary>
        /// 根据客户端类型推送
        /// </summary>
        /// <param name="rps"></param>
        /// <param name="type">客户端类型推送</param>
        /// <returns></returns>
        public int SendPackageCacheBaseAsync(PackageCacheDB rps, int type)
        {
            try
            {
                List<SessionInfo> sessionList = _communicator.GetSessionListByType(type);
                for (int i = 0; i < sessionList.Count; i++)
                {
                    var sendflag = _communicator.SendPackageCacheBaseAsync(rps, sessionList[i].ConnectionId);
                    if (!sendflag)
                    {
                        actionMsg?.Invoke($"PushMsgError:{sendflag},Async:{true}\n{rps.ServiceNo}");
                        LogHelper.WriteLog("PushMsgError", rps.toJsonStr());
                    }
                }
            }
            catch (Exception ex)
            {
                App._ActionAll.OnSystemMessageEvent($"发送推送SendPackageCacheBaseAsync消息失败！ServiceNo:{rps.ServiceNo}", 2);
                LogHelper.WriteLog("ErrorLog", $"Push SendPackageCacheBaseAsync message faild！Content:{rps.toJsonStr()} ,\nError:{ex}");
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// 根据客户端类型推送
        /// </summary>
        /// <param name="rps"></param>
        /// <param name="type">客户端类型推送</param>
        /// <returns></returns>
        public int SendPackageCacheTradeAsync(CacheTradeDB rps, int type)
        {
            try
            {
                List<SessionInfo> sessionList = _communicator.GetSessionListByType(type);
                for (int i = 0; i < sessionList.Count; i++)
                {
                    var sendflag = _communicator.SendPackageCacheTradeAsync(rps, sessionList[i].ConnectionId);
                    if (!sendflag)
                    {
                        actionMsg?.Invoke($"PushMsgError:{sendflag},SendPackageCacheTradeAsync Async:{true}\n{rps.PackageData?.ServiceNo}");
                        LogHelper.WriteLog("PushMsgError", rps.toJsonStr());
                    }
                }
            }
            catch (Exception ex)
            {
                App._ActionAll.OnSystemMessageEvent($"发送推送 SendPackageCacheTradeAsync 消息失败！ServiceNo:{rps.PackageData?.ServiceNo}", 2);
                LogHelper.WriteLog("ErrorLog", $"Push SendPackageCacheTradeAsync message faild！Content:{rps.toJsonStr()} ,\nError:{ex}");
                return -1;
            }

            return 0;
        }
        #endregion

        public List<SessionInfo> GetClientList()
        {
            return _communicator.GetSessionList();
        }
        public ConcurrentQueue<int> queueList = new ConcurrentQueue<int>();
        /// <summary>
        /// 收到前置请求
        /// </summary>
        private ResponsePackage onReceivePackage(RequestPackageEx param)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var noticemsg = string.Empty;
                    try
                    {
                        var msg = param.toJsonStr();
                        //Console.WriteLine(msg);
                        noticemsg = $"收到数据服务号：{param.ServiceNo},内容大小：{msg.GetBytesOfString(2)}KB";
                        actionMsg?.Invoke(noticemsg);
                        LogHelper.WriteLog("ReceiveMsg", noticemsg);
                        if (isShowAllMsg)
                        {
                            actionMsg?.Invoke($"收到数据：{msg}");
                            LogHelper.WriteLog("ReceiveMsg", msg);
                        }
                        //queueList.Enqueue(param.ServiceNo);  
                    }
                    catch (Exception ex)
                    {
                        noticemsg = $"收到数据服务号：{param.ServiceNo},BodySize:{param.PackageData.DataBody?.Length},收到数据成功：处理出错。{ex}";
                        actionMsg?.Invoke(noticemsg);
                        LogHelper.WriteErrorLog(ex);
                    }
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            return getErrResponsePackage(param.ServiceNo, param.ClientId, ErrCode.OK, "Test.PackageServer OK Success");
        }

        /// <summary>
        /// 收到前置list请求
        /// </summary>
        private ResponsePackageOrderBackMsg onReceivePackageList(RequestPackageOrder[] param)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        if (isPushOver)
                        {
                            foreach (var item in param)
                            {
                                foreach (var ctype in ClientTypeList)
                                {
                                    ResponsePackageOrder responsePackage = new ResponsePackageOrder()
                                    {
                                        DataBody = item.DataBody,
                                        PushMatchStateBody = item.PushMatchStateBody,
                                        PushOrderStateBody = item.PushOrderStateBody,
                                        SlmMatchBody = item.SlmMatchBody,
                                        SlmOrderBody = item.SlmOrderBody,
                                        SlmOrderJourBody = item.SlmOrderJourBody,
                                        SendOrderStatusBody = item.SendOrderStatusBody,
                                        ServiceNo = item.ServiceNo
                                    };

                                    BatchPushDemoHelper.GetInstance().AddData(responsePackage, ctype);
                                }
                            }
                        }
                        //Console.WriteLine(msg);
                        GetInfo(param);
                        var msg = param.toJsonStr();
                        actionMsg?.Invoke($"收到合包条数：{param.Length},内容大小：{msg.GetBytesOfString(2)}KB");
                        if (isShowAllMsg)
                        {
                            actionMsg?.Invoke($"收到数据：{msg}");
                            LogHelper.WriteLog("ReceiveMsgList", msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        actionMsg?.Invoke($"Size:{param.Length},收到数据成功：处理出错。{ex}");
                        LogHelper.WriteErrorLog(ex);
                    }
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            try
            {

                //queueList.Enqueue(param.ServiceNo); 
                if (param.Length >= 1)
                {
                    var first = param.First();
                    switch (first.ServiceNo)
                    {
                        case (int)ArbittrationSeverServiceNo.RequestServerIdentify:
                            if (first.DataBody.Length > 0)
                            {
                                var currClientInfo = first.DataBody.StringArray2Object<CommunicationBaseInfo>();
                                if (currClientInfo.HasItem())
                                {
                                    var firstInfo = currClientInfo.First();
                                    if (ActiveList.First() == firstInfo.ClientId)
                                    {
                                        firstInfo.ServerIdentify = ServerIdentify.Active;
                                        if (currClientInfo.Count == 1)
                                        {
                                            currClientInfo.Add(new CommunicationBaseInfo() { ClientId = ActiveList.Last(), ClientType = firstInfo.ClientType, IPInfo = firstInfo.IPInfo, IsCrash = IsCrash.NotCrash, ServerIdentify = ServerIdentify.Standby });
                                        }
                                    }
                                    else
                                    {
                                        firstInfo.ServerIdentify = ServerIdentify.Standby;
                                        if (currClientInfo.Count == 1)
                                        {
                                            currClientInfo.Add(new CommunicationBaseInfo() { ClientId = ActiveList.First(), ClientType = firstInfo.ClientType, IPInfo = firstInfo.IPInfo, IsCrash = IsCrash.NotCrash, ServerIdentify = ServerIdentify.Active });
                                        }
                                    }
                                    //主备交易服务器
                                    string clientId = "7777";
                                    for (int i = 0; i < 2; i++)
                                    {
                                        clientId = ActiveListTrade[i];
                                        currClientInfo.Add(new CommunicationBaseInfo() { ClientId = clientId, ClientType = ClientType.TRADE_SERVER, IPInfo = firstInfo.IPInfo, IsCrash = IsCrash.NotCrash, ServerIdentify = i == 0 ? ServerIdentify.Active : ServerIdentify.Standby });
                                    }


                                }
                                return new ResponsePackageOrderBackMsg() { ServiceNo = first.ServiceNo, Head = first.Head, DataBody = currClientInfo.GetListObj().List2StringArray() };
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                actionMsg?.Invoke($"Size:{param.Length},收到数据成功：处理出错。{ex}");
                LogHelper.WriteErrorLog(ex);
            }
            return new ResponsePackageOrderBackMsg() { ErrorNo = ErrCode.OK, ErrorInfo = "Test.PackageServer OK Success" };
        }
        public void GetInfo(RequestPackageOrder[] items)
        {
            foreach (var item in items)
            {
                if (item.SlmOrderBody?.OrderNo <= 0 || item.SlmOrderBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.SlmOrderBody = null;
                }
                if (item.SlmOrderJourBody?.OrderNo <= 0 || item.SlmOrderJourBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.SlmOrderJourBody = null;
                }
                if (item.PushMatchStateBody?.OrderNo <= 0 || item.PushMatchStateBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.PushMatchStateBody = null;
                }
                if (item.PushOrderStateBody?.OrderNo <= 0 || item.PushOrderStateBody.OrderJourNo.IsNullOrWhiteSpace())
                {
                    item.PushOrderStateBody = null;
                }
                if (item.SendOrderStatusBody?.OrderNo <= 0 || item.SendOrderStatusBody.AccountNo.IsNullOrWhiteSpace())
                {
                    item.SendOrderStatusBody = null;
                }
            }
        }
        private CacheTradeDB onSendPackageCacheTrade(CacheTradeDB param)
        {
            if (param.PackageData == null)
            {
                param.PackageData = new PackageCacheDB() { ErrorInfo = "PackageData is Null", ErrorNo = -1 };
            }
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var noticemsg = string.Empty;
                    try
                    {
                        var msg = param.toJsonStr();
                        //Console.WriteLine(msg);
                        noticemsg = $"onSendPackageCacheTrade 收到数据服务号：{param.PackageData?.ServiceNo},内容大小：{msg.GetBytesOfString(2)}KB";
                        actionMsg?.Invoke(noticemsg);
                        LogHelper.WriteLog("ReceiveMsg", noticemsg);
                        if (isShowAllMsg)
                        {
                            actionMsg?.Invoke($"收到数据：{msg}");
                            LogHelper.WriteLog("ReceiveMsg", msg);
                        }
                        //queueList.Enqueue(param.ServiceNo);  
                    }
                    catch (Exception ex)
                    {
                        noticemsg = $"onSendPackageCacheTrade 收到数据服务号：{param.PackageData?.ServiceNo},BodySize:{param.PackageData?.DataBody?.Length},收到数据成功：处理出错。{ex}";
                        actionMsg?.Invoke(noticemsg);
                        LogHelper.WriteErrorLog(ex);
                    }
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();

            var package = getErrPackageCacheDB(param.PackageData.ServiceNo, param.PackageData.ClientId, ErrCode.OK, "Test.PackageServer onSendPackageCacheTrade OK Success");
            return new CacheTradeDB() { PackageData = package };
        }

        private PackageCacheDB onSendPackageCacheDB(PackageCacheDB param)
        {
            Thread checkThread = new Thread(() =>
            {
                var currTask = Task.Factory.StartNew(() =>
                {
                    var noticemsg = string.Empty;
                    try
                    {
                        var msg = param.toJsonStr();
                        //Console.WriteLine(msg);
                        noticemsg = $"onSendPackageCacheDB 收到数据服务号：{param.ServiceNo},内容大小：{msg.GetBytesOfString(2)}KB";
                        actionMsg?.Invoke(noticemsg);
                        LogHelper.WriteLog("ReceiveMsg", noticemsg);
                        if (isShowAllMsg)
                        {
                            actionMsg?.Invoke($"收到数据：{msg}");
                            LogHelper.WriteLog("ReceiveMsg", msg);
                        }
                        //queueList.Enqueue(param.ServiceNo);  
                    }
                    catch (Exception ex)
                    {
                        noticemsg = $"onSendPackageCacheDB 收到数据服务号：{param.ServiceNo},BodySize:{param.DataBody?.Length},收到数据成功：处理出错。{ex}";
                        actionMsg?.Invoke(noticemsg);
                        LogHelper.WriteErrorLog(ex);
                    }
                });
            });
            checkThread.IsBackground = true;
            checkThread.Start();
            var package = getErrPackageCacheDB(param.ServiceNo, param.ClientId, ErrCode.OK, "Test.PackageServer onSendPackageCacheDB OK Success");
            return package;
        }

    }


}
