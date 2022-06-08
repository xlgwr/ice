using SlmCommunicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageServer
{
    /// <summary>
    /// 实体类型
    /// </summary>
    public enum emMType
    {
        /// <summary>
        /// old 消息类
        /// </summary>
        Base = 0,
        /// <summary>
        /// 批量类
        /// </summary>
        OrderList = 1,
        /// <summary>
        /// 内存 通用
        /// </summary>
        CacheDB = 2,
        /// <summary>
        /// 内存 交易
        /// </summary>
        TradeDB = 3

    }
    /// <summary>
    /// Dto通用标记属性
    /// </summary>
    public class ComDto
    {
        public int ServiceNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public emMType MType
        {
            get; set;
        } 
    }
    /// <summary>
    /// 入参数类
    /// </summary>
    public class PackageCacheDBDto : ComDto
    {
        public PackageCacheDBDto()
        {
            base.MType = emMType.CacheDB;
        }
        public string ServiceName { get; set; }
        public string ClientId { get; set; }
        public string UUID { get; set; }
        public string MsgId { get; set; }
        public int ErrorNo { get; set; }
        public string ErrorInfo { get; set; }
        public long SyncPoint { get; set; }
        public object PagingInfo { get; set; }
        public Dictionary<string, string> DatasDic { get; set; }
        public List<string> Head { get; set; }
        public Dictionary<int, List<string>> DataBody { get; set; }

        public PackageCacheDBDto Copy()
        {
            var result = new PackageCacheDBDto()
            {
                ClientId = this.ClientId,
                UUID = this.UUID,
                DataBody = this.DataBody,
                Head = this.Head,
                ServiceName = this.ServiceName,
                ServiceNo = this.ServiceNo,
                ErrorInfo = this.ErrorInfo,
                ErrorNo = this.ErrorNo,
                DatasDic = this.DatasDic,
                MsgId = this.MsgId,
                PagingInfo = this.PagingInfo,
                SyncPoint = this.SyncPoint,
                MType = this.MType
            };
            return result;
        }

    }

    public class CacheTradeDBDto : ComDto
    {
        public CacheTradeDBDto()
        {
            base.MType = emMType.TradeDB;
        }
        public PackageCacheDBDto PackageData { get; set; }
        public List<OrderCacheDB> Orders { get; set; }
        public List<OrderjourCacheDB> Orderjours { get; set; }
        public List<TradeCacheDB> Trades { get; set; }
        public List<PositionCacheDB> Positions { get; set; }
        public List<PositionCacheDB> PrePositions { get; set; }
        public List<DepositCacheDB> Deposits { get; set; }
    }

    /// <summary>
    /// 入参数类
    /// </summary>
    public class ResponsePackageOrderDto : ComDto
    {
        public ResponsePackageOrderDto()
        {
            base.MType = emMType.OrderList;
        }
        public string ServiceName { get; set; }
        public string ClientId { get; set; }
        public string UUId { get; set; }
        public string MsgId { get; set; }
        public List<string> Head { get; set; }
        public Dictionary<int, List<string>> DataBody { get; set; }
        public SlmOrderRow SlmOrderBody { get; set; }
        public PushMatchStateRow PushMatchStateBody { get; set; }
        public PushOrderStateRow PushOrderStateBody { get; set; }
        public SendOrderStatusRow SendOrderStatusBody { get; set; }
        public SlmOrderJourRow SlmOrderJourBody { get; set; }
        public List<SlmMatchRow> SlmMatchBody { get; set; }
        public ResponsePackageOrderDto Copy()
        {
            var listSlms = new List<SlmMatchRow>();
            if (this.SlmMatchBody != null)
            {
                foreach (var item in this.SlmMatchBody)
                {
                    listSlms.Add((SlmMatchRow)item.Clone());
                }
            }
            var result = new ResponsePackageOrderDto()
            {
                ClientId = this.ClientId,
                UUId = this.UUId,
                DataBody = this.DataBody,
                Head = this.Head,
                MsgId = this.MsgId,
                ServiceName = this.ServiceName,
                ServiceNo = this.ServiceNo,
                SlmOrderBody = (SlmOrderRow)this.SlmOrderBody?.Clone(),
                PushMatchStateBody = (PushMatchStateRow)this.PushMatchStateBody?.Clone(),
                PushOrderStateBody = (PushOrderStateRow)this.PushOrderStateBody?.Clone(),
                SendOrderStatusBody = (SendOrderStatusRow)this.SendOrderStatusBody?.Clone(),
                SlmOrderJourBody = (SlmOrderJourRow)this.SlmOrderJourBody?.Clone(),
                SlmMatchBody = listSlms
            };
            return result;
        }

    }
}
