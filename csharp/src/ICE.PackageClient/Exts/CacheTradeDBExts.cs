using SlmCommunicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageClient.Exts
{
    public static class CacheTradeDBExts
    {
        public static OrderCacheDB Copy(this OrderCacheDB request, bool isAdd, int addNum)
        {
            var curr = request.Clone() as OrderCacheDB;
            if (isAdd)
            {
                curr.MsgNum += addNum;
                curr.OrderNo += addNum;
            }
            return curr;
        }
        public static OrderjourCacheDB Copy(this OrderjourCacheDB request, bool isAdd, int addNum)
        {
            var curr = request.Clone() as OrderjourCacheDB;
            if (isAdd)
            {
                curr.MsgNum += addNum;
                curr.OrderNo += addNum;
            }
            return curr;
        }
        public static TradeCacheDB Copy(this TradeCacheDB request, bool isAdd, int addNum)
        {
            var curr = request.Clone() as TradeCacheDB;
            if (isAdd)
            {
                curr.MsgNum += addNum;
                curr.OrderNo += addNum;
                curr.TradeNo += addNum;
            }
            return curr;
        }
        public static DepositCacheDB Copy(this DepositCacheDB request, bool isAdd, int addNum)
        {
            var curr = request.Clone() as DepositCacheDB;
            if (isAdd)
            {
                curr.MsgNum += addNum;
                curr.BusinessNo = (curr.BusinessNo.TryToLong() + addNum).ToString();
            }
            return curr;
        }
    }
}
