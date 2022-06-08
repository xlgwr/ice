using Ice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlmCommunicator.Server
{
    class PackageSenderServerI : PackageSenderDisp_
    {
        public override ResponsePackage SendPackage(RequestPackage sendData, Ice.Current current)
        {
            //获取相应的服务
            SendPackageServerEventHandler sendHandle = EventServerHelper.GetInstance().GetReceivePackageEvent();
            if (sendHandle != null)
            {
                return sendHandle(new RequestPackageEx(sendData));
            }
            ResponsePackage package = new ResponsePackage();
            package.ErrorInfo = "没有关联服务处理方法！";
            package.ErrorNo = -1;
            package.ServiceNo = sendData.ServiceNo;
            return package;
        }
        public override ResponsePackageOrderBackMsg SendPackageList(RequestPackageOrder[] requestFields, Current current__)
        {
            //获取相应的服务
            SendPackageListServerEventHandler sendHandle = EventServerHelper.GetInstance().GetReceivePackageListEvent();
            if (sendHandle != null)
            {
                return sendHandle(requestFields);
            }
            var rsp = new ResponsePackageOrderBackMsg() { ErrorInfo = "SendPackageListEventHandler 没有关联服务处理方法!", ErrorNo = -1 };
            return rsp;
        }

        public override int RegClient(Ice.Identity ident, int type, Ice.Current current)
        {
            lock (this)
            {
                Ice.ObjectPrx @base = current.con.createProxy(ident);
                PackageReceiverPrx client = PackageReceiverPrxHelper.uncheckedCast(@base);
                Ice.IPConnectionInfo info = current.con.getInfo() as Ice.IPConnectionInfo;
                ConnectionServerEvent conn = new ConnectionServerEvent(ident.name);
                current.con.setCallback(conn);
                if (info != null)
                {
                    return SessionServerManager.GetInstance().AddClient(ident.category, ident.name, info.remoteAddress, info.remotePort.ToString(), type, client, null, conn);
                }
                return 2;
            }
        }

        public override int RegClientExt(PackageReceiverPrx proxy, Ice.Identity ident, int type, Ice.Current current)
        {
            lock (this)
            {
                Ice.IPConnectionInfo info = current.con.getInfo() as Ice.IPConnectionInfo;
                ConnectionServerEvent conn = new ConnectionServerEvent(ident.name);
                //反注册
                proxy.RegCallbackClient();
                current.con.setCallback(conn);
                if (info != null)
                {
                    return SessionServerManager.GetInstance().AddClient(ident.category, ident.name, info.remoteAddress, info.remotePort.ToString(), type, proxy, null, conn);
                }
                return 2;
            }
        }

        public override int RegQuotClient(Ice.Identity ident, Ice.Identity quotIdent, int type, Ice.Current current)
        {
            lock (this)
            {
                //设置成推送模式
                Ice.ObjectPrx @base = current.con.createProxy(ident);
                PackageReceiverPrx client = PackageReceiverPrxHelper.uncheckedCast(@base);

                Ice.ObjectPrx @base2 = current.con.createProxy(quotIdent).ice_oneway();
                QuotPackagePrx client2 = QuotPackagePrxHelper.uncheckedCast(@base2);

                Ice.IPConnectionInfo info = current.con.getInfo() as Ice.IPConnectionInfo;
                ConnectionServerEvent conn = new ConnectionServerEvent(ident.name);
                current.con.setCallback(conn);
                if (info != null)
                {
                    return SessionServerManager.GetInstance().AddClient(ident.category, ident.name, info.remoteAddress, info.remotePort.ToString(), type, client, client2, conn);
                }
                return 2;
            }
        }
        public override int SendLogPackage(RequestPackageLog responseField, Current current__)
        {
            //获取相应的服务
            SendPackageLogServerEventHandler sendHandle = EventServerHelper.GetInstance().GetReceivePackageLogEvent();
            if (sendHandle != null)
            {
                return sendHandle(responseField);
            }
            return -1;
        }

        public override int SendLogPackageList(RequestPackageOrderLog[] responseFields, Current current__)
        {
            //获取相应的服务
            SendPackageLogListServerEventHandler sendHandle = EventServerHelper.GetInstance().GetReceivePackageLogListEvent();
            if (sendHandle != null)
            {
                return sendHandle(responseFields);
            }
            return -1;
        }

        public override int SendLogResponsePackageList(ResponsePackageOrder[] responseFields, int mstype, Current current__)
        {
            //获取相应的服务
            var sendHandle = EventServerHelper.GetInstance().GetSendLogResponsePackageOrderListServerEvent();
            if (sendHandle != null)
            {
                return sendHandle(responseFields, mstype);
            }
            return -1;
        }

        public override int SendLogRequestPackageList(RequestPackageOrder[] responseFields, int mstype, Current current__)
        {
            //获取相应的服务
            var sendHandle = EventServerHelper.GetInstance().GetSendLogRequestPackageOrderListServerEvent();
            if (sendHandle != null)
            {
                return sendHandle(responseFields, mstype);
            }
            return -1;
        }

        public override CacheTradeDB SendPackageCacheTrade(CacheTradeDB requestField, Current current__)
        {
            //获取相应的服务
            var sendHandle = EventServerHelper.GetInstance().GetReceivePackageCacheTradeEvent();
            if (sendHandle != null)
            {
                return sendHandle(requestField);
            }
            int? serverNo = requestField?.PackageData?.ServiceNo;
            var rsp = new CacheTradeDB() { PackageData = new PackageCacheDB() { ServiceNo = serverNo.GetValueOrDefault(), ErrorInfo = "SendPackageCacheTrade 没有关联注册服务sendHandle处理方法!", ErrorNo = -1 } };
            return rsp;
        }

        public override PackageCacheDB SendPackageCacheBase(PackageCacheDB requestField, Current current__)
        {
            //获取相应的服务
            var sendHandle = EventServerHelper.GetInstance().GetReceivePackageCacheDBEvent();
            if (sendHandle != null)
            {
                return sendHandle(requestField);
            }
            var rsp = new PackageCacheDB() { ErrorInfo = "SendPackageCacheBase 没有关联注册服务sendHandle处理方法!", ErrorNo = -1 };
            return rsp;
        }
    }
}
