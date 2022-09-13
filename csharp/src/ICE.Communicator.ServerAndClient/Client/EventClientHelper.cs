using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlmCommunicator.Client
{
    //连接状态
    public enum ConnectStateClient
    {
        //连接正常
        CONNECT_OK,
        //连接失败
        CONNECT_FAIL,
        //重连
        CONNECT_RE,
    }

    #region 事件委托申明
    public delegate void ReceivePackageClientEventHandler(ResponsePackage responsePackagePackage);
    public delegate int ReceivePackageListClientEventHandler(ResponsePackageOrder[] responsePackagePackage);
    public delegate void SendPackageListAsynCallBackEventHandler(ResponsePackageOrderBackMsg responsePackagePackage);
    public delegate PackageCacheDB ReceivePackageCacheTradeClientEventHandler(CacheTradeDB responsePackagePackage);
    public delegate PackageCacheDB ReceivePackageCacheTradeListClientEventHandler(CacheTradeDB[] responsePackagePackage);
    public delegate PackageCacheDB ReceivePackageCacheBaseListClientEventHandler(PackageCacheDB[] responsePackagePackage);
    public delegate StreamPackage ReceiveStreamPackageListClientEventHandler(StreamPackage[] responsePackagePackage, string msgName);
    public delegate void ReceivePackageCacheTradeListAsynCallBackClientEventHandler(CacheTradeDB responsePackagePackage);
    public delegate PackageCacheDB ReceivePackageCacheDBClientEventHandler(PackageCacheDB responsePackagePackage);
    public delegate StreamPackage ReceiveStreamPackageClientEventHandler(StreamPackage responsePackagePackage);
    public delegate void ConnectionEventClientHandler(string uuid, ConnectStateClient state);
    public delegate void RecQuotEventClientHandler(byte type, NewFlashData[] nfArray, DeepData[] dArray);
    public delegate void RequestStartVoteHandler(string idAndType);

    #endregion

    public class EventClientHelper
    {
        #region 属性变量
        #endregion

        #region 事件
        private event ReceivePackageClientEventHandler ReceivePackageEvent;
        private event ReceivePackageListClientEventHandler ReceivePackageListEvent;
        private event SendPackageListAsynCallBackEventHandler SendPackageListAsynCallBackEvent;
        private event ReceivePackageCacheTradeClientEventHandler ReceivePackageCacheTradeEvent;
        private event ReceivePackageCacheTradeListClientEventHandler ReceivePackageCacheTradeListEvent;
        private event ReceivePackageCacheBaseListClientEventHandler ReceivePackageCacheBaseListEvent;
        private event ReceiveStreamPackageListClientEventHandler ReceiveStreamPackageListEvent;
        private event ReceivePackageCacheTradeListAsynCallBackClientEventHandler ReceivePackageCacheTradeListAsynCallBackEvent;
        private event ReceivePackageCacheDBClientEventHandler ReceivePackageCacheDBEvent;
        private event ReceiveStreamPackageClientEventHandler ReceiveStreamPackageEvent;
        private event ConnectionEventClientHandler ConnectionEvent;
        private event RecQuotEventClientHandler RecQuotEvent;
        private event RequestStartVoteHandler RequestStartVoteEvent;
        #endregion

        #region 构造方法
        public EventClientHelper()
        {

        }

        ~EventClientHelper()
        {

        }
        #endregion

        #region 私有方法

        #endregion

        #region 公有方法
        #region 收到打包数据
        public void RegReceivePackageEvent(ReceivePackageClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageEvent += messageHandler;
            }
        }

        public void OnReceivePackageEvent(ResponsePackage responsePackagePackage)
        {
            if (ReceivePackageEvent != null)
            {
                ReceivePackageEvent(responsePackagePackage);
            }
        }
        #endregion
        #region 收到打包数据list
        public int RegReceivePackageListEvent(ReceivePackageListClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageListEvent += messageHandler;
            }
            return 0;
        }

        public int OnReceivePackageListEvent(ResponsePackageOrder[] responsePackagePackages)
        {
            if (ReceivePackageListEvent != null)
            {
                return ReceivePackageListEvent(responsePackagePackages);
            }
            return 0;
        }
        /// <summary>
        /// 注册 SendPackageListAsyn 发送回包 异步回调
        /// </summary>
        /// <param name="rsp"></param>
        public void RegSendPackageListAsynCallBackEvent(SendPackageListAsynCallBackEventHandler handler)
        {
            if (handler != null)
            {
                SendPackageListAsynCallBackEvent += handler;
            }
        }
        /// <summary>
        /// 调用 SendPackageListAsyn 发送回包 异步回调
        /// </summary>
        /// <param name="rsp"></param>
        public void OnSendPackageListAsynCallBackEvent(ResponsePackageOrderBackMsg rsp)
        {
            if (SendPackageListAsynCallBackEvent != null)
            {
                SendPackageListAsynCallBackEvent(rsp);
            }
        }
        #endregion
        #region 收到打包数据 ReceivePackageCacheTrade
        public int RegReceivePackageCacheTradeEvent(ReceivePackageCacheTradeClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheTradeEvent += messageHandler;
            }
            return 0;
        }

        public PackageCacheDB OnReceivePackageCacheTradeEvent(CacheTradeDB responsePackagePackages)
        {
            if (ReceivePackageCacheTradeEvent != null)
            {
                return ReceivePackageCacheTradeEvent(responsePackagePackages);
            }
            return new PackageCacheDB() { ErrorInfo = "未注册ReceivePackageCacheTradeEvent事件处理方法", ErrorNo = -1 };
        }
        #endregion

        #region 收到打包数据 ReceivePackageCacheTradeList
        public int RegReceivePackageCacheTradeListEvent(ReceivePackageCacheTradeListClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheTradeListEvent += messageHandler;
            }
            return 0;
        }

        public PackageCacheDB OnReceivePackageCacheTradeListEvent(CacheTradeDB[] responsePackagePackages)
        {
            if (ReceivePackageCacheTradeListEvent != null)
            {
                return ReceivePackageCacheTradeListEvent(responsePackagePackages);
            }
            return new PackageCacheDB() { ErrorInfo = "未注册 ReceivePackageCacheTradeListEvent 事件处理方法", ErrorNo = -1 };
        }
        #endregion
        #region 收到打包数据 ReceivePackageCacheBaseList
        public int RegReceivePackageCacheBaseListEvent(ReceivePackageCacheBaseListClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheBaseListEvent += messageHandler;
            }
            return 0;
        }

        public PackageCacheDB OnReceivePackageCacheBaseListEvent(PackageCacheDB[] responsePackagePackages)
        {
            if (ReceivePackageCacheBaseListEvent != null)
            {
                return ReceivePackageCacheBaseListEvent(responsePackagePackages);
            }
            return new PackageCacheDB() { ErrorInfo = "未注册 ReceivePackageCacheBaseListEvent 事件处理方法", ErrorNo = -1 };
        }
        #endregion
        #region 收到打包数据 ReceiveStreamPackageList
        public int RegReceiveStreamPackageListEvent(ReceiveStreamPackageListClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceiveStreamPackageListEvent += messageHandler;
            }
            return 0;
        }

        public StreamPackage OnReceiveStreamPackageListEvent(StreamPackage[] responsePackagePackages, string msgName)
        {
            if (ReceiveStreamPackageListEvent != null)
            {
                return ReceiveStreamPackageListEvent(responsePackagePackages, msgName);
            }
            return new StreamPackage() { ErrorInfo = "未注册 ReceiveStreamPackageListEvent 事件处理方法", ErrorNo = -1 };
        }
        #endregion
        #region 收到打包数据 ReceivePackageCacheTradeListAsynCallBack
        public int RegReceivePackageCacheTradeListAsynCallBackEvent(ReceivePackageCacheTradeListAsynCallBackClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheTradeListAsynCallBackEvent += messageHandler;
            }
            return 0;
        }

        public void OnReceivePackageCacheTradeListAsynCallBackEvent(CacheTradeDB responsePackagePackages)
        {
            if (ReceivePackageCacheTradeListAsynCallBackEvent != null)
            {
                ReceivePackageCacheTradeListAsynCallBackEvent(responsePackagePackages);
            }
        }
        #endregion
        #region 收到打包数据 ReceivePackageCacheBase
        public int RegReceivePackageCacheDBEvent(ReceivePackageCacheDBClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceivePackageCacheDBEvent += messageHandler;
            }
            return 0;
        }

        public PackageCacheDB OnReceivePackageCacheDBEvent(PackageCacheDB responsePackagePackages)
        {
            if (ReceivePackageCacheDBEvent != null)
            {
                return ReceivePackageCacheDBEvent(responsePackagePackages);
            }
            return new PackageCacheDB() { ErrorInfo = "未注册ReceivePackageCacheDBEvent事件处理方法", ErrorNo = -1 };
        }
        #endregion
        #region 收到打包数据 ReceiveStreamPackage
        public int RegReceiveStreamPackageEvent(ReceiveStreamPackageClientEventHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ReceiveStreamPackageEvent += messageHandler;
            }
            return 0;
        }

        public StreamPackage OnReceiveStreamPackageEvent(StreamPackage responsePackagePackages)
        {
            if (ReceiveStreamPackageEvent != null)
            {
                return ReceiveStreamPackageEvent(responsePackagePackages);
            }
            return new StreamPackage() { ErrorInfo = "未注册ReceiveStreamPackageEvent事件处理方法", ErrorNo = -1 };
        }
        #endregion
        #region 连接状态通知
        public void RegConnectionEvent(ConnectionEventClientHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ConnectionEvent += messageHandler;
            }
        }

        public void ConRegConnectionEvent(ConnectionEventClientHandler messageHandler)
        {
            if (messageHandler != null)
            {
                ConnectionEvent -= messageHandler;
            }
        }
        public void OnConnectionEvent(string uuid, ConnectStateClient state)
        {
            if (ConnectionEvent != null)
            {
                ConnectionEvent(uuid, state);
            }
        }
        #endregion

        #region 接收行情变化通知
        public void RegRecQuotEvent(RecQuotEventClientHandler messageHandler)
        {
            if (messageHandler != null)
            {
                RecQuotEvent += messageHandler;
            }
        }

        public void OnRecQuotEvent(byte type, NewFlashData[] nfArray, DeepData[] dArray)
        {
            if (RecQuotEvent != null)
            {
                RecQuotEvent(type, nfArray, dArray);
            }
        }
        #endregion

        #region 请求发起投票
        /// <summary>
        /// 连接成功后,第一次断线重连时,失败后,调用投票功能
        /// </summary>
        /// <param name="messageHandler"></param>
        public void RegRequestStartVoteEvent(RequestStartVoteHandler messageHandler)
        {
            if (messageHandler != null)
            {
                RequestStartVoteEvent += messageHandler;
            }
        }

        public void OnRequestStartVoteEvent(string idAndType)
        {
            if (RequestStartVoteEvent != null)
            {
                RequestStartVoteEvent(idAndType);
            }
        }
        #endregion
        #endregion
    }
}
