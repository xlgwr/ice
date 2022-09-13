using Ice;
using IceInternal;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.DataType;

namespace SlmCommunicator
{
    public static class SteamPackageExts
    {
        #region SteamPackage 实体通用方法

        /// <summary>
        /// 更新  errorNo，errorInfo
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="errorNo"></param>
        /// <param name="errorInfo"></param>
        public static void UpdateError(this StreamPackage curr, int errorNo = 0, string errorInfo = "")
        {
            if (curr == null)
            {
                return;
            }
            curr.ErrorNo = errorNo;
            curr.ErrorInfo = errorInfo;
        }
        /// <summary>
        /// 不显示byte[][]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string toJsonNoByteStr(this StreamPackage s)
        {
            return $"ServiceNo:{s.ServiceNo},ClientId:{s.ClientId},UUID:{s.UUID},MsgId:{s.MsgId},ErrorNo:{s.ErrorNo},ErrorInfo:{s.ErrorInfo},DatasDic:{s.DatasDic.toJsonStr()},Head:{s.Head.toJsonStr()},DataBody:{s.DataBody.toJsonStr()},DataStreamBody.Len:{s.DataStreamBody?.Length}";
        }

        public static void AddMsgTypeForLog(this StreamPackage c, string fileName = MsgTypeForLog.Send)
        {
            try
            {
                //if (c == null || c.PackageData == null)
                //{
                //    return;
                //}
                if (c.DatasDic == null)
                {
                    c.DatasDic = new Dictionary<string, string>();
                }
                c.DatasDic[MsgTypeForLog.MsgTypeForLogKey] = fileName;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
        /// <summary>
        /// 主要用于日志记录
        /// </summary>
        /// <param name="c"></param>
        /// <param name="fileName"></param>
        public static void AddMsgTypeForNewLog(this StreamPackage c, string fileName = MsgTypeForLog.Send)
        {
            try
            {
                var DatasDic = new Dictionary<string, string>();

                foreach (var item in c.DatasDic)
                {
                    DatasDic[item.Key] = item.Value;
                }
                DatasDic[MsgTypeForLog.MsgTypeForLogKey] = fileName;
                c.DatasDic = DatasDic;
            }
            catch (System.Exception ex)
            {
                LogHelper.Error(ex);
            }
        }
        public static string GetMsgTypeForLog(this StreamPackage c, string defaultFileName = "SendPersistentLogError")
        {
            try
            {
                return c.DatasDic[MsgTypeForLog.MsgTypeForLogKey];
            }
            catch (System.Exception ex)
            {
                //LogHelper.Error(ex);
                return defaultFileName;
            }
        }
        /// <summary>
        /// CacheSeverServiceNo
        /// </summary>
        /// <param name="serviceNo"></param>
        /// <returns></returns>
        public static PackageOrderType GetPackageOrderType(this int serviceNo)
        {
            PackageOrderType packageOrderType;
            switch (serviceNo)
            {
                case (int)CacheSeverServiceNo.TE_ORDER_QRY:
                case (int)CacheSeverServiceNo.TE_ORDER_ADD:
                case (int)CacheSeverServiceNo.TE_ORDER_UPD:
                case (int)CacheSeverServiceNo.TE_ORDER_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDER_RECOVERY:
                    packageOrderType = PackageOrderType.Orders;
                    break;
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_QRY:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_ADD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_UPD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_RECOVERY:
                    packageOrderType = PackageOrderType.Orderjours;
                    break;
                case (int)CacheSeverServiceNo.TE_TRADE_QRY:
                case (int)CacheSeverServiceNo.TE_TRADE_ADD:
                case (int)CacheSeverServiceNo.TE_TRADE_UPD:
                case (int)CacheSeverServiceNo.TE_TRADE_OFFSET:
                case (int)CacheSeverServiceNo.TE_TRADE_RECOVERY:
                    packageOrderType = PackageOrderType.Trades;
                    break;
                case (int)CacheSeverServiceNo.TE_DEPOSIT_QRY:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_ADD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_UPD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_OFFSET:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_RECOVERY:
                    packageOrderType = PackageOrderType.Deposits;
                    break;
                default:
                    throw new System.Exception($"{serviceNo}:服务号不支持类型对象！");
            }
            return packageOrderType;
        }
        public static PackageOrderType GetPackageOrderType(this StreamPackage c)
        {
            PackageOrderType packageOrderType;
            switch (c.ServiceNo)
            {
                case (int)CacheSeverServiceNo.TE_ORDER_QRY:
                case (int)CacheSeverServiceNo.TE_ORDER_ADD:
                case (int)CacheSeverServiceNo.TE_ORDER_UPD:
                case (int)CacheSeverServiceNo.TE_ORDER_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDER_RECOVERY:
                    packageOrderType = PackageOrderType.Orders;
                    break;
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_QRY:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_ADD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_UPD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_RECOVERY:
                    packageOrderType = PackageOrderType.Orderjours;
                    break;
                case (int)CacheSeverServiceNo.TE_TRADE_QRY:
                case (int)CacheSeverServiceNo.TE_TRADE_ADD:
                case (int)CacheSeverServiceNo.TE_TRADE_UPD:
                case (int)CacheSeverServiceNo.TE_TRADE_OFFSET:
                case (int)CacheSeverServiceNo.TE_TRADE_RECOVERY:
                    packageOrderType = PackageOrderType.Trades;
                    break;
                case (int)CacheSeverServiceNo.TE_DEPOSIT_QRY:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_ADD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_UPD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_OFFSET:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_RECOVERY:
                    packageOrderType = PackageOrderType.Deposits;
                    break;
                default:
                    throw new System.Exception($"{c.ServiceNo}:服务号不支持类型对象！");
            }
            return packageOrderType;
        }

        public static CacheSeverType GetCacheSeverTypeByDB(this StreamPackage c, int serverNo)
        {
            CacheSeverType tmpType = CacheSeverType.Other;
            switch (serverNo)
            {
                case (int)TradeServerAvailable.DB_ADD_ORDER_ICE:
                case (int)TradeServerAvailable.DB_UPD_ORDER_ICE:
                    tmpType = CacheSeverType.Order;
                    break;
                case (int)TradeServerAvailable.DB_ADD_ORDER_JOUR_ICE:
                    tmpType = CacheSeverType.OrderJour;
                    break;
                case (int)TradeServerAvailable.DB_ADD_MATCH_ICE:
                    tmpType = CacheSeverType.Trade;
                    break;
                default:
                    break;
            }
            return tmpType;
        }
        public static Tuple<CacheSeverType, long> GetCacheSeverTypeByDB(this CacheTradeDB c, int serverNo)
        {
            CacheSeverType tmpType = CacheSeverType.Other;
            long msgNum = 0;
            switch (serverNo)
            {
                case (int)TradeServerAvailable.DB_ADD_ORDER_ICE:
                case (int)TradeServerAvailable.DB_UPD_ORDER_ICE:
                    tmpType = CacheSeverType.Order;
                    msgNum = c.GetMaxMsgNum(CacheSeverServiceNo.TE_ORDER_ADD);
                    break;
                case (int)TradeServerAvailable.DB_ADD_ORDER_JOUR_ICE:
                    tmpType = CacheSeverType.OrderJour;
                    msgNum = c.GetMaxMsgNum(CacheSeverServiceNo.TE_ORDERJOUR_ADD);
                    break;
                case (int)TradeServerAvailable.DB_ADD_MATCH_ICE:
                    tmpType = CacheSeverType.Trade;
                    msgNum = c.GetMaxMsgNum(CacheSeverServiceNo.TE_TRADE_ADD);
                    break;
                default:
                    break;
            }
            return new Tuple<CacheSeverType, long>(tmpType, msgNum);
        }
        #endregion

        #region 日志类bin相关
        public static byte[] readByteSeq(this StreamBinFilePackage streamPackage, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.StreamBinFilePackage.ice_write(os__, streamPackage);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static StreamBinFilePackage GetStreamBinFilePackage(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var result = new StreamBinFilePackage();
            InputStream os__ = new InputStream(i, v, readByteSeq);
            result = SlmCommunicator.StreamBinFilePackage.ice_read(os__);
            return result;
        }
        /// <summary>
        /// 从日志文件二进制流中读取对应ice对象流
        /// </summary>
        /// <param name="readByteSeqLog"></param>
        /// <param name="i"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static List<StreamBinFilePackage> GetStreamBinFilePackageLogBin(this byte[] readByteSeqLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var result = new List<StreamBinFilePackage>();
            InputStream os__ = new InputStream(i, v, readByteSeqLog);
            int offset = 0;
            while (offset < readByteSeqLog.Length)
            {
                var tmpResult = new StreamBinFilePackage();
                tmpResult = SlmCommunicator.StreamBinFilePackage.ice_read(os__);

                result.Add(tmpResult);

                offset = os__.pos();
            }
            return result;
        }
        /// <summary>
        /// 从日志文件中读取二进制流
        /// </summary>
        /// <param name="readByteSeq"></param>
        /// <param name="i"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Dictionary<PackageOrderType, List<byte[]>> GetStreamPackageFromLogBin(this byte[] readByteSeqLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var result = new Dictionary<PackageOrderType, List<byte[]>>();
            var getStreamBinFilePackage = readByteSeqLog.GetStreamBinFilePackageLogBin(i, v);
            foreach (var item in getStreamBinFilePackage)
            {
                var tmpType = (PackageOrderType)item.PackageOrderType;
                if (!result.ContainsKey(tmpType))
                {
                    result[tmpType] = new List<byte[]> { item.DataStreamBody };
                    continue;
                }
                result[tmpType].Add(item.DataStreamBody);
            }

            return result;
        }
        public static CacheTradeDB GetCacheTradeDB(this PackageOrderType getType, byte[] DataStreamBody, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            CacheTradeDB tmpobj = new CacheTradeDB() { PackageData = new PackageCacheDB() { MsgId = Guid.NewGuid().ToString() } };
            switch (getType)
            {
                case PackageOrderType.SlmOrderBody:
                case PackageOrderType.Orders:
                    tmpobj.Orders = new OrderCacheDB[] { DataStreamBody.GetOrderCacheDB(iOs, v) };
                    break;
                case PackageOrderType.SlmOrderJourBody:
                case PackageOrderType.Orderjours:
                    tmpobj.Orderjours = new OrderjourCacheDB[] { DataStreamBody.GetOrderjourCacheDB(iOs, v) };
                    break;
                case PackageOrderType.Trades:
                case PackageOrderType.SlmMatchBody:
                    tmpobj.Trades = new TradeCacheDB[] { DataStreamBody.GetTradeCacheDB(iOs, v) };
                    break;
                case PackageOrderType.Deposits:
                    tmpobj.Deposits = new DepositCacheDB[] { DataStreamBody.GetDepositCacheDB(iOs, v) };
                    break;
                default:
                    throw new System.Exception($"{getType}当前对象未处理转换");
                    break;
            }
            return tmpobj;
        }
        public static object GetIceObject(this PackageOrderType getType, byte[] DataStreamBody, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            object tmpobj;
            switch (getType)
            {
                case PackageOrderType.SlmOrderBody:
                    tmpobj = DataStreamBody.GetSlmOrderRow(iOs, v);
                    break;
                case PackageOrderType.PushMatchStateBody:
                    tmpobj = DataStreamBody.GetPushMatchStateRows(iOs, v);
                    break;
                case PackageOrderType.PushOrderStateBody:
                    tmpobj = DataStreamBody.GetPushOrderStateRow(iOs, v);
                    break;
                case PackageOrderType.SendOrderStatusBody:
                    tmpobj = DataStreamBody.GetSendOrderStatusRow(iOs, v);
                    break;
                case PackageOrderType.SlmOrderJourBody:
                    tmpobj = DataStreamBody.GetSlmOrderJourRow(iOs, v);
                    break;
                case PackageOrderType.SlmMatchBody:
                    tmpobj = DataStreamBody.GetSlmMatchRow(iOs, v);
                    break;
                case PackageOrderType.Orders:
                    tmpobj = DataStreamBody.GetOrderCacheDB(iOs, v);
                    break;
                case PackageOrderType.Orderjours:
                    tmpobj = DataStreamBody.GetOrderjourCacheDB(iOs, v);
                    break;
                case PackageOrderType.Trades:
                    tmpobj = DataStreamBody.GetTradeCacheDB(iOs, v);
                    break;
                case PackageOrderType.Deposits:
                    tmpobj = DataStreamBody.GetDepositCacheDB(iOs, v);
                    break;
                default:
                    throw new System.Exception($"{getType}当前对象未处理转换");
                    break;
            }
            return tmpobj;
        }
        #endregion

        #region 实体类转byte[]

        public static byte[] readByteSeq(this SlmOrderRow ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.SlmOrderRow.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static SlmOrderRow GetSlmOrderRow(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.SlmOrderRow.ice_read(os__);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this SlmOrderJourRow ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.SlmOrderJourRow.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static SlmOrderJourRow GetSlmOrderJourRow(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.SlmOrderJourRow.ice_read(os__);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this SlmMatchRow ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.SlmMatchRow.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static SlmMatchRow GetSlmMatchRow(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.SlmMatchRow.ice_read(os__);
            return result;
        }
        public static SlmMatchRow[] GetSlmMatchRow(this byte[][] readByteSeq, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var result = new SlmMatchRow[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetSlmMatchRow(iOs, v);
            }
            return result;
        }
        public static byte[][] readByteSeq(this SlmMatchRow[] ResponsePackageOrderLogs, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var dd = new byte[ResponsePackageOrderLogs.Length][];
            for (int i = 0; i < ResponsePackageOrderLogs.Length; i++)
            {
                dd[i] = ResponsePackageOrderLogs[i].readByteSeq(iOs, v);
            }
            return dd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this SendOrderStatusRow ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.SendOrderStatusRow.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static SendOrderStatusRow GetSendOrderStatusRow(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.SendOrderStatusRow.ice_read(os__);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this PushOrderStateRow ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.PushOrderStateRow.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static PushOrderStateRow GetPushOrderStateRow(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.PushOrderStateRow.ice_read(os__);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this PushMatchStateRow ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.PushMatchStateRow.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static PushMatchStateRow GetPushMatchStateRows(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.PushMatchStateRow.ice_read(os__);
            return result;
        }

        public static byte[] readByteSeq(this PackageCacheDB ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.PackageCacheDB.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static PackageCacheDB GetPackageCacheDB(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            var os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.PackageCacheDB.ice_read(os__);
            return result;
        }
        #endregion

        #region ResponsePackageOrder  StreamPackage 转换
        /// <summary>
        /// ResponsePackageOrder 对象返回 StreamPackage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iOs"></param>
        /// <returns></returns>

        public static StreamPackage GetStreamPackage(this ResponsePackageOrder p, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            StreamPackage r = new StreamPackage();
            if (p == null)
            {
                return r;
            }
            p.GetInfo();
            r.ClientId = p.ClientId;
            r.ServiceNo = p.ServiceNo;
            r.MsgId = p.MsgId;
            r.UUID = p.UUID;
            r.Head = p.Head;
            r.DataBody = p.DataBody;

            r.ErrorInfo = p.ErrorInfo;
            r.ErrorNo = p.ErrorNo;

            r.DatasDic = new Dictionary<string, string>();

            if (p.SlmOrderBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.SlmOrderBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SlmOrderBody.ToIntString();
                return r;
            }
            if (p.SlmOrderJourBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.SlmOrderJourBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SlmOrderJourBody.ToIntString();
                return r;
            }

            if (p.SendOrderStatusBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.SendOrderStatusBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SendOrderStatusBody.ToIntString();
                return r;
            }

            if (p.PushMatchStateBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.PushMatchStateBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.PushMatchStateBody.ToIntString();
                return r;
            }

            if (p.PushOrderStateBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.PushOrderStateBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.PushOrderStateBody.ToIntString();
                return r;
            }

            if (p.SlmMatchBody.HasItem())
            {
                r.DataStreamBody = p.SlmMatchBody.readByteSeq(iOs, v);
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SlmMatchBody.ToIntString();
                return r;
            }

            return r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iceObjType">1:SlmOrderBody 2:SlmOrderJourBody 3:SendOrderStatusBody 4:SlmMatchBody 5:PushOrderStateBody 6:PushMatchStateBody</param>
        /// <param name="iOs"></param>
        /// <returns></returns>
        public static ResponsePackageOrder GetResponsePackageOrder(this StreamPackage p, PackageOrderType iceObjType, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            ResponsePackageOrder r = new ResponsePackageOrder();
            if (p == null)
            {
                return r;
            }
            r.ClientId = p.ClientId;
            r.ServiceNo = p.ServiceNo;
            r.MsgId = p.MsgId;
            r.Head = p.Head;
            r.UUID = p.UUID;
            r.DataBody = p.DataBody;

            if (!p.DataStreamBody.HasItem())
            {
                return r;
            }

            switch (iceObjType)
            {
                case PackageOrderType.SlmOrderBody:
                    r.SlmOrderBody = p.DataStreamBody[0].GetSlmOrderRow(iOs, v);
                    break;
                case PackageOrderType.PushMatchStateBody:
                    r.PushMatchStateBody = p.DataStreamBody[0].GetPushMatchStateRows(iOs, v);
                    break;
                case PackageOrderType.PushOrderStateBody:
                    r.PushOrderStateBody = p.DataStreamBody[0].GetPushOrderStateRow(iOs, v);
                    break;
                case PackageOrderType.SendOrderStatusBody:
                    r.SendOrderStatusBody = p.DataStreamBody[0].GetSendOrderStatusRow(iOs, v);
                    break;
                case PackageOrderType.SlmOrderJourBody:
                    r.SlmOrderJourBody = p.DataStreamBody[0].GetSlmOrderJourRow(iOs, v);
                    break;
                case PackageOrderType.SlmMatchBody:
                    r.SlmMatchBody = new SlmMatchRow[p.DataStreamBody.Length];
                    for (int i = 0; i < p.DataStreamBody.Length; i++)
                    {
                        r.SlmMatchBody[i] = p.DataStreamBody[i].GetSlmMatchRow(iOs, v);
                    }
                    break;
                default:
                    throw new System.Exception($"{iceObjType}:类型对象不支持转换");
                    break;
            }

            return r;
        }
        #endregion

        #region RequestPackageOrder  StreamPackage 转换
        /// <summary>
        /// RequestPackageOrder 对象返回 StreamPackage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iOs"></param>
        /// <returns></returns>

        public static StreamPackage GetStreamPackage(this RequestPackageOrder p, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            StreamPackage r = new StreamPackage();
            if (p == null)
            {
                return r;
            }

            p.GetInfo();

            r.ClientId = p.ClientId;
            r.ServiceNo = p.ServiceNo;
            r.MsgId = p.MsgId;
            r.UUID = p.UUID;
            r.Head = p.Head;
            r.DataBody = p.DataBody;

            r.DatasDic = new Dictionary<string, string>();

            //r.ErrorInfo = p.ErrorInfo;
            //r.ErrorNo = p.ErrorNo;

            if (p.SlmOrderBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.SlmOrderBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SlmOrderBody.ToIntString();
                return r;
            }
            if (p.SlmOrderJourBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.SlmOrderJourBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SlmOrderJourBody.ToIntString();
                return r;
            }

            if (p.SendOrderStatusBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.SendOrderStatusBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SendOrderStatusBody.ToIntString();
                return r;
            }

            if (p.PushMatchStateBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.PushMatchStateBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.PushMatchStateBody.ToIntString();
                return r;
            }

            if (p.PushOrderStateBody != null)
            {
                r.DataStreamBody = new byte[1][] { p.PushOrderStateBody.readByteSeq(iOs, v) };
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.PushOrderStateBody.ToIntString();
                return r;
            }

            if (p.SlmMatchBody.HasItem())
            {
                r.DataStreamBody = p.SlmMatchBody.readByteSeq(iOs, v);
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.SlmMatchBody.ToIntString();
                return r;
            }

            return r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iceObjType">1:SlmOrderBody 2:SlmOrderJourBody 3:SendOrderStatusBody 4:SlmMatchBody 5:PushOrderStateBody 6:PushMatchStateBody</param>
        /// <param name="iOs"></param>
        /// <returns></returns>
        public static RequestPackageOrder GetRequestPackageOrder(this StreamPackage p, PackageOrderType iceObjType, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            RequestPackageOrder r = new RequestPackageOrder();
            if (p == null)
            {
                return r;
            }
            r.ClientId = p.ClientId;
            r.ServiceNo = p.ServiceNo;
            r.MsgId = p.MsgId;
            r.Head = p.Head;
            r.DataBody = p.DataBody;

            if (!p.DataStreamBody.HasItem())
            {
                return r;
            }

            switch (iceObjType)
            {
                case PackageOrderType.SlmOrderBody:
                    r.SlmOrderBody = p.DataStreamBody[0].GetSlmOrderRow(iOs, v);
                    break;
                case PackageOrderType.PushMatchStateBody:
                    r.PushMatchStateBody = p.DataStreamBody[0].GetPushMatchStateRows(iOs, v);
                    break;
                case PackageOrderType.PushOrderStateBody:
                    r.PushOrderStateBody = p.DataStreamBody[0].GetPushOrderStateRow(iOs, v);
                    break;
                case PackageOrderType.SendOrderStatusBody:
                    r.SendOrderStatusBody = p.DataStreamBody[0].GetSendOrderStatusRow(iOs, v);
                    break;
                case PackageOrderType.SlmOrderJourBody:
                    r.SlmOrderJourBody = p.DataStreamBody[0].GetSlmOrderJourRow(iOs, v);
                    break;
                case PackageOrderType.SlmMatchBody:
                    r.SlmMatchBody = new SlmMatchRow[p.DataStreamBody.Length];
                    for (int i = 0; i < p.DataStreamBody.Length; i++)
                    {
                        r.SlmMatchBody[i] = p.DataStreamBody[i].GetSlmMatchRow(iOs, v);
                    }
                    break;
                default:
                    throw new System.Exception($"{iceObjType}:类型对象不支持转换");
                    break;
            }

            return r;
        }
        #endregion

        #region CacheTradeDB PackageCacheDB StreamPackage 转换
        #region 实体类转byte[]
        /// <summary>
        /// OrderCacheDB
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this OrderCacheDB ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.OrderCacheDB.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static OrderCacheDB GetOrderCacheDB(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            InputStream os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.OrderCacheDB.ice_read(os__);
            return result;
        }
        public static OrderCacheDB[] GetOrderCacheDB(this byte[][] readByteSeq, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var result = new OrderCacheDB[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetOrderCacheDB(iOs, v);
            }
            return result;
        }
        public static byte[][] readByteSeq(this OrderCacheDB[] packages, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var dd = new byte[packages.Length][];
            for (int i = 0; i < packages.Length; i++)
            {
                dd[i] = packages[i].readByteSeq(iOs, v);
            }
            return dd;
        }
        /// <summary>
        /// OrderjourCacheDB
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this OrderjourCacheDB ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.OrderjourCacheDB.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static OrderjourCacheDB GetOrderjourCacheDB(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            InputStream os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.OrderjourCacheDB.ice_read(os__);
            return result;
        }
        public static OrderjourCacheDB[] GetOrderjourCacheDB(this byte[][] readByteSeq, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var result = new OrderjourCacheDB[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetOrderjourCacheDB(iOs, v);
            }
            return result;
        }
        public static byte[][] readByteSeq(this OrderjourCacheDB[] packages, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var dd = new byte[packages.Length][];
            for (int i = 0; i < packages.Length; i++)
            {
                dd[i] = packages[i].readByteSeq(iOs, v);
            }
            return dd;
        }
        /// <summary>
        /// TradeCacheDB
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this TradeCacheDB ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.TradeCacheDB.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static TradeCacheDB GetTradeCacheDB(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            InputStream os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.TradeCacheDB.ice_read(os__);
            return result;
        }
        public static TradeCacheDB[] GetTradeCacheDB(this byte[][] readByteSeq, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var result = new TradeCacheDB[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetTradeCacheDB(iOs, v);
            }
            return result;
        }
        public static byte[][] readByteSeq(this TradeCacheDB[] packages, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var dd = new byte[packages.Length][];
            for (int i = 0; i < packages.Length; i++)
            {
                dd[i] = packages[i].readByteSeq(iOs, v);
            }
            return dd;
        }
        /// <summary>
        /// DepositCacheDB
        /// </summary>
        /// <param name="ResponsePackageOrderLog"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static byte[] readByteSeq(this DepositCacheDB ResponsePackageOrderLog, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            OutputStream os__ = new OutputStream(i, v);
            SlmCommunicator.DepositCacheDB.ice_write(os__, ResponsePackageOrderLog);
            var dd = os__.getBuffer();
            return dd.b.rawBytes(0, dd.size());
        }
        public static DepositCacheDB GetDepositCacheDB(this byte[] readByteSeq, IceInternal.Instance i, Ice.EncodingVersion v)
        {
            InputStream os__ = new InputStream(i, v, readByteSeq);
            var result = SlmCommunicator.DepositCacheDB.ice_read(os__);
            return result;
        }
        public static DepositCacheDB[] GetDepositCacheDB(this byte[][] readByteSeq, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var result = new DepositCacheDB[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetDepositCacheDB(iOs, v);
            }
            return result;
        }
        public static byte[][] readByteSeq(this DepositCacheDB[] packages, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var dd = new byte[packages.Length][];
            for (int i = 0; i < packages.Length; i++)
            {
                dd[i] = packages[i].readByteSeq(iOs, v);
            }
            return dd;
        }
        #endregion
        #region 转Stream
        /// <summary>
        /// PackageCacheDB CacheTradeDB 对象返回 StreamPackage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iOs"></param>
        /// <returns></returns>

        public static StreamPackage GetStreamPackage(this PackageCacheDB p)
        {
            StreamPackage r = new StreamPackage();
            if (p == null)
            {
                return r;
            }
            r.ClientId = p.ClientId;
            r.ServiceNo = p.ServiceNo;
            r.MsgId = p.MsgId;
            r.Head = p.Head;
            r.DataBody = p.DataBody;

            r.ErrorInfo = p.ErrorInfo;
            r.ErrorNo = p.ErrorNo;

            r.DatasDic = new Dictionary<string, string>();

            //if (p.DatasDic != null)
            //{
            //    foreach (var item in p.DatasDic)
            //    {
            //        r.DatasDic[item.Key] = item.Value;
            //    }
            //}
            return r;
        }

        public static StreamPackage GetStreamPackage(this CacheTradeDB p, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            StreamPackage r = new StreamPackage();
            if (p == null)
            {
                return r;
            }
            r = p.PackageData.GetStreamPackage();

            if (p.Orders.HasItem())
            {
                r.DataStreamBody = p.Orders.readByteSeq(iOs, v);
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.Orders.ToIntString();
                return r;
            }
            if (p.Orderjours.HasItem())
            {
                r.DataStreamBody = p.Orderjours.readByteSeq(iOs, v);
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.Orderjours.ToIntString();
                return r;
            }
            if (p.Trades.HasItem())
            {
                r.DataStreamBody = p.Trades.readByteSeq(iOs, v);
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.Trades.ToIntString();
                return r;
            }
            if (p.Deposits.HasItem())
            {
                r.DataStreamBody = p.Deposits.readByteSeq(iOs, v);
                r.DatasDic[MsgTypeForLog.PackageOrderType] = PackageOrderType.Deposits.ToIntString();
                return r;
            }

            return r;
        }
        #endregion
        #region 反转stream
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iceObjType">1:SlmOrderBody 2:SlmOrderJourBody 3:SendOrderStatusBody 4:SlmMatchBody 5:PushOrderStateBody 6:PushMatchStateBody</param>
        /// <param name="iOs"></param>
        /// <returns></returns>
        public static CacheTradeDB GetCacheTradeDB(this StreamPackage p, PackageOrderType iceObjType, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            CacheTradeDB r = new CacheTradeDB();
            if (p == null)
            {
                return r;
            }
            PackageCacheDB packageBase = new PackageCacheDB();
            packageBase.ClientId = p.ClientId;
            packageBase.ServiceNo = p.ServiceNo;
            packageBase.MsgId = p.MsgId;
            packageBase.UUID = p.UUID;
            packageBase.Head = p.Head;
            packageBase.DataBody = p.DataBody;
            packageBase.DatasDic = p.DatasDic;
            r.PackageData = packageBase;

            if (!p.DataStreamBody.HasItem())
            {
                return r;
            }

            switch (iceObjType)
            {
                case PackageOrderType.Orders:
                    r.Orders = p.DataStreamBody.GetOrderCacheDB(iOs, v);
                    break;
                case PackageOrderType.Orderjours:
                    r.Orderjours = p.DataStreamBody.GetOrderjourCacheDB(iOs, v);
                    break;
                case PackageOrderType.Trades:
                    r.Trades = p.DataStreamBody.GetTradeCacheDB(iOs, v);
                    break;
                case PackageOrderType.Deposits:
                    r.Deposits = p.DataStreamBody.GetDepositCacheDB(iOs, v);
                    break;
                default:
                    throw new System.Exception($"{iceObjType}:类型对象不支持转换");
                    break;
            }

            return r;
        }

        public static CacheTradeDB GetCacheTradeDB(this StreamPackage p, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var iceObjType = p.GetPackageOrderType();
            var r = p.GetCacheTradeDB(iceObjType, iOs, v);
            return r;
        }

        /// <summary>
        /// int, string, string, byte[][]
        /// serverno,uuid,msgid,DataStreamBody
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static CacheTradeDB GetCacheTradeDB(this Tuple<int, string, string, byte[]> p, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            CacheTradeDB r = new CacheTradeDB();
            if (p == null || !p.Item4.HasItem())
            {
                return null;
            }

            PackageCacheDB packageBase = new PackageCacheDB();
            packageBase.ServiceNo = p.Item1;
            packageBase.UUID = p.Item2;
            packageBase.MsgId = p.Item3;
            packageBase.DatasDic = new Dictionary<string, string>();
            r.PackageData = packageBase;

            var iceObjType = p.Item1.GetPackageOrderType();
            switch (iceObjType)
            {
                case PackageOrderType.Orders:
                    r.Orders = new OrderCacheDB[1] { p.Item4.GetOrderCacheDB(iOs, v) };
                    break;
                case PackageOrderType.Orderjours:
                    r.Orderjours = new OrderjourCacheDB[1] { p.Item4.GetOrderjourCacheDB(iOs, v) };
                    break;
                case PackageOrderType.Trades:
                    r.Trades = new TradeCacheDB[1] { p.Item4.GetTradeCacheDB(iOs, v) };
                    break;
                case PackageOrderType.Deposits:
                    r.Deposits = new DepositCacheDB[1] { p.Item4.GetDepositCacheDB(iOs, v) };
                    break;
                default:
                    throw new System.Exception($"{iceObjType}:类型对象不支持转换");
                    break;
            }
            return r;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static CacheTradeDB GetCacheTradeDB<T>(this Tuple<PackageOrderType, T> tuple)
            where T : class
        {
            var result = new CacheTradeDB() { };

            result.PackageData = new PackageCacheDB() { };

            switch (tuple.Item1)
            {
                case PackageOrderType.SlmOrderBody:
                case PackageOrderType.Orders:
                    result.PackageData.ServiceNo = (int)CacheSeverServiceNo.TE_ORDER_OFFSET;
                    result.Orders = new OrderCacheDB[1] { tuple.Item2.JsonTo<OrderCacheDB>() };
                    break;
                case PackageOrderType.SlmOrderJourBody:
                case PackageOrderType.Orderjours:
                    result.PackageData.ServiceNo = (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET;
                    result.Orderjours = new OrderjourCacheDB[1] { tuple.Item2.JsonTo<OrderjourCacheDB>()
};
                    break;
                case PackageOrderType.SlmMatchBody:
                case PackageOrderType.Trades:
                    result.PackageData.ServiceNo = (int)CacheSeverServiceNo.TE_TRADE_OFFSET;
                    result.Trades = new TradeCacheDB[1] { tuple.Item2.JsonTo<TradeCacheDB>() };
                    break;
                case PackageOrderType.Deposits:
                    result.PackageData.ServiceNo = (int)CacheSeverServiceNo.TE_DEPOSIT_OFFSET;
                    result.Deposits = new DepositCacheDB[1] { tuple.Item2.JsonTo<DepositCacheDB>() };
                    break;
                default:
                    return null;
                    break;
            }
            return result;
        }
        public static object GetICEObj(this StreamPackage p, PackageOrderType iceObjType, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            object r = null;
            if (p == null)
            {
                return r;
            }

            if (!p.DataStreamBody.HasItem())
            {
                return r;
            }

            switch (iceObjType)
            {
                case PackageOrderType.SlmOrderBody:
                    r = p.DataStreamBody[0].GetSlmOrderRow(iOs, v);
                    break;
                case PackageOrderType.PushMatchStateBody:
                    r = p.DataStreamBody[0].GetPushMatchStateRows(iOs, v);
                    break;
                case PackageOrderType.PushOrderStateBody:
                    r = p.DataStreamBody[0].GetPushOrderStateRow(iOs, v);
                    break;
                case PackageOrderType.SendOrderStatusBody:
                    r = p.DataStreamBody[0].GetSendOrderStatusRow(iOs, v);
                    break;
                case PackageOrderType.SlmOrderJourBody:
                    r = p.DataStreamBody[0].GetSlmOrderJourRow(iOs, v);
                    break;
                case PackageOrderType.SlmMatchBody:
                    r = p.DataStreamBody.GetSlmMatchRow(iOs, v);
                    break;
                case PackageOrderType.Orders:
                    r = p.DataStreamBody.GetOrderCacheDB(iOs, v);
                    break;
                case PackageOrderType.Orderjours:
                    r = p.DataStreamBody.GetOrderjourCacheDB(iOs, v);
                    break;
                case PackageOrderType.Trades:
                    r = p.DataStreamBody.GetTradeCacheDB(iOs, v);
                    break;
                case PackageOrderType.Deposits:
                    r = p.DataStreamBody.GetDepositCacheDB(iOs, v);
                    break;
                default:
                    throw new System.Exception($"{iceObjType}:类型对象不支持转换");
                    break;
            }

            return r;
        }
        public static object GetICEObj(this StreamPackage p, IceInternal.Instance iOs, Ice.EncodingVersion v)
        {
            var r = p.GetICEObj(p.GetPackageOrderType(), iOs, v);
            return r;
        }

        #endregion
        #endregion
    }
}
