using SlmCommunicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageClient
{
    /// <summary>
    /// 实体类型
    /// 0:old 消息类,1:批量类,2:内存 通用,3:内存 交易
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
        } = emMType.Base;
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
        public long OffSet { get; set; }
        public object PagingInfo { get; set; }
        public Dictionary<string, string> DatasDic { get; set; }
        public List<string> Head { get; set; }
        public Dictionary<int, List<string>> DataBody { get; set; }

        public PackageCacheDB Copy()
        {
            var arrbody = new string[this.DataBody.Count][];
            var first = 0;
            foreach (var item in this.DataBody.Values)
            {
                arrbody[first] = item.ToArray();
                first++;
            }

            var result = new PackageCacheDB()
            {
                ClientId = this.ClientId,
                UUID = this.UUID,
                DataBody = arrbody,
                Head = this.Head?.ToArray(),
                ServiceNo = this.ServiceNo,
                ErrorInfo = this.ErrorInfo,
                ErrorNo = this.ErrorNo,
                DatasDic = this.DatasDic,
                MsgId = this.MsgId,
                PagingInfo = this.PagingInfo != null ? (PagingInfoBase)this.PagingInfo : new PagingInfoBase(),
                //OffSet = this.OffSet
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
        public List<OrderCacheDBDTO> Orders { get; set; }
        public List<OrderjourCacheDBDTO> Orderjours { get; set; }
        public List<TradeCacheDBDTO> Trades { get; set; }
        public List<PositionCacheDBDTO> Positions { get; set; }
        public List<PositionCacheDBDTO> PrePositions { get; set; }
        public List<DepositCacheDBDTO> Deposits { get; set; }

        public CacheTradeDB Copy()
        {
            CacheTradeDB curr = new CacheTradeDB();

            curr.PackageData = this.PackageData.Copy();
            var tmpListOrder = new List<OrderCacheDB>();
            var tmpListOrderJour = new List<OrderjourCacheDB>();
            var tmpListTrade = new List<TradeCacheDB>();
            var tmpListPositionPre = new List<PositionCacheDB>();
            var tmpListPosition = new List<PositionCacheDB>();
            var tmpListDeposit = new List<DepositCacheDB>();

            this.Orders?.ForEach(a =>
            {
                tmpListOrder.Add(a.Copy());
            });
            curr.Orders = tmpListOrder?.ToArray();

            this.Orderjours?.ForEach(a =>
            {
                tmpListOrderJour.Add(a.Copy());
            });
            curr.Orderjours = tmpListOrderJour?.ToArray();

            this.Trades?.ForEach(a =>
            {
                tmpListTrade.Add(a.Copy());
            });
            curr.Trades = tmpListTrade?.ToArray();

            this.Positions?.ForEach(a =>
            {
                tmpListPosition.Add(a.Copy());
            });
            curr.Positions = tmpListPosition?.ToArray();

            this.PrePositions?.ForEach(a =>
            {
                tmpListPositionPre.Add(a.Copy());
            });
            curr.PrePositions = tmpListPositionPre?.ToArray();

            this.Deposits?.ForEach(a =>
            {
                tmpListDeposit.Add(a.Copy());
            });
            curr.Deposits = tmpListDeposit?.ToArray();

            return curr;
        }
    }
    public class EntityDto
    {
        public long MsgNum { get; set; }
    }
    public class OrderCacheDBDTO : EntityDto
    {

        public int InitDate { get; set; }


        public string RouterNo { get; set; }


        public string OperatorNo { get; set; }


        public string OrderPerson { get; set; }


        public string UpdatePerson { get; set; }


        public int ProxyId { get; set; }


        public string AccountNo { get; set; }


        public byte ProductType { get; set; }


        public string ExchangeCode { get; set; }


        public string CommodityCode { get; set; }


        public string ContractCode { get; set; }


        public byte CallPut { get; set; }


        public string StrikePrice { get; set; }


        public byte OpenClose { get; set; }


        public byte BuySell { get; set; }


        public double OrderPrice { get; set; }


        public short OrderNum { get; set; }


        public byte OrderType { get; set; }


        public byte OrderLife { get; set; }


        public byte OrderInsertType { get; set; }


        public byte DelTag { get; set; }


        public string OrderFrom { get; set; }


        public short OrderSubNumMin { get; set; }


        public short OrderSubNumMax { get; set; }


        public int OrderExpiryDate { get; set; }


        public double TriggerPrice { get; set; }


        public byte OrderStatus { get; set; }


        public short FilledNum { get; set; }


        public int OrderDate { get; set; }


        public int OrderTime { get; set; }


        public int UpdateDate { get; set; }


        public int UpdateTime { get; set; }


        public int ExchangeDate { get; set; }


        public int ExchangeTime { get; set; }


        public byte OrderRemain { get; set; }


        public long OrderNo { get; set; }


        public short OrderJourNo { get; set; }


        public string BrokerOrderNo { get; set; }


        public string BrokerOrderJourNo { get; set; }


        public string ExchangeOrderNo { get; set; }


        public string CarryNo { get; set; }


        public short ErrorNo { get; set; }


        public string ErrorInfo { get; set; }


        public string RemarkCus { get; set; }


        public string InfoS1 { get; set; }


        public string InfoS2 { get; set; }


        public string LocalNo { get; set; }

        public byte Action { get; set; }
        public OrderCacheDB Copy()
        {
            var m = this;
            OrderCacheDB curr = new OrderCacheDB();

            curr.MsgNum = m.MsgNum;

            curr.InitDate = m.InitDate;

            curr.RouterNo = m.RouterNo;

            curr.OperatorNo = m.OperatorNo;

            curr.OrderPerson = m.OrderPerson;

            curr.UpdatePerson = m.UpdatePerson;

            curr.ProxyId = m.ProxyId;

            curr.AccountNo = m.AccountNo;

            curr.ProductType = m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.CallPut = m.CallPut;

            curr.StrikePrice = m.StrikePrice;

            curr.OpenClose = m.OpenClose;

            curr.BuySell = m.BuySell;

            curr.OrderPrice = m.OrderPrice;

            curr.OrderNum = m.OrderNum;

            curr.OrderType = m.OrderType;

            curr.OrderLife = m.OrderLife;

            curr.OrderInsertType = m.OrderInsertType;

            curr.DelTag = m.DelTag;

            curr.OrderFrom = m.OrderFrom;

            curr.OrderSubNumMin = m.OrderSubNumMin;

            curr.OrderSubNumMax = m.OrderSubNumMax;

            curr.OrderExpiryDate = m.OrderExpiryDate;

            curr.TriggerPrice = m.TriggerPrice;

            curr.OrderStatus = m.OrderStatus;

            curr.FilledNum = m.FilledNum;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.UpdateDate = m.UpdateDate;

            curr.UpdateTime = m.UpdateTime;

            curr.ExchangeDate = m.ExchangeDate;

            curr.ExchangeTime = m.ExchangeTime;

            curr.OrderRemain = m.OrderRemain;

            curr.OrderNo = m.OrderNo;

            curr.OrderJourNo = m.OrderJourNo;

            curr.BrokerOrderNo = m.BrokerOrderNo;

            curr.BrokerOrderJourNo = m.BrokerOrderJourNo;

            curr.ExchangeOrderNo = m.ExchangeOrderNo;

            curr.CarryNo = m.CarryNo;

            curr.ErrorNo = m.ErrorNo;

            curr.ErrorInfo = m.ErrorInfo;

            curr.RemarkCus = m.RemarkCus;

            curr.InfoS1 = m.InfoS1;

            curr.InfoS2 = m.InfoS2;

            curr.LocalNo = m.LocalNo;
            curr.Action = m.Action;

            return curr;
        }
    }

    public class OrderjourCacheDBDTO : EntityDto
    {
        #region Slice data members


        public int InitDate { get; set; }


        public string RouterNo { get; set; }


        public string OperatorNo { get; set; }


        public string OrderPerson { get; set; }


        public string UpdatePerson { get; set; }


        public int ProxyId { get; set; }


        public string AccountNo { get; set; }


        public byte ProductType { get; set; }


        public string ExchangeCode { get; set; }


        public string CommodityCode { get; set; }


        public string ContractCode { get; set; }


        public byte CallPut { get; set; }


        public string StrikePrice { get; set; }


        public byte BuySell { get; set; }


        public double OrderPrice { get; set; }


        public int OrderNum { get; set; }


        public byte OrderType { get; set; }


        public byte OrderLife { get; set; }


        public string OrderFrom { get; set; }


        public double TriggerPrice { get; set; }


        public byte OrderStatus { get; set; }


        public int FilledNum { get; set; }


        public int OrderDate { get; set; }


        public int OrderTime { get; set; }


        public int UpdateDate { get; set; }


        public int UpdateTime { get; set; }


        public int ExchangeDate { get; set; }


        public int ExchangeTime { get; set; }


        public long OrderNo { get; set; }


        public int OrderJourNo { get; set; }


        public string BrokerOrderNo { get; set; }


        public string BrokerOrderJourNo { get; set; }


        public string ExchangeOrderNo { get; set; }


        public int ErrorNo { get; set; }


        public string ErrorInfo { get; set; }


        public string RemarkCus { get; set; }


        public string InfoS1 { get; set; }


        public string InfoS2 { get; set; }
        public byte Action { get; set; }

        #endregion

        public OrderjourCacheDB Copy()
        {
            var m = this;
            OrderjourCacheDB curr = new OrderjourCacheDB();
            curr.MsgNum = m.MsgNum;
            curr.InitDate = m.InitDate;

            curr.RouterNo = m.RouterNo;

            curr.OperatorNo = m.OperatorNo;

            curr.OrderPerson = m.OrderPerson;

            curr.UpdatePerson = m.UpdatePerson;

            curr.ProxyId = m.ProxyId;

            curr.AccountNo = m.AccountNo;

            curr.ProductType = m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.CallPut = m.CallPut;

            curr.StrikePrice = m.StrikePrice;

            curr.BuySell = m.BuySell;

            curr.OrderPrice = m.OrderPrice;

            curr.OrderNum = m.OrderNum;

            curr.OrderType = m.OrderType;

            curr.OrderLife = m.OrderLife;

            curr.OrderFrom = m.OrderFrom;

            curr.TriggerPrice = m.TriggerPrice;

            curr.OrderStatus = m.OrderStatus;

            curr.FilledNum = m.FilledNum;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.UpdateDate = m.UpdateDate;

            curr.UpdateTime = m.UpdateTime;

            curr.ExchangeDate = m.ExchangeDate;

            curr.ExchangeTime = m.ExchangeTime;

            curr.OrderNo = m.OrderNo;

            curr.OrderJourNo = m.OrderJourNo;

            curr.BrokerOrderNo = m.BrokerOrderNo;

            curr.BrokerOrderJourNo = m.BrokerOrderJourNo;

            curr.ExchangeOrderNo = m.ExchangeOrderNo;

            curr.ErrorNo = m.ErrorNo;

            curr.ErrorInfo = m.ErrorInfo;

            curr.RemarkCus = m.RemarkCus;

            curr.InfoS1 = m.InfoS1;

            curr.InfoS2 = m.InfoS2;

            curr.Action = m.Action;
            return curr;
        }
    }

    public class TradeCacheDBDTO : EntityDto
    {
        #region Slice data members


        public int InitDate { get; set; }


        public string AccountNo { get; set; }


        public int OrderDate { get; set; }


        public int OrderTime { get; set; }


        public long OrderNo { get; set; }


        public int TradeDate { get; set; }


        public long TradeNo { get; set; }


        public int TradeTime { get; set; }


        public double TradePrice { get; set; }


        public int TradeVol { get; set; }


        public string CurrencyCode { get; set; }


        public byte IsTplusone { get; set; }


        public byte OpenClose { get; set; }


        public int ProxyId { get; set; }


        public string BrokerTradeNo { get; set; }


        public string ExchangeTradeNo { get; set; }


        public byte ProductType { get; set; }


        public string ExchangeCode { get; set; }


        public string CommodityCode { get; set; }


        public string ContractCode { get; set; }


        public byte BuySell { get; set; }


        public float TradeFee { get; set; }


        public string OperatorNo { get; set; }


        public string Info1 { get; set; }


        public string Info2 { get; set; }


        public string Info3 { get; set; }


        public byte OrderInsertType { get; set; }


        public string OrderFrom { get; set; }

        #endregion

        public TradeCacheDB Copy()
        {
            var m = this;

            TradeCacheDB curr = new TradeCacheDB();
            curr.MsgNum = m.MsgNum;
            curr.InitDate = m.InitDate;

            curr.AccountNo = m.AccountNo;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.OrderNo = m.OrderNo;

            curr.TradeDate = m.TradeDate;

            curr.TradeNo = m.TradeNo;

            curr.TradeTime = m.TradeTime;

            curr.TradePrice = m.TradePrice;

            curr.TradeVol = m.TradeVol;

            curr.CurrencyCode = m.CurrencyCode;

            curr.IsTplusone = m.IsTplusone;

            curr.OpenClose = m.OpenClose;

            curr.ProxyId = m.ProxyId;

            curr.BrokerTradeNo = m.BrokerTradeNo;

            curr.ExchangeTradeNo = m.ExchangeTradeNo;

            curr.ProductType = m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.BuySell = m.BuySell;

            curr.TradeFee = m.TradeFee;

            curr.OperatorNo = m.OperatorNo;

            curr.Info1 = m.Info1;

            curr.Info2 = m.Info2;

            curr.Info3 = m.Info3;

            curr.OrderInsertType = m.OrderInsertType;

            curr.OrderFrom = m.OrderFrom;

            return curr;
        }
    }

    public class PositionCacheDBDTO : EntityDto
    {
        #region Slice data members


        public int InitDate { get; set; }


        public string AccountNo { get; set; }


        public int OpenDate { get; set; }


        public long OpenNo { get; set; }


        public double OpenPrice { get; set; }


        public int OpenNum { get; set; }


        public string CurrencyCode { get; set; }


        public int ProxyId { get; set; }


        public byte ProductType { get; set; }


        public string ExchangeCode { get; set; }


        public string CommodityCode { get; set; }


        public string ContractCode { get; set; }


        public byte BuySell { get; set; }


        public int ExpiryDate { get; set; }


        public int LastTradeDate { get; set; }


        public double MarketValue { get; set; }

        #endregion

        public PositionCacheDB Copy()
        {
            var m = this;
            PositionCacheDB curr = new PositionCacheDB();
            curr.MsgNum = m.MsgNum;
            curr.InitDate = m.InitDate;

            curr.AccountNo = m.AccountNo;

            curr.OpenDate = m.OpenDate;

            curr.OpenNo = m.OpenNo;

            curr.OpenPrice = m.OpenPrice;

            curr.OpenNum = m.OpenNum;

            curr.CurrencyCode = m.CurrencyCode;

            curr.ProxyId = m.ProxyId;

            curr.ProductType = m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.BuySell = m.BuySell;

            curr.ExpiryDate = m.ExpiryDate;

            curr.LastTradeDate = m.LastTradeDate;

            curr.MarketValue = m.MarketValue;

            return curr;
        }
    }

    public class DepositCacheDBDTO : EntityDto
    {
        #region Slice data members

        
        public long MsgNum { get; set; }

        
        public int InitDate { get; set; }

        
        public int BusinessFlag { get; set; }

        
        public string BusinessNo { get; set; }

        
        public int OccurTime { get; set; }

        
        public string OperatorNo { get; set; }

        
        public string AccountNo { get; set; }

        
        public string CurrencyCode { get; set; }

        
        public double OccurBalance { get; set; }

        
        public string Remark { get; set; }

        
        public byte AuditFlag { get; set; }

        
        public int OccurDate { get; set; }

        
        public int TplusFlag { get; set; }

        #endregion

        public DepositCacheDB Copy()
        {
            var m = this;
            DepositCacheDB curr = new DepositCacheDB();
            curr.InitDate = m.InitDate;

            curr.BusinessFlag = m.BusinessFlag;

            curr.BusinessNo = m.BusinessNo;

            curr.OccurTime = m.OccurTime;

            curr.OperatorNo = m.OperatorNo;

            curr.AccountNo = m.AccountNo;

            curr.CurrencyCode = m.CurrencyCode;

            curr.OccurBalance = m.OccurBalance;

            curr.Remark = m.Remark;

            curr.AuditFlag = m.AuditFlag;

            curr.OccurDate = m.OccurDate;

            curr.TplusFlag = m.TplusFlag;

            curr.MsgNum = m.MsgNum;

            return curr;
        }
    }
}
