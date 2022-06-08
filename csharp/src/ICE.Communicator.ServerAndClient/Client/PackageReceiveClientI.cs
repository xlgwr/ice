using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ice;
using IceInternal;

namespace SlmCommunicator.Client
{
    class PackageReceiveClientI : PackageReceiverDisp_
    {
        private ConnectionClientEvent _connEvent;

        public PackageReceiveClientI(ConnectionClientEvent connEvent)
        {
            _connEvent = connEvent;
        }


        public override int ReceivePackage(ResponsePackage responsePackage, Ice.Current current)
        {
            //发送数据
            if (responsePackage != null)
            {
                _connEvent.EventObj.OnReceivePackageEvent(responsePackage);
            }
            return 0;
        }
        public override int ReceivePackageList(ResponsePackageOrder[] responseFields, Current current__)
        {
            //发送数据
            if (responseFields != null)
            {
                return _connEvent.EventObj.OnReceivePackageListEvent(responseFields);
            }
            return 0;
        }

        public override void RegCallbackClient(Ice.Current current)
        {
            current.con.setCloseCallback(_connEvent.closed);
            current.con.setHeartbeatCallback(_connEvent.heartbeat);
        }

        public override PackageCacheDB ReceivePackageCacheTrade(CacheTradeDB responseField, Current current__)
        {
            //推送数据
            if (responseField != null)
            {
                return _connEvent.EventObj.OnReceivePackageCacheTradeEvent(responseField);
            }
            return new PackageCacheDB() { ErrorInfo = "responseField参数为空", ErrorNo = -1 };
        }

        public override PackageCacheDB ReceivePackageCacheBase(PackageCacheDB responseField, Current current__)
        {
            //推送数据
            if (responseField != null)
            {
                return _connEvent.EventObj.OnReceivePackageCacheDBEvent(responseField);
            }
            return new PackageCacheDB() { ErrorInfo = "responseField参数为空", ErrorNo = -1 };
        }
    }

    class QuotPackageI : QuotPackageDisp_
    {
        private ConnectionClientEvent _connEvent;

        public QuotPackageI(ConnectionClientEvent connEvent)
        {
            _connEvent = connEvent;
        }

        public override void PushPackage(byte type, NewFlashData[] nfArray, DeepData[] dArray, Current current__)
        {
            _connEvent.EventObj.OnRecQuotEvent(type, nfArray, dArray);
        }
    }
}
