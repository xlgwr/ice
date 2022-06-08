using Ice;
using System;
using System.Collections.Generic;
using System.IO;

using Solomon.Log;
using System.Diagnostics;
using Solomon.Logger;

namespace SlmCommunicator.Server
{
    public class SessionInfo
    {
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string LoginDateTime { get; set; }
        public bool IsLogin { get; set; }
        public string Ver { get; set; }
        public string ClientType { get; set; }
        public string UserType { get; set; }

        private Dictionary<string, string> _userDic = new Dictionary<string, string>();
        public Dictionary<string, string> UserDic
        {
            get { return _userDic; }
        }

        public ConnectionServerEvent ConnEvent = null;
        public PackageReceiverPrx PackagePrx;
        public QuotPackagePrx QuotPackage;
        public string ClientTypeDesc
        {
            get
            {
                try
                {
                    var getEnum = EnumHelper.GetValue<ClientType>(ClientType);
                    return getEnum.GetDescription();
                }
                catch (System.Exception ex)
                {

                }
                return ClientType;
            }
        }
        public string IpPort
        {
            get
            {
                return $"{IP}:{Port}";
            }
        }
    }

    public class ConnectionServerEvent
    {
        private string _connectionId = string.Empty;

        public ConnectionServerEvent(string connectionId)
        {
            _connectionId = connectionId;
        }

        public void closed(Connection con)
        {
            LogHelper.WriteLog("HeartBeat", $"Server Closed: {_connectionId} {con.ice_toString_()}");
            SessionServerManager.GetInstance().RemoveClient(_connectionId, true);

        }

        public void heartbeat(Connection con)
        {
            LogHelper.WriteLog("HeartBeat", $"Server {_connectionId} {con.getInfo().toJsonStr()}");
        }
    }

    /// <summary>
    /// 会话帮助类
    /// </summary>
    public class SessionServerManager
    {
        #region 属性变量
        private static SessionServerManager _instance = null;
        private static object _lock = new object();
        private static object _lockPush = new object();
        private static object _lockPushlist = new object();
        private static object _lockRemove = new object();
        //private List<SessionInfo> _sessionList = new List<SessionInfo>();
        private Dictionary<string, SessionInfo> _sessionList = new Dictionary<string, SessionInfo>();
        private Dictionary<string, List<SessionInfo>> _sessionGroupList = new Dictionary<string, List<SessionInfo>>();
        #endregion

        #region 构造方法
        public SessionServerManager()
        {

        }

        public static SessionServerManager GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionServerManager();
                    }
                }
            }
            return _instance;
        }

        ~SessionServerManager()
        {

        }
        #endregion

        #region 私有方法


        #endregion

        #region 公有方法
        public int AddClient(string clientId, string connectionId, string ip, string port, int type, PackageReceiverPrx callback, QuotPackagePrx quotCallback, ConnectionServerEvent conn)
        {
            lock (_lock)
            {
                if (_sessionList.ContainsKey(connectionId))
                {
                    //清理旧对象
                    var oldSessionInfo = _sessionList[connectionId];
                    LogHelper.CoreJsonLog($"开始断开旧对象:{connectionId}:{oldSessionInfo.IP}:{oldSessionInfo.Port}");
                    oldSessionInfo.PackagePrx?.ice_getConnection()?.setCallbackNull();
                    oldSessionInfo.QuotPackage?.ice_getConnection()?.setCallbackNull();
                    oldSessionInfo.PackagePrx?.ice_getConnection()?.close(ConnectionClose.Forcefully);
                    oldSessionInfo.QuotPackage?.ice_getConnection()?.close(ConnectionClose.Forcefully);
                    LogHelper.CoreJsonLog($"开始清理旧对象:{connectionId}:{oldSessionInfo.IP}:{oldSessionInfo.Port}");
                    RemoveClient(connectionId, false);
                    //return 1;
                }
                SessionInfo info = new SessionInfo();
                info.ClientId = clientId;
                info.ConnectionId = connectionId;
                info.IP = ip;
                info.Port = port;
                info.ConnEvent = conn;
                info.QuotPackage = quotCallback;
                info.PackagePrx = callback;
                info.ClientType = type.ToString();
                info.LoginDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _sessionList.Add(connectionId, info);
                LogHelper.CoreJsonLog("AddClient" + info.toJsonStr());
                if (!_sessionGroupList.ContainsKey(info.ClientType))
                {
                    List<SessionInfo> sessionList = new List<SessionInfo>();
                    sessionList.Add(info);
                    _sessionGroupList.Add(info.ClientType, sessionList);
                }
                else
                {
                    _sessionGroupList[info.ClientType].Add(info);
                }
                //发送变化事件
                EventServerHelper.GetInstance().OnSessionChangeEvent(info, true);
                return 0;
            }
        }

        public void RemoveClient(string connectionId, bool checkPing = true)
        {
            lock (_lockRemove)
            {
                if (_sessionList.ContainsKey(connectionId))
                {
                    SessionInfo info = _sessionList[connectionId];

                    if (checkPing)
                    {
                        try
                        {
                            info.PackagePrx?.ice_ping();
                            info.QuotPackage?.ice_ping();
                            LogHelper.CoreJsonLog($"RemoveClient ice_ping OK ,{info.toJsonStr()}");
                            return;
                        }
                        catch (System.Exception ex)
                        {
                            LogHelper.CoreJsonLog($"RemoveClient ice_ping false ,{info.toJsonStr()},error:{ex}");
                        }
                    }

                    LogHelper.CoreJsonLog("RemoveClient" + info.toJsonStr());
                    _sessionList.Remove(connectionId);
                    if (_sessionGroupList.ContainsKey(info.ClientType))
                    {
                        _sessionGroupList[info.ClientType].RemoveAll(p => p.ConnectionId.Equals(info.ConnectionId));
                    }
                    //发送变化事件
                    EventServerHelper.GetInstance().OnSessionChangeEvent(info, false);
                }
            }
        }

        public List<SessionInfo> GetSessionList()
        {
            lock (_lock)
            {
                List<SessionInfo> slist = new List<SessionInfo>();
                foreach (SessionInfo info in _sessionList.Values)
                {
                    slist.Add(info);
                }
                return slist;
            }
        }
        public Dictionary<string, SessionInfo> GetSessionDic()
        {
            lock (_lock)
            {
                Dictionary<string, SessionInfo> slist = new Dictionary<string, SessionInfo>();
                foreach (var info in _sessionList)
                {
                    slist.Add(info.Key, info.Value);
                }
                return slist;
            }
        }
        public List<SessionInfo> GetSessionList(int type)
        {
            lock (_lock)
            {
                List<SessionInfo> slist = new List<SessionInfo>();
                foreach (SessionInfo info in _sessionList.Values)
                {
                    if (info.ClientType.Equals(type.ToString()))
                    {
                        slist.Add(info);
                    }
                }
                return slist;
            }
        }
        public SessionInfo GetSessionConnectId(string connectId)
        {
            lock (_lock)
            {
                foreach (SessionInfo info in _sessionList.Values)
                {
                    if (info.ConnectionId.Equals(connectId))
                    {
                        return info;
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// $"{info.ClientId}:{info.ClientType}"
        /// </summary>
        /// <param name="idAndType"></param>
        /// <returns></returns>
        public SessionInfo GetSessionConnectIdAndType(string idAndType)
        {
            lock (_lock)
            {
                string key = "";
                foreach (SessionInfo info in _sessionList.Values)
                {
                    key = $"{info.ClientId}:{info.ClientType}";
                    if (key == idAndType)
                    {
                        return info;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_sessionList.Count > 0)
            {
                //断开连接
                foreach (SessionInfo info in _sessionList.Values)
                {
                    info.PackagePrx.ice_getConnection().close(ConnectionClose.Forcefully);
                }
                _sessionList.Clear();
            }
        }
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="package"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool ReceivePackage(ResponsePackage package, string connectionId)
        {
            try
            {

                if (!_sessionList.ContainsKey(connectionId))
                {
                    return false;
                }
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                var currSessionInfo = _sessionList[connectionId];
                UpdateUUID(currSessionInfo, package);
                currSessionInfo.PackagePrx.begin_ReceivePackage(package).whenCompleted(
                        (Ice.Exception ex) =>
                        {
                            LogHelper.IceError(ex, "ReceivePackage");
                        });
                //var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                //if (totalSeconds > 3)
                //{
                //	LogHelper.Error($"ReceivePackage:本次推送耗时{totalSeconds}");
                //}
                return true;
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 同步批量推送
        /// </summary>
        /// <param name="package"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool ReceivePackageList(ResponsePackageOrder[] package, string connectionId)
        {
            try
            {

                if (!_sessionList.ContainsKey(connectionId) || package == null)
                {
                    return false;
                }
                var currSessionInfo = _sessionList[connectionId];
                UpdateUUID(currSessionInfo, package);

                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                var result = currSessionInfo.PackagePrx.ReceivePackageList(package);
                //var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                //if (totalSeconds > 3)
                //{
                //	LogHelper.Error($"ReceivePackage:本次推送耗时{totalSeconds}");
                //}
                return result == 0;
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 异步批量推送
        /// </summary>
        /// <param name="package"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool ReceivePackageListAsync(ResponsePackageOrder[] package, string connectionId)
        {
            try
            {
                lock (_lockPushlist)
                {
                    if (!_sessionList.ContainsKey(connectionId) || package == null)
                    {
                        return false;
                    }
                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Start();
                    var currSessionInfo = _sessionList[connectionId];
                    UpdateUUID(currSessionInfo, package);

                    currSessionInfo.PackagePrx.begin_ReceivePackageList(package).whenCompleted((rsp) =>
                    {
                        //LogHelper.WriteLog("whenCompleted", $"ReceivePackageListAsync Count :{package.Length},{rsp}");

                    }, (Ice.Exception ex) =>
                    {
                        LogHelper.IceError(ex, $"ReceivePackageListAsync send error Count :{package.Length}");
                    });
                    //var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                    //if (totalSeconds > 3)
                    //{
                    //	LogHelper.Error($"ReceivePackage:本次推送耗时{totalSeconds}");
                    //}
                    return true;

                }
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                return false;
            }
        }
        #region DB Cache推送
        #region 同步
        /// <summary>
        /// 同步 服务端向客户端推送数据
        /// </summary>
        /// <param name="package"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public PackageCacheDB ReceivePackageCacheTrade(CacheTradeDB package, string connectionId)
        {
            try
            {

                if (!_sessionList.ContainsKey(connectionId) || package == null)
                {
                    return null;
                }
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                var currSessionInfo = _sessionList[connectionId];
                UpdateUUID(currSessionInfo, package);

                var result = currSessionInfo.PackagePrx.ReceivePackageCacheTrade(package);
                //var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                //if (totalSeconds > 3)
                //{
                //	LogHelper.Error($"ReceivePackage:本次推送耗时{totalSeconds}");
                //}
                return result;
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "SendPackageCacheTrade");
                RemoveClient(connectionId);
                return null;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "SendPackageCacheTrade");
                RemoveClient(connectionId);
                return null;
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region Async

        /// <summary>
        /// 推送 ReceivePackageCacheTrade
        /// </summary>
        /// <param name="package"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool ReceivePackageCacheTradeAsync(CacheTradeDB package, string connectionId)
        {
            try
            {
                lock (_lockPush)
                {


                    if (!_sessionList.ContainsKey(connectionId))
                    {
                        return false;
                    }
                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Start();
                    var currSessionInfo = _sessionList[connectionId];
                    UpdateUUID(currSessionInfo, package);
                    currSessionInfo.PackagePrx.begin_ReceivePackageCacheTrade(package).whenCompleted(
                        (rsp) =>
                            {
                                LogHelper.WriteLog("whenCompleted_ReceivePackageCacheTrade", $"MsgId:{package.PackageData.MsgId},CallBack MsgId:{rsp.MsgId},{rsp.ServiceNo}");
                                //收到回报处理回调
                                EventServerHelper.GetInstance().OnReceivePackageCacheTradeAsyncCallBackEvent(rsp);

                            },
                            (Ice.Exception ex) =>
                            {
                                LogHelper.IceError(ex, "ReceivePackageCacheTrade1", package.toJsonStr());
                            });
                    //var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                    //if (totalSeconds > 3)
                    //{
                    //	LogHelper.Error($"ReceivePackage:本次推送耗时{totalSeconds}");
                    //}
                    return true;
                }
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackageCacheTrade2", package.toJsonStr());
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackageCacheTrade3", package.toJsonStr());
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 推送 ReceivePackageCacheBase
        /// </summary>
        /// <param name="package"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool ReceivePackageCacheBaseAsync(PackageCacheDB package, string connectionId)
        {
            try
            {
                lock (_lockPush)
                {

                    if (!_sessionList.ContainsKey(connectionId))
                    {
                        return false;
                    }
                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Start();
                    var currSession = _sessionList[connectionId];
                    UpdateUUID(currSession, package);

                    currSession.PackagePrx.begin_ReceivePackageCacheBase(package).whenCompleted(
                          (rsp) =>
                          {
                              LogHelper.WriteLog("whenCompleted", $"ReceivePackageCacheBaseAsync :{package.MsgId},{rsp.ServiceNo}");
                          },
                        (Ice.Exception ex) =>
                        {
                            LogHelper.IceError(ex, "ReceivePackageCacheBase");
                        });
                    //var totalSeconds = stopwatch.Elapsed.TotalSeconds;
                    //if (totalSeconds > 3)
                    //{
                    //	LogHelper.Error($"ReceivePackage:本次推送耗时{totalSeconds}");
                    //}
                    return true;
                }
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackageCacheBase");
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackageCacheBase");
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                return false;
            }
        }


        #endregion
        #endregion
        public bool ReceivePackageWithResult(ResponsePackage package, string connectionId)
        {
            try
            {
                if (!_sessionList.ContainsKey(connectionId))
                {
                    return false;
                }
                var currSessionInfo = _sessionList[connectionId];
                UpdateUUID(currSessionInfo, package);
                currSessionInfo.PackagePrx.begin_ReceivePackage(package).whenCompleted((int i) =>
                {

                }, (Ice.Exception ex) =>
                {
                    LogHelper.IceError(ex, "ReceivePackage");
                }).whenSent((bool b) =>
                {

                });
                return true;
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackage");
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ReceivePackageByType(ResponsePackage package, int type)
        {
            string connectionId = string.Empty;
            try
            {
                if (!_sessionGroupList.ContainsKey(type.ToString()))
                {
                    return false;
                }
                for (int i = 0; i < _sessionGroupList[type.ToString()].Count; i++)
                {
                    var currSessionInfo = _sessionGroupList[type.ToString()][i];
                    connectionId = currSessionInfo.ConnectionId;
                    UpdateUUID(currSessionInfo, package);
                    _sessionGroupList[type.ToString()][i].PackagePrx.begin_ReceivePackage(package).whenCompleted(
                        (Ice.Exception ex) =>
                        {

                            LogHelper.IceError(ex, "ReceivePackageByType");
                        });

                }
                return true;
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "ReceivePackageByType");
                RemoveClient(connectionId);
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "ReceivePackageByType");
                RemoveClient(connectionId);
                return false;
            }
            catch
            {
                //RemoveClient(connectionId);
                return false;
            }
        }

        /// <summary>
        /// 推送行情专用
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="nfArray"></param>
        /// <param name="dArray"></param>
        /// <returns></returns>
        public bool PushPackage(string connectionId, byte type, List<NewFlashData> nfArray, List<DeepData> dArray)
        {
            try
            {
                //推送所有
                if (connectionId.Equals(string.Empty))
                {
                    foreach (var info in _sessionList.Values)
                    {
                        info.QuotPackage.begin_PushPackage(type, nfArray.ToArray(), dArray.ToArray());
                    }
                    return true;
                }
                //推送某个客户端
                if (!_sessionList.ContainsKey(connectionId))
                {
                    return false;
                }
                _sessionList[connectionId].QuotPackage.begin_PushPackage(type, nfArray.ToArray(), dArray.ToArray());
                return true;
            }
            catch (Ice.UnknownException ex)
            {
                LogHelper.IceError(ex, "PushPackage");
                return false;
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "PushPackage");
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 主动断开客户连接
        /// </summary>
        public void CloseClientById(string id, ConnectionClose force = ConnectionClose.Gracefully)
        {
            if (_sessionList.ContainsKey(id))
            {
                _sessionList[id].PackagePrx.ice_getConnection().close(force);
            }
        }
        #region UpdateUUid 
        void UpdateUUID(SessionInfo uuid, ResponsePackageOrder[] package)
        {
            foreach (var item in package)
            {
                item.ClientId = uuid.ClientId;
                item.UUID = uuid.ConnectionId;
            }
        }
        void UpdateUUID(SessionInfo uuid, ResponsePackage package)
        {
            package.UUID = uuid.ConnectionId;
            package.ClientId = uuid.ClientId;
        }
        void UpdateUUID(SessionInfo uuid, PackageCacheDB package)
        {
            package.UUID = uuid.ConnectionId;
            package.ClientId = uuid.ClientId;
        }
        void UpdateUUID(SessionInfo uuid, CacheTradeDB package)
        {
            package.PackageData.UUID = uuid.ConnectionId;
            package.PackageData.ClientId = uuid.ClientId;
        }
        #endregion
        #endregion
    }
}
