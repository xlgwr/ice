using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlmCommunicator.Server
{
    public enum ConnectStateServer
    {
        Connect_Ready = 0,
        Connect_Fail,
    }

    #region 事件委托申明
    public delegate ResponsePackage SendPackageServerEventHandler(RequestPackageEx param);
    public delegate ResponsePackageOrderBackMsg SendPackageListServerEventHandler(RequestPackageOrder[] param);
    public delegate CacheTradeDB SendPackageCacheTradeServerEventHandler(CacheTradeDB param);
    public delegate PackageCacheDB SendPackageCacheDBServerEventHandler(PackageCacheDB param);
    public delegate int SendPackageLogServerEventHandler(RequestPackageLog param);
    public delegate int SendPackageLogListServerEventHandler(RequestPackageOrderLog[] param);
    public delegate int SendLogResponsePackageOrderServerEventHandler(ResponsePackageOrder[] param, int mstype);
    public delegate int SendLogRequestPackageOrderServerEventHandler(RequestPackageOrder[] param, int mstype);
    //服务端推送，客户端接收
    public delegate bool ReceivePackageServerEventHandler(ResponsePackage package, string uuid);
    public delegate PackageCacheDB ReceivePackageCacheTradeServerEventHandler(CacheTradeDB package, string uuid);
    public delegate bool ReceivePackageCacheTradeServerAsyncEventHandler(CacheTradeDB package, string uuid);
    public delegate void ReceivePackageCacheTradeServerAsyncCallBackEventHandler(PackageCacheDB package);
    public delegate bool ReceivePackageCacheBaseServerEventHandler(PackageCacheDB package, string uuid);
    public delegate bool ReceivePackageListServerEventHandler(ResponsePackageOrder[] package, string uuid);
    public delegate bool ReceivePackageListAsyncServerEventHandler(ResponsePackageOrder[] package, string uuid);
    public delegate bool SendPackageByTypeServerEventHandler(ResponsePackage package, int type);
    public delegate void SessionChangeServerEventHandler(SessionInfo info, bool isAdd);
    public delegate void ConnectStateServerEventHandler(ConnectStateServer state);
    #endregion

    class EventServerHelper
    {
        #region 属性变量
        private static EventServerHelper _instance = null;
        private static object _lock = new object();
        #endregion

        #region 事件
        private event SendPackageServerEventHandler SendPackageEvent;
        private event SendPackageListServerEventHandler SendPackageListEvent;
        private event SendPackageCacheTradeServerEventHandler SendPackageCacheTradeEvent;
        private event SendPackageCacheDBServerEventHandler SendPackageCacheDBEvent;
        private event SendPackageLogServerEventHandler SendPackageLogEvent;
        private event SendPackageLogListServerEventHandler SendPackageLogListEvent;
        private event SendLogResponsePackageOrderServerEventHandler SendLogResponsePackageOrderListEvent;
        private event SendLogRequestPackageOrderServerEventHandler SendLogRequestPackageOrderListEvent;
        //服务端推送，客户端接收
        private event ReceivePackageServerEventHandler ReceivePackageEvent;
        private event ReceivePackageCacheTradeServerEventHandler ReceivePackageCacheTradeEvent;
        private event ReceivePackageCacheTradeServerAsyncEventHandler ReceivePackageCacheTradeAsyncEvent;
        private event ReceivePackageCacheTradeServerAsyncCallBackEventHandler ReceivePackageCacheTradeAsyncCallBackEvent;
        private event ReceivePackageCacheBaseServerEventHandler ReceivePackageCacheBaseAsyncEvent;
        private event ReceivePackageListServerEventHandler ReceivePackageListEvent;
        private event ReceivePackageListAsyncServerEventHandler ReceivePackageListAsyncEvent;
        private event SessionChangeServerEventHandler SessionChangeEvent;
        private event ConnectStateServerEventHandler ConnectStateEvent;
        private event SendPackageByTypeServerEventHandler SendPackageByTypeEvent;
        #endregion

        #region 构造方法
        public EventServerHelper()
        {

        }

        ~EventServerHelper()
        {
            SendPackageEvent = null;
        }

        public static EventServerHelper GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new EventServerHelper();
                    }
                }
            }
            return _instance;
        }
        #endregion

        #region 私有方法

        #endregion

        #region 公有方法
        public void ResetAll()
        {
            SessionChangeEvent = null;
            ConnectStateEvent = null;

            ResetSendCallback();
        }

        public void ResetSendCallback()
        {
            SendPackageEvent = null;
            SendPackageByTypeEvent = null;
            SendPackageListEvent = null;

            SendPackageLogEvent = null;
            SendPackageLogListEvent = null;
            SendLogRequestPackageOrderListEvent = null;
            SendLogResponsePackageOrderListEvent = null;
            SendPackageCacheDBEvent = null;
            SendPackageCacheTradeEvent = null;

            ReceivePackageEvent = null;
            ReceivePackageListEvent = null;
            ReceivePackageListAsyncEvent = null;

            ReceivePackageCacheBaseAsyncEvent = null;
            ReceivePackageCacheTradeAsyncEvent = null;
        }

        #region 发送打包数据
        public void RegSendPackageEvent(SendPackageServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageEvent += messageHandler;
            }
        }

        public SendPackageServerEventHandler GetReceivePackageEvent()
        {
            return SendPackageEvent;
        }
        #endregion

        #region 发送打包数据,list
        public void RegSendPackageListEvent(SendPackageListServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageListEvent += messageHandler;
            }
        }

        public SendPackageListServerEventHandler GetReceivePackageListEvent()
        {
            return SendPackageListEvent;
        }
        #endregion
        #region 发送打包数据,SendPackageCacheTrade
        public void RegSendPackageCacheTradeEvent(SendPackageCacheTradeServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageCacheTradeEvent += messageHandler;
            }
        }

        public SendPackageCacheTradeServerEventHandler GetReceivePackageCacheTradeEvent()
        {
            return SendPackageCacheTradeEvent;
        }
        #endregion
        #region 发送打包数据,SendPackageCacheDB
        public void RegSendPackageCacheDBEvent(SendPackageCacheDBServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageCacheDBEvent += messageHandler;
            }
        }

        public SendPackageCacheDBServerEventHandler GetReceivePackageCacheDBEvent()
        {
            return SendPackageCacheDBEvent;
        }
        #endregion

        #region 发送打包log数据
        public void RegSendPackageLogEvent(SendPackageLogServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageLogEvent += messageHandler;
            }
        }

        public SendPackageLogServerEventHandler GetReceivePackageLogEvent()
        {
            return SendPackageLogEvent;
        }
        #endregion

        #region 发送打包log数据,list
        public void RegSendLogPackageListEvent(SendPackageLogListServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageLogListEvent += messageHandler;
            }
        }

        public SendPackageLogListServerEventHandler GetReceivePackageLogListEvent()
        {
            return SendPackageLogListEvent;
        }
        #endregion
        #region 发送打包log数据,list Response
        public void RegSendLogResponsePackageOrderListServerEvent(SendLogResponsePackageOrderServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendLogResponsePackageOrderListEvent += messageHandler;
            }
        }

        public SendLogResponsePackageOrderServerEventHandler GetSendLogResponsePackageOrderListServerEvent()
        {
            return SendLogResponsePackageOrderListEvent;
        }
        #endregion
        #region 发送打包log数据,list Request
        public void RegSendLogRequestPackageOrderListServerEvent(SendLogRequestPackageOrderServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendLogRequestPackageOrderListEvent += messageHandler;
            }
        }

        public SendLogRequestPackageOrderServerEventHandler GetSendLogRequestPackageOrderListServerEvent()
        {
            return SendLogRequestPackageOrderListEvent;
        }
        #endregion

        #region 接收打包数据
        public void RegReceivePackageEvent(ReceivePackageServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageEvent += messageHandler;
            }
        }

        public bool OnReceivePackageEvent(ResponsePackage package, string uuid)
        {
            if (ReceivePackageEvent != null)
            {
                return ReceivePackageEvent(package, uuid);
            }
            return false;
        }

        public void RemovePackageEvent(ReceivePackageServerEventHandler messageHandler)
        {
            if (ReceivePackageEvent != null)
            {
                ReceivePackageEvent -= messageHandler;
            }
        }
        #endregion

        #region 接收打包数据list 同步
        public void RegReceivePackageListEvent(ReceivePackageListServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageListEvent += messageHandler;
            }
        }

        public bool OnReceivePackageListEvent(ResponsePackageOrder[] package, string uuid)
        {
            if (ReceivePackageListEvent != null)
            {
                return ReceivePackageListEvent(package, uuid);
            }
            return false;
        }

        public void RemovePackageListEvent(ReceivePackageListServerEventHandler messageHandler)
        {
            if (ReceivePackageListEvent != null)
            {
                ReceivePackageListEvent -= messageHandler;
            }
        }
        #endregion
        #region 接收打包数据list Async
        public void RegReceivePackageListAsyncEvent(ReceivePackageListAsyncServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageListAsyncEvent += messageHandler;
            }
        }

        public bool OnReceivePackageListAsyncEvent(ResponsePackageOrder[] package, string uuid)
        {
            if (ReceivePackageListAsyncEvent != null)
            {
                return ReceivePackageListAsyncEvent(package, uuid);
            }
            return false;
        }

        public void RemovePackageListAsyncEvent(ReceivePackageListAsyncServerEventHandler messageHandler)
        {
            if (ReceivePackageListAsyncEvent != null)
            {
                ReceivePackageListAsyncEvent -= messageHandler;
            }
        }
        #endregion

        #region 接收打包数据 ReceivePackageCacheBase Async
        public void RegReceivePackageCacheBaseAsyncEvent(ReceivePackageCacheBaseServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheBaseAsyncEvent += messageHandler;
            }
        }

        public bool OnReceivePackageCacheBaseAsyncEvent(PackageCacheDB package, string uuid)
        {
            if (ReceivePackageCacheBaseAsyncEvent != null)
            {
                return ReceivePackageCacheBaseAsyncEvent(package, uuid);
            }
            return false;
        }
        #endregion

        #region 接收打包数据 ReceivePackageCacheTrade 
        public void RegReceivePackageCacheTradeEvent(ReceivePackageCacheTradeServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheTradeEvent += messageHandler;
            }
        }

        public PackageCacheDB OnReceivePackageCacheTradeEvent(CacheTradeDB package, string uuid)
        {
            if (ReceivePackageCacheTradeEvent != null)
            {
                return ReceivePackageCacheTradeEvent(package, uuid);
            }
            return null;
        }
        #endregion

        #region 接收打包数据 ReceivePackageCacheTrade Async
        public void RegReceivePackageCacheTradeAsyncEvent(ReceivePackageCacheTradeServerAsyncEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheTradeAsyncEvent += messageHandler;
            }
        }

        public bool OnReceivePackageCacheTradeAsyncEvent(CacheTradeDB package, string uuid)
        {
            if (ReceivePackageCacheTradeAsyncEvent != null)
            {
                return ReceivePackageCacheTradeAsyncEvent(package, uuid);
            }
            return false;
        }
        #endregion
        #region 接收打包数据 ReceivePackageCacheTrade CallBack Async
        /// <summary>
        /// 注册异步回调,回包处理
        /// </summary>
        /// <param name="messageHandler"></param>
        public void RegReceivePackageCacheTradeAsyncCallBackEvent(ReceivePackageCacheTradeServerAsyncCallBackEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheTradeAsyncCallBackEvent += messageHandler;
            }
        }
        /// <summary>
        /// 推送后的回包处理
        /// </summary>
        /// <param name="package"></param>
        public void OnReceivePackageCacheTradeAsyncCallBackEvent(PackageCacheDB package)
        {
            if (ReceivePackageCacheTradeAsyncCallBackEvent != null)
            {
                ReceivePackageCacheTradeAsyncCallBackEvent(package);
            }
        }
        #endregion


        #region 客户端连接变化事件
        public void RegSessionChangeEvent(SessionChangeServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SessionChangeEvent += messageHandler;
            }
        }

        public void OnSessionChangeEvent(SessionInfo info, bool isAdd)
        {
            if (SessionChangeEvent != null)
            {
                SessionChangeEvent(info, isAdd);
            }
        }
        #endregion

        #region 连接状态变化事件
        public void RegConnectChangeEvent(ConnectStateServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ConnectStateEvent += messageHandler;
            }
        }

        public void OnConnectChangeEvent(ConnectStateServer state)
        {
            if (ConnectStateEvent != null)
            {
                ConnectStateEvent(state);
            }
        }
        #endregion

        #region 接收打包数据
        public void RegSendPackageByTypeEvent(SendPackageByTypeServerEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                SendPackageByTypeEvent += messageHandler;
            }
        }

        public bool OnSendPackageByTypeEvent(ResponsePackage package, int type)
        {
            if (SendPackageByTypeEvent != null)
            {
                return SendPackageByTypeEvent(package, type);
            }
            return false;
        }

        public void RemoveSendPackageByTypeEvent(SendPackageByTypeServerEventHandler messageHandler)
        {
            if (SendPackageByTypeEvent != null)
            {
                SendPackageByTypeEvent -= messageHandler;
            }
        }
        #endregion

        #endregion
    }
}
