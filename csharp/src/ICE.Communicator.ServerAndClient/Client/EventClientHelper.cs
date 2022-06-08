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
    public delegate PackageCacheDB ReceivePackageCacheDBClientEventHandler(PackageCacheDB responsePackagePackage);
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
        private event ReceivePackageCacheDBClientEventHandler ReceivePackageCacheDBEvent;
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
