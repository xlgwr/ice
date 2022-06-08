using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solomon.Logger;

namespace SlmCommunicator
{
    public static class PackageCacheDBExts
    {
        #region PackageCacheDBExts
        /// <summary>
        /// 更新ErrorNo，ErrorInfo
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="errorNo"></param>
        /// <param name="errorInfo"></param>
        public static void UpdateError(this PackageCacheDB curr, int errorNo = 0, string errorInfo = "")
        {
            if (curr == null)
            {
                return;
            }
            curr.ErrorNo = errorNo;
            curr.ErrorInfo = errorInfo;
        }

        public static PackageCacheDB GetNewByServerNo(this PackageCacheDB package, int serverNo, int errorNo = 0, string errorInfo = "")
        {
            return new PackageCacheDB() { ServiceNo = serverNo, ErrorNo = errorNo, ErrorInfo = errorInfo };
        }
        /// <summary>
        /// 获取包对应Heart log
        /// $"Heart:{param.ServiceNo}:{param.ClientId}:MsgId:{param.MsgId},UUID:{param.UUID}";
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetLogHeart(this PackageCacheDB param, string title = "Heart")
        {
            if (param == null)
            {
                return "";
            }
            return $"{title}:{param.ServiceNo}:{param.ClientId}:MsgId:{param.MsgId},UUID:{param.UUID}";
        }

        public static string GetLogHeart(this RequestPackageOrder param, string title = "Heart")
        {
            if (param == null)
            {
                return "";
            }
            return $"{title}:{param.ServiceNo}:{param.ClientId}:MsgId:{param.MsgId},UUID:{param.UUID}";
        }
        public static string GetLogHeart(this ResponsePackageOrder param, string title = "Heart")
        {
            if (param == null)
            {
                return "";
            }
            return $"{title}:{param.ServiceNo}:{param.ClientId}:MsgId:{param.MsgId},UUID:{param.UUID}";
        }
        #endregion
        #region 根据服务号生成对象

        public static PackageCacheDB GetNewPackageCacheDB(this CacheSeverServiceNo serverNo, int errorNo = 0, string errorInfo = "")
        {
            return new PackageCacheDB() { ServiceNo = (int)serverNo, ErrorNo = errorNo, ErrorInfo = errorInfo };
        }
        public static PackageCacheDB GetNewPackageCacheDB(this CacheSeverServiceNo serverNo, long msgNum, string msgid = "", int errorNo = 0, string errorInfo = "")
        {
            return new PackageCacheDB() { ServiceNo = (int)serverNo, MsgId = msgid, ErrorNo = errorNo, ErrorInfo = errorInfo, DataBody = new string[1][] { new string[] { $"{msgNum}" } } };
        }
        public static PackageCacheDB GetNewPackageCacheDB(this CacheSeverServiceNo serverNo, string[][] databody, string[] header = null, int errorNo = 0, string errorInfo = "")
        {
            return new PackageCacheDB() { ServiceNo = (int)serverNo, ErrorNo = errorNo, ErrorInfo = errorInfo, Head = header, DataBody = databody };
        }
        public static PackageCacheDB GetNewPackageCacheDB(this CacheSeverServiceNo serverNo, string[][] databody, string msgid = "", string[] header = null, int errorNo = 0, string errorInfo = "")
        {
            return new PackageCacheDB() { ServiceNo = (int)serverNo, ErrorNo = errorNo, ErrorInfo = errorInfo, Head = header, DataBody = databody, MsgId = msgid };
        }
        public static CacheTradeDB GetNewCacheTradeDB(this CacheSeverServiceNo serverNo, int errorNo = 0, string errorInfo = "")
        {
            return new CacheTradeDB() { PackageData = serverNo.GetNewPackageCacheDB(errorNo, errorInfo) };
        }
        public static CacheTradeDB GetNewCacheTradeDB(this CacheSeverServiceNo serverNo, long msgNum, string msgid = "", int errorNo = 0, string errorInfo = "")
        {
            return new CacheTradeDB() { PackageData = serverNo.GetNewPackageCacheDB(msgNum, msgid, errorNo, errorInfo) };
        }
        public static CacheTradeDB GetNewCacheTradeDB(this CacheSeverServiceNo serverNo, string[][] databody, string[] header = null, int errorNo = 0, string errorInfo = "")
        {
            return new CacheTradeDB() { PackageData = serverNo.GetNewPackageCacheDB(databody, header, errorNo, errorInfo) };
        }
        public static CacheTradeDB GetNewCacheTradeDB(this CacheSeverServiceNo serverNo, string[][] databody, string msgid, string[] header = null, int errorNo = 0, string errorInfo = "")
        {
            return new CacheTradeDB() { PackageData = serverNo.GetNewPackageCacheDB(databody, msgid, header, errorNo, errorInfo) };
        }
        /// <summary>
        /// 获取 CacheSeverServiceNo 对应的name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetCacheSeverServiceNoName(this int? value)
        {
            try
            {
                return value.Value.GetCacheSeverServiceNoName();
            }
            catch (Exception ex)
            {
                return $"{value}";
            }
        }
        public static string GetCacheSeverServiceNoName(this int value)
        {
            try
            {
                return $"{value.GetValue<CacheSeverServiceNo>()}";
            }
            catch (Exception ex)
            {
                return $"{value}";
            }
        }
        #endregion
        #region CacheTradeDB 对象扩展
        /// <summary>
        /// 获取增量对应对象最大msgNum
        /// </summary>
        /// <param name="param"></param>
        /// <param name="serverno"></param>
        /// <returns></returns>
        public static long GetMaxMsgNum(this CacheTradeDB param, CacheSeverServiceNo serverno = 0)
        {
            long maxMsgNum = 0;
            var servernoSw = serverno == 0 ? param.PackageData.ServiceNo : (int)serverno;
            switch (servernoSw)
            {
                case (int)CacheSeverServiceNo.TE_ORDER_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDER_ADD:
                case (int)CacheSeverServiceNo.TE_ORDER_UPD:
                case (int)CacheSeverServiceNo.TE_ORDER_RECOVERY:
                    if (param.Orders.HasItem())
                    {
                        maxMsgNum = param.Orders.Max(a => a.MsgNum);
                    }
                    break;
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_ADD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_UPD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_RECOVERY:
                    if (param.Orderjours.HasItem())
                    {
                        maxMsgNum = param.Orderjours.Max(a => a.MsgNum);
                    }
                    break;
                case (int)CacheSeverServiceNo.TE_TRADE_ADD:
                case (int)CacheSeverServiceNo.TE_TRADE_UPD:
                case (int)CacheSeverServiceNo.TE_TRADE_OFFSET:
                case (int)CacheSeverServiceNo.TE_TRADE_RECOVERY:
                    if (param.Trades.HasItem())
                    {
                        maxMsgNum = param.Trades.Max(a => a.MsgNum);
                    }
                    break;

                case (int)CacheSeverServiceNo.TE_DEPOSIT_ADD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_UPD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_OFFSET:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_RECOVERY:
                    if (param.Deposits.HasItem())
                    {
                        maxMsgNum = param.Deposits.Max(a => a.MsgNum);
                    }
                    break;
                case (int)CacheSeverServiceNo.TE_Position_OFFSET:
                    if (param.Positions.HasItem())
                    {
                        maxMsgNum = param.Positions.Max(a => a.MsgNum);
                    }
                    break;
                default:
                    break;
            }
            return maxMsgNum;
        }

        /// <summary>
        /// 获取增量对应对象最小msgNum
        /// </summary>
        /// <param name="param"></param>
        /// <param name="serverno"></param>
        /// <returns></returns>
        public static long GetMinMsgNum(this CacheTradeDB param, CacheSeverServiceNo serverno = 0)
        {
            long maxMsgNum = 0;
            var servernoSw = serverno == 0 ? param.PackageData.ServiceNo : (int)serverno;
            switch (servernoSw)
            {
                case (int)CacheSeverServiceNo.TE_ORDER_OFFSET:
                case (int)CacheSeverServiceNo.TE_ORDER_ADD:
                case (int)CacheSeverServiceNo.TE_ORDER_UPD:
                case (int)CacheSeverServiceNo.TE_ORDER_RECOVERY:
                    if (param.Orders.HasItem())
                    {
                        maxMsgNum = param.Orders.Min(a => a.MsgNum);
                    }
                    break;
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_ADD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_UPD:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_RECOVERY:
                case (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET:
                    if (param.Orderjours.HasItem())
                    {
                        maxMsgNum = param.Orderjours.Min(a => a.MsgNum);
                    }
                    break;
                case (int)CacheSeverServiceNo.TE_TRADE_ADD:
                case (int)CacheSeverServiceNo.TE_TRADE_UPD:
                case (int)CacheSeverServiceNo.TE_TRADE_OFFSET:
                case (int)CacheSeverServiceNo.TE_TRADE_RECOVERY:
                    if (param.Trades.HasItem())
                    {
                        maxMsgNum = param.Trades.Min(a => a.MsgNum);
                    }
                    break;

                case (int)CacheSeverServiceNo.TE_DEPOSIT_ADD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_UPD:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_OFFSET:
                case (int)CacheSeverServiceNo.TE_DEPOSIT_RECOVERY:
                    if (param.Deposits.HasItem())
                    {
                        maxMsgNum = param.Deposits.Min(a => a.MsgNum);
                    }
                    break;
                case (int)CacheSeverServiceNo.TE_Position_OFFSET:
                    if (param.Positions.HasItem())
                    {
                        maxMsgNum = param.Positions.Min(a => a.MsgNum);
                    }
                    break;
                default:
                    break;
            }
            return maxMsgNum;
        }
        #endregion
        #region Key

        public static string GetKey(this OrderCacheDB m)
        {
            return $"{m.InitDate}_{m.OrderNo}";
        }
        public static string GetKey(this OrderjourCacheDB m)
        {
            return $"{m.InitDate}_{m.OrderNo}_{m.OrderJourNo}";
        }
        public static string GetKey(this PositionCacheDB m)
        {
            return $"{m.InitDate}_{m.OpenNo}";
        }
        public static string GetKey(this TradeCacheDB m)
        {
            return $"{m.InitDate}_{m.TradeNo}";
        }
        public static string GetKey(this DepositCacheDB m)
        {
            return $"{m.InitDate}_{m.BusinessNo}";
        }
        #endregion
        #region check data is ok
        /// <summary>
        /// 检测数据对象是否正常
        /// </summary>
        /// <param name="orderCacheDB"></param>
        /// <returns></returns>
        public static bool IsCheckIsOk(this OrderCacheDB curr)
        {
            if (curr.AccountNo.IsNullOrWhiteSpace() || curr.OrderNo <= 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检测数据对象是否正常
        /// </summary>
        /// <param name="orderCacheDB"></param>
        /// <returns></returns>
        public static bool IsCheckIsOk(this OrderjourCacheDB curr)
        {
            if (curr.AccountNo.IsNullOrWhiteSpace() || curr.OrderNo <= 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检测数据对象是否正常
        /// </summary>
        /// <param name="orderCacheDB"></param>
        /// <returns></returns>
        public static bool IsCheckIsOk(this TradeCacheDB curr)
        {
            if (curr.AccountNo.IsNullOrWhiteSpace() || curr.OrderNo <= 0 || curr.TradeNo <= 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检测数据对象是否正常
        /// </summary>
        /// <param name="orderCacheDB"></param>
        /// <returns></returns>
        public static bool IsCheckIsOk(this DepositCacheDB curr)
        {
            if (curr.AccountNo.IsNullOrWhiteSpace() || curr.BusinessNo.IsNullOrWhiteSpace())
            {
                return false;
            }
            return true;
        }
        #endregion
        #region MaxMsgNum
        public static long GetMaxMsgNum(this ConcurrentDictionary<string, OrderCacheDB> m)
        {
            return m.HasItem() ? m.Values.Max(a => a.MsgNum) : 0;
        }
        public static long GetMaxMsgNum(this ConcurrentDictionary<string, OrderjourCacheDB> m)
        {
            return m.HasItem() ? m.Values.Max(a => a.MsgNum) : 0;
        }
        public static long GetMaxMsgNum(this ConcurrentDictionary<string, PositionCacheDB> m)
        {
            return m.HasItem() ? m.Values.Max(a => a.MsgNum) : 0;
        }
        public static long GetMaxMsgNum(this ConcurrentDictionary<string, TradeCacheDB> m)
        {
            return m.HasItem() ? m.Values.Max(a => a.MsgNum) : 0;
        }
        public static long GetMaxMsgNum(this ConcurrentDictionary<string, DepositCacheDB> m)
        {
            return m.HasItem() ? m.Values.Max(a => a.MsgNum) : 0;
        }
        #endregion
        #region 字典类相关
        /// <summary>
        ///  限制字典容量总数，超过移除处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cdic"></param>
        /// <param name="limitSize"></param>
        public static int CheckSizeAndClear<T>(this ConcurrentDictionary<long, T> cdic, int limitSize = 10000)
        {
            if (cdic == null)
            {
                return 0;
            }
            //算出差值
            int toDiv = cdic.Count - limitSize;
            if (toDiv > 0)
            {
                var toDivList = cdic.Keys.OrderBy(a => a).Take(toDiv).ToList();
                foreach (var item in toDivList)
                {
                    cdic.TryRemove(item, out _);
                }
            }
            return toDiv;
        }
        #endregion
    }
}
