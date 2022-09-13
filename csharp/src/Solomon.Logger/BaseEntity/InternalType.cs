using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.CommonTools;

namespace Trade.DataType
{
    #region 基类
    public class SolomonType
    {

    }
    #endregion

    #region 下单结构
    public class SlmOrderInsert : SolomonType
    {
        private string _accountNo = string.Empty;
        private string _operatorNo = string.Empty;
        private string _orderPerson = string.Empty;
        private sbyte _productType;
        private string _exchangeCode = string.Empty;
        private string _commodityCode = string.Empty;
        private string _contractCode = string.Empty;
        private sbyte _callPut;
        private string _strikePrice;
        private sbyte _openClose;
        private sbyte _buySell;
        private Decimal _orderPrice;
        private short _orderNum;
        private sbyte _orderType;
        private sbyte _orderLife;
        private sbyte _orderInsertType;
        private sbyte _delTag;
        private string _orderFrom = string.Empty;
        private short _orderSubNumMin;
        private short _orderSubNumMax;
        private int _orderExpiryDate;
        private Decimal _triggerPrice;
        private sbyte _orderRemain;
        private string _remarkCus = string.Empty;
        private string _infoS1 = string.Empty;
        private string _infoS2 = string.Empty;
        private string _localNo = string.Empty;
        private Int64 _orderNo;
        private int _orderDate;
        private int _orderTime;
        private short _proxyId;
        private string _routerNo = string.Empty;
        private int _initDate;

        #region public
        /// <summary>
        /// 账户号码
        /// </summary>
        public string AccountNo
        {
            get { return _accountNo; }
            set { _accountNo = value; }
        }

        /// <summary>
        /// 交易员编号
        /// </summary>
        public string OperatorNo
        {
            get { return _operatorNo; }
            set { _operatorNo = value; }
        }

        /// <summary>
        /// 委托人
        /// </summary>
        public string OrderPerson
        {
            get { return _orderPerson; }
            set { _orderPerson = value; }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public sbyte ProductType
        {
            get { return _productType; }
            set { _productType = value; }
        }

        /// <summary>
        /// 市场代码
        /// </summary>
        public string ExchangeCode
        {
            get { return _exchangeCode; }
            set { _exchangeCode = value; }
        }

        /// <summary>
        /// 商品代码
        /// </summary>
        public string CommodityCode
        {
            get { return _commodityCode; }
            set { _commodityCode = value; }
        }

        /// <summary>
        /// 合约代码
        /// </summary>
        public string ContractCode
        {
            get { return _contractCode; }
            set { _contractCode = value; }
        }

        /// <summary>
        /// 看涨看跌属性
        /// </summary>
        public sbyte CallPut
        {
            get { return _callPut; }
            set { _callPut = value; }
        }

        /// <summary>
        /// 行权价
        /// </summary>
        public string StrikePrice
        {
            get { return _strikePrice; }
            set { _strikePrice = value; }
        }

        /// <summary>
        /// 开平仓
        /// </summary>
        public sbyte OpenClose
        {
            get { return _openClose; }
            set { _openClose = value; }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public sbyte BuySell
        {
            get { return _buySell; }
            set { _buySell = value; }
        }

        /// <summary>
        /// 委托价格
        /// </summary>
        public Decimal OrderPrice
        {
            get { return _orderPrice; }
            set { _orderPrice = value; }
        }

        /// <summary>
        /// 委托数量
        /// </summary>
        public short OrderNum
        {
            get { return _orderNum; }
            set { _orderNum = value; }
        }

        /// <summary>
        /// 委托类型 市价，限价等
        /// </summary>
        public sbyte OrderType
        {
            get { return _orderType; }
            set { _orderType = value; }
        }

        /// <summary>
        /// GTD,DTC,FOK,FAK等
        /// </summary>
        public sbyte OrderLife
        {
            get { return _orderLife; }
            set { _orderLife = value; }
        }

        /// <summary>
        /// 人工录入单，程序传入单
        /// </summary>
        public sbyte OrderInsertType
        {
            get { return _orderInsertType; }
            set { _orderInsertType = value; }
        }

        /// <summary>
        /// 删除标志
        /// </summary>
        public sbyte DelTag
        {
            get { return _delTag; }
            set { _delTag = value; }
        }

        /// <summary>
        /// 委托来源
        /// </summary>
        public string OrderFrom
        {
            get { return _orderFrom; }
            set { _orderFrom = value; }
        }

        /// <summary>
        /// 子委托数量(针对IceBerg)
        /// </summary>
        public short OrderSubNumMin
        {
            get { return _orderSubNumMin; }
            set { _orderSubNumMin = value; }
        }

        /// <summary>
        /// 子委托数量(针对IceBerg)
        /// </summary>
        public short OrderSubNumMax
        {
            get { return _orderSubNumMax; }
            set { _orderSubNumMax = value; }
        }

        /// <summary>
        /// 用于GTD等
        /// </summary>
        public int OrderExpiryDate
        {
            get { return _orderExpiryDate; }
            set { _orderExpiryDate = value; }
        }

        /// <summary>
        /// 止损触发价
        /// </summary>
        public Decimal TriggerPrice
        {
            get { return _triggerPrice; }
            set { _triggerPrice = value; }
        }

        /// <summary>
        /// 是否T+1
        /// </summary>
        public sbyte OrderRemain
        {
            get { return _orderRemain; }
            set { _orderRemain = value; }
        }

        /// <summary>
        /// 用户输入信息
        /// </summary>
        public string RemarkCus
        {
            get { return _remarkCus; }
            set { _remarkCus = value; }
        }

        /// <summary>
        /// 用户交易关联字段1
        /// </summary>
        public string InfoS1
        {
            get { return _infoS1; }
            set { _infoS1 = value; }
        }

        /// <summary>
        /// 用户交易关联字段2
        /// </summary>
        public string InfoS2
        {
            get { return _infoS2; }
            set { _infoS2 = value; }
        }

        /// <summary>
        /// 本地号
        /// </summary>
        public string LocalNo
        {
            get { return _localNo; }
            set { _localNo = value; }
        }

        /// <summary>
        /// 定单号
        /// </summary>
        public Int64 OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }

        /// <summary>
        /// 定单日期
        /// </summary>
        public int OrderDate
        {
            get { return _orderDate; }
            set { _orderDate = value; }
        }

        /// <summary>
        /// 定单时间
        /// </summary>
        public int OrderTime
        {
            get { return _orderTime; }
            set { _orderTime = value; }
        }

        /// <summary>
        /// 上手号
        /// </summary>
        public short ProxyId
        {
            get { return _proxyId; }
            set { _proxyId = value; }
        }

        /// <summary>
        /// 前置号
        /// </summary>
        public string RouterNo
        {
            get { return _routerNo; }
            set { _routerNo = value; }
        }

        /// <summary>
        /// 交易日期
        /// </summary>
        public int InitDate
        {
            get { return _initDate; }
            set { _initDate = value; }
        }
        #endregion

    }
    #endregion

    #region 改单结构
    public class SlmOrderModify : SolomonType
    {
        private int _orderNo;
        private Decimal _orderPrise;
        private Decimal _triggerPrice;
        private short _orderNum;

        /// <summary>
        /// 委托编号
        /// </summary>
        public int OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }

        /// <summary>
        /// 委托价格
        /// </summary>
        public Decimal OrderPrise
        {
            get { return _orderPrise; }
            set { _orderPrise = value; }
        }

        /// <summary>
        /// 触发价格
        /// </summary>
        public Decimal TriggerPrice
        {
            get { return _triggerPrice; }
            set { _triggerPrice = value; }
        }

        /// <summary>
        /// 委托数量
        /// </summary>
        public short OrderNum
        {
            get { return _orderNum; }
            set { _orderNum = value; }
        }

    }
    #endregion

    #region 撤单结构
    public class SlmOrderDelete : SolomonType
    {
        private int _orderNo;
        /// <summary>
        /// 委托编号
        /// </summary>
        public int OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }
    }
    #endregion

    #region 委托结构

    /// <summary>
    /// 用于显示相关，共用属性
    /// </summary>
    public class ShowDto
    {
        public long MsgNum { get; set; }
    }

    public class SlmOrderBind : ShowDto
    {
        private SlmOrder _order;

        public SlmOrderBind(SlmOrder order)
        {
            _order = order;
        }

        public SlmOrderBind(SlmOrder order, long msgNum)
        {
            _order = order;
            this.MsgNum = msgNum;
        }
        /// <summary>
        /// 账户号码
        /// </summary>
        public string AccountNo
        {
            get { return _order.AccountNo; }
        }

        /// <summary>
        /// 交易员编号
        /// </summary>
        public string OperatorNo
        {
            get { return _order.OperatorNo; }
        }

        /// <summary>
        /// 委托人
        /// </summary>
        public string OrderPerson
        {
            get { return _order.OrderPerson; }
        }

        /// <summary>
        /// 市场代码
        /// </summary>
        public string ExchangeCode
        {
            get { return _order.ExchangeCode; }
        }

        /// <summary>
        /// 商品代码
        /// </summary>
        public string CommodityCode
        {
            get { return _order.CommodityCode; }
        }

        /// <summary>
        /// 合约代码
        /// </summary>
        public string ContractCode
        {
            get { return _order.ContractCode; }
        }

        /// <summary>
        /// 行权价
        /// </summary>
        public string StrikePrice
        {
            get { return _order.StrikePrice; }
        }

        /// <summary>
        /// 委托价格
        /// </summary>
        public Decimal OrderPrice
        {
            get { return _order.OrderPrice; }
        }

        /// <summary>
        /// 委托数量
        /// </summary>
        public short OrderNum
        {
            get { return _order.OrderNum; }
        }

        /// <summary>
        /// 子委托数量(针对IceBerg)
        /// </summary>
        public short OrderSubNumMin
        {
            get { return _order.OrderSubNumMin; }
        }

        /// <summary>
        /// 子委托数量(针对IceBerg)
        /// </summary>
        public short OrderSubNumMax
        {
            get { return _order.OrderSubNumMax; }
        }

        /// <summary>
        /// 用于GTD等
        /// </summary>
        public int OrderExpiryDate
        {
            get { return _order.OrderExpiryDate; }
        }


        /// <summary>
        /// 止损触发价
        /// </summary>
        public Decimal TriggerPrice
        {
            get { return _order.TriggerPrice; }
        }

        /// <summary>
        /// 用户输入信息
        /// </summary>
        public string RemarkCus
        {
            get { return _order.RemarkCus; }
        }

        /// <summary>
        /// 用户交易关联字段1
        /// </summary>
        public string InfoS1
        {
            get { return _order.InfoS1; }
        }

        /// <summary>
        /// 用户交易关联字段2
        /// </summary>
        public string InfoS2
        {
            get { return _order.InfoS2; }
        }

        /// <summary>
        /// 本地号
        /// </summary>
        public string LocalNo
        {
            get { return _order.LocalNo; }
        }

        /// <summary>
        /// 定单号
        /// </summary>
        public Int64 OrderNo
        {
            get { return _order.OrderNo; }
        }

        /// <summary>
        /// 定单日期
        /// </summary>
        public int OrderDate
        {
            get { return _order.OrderDate; }
        }

        /// <summary>
        /// 定单时间
        /// </summary>
        public int OrderTime
        {
            get { return _order.OrderTime; }
        }

        /// <summary>
        /// 前置号
        /// </summary>
        public string RouterNo
        {
            get { return _order.RouterNo; }
        }

        /// <summary>
        /// 交易日期
        /// </summary>
        public int InitDate
        {
            get { return _order.InitDate; }
        }

        /// <summary>
        /// 最后一次修改委托的人
        /// </summary>
        public string UpdatePerson
        {
            get { return _order.UpdatePerson; }
        }

        /// <summary>
        /// 上手代码
        /// </summary>
        //public short BrokerNo
        //{
        //    get { return _order.BrokerNo; }
        //}

        /// <summary>
        /// 已成交数量
        /// </summary>
        public short FilledNum
        {
            get { return _order.FilledNum; }
        }

        /// <summary>
        /// 订单修改日期（本地日期）
        /// </summary>
        public int UpdateDate
        {
            get { return _order.UpdateDate; }
        }

        /// <summary>
        /// 订单修改时间（本地时间）
        /// </summary>
        public int UpdateTime
        {
            get { return _order.UpdateTime; }
        }

        /// <summary>
        /// 交易所日期
        /// </summary>
        public int ExchangeDate
        {
            get { return _order.ExchangeDate; }
        }

        /// <summary>
        /// 交易所时间
        /// </summary>
        public int ExchangeTime
        {
            get { return _order.ExchangeTime; }
        }

        /// <summary>
        /// 内部委托最后流水号
        /// </summary>
        public short OrderJourNo
        {
            get { return _order.OrderJourNo; }
        }

        /// <summary>
        /// 上级委托编号
        /// </summary>
        public string BrokerOrderNo
        {
            get { return _order.BrokerOrderNo; }
        }

        /// <summary>
        /// 上级委托最后流水号
        /// </summary>
        public string BrokerOrderJourNo
        {
            get { return _order.BrokerOrderJourNo; }
        }

        /// <summary>
        /// 交易所编号
        /// </summary>
        public string ExchangeOrderNo
        {
            get { return _order.ExchangeOrderNo; }
        }

        /// <summary>
        /// 用于LME Carry录入后系统自动配对，carryNo由系统自动生成
        /// </summary>
        public string CarryNo
        {
            get { return _order.CarryNo; }
        }

        /// <summary>
        /// 错误编码
        /// </summary>
        public short ErrorNo
        {
            get { return _order.ErrorNo; }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo
        {
            get { return _order.ErrorInfo; }
        }

        /// <summary>
        /// 上级代码
        /// </summary>
        public short ProxyId
        {
            get { return _order.ProxyId; }
        }

        /// <summary>
        /// 平仓数量
        /// </summary>
        public int CloseNum
        {
            get { return _order.CloseNum; }
        }

        /// <summary>
        /// 开仓数量
        /// </summary>
        public int OpenNum
        {
            get { return _order.OpenNum; }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public string ProductTypeBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.ProductType == DictionaryHelper.COMMODITY_TYPE.FUTURE)
                {
                    s = "期货";
                }
                else if (_order.ProductType == DictionaryHelper.COMMODITY_TYPE.SPEARD)
                {
                    s = "价差";
                }
                else if (_order.ProductType == DictionaryHelper.COMMODITY_TYPE.OPTION)
                {
                    s = "期权";
                }
                return s;
            }
        }

        /// <summary>
        /// 看涨看跌属性
        /// </summary>
        public string CallPutBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.CallPut == 0)
                {
                    s = "涨";
                }
                else if (_order.CallPut == 1)
                {
                    s = "跌";
                }
                return s;
            }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public string BuySellBinding
        {
            get
            {
                return _order.BuySell == 0 ? "买入" : "卖出";
            }
        }

        /// <summary>
        /// 委托类型 市价，限价等
        /// </summary>
        public string OrderTypeBinding
        {
            get
            {
                char c = Convert.ToChar(_order.OrderType);
                string s = string.Empty;
                if (c == 0)
                {
                    s = "限价";
                }
                else if (c == 1)
                {
                    s = "市价";
                }
                else if (c == 2)
                {
                    s = "止损";
                }
                else if (c == 3)
                {
                    s = "限价止损";
                }
                else if (c == 4)
                {
                    s = "止盈止损";
                }
                return s;
            }
        }

        /// <summary>
        /// 有效时间
        /// </summary>
        public string OrderLifeBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.OrderLife == 0)
                {
                    s = "当日有效GFD";
                }
                else if (_order.OrderLife == 1)
                {
                    s = "长期有效GTC";
                }
                else if (_order.OrderLife == 2)
                {
                    s = "FOK";
                }
                else if (_order.OrderLife == 3)
                {
                    s = "FAK";
                }
                return s;
            }
        }

        /// <summary>
        /// 委托状态
        /// </summary>
        public string OrderStateBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_SENDED)
                {
                    s = "已发出";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_RECEIVED)
                {
                    s = "已接收";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_RECEPT)
                {
                    s = "已受理";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_ACTIVED)
                {
                    s = "已报入";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_PART_FILLED)
                {
                    s = "部分成交";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_ALL_FILLED)
                {
                    s = "完全成交";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_WAIT_UPDATE)
                {
                    s = "待改";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_WAIT_DEL)
                {
                    s = "待撤";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_DELED)
                {
                    s = "已撤单";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_FAILED)
                {
                    s = "委托失败";
                }
                else if (_order.OrderState == DictionaryHelper.ORDER_STATUS.ORDER_PART_DELETE)
                {
                    s = "部分撤单";
                }
                return s;
            }
        }

        /// <summary>
        /// 开平仓
        /// </summary>
        public string OpenCloseBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.OpenClose == 0)
                {
                    s = "开仓";
                }
                else if (_order.OpenClose == 1)
                {
                    s = "平仓";
                }
                else if (_order.OpenClose == 2)
                {
                    s = "开平仓";
                }
                return s;
            }
        }

        /// <summary>
        /// T+1
        /// </summary>
        public string OrderRemainBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.OrderRemain == 0)
                {
                    s = "否";
                }
                else if (_order.OrderRemain == 1)
                {
                    s = "是";
                }
                return s;
            }
        }

        /// <summary>
        /// 删除标记
        /// </summary>
        public string DelTagBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.DelTag == 0)
                {
                    s = "否";
                }
                else if (_order.DelTag == 1)
                {
                    s = "是";
                }
                return s;
            }
        }

        /// <summary>
        /// 委托方式
        /// </summary>
        public string OrderInsertTypeBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.OrderInsertType == 0)
                {
                    s = "程序单";
                }
                else if (_order.OrderInsertType == 1)
                {
                    s = "人工录入单";
                }
                else if (_order.OrderInsertType == 2)
                {
                    s = "CD录单";
                }
                return s;
            }
        }

        /// <summary>
        /// 委托来源
        /// </summary>
        public string OrderFromBinding
        {
            get
            {
                string s = string.Empty;
                if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.CLIENT_IN)
                {
                    s = "客户端";
                }
                //else if (_orderFrom == DictionaryHelper.ORDER_FROM.MANU_IN.ToString())
                //{
                //    s = "人工";
                //}
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.API_IN)
                {
                    s = "API";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.RISK_IN)
                {
                    s = "风控";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.IOS_IN)
                {
                    s = "IOS";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.ANDROID_IN)
                {
                    s = "Android";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.COUNTER_IN)
                {
                    s = "柜台录单";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.MANUAL_CLOSE)
                {
                    s = "人工强平";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.MANUAL_STATUS_MODIFY)
                {
                    s = "订单状态修改";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.FtradeAuto)
                {
                    s = "自动休市撤单";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.Modify)
                {
                    s = "修复";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.ForceCloseModify)
                {
                    s = "强平修复";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.ClientModify)
                {
                    s = "客户端下单修复";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.ManuInSelf)
                {
                    s = "录单给自营";
                }
                else if (_order.OrderFrom == DictionaryHelper.ORDER_FROM.OrderInSelf)
                {
                    s = "下单给自营";
                }
                return s;
            }
        }

    }

    #endregion

    #region 成交结构
    public class SlmMatch : SolomonType
    {
        private int _init_date;
        private string _account_no = string.Empty;
        private int _order_date;
        private int _order_time;
        private Int64 _order_no;
        private int _trade_date;
        private Int64 _trade_no;
        private int _trade_time;
        private Decimal _trade_price;
        private int _trade_vol;
        private string _currency_code = string.Empty;
        private sbyte _is_tplusone;
        private sbyte _open_close;
        private short _proxy_id;
        private string _broker_trade_no = string.Empty;
        private string _exchange_trade_no = string.Empty;
        private sbyte _product_type;
        private string _exchange_code = string.Empty;
        private string _commodity_code = string.Empty;
        private string _contract_code = string.Empty;
        private sbyte _buy_sell;
        private Decimal _trade_fee;
        private string _operator_no = string.Empty;
        private string _info1 = string.Empty;
        private string _info2 = string.Empty;
        private string _info3 = string.Empty;
        private sbyte _order_insert_type;
        private string _order_from = string.Empty;

        private long _msgNum;////此字段必须在最末尾，加字段在前面插入

        private string[] _arrayData = new string[29];

        public int InitDate
        {
            get
            {
                return _init_date;
            }
            set
            {
                _init_date = value;
                //_arrayData[0] = value.ToString();
            }
        }

        public string AccountNo
        {
            get
            {
                return _account_no;
            }
            set
            {
                _account_no = value;
                //_arrayData[1] = value;
            }
        }


        public int OrderDate
        {
            get
            {
                return _order_date;
            }
            set
            {
                _order_date = value;
                //_arrayData[2] = value.ToString();
            }
        }

        public int OrderTime
        {
            get
            {
                return _order_time;
            }
            set
            {
                _order_time = value;
                //_arrayData[3] = value.ToString();
            }
        }

        public Int64 OrderNo
        {
            get
            {
                return _order_no;
            }
            set
            {
                _order_no = value;
                //_arrayData[4] = value.ToString();
            }
        }

        public int TradeDate
        {
            get
            {
                return _trade_date;
            }
            set
            {
                _trade_date = value;
                //_arrayData[5] = value.ToString();
            }
        }

        public Int64 TradeNo
        {
            get
            {
                return _trade_no;
            }
            set
            {
                _trade_no = value;
                //_arrayData[6] = value.ToString();
            }
        }

        public int TradeTime
        {
            get
            {
                return _trade_time;
            }
            set
            {
                _trade_time = value;
                //_arrayData[7] = value.ToString();
            }
        }

        public Decimal TradePrice
        {
            get
            {
                return _trade_price;
            }
            set
            {
                _trade_price = value;
                //_arrayData[8] = value.ToString();
            }
        }

        public int TradeVol
        {
            get
            {
                return _trade_vol;
            }
            set
            {
                _trade_vol = value;
                //_arrayData[9] = value.ToString();
            }
        }

        public string CurrencyCode
        {
            get
            {
                return _currency_code;
            }
            set
            {
                _currency_code = value;
                //_arrayData[10] = value;
            }
        }
        public sbyte IsTplusone
        {
            get
            {
                return _is_tplusone;
            }
            set
            {
                _is_tplusone = value;
                //_arrayData[11] = value.ToString();
            }
        }

        public sbyte OpenClose
        {
            get
            {
                return _open_close;
            }
            set
            {
                _open_close = value;
                //_arrayData[12] = value.ToString();
            }
        }

        public short ProxyId
        {
            get
            {
                return _proxy_id;
            }
            set
            {
                _proxy_id = value;
                //_arrayData[13] = value.ToString();
            }
        }

        public string BrokerTradeNo
        {
            get
            {
                return _broker_trade_no;
            }
            set
            {
                _broker_trade_no = value;
                //_arrayData[14] = value;
            }
        }

        public string ExchangeTradeNo
        {
            get
            {
                return _exchange_trade_no;
            }
            set
            {
                _exchange_trade_no = value;
                //_arrayData[15] = value;
            }
        }

        public sbyte ProductType
        {
            get
            {
                return _product_type;
            }
            set
            {
                _product_type = value;
                //_arrayData[16] = value.ToString();
            }
        }

        public string ExchangeCode
        {
            get
            {
                return _exchange_code;
            }
            set
            {
                _exchange_code = value;
                //_arrayData[17] = value;
            }
        }

        public string CommodityCode
        {
            get
            {
                return _commodity_code;
            }
            set
            {
                _commodity_code = value;
                //_arrayData[18] = value;
            }
        }

        public string ContractCode
        {
            get
            {
                return _contract_code;
            }
            set
            {
                _contract_code = value;
                //_arrayData[19] = value;
            }
        }

        public sbyte BuySell
        {
            get
            {
                return _buy_sell;
            }
            set
            {
                _buy_sell = value;
                //_arrayData[20] = value.ToString();
            }
        }

        public Decimal TradeFee
        {
            get
            {
                return _trade_fee;
            }
            set
            {
                _trade_fee = value;
                //_arrayData[21] = value.ToString();
            }
        }

        public string OperatorNo
        {
            get { return _operator_no; }
            set
            {
                _operator_no = value;
                //_arrayData[22] = value;
            }
        }

        public string Info1
        {
            get { return _info1; }
            set
            {
                _info1 = value;
                //_arrayData[23] = value;
            }
        }

        public string Info2
        {
            get { return _info2; }
            set
            {
                _info2 = value;
                //_arrayData[24] = value;
            }
        }

        public string Info3
        {
            get { return _info3; }
            set
            {
                _info3 = value;
                //_arrayData[25] = value;
            }
        }

        public sbyte OrderInsertType
        {
            get { return _order_insert_type; }
            set
            {
                _order_insert_type = value;
                //_arrayData[26] = value.ToString();
            }
        }

        public string OrderFrom
        {
            get { return _order_from; }
            set
            {
                _order_from = value;
                //_arrayData[27] = value;
            }
        }

        public long MsgNum
        {
            get { return _msgNum; }
            set
            {
                _msgNum = value;
            }
        }

        public string[] GetDataArray()
        {
            _arrayData[0] = _init_date.ToString();
            _arrayData[1] = _account_no;
            _arrayData[2] = _order_date.ToString();
            _arrayData[3] = _order_time.ToString();
            _arrayData[4] = _order_no.ToString();
            _arrayData[5] = _trade_date.ToString();
            _arrayData[6] = _trade_no.ToString();
            _arrayData[7] = _trade_time.ToString();
            _arrayData[8] = _trade_price.ToString();
            _arrayData[9] = _trade_vol.ToString();
            _arrayData[10] = _currency_code;
            _arrayData[11] = _is_tplusone.ToString();
            _arrayData[12] = _open_close.ToString();
            _arrayData[13] = _proxy_id.ToString();
            _arrayData[14] = _broker_trade_no;
            _arrayData[15] = _exchange_trade_no;
            _arrayData[16] = _product_type.ToString();
            _arrayData[17] = _exchange_code;
            _arrayData[18] = _commodity_code;
            _arrayData[19] = _contract_code;
            _arrayData[20] = _buy_sell.ToString();
            _arrayData[21] = _trade_fee.ToString();
            _arrayData[22] = _operator_no;
            _arrayData[23] = _info1;
            _arrayData[24] = _info2;
            _arrayData[25] = _info3;
            _arrayData[26] = _order_insert_type.ToString();
            _arrayData[27] = _order_from;
            _arrayData[28] = _msgNum.ToString();

            return _arrayData;
        }

        public bool IsNotHKFEAndSGX()
        {
            if (ExchangeCode != DictionaryHelper.Exchange.HKFE && ExchangeCode != DictionaryHelper.Exchange.SGX)
            {
                return true;
            }
            return false;
        }

        public bool IsHKFEAndSGX()
        {
            if (ExchangeCode == DictionaryHelper.Exchange.HKFE || ExchangeCode == DictionaryHelper.Exchange.SGX)
            {
                return true;
            }
            return false;
        }
        public SlmMatch GetSlmMatchCopy()
        {
            var result = new SlmMatch();
            result.InitDate = this.InitDate;
            result.AccountNo = this.AccountNo;
            result.OrderDate = this.OrderDate;
            result.OrderTime = this.OrderTime;
            result.OrderNo = this.OrderNo;
            result.TradeDate = this.TradeDate;
            result.TradeNo = this.TradeNo;
            result.TradeTime = this.TradeTime;
            result.TradePrice = this.TradePrice;
            result.TradeVol = this.TradeVol;
            result.CurrencyCode = this.CurrencyCode;
            result.IsTplusone = this.IsTplusone;
            result.OpenClose = this.OpenClose;
            result.ProxyId = this.ProxyId;
            result.BrokerTradeNo = this.BrokerTradeNo;
            result.ExchangeTradeNo = this.ExchangeTradeNo;
            result.ProductType = this.ProductType;
            result.ExchangeCode = this.ExchangeCode;
            result.CommodityCode = this.CommodityCode;
            result.ContractCode = this.ContractCode;
            result.BuySell = this.BuySell;
            result.TradeFee = this.TradeFee;
            result.OperatorNo = this.OperatorNo;
            result.Info1 = this.Info1;
            result.Info2 = this.Info2;
            result.Info3 = this.Info3;
            result.OrderInsertType = this.OrderInsertType;
            result.OrderFrom = this.OrderFrom;
            return result;
        }

        public SlmMatch Clone()
        {
            return (SlmMatch)this.MemberwiseClone();

        }
    }

    /// <summary>
    /// 成交-绑定用
    /// </summary>
    public class SlmMatchBind : ShowDto
    {
        private SlmMatch _match;

        public SlmMatchBind(SlmMatch match)
        {
            _match = match;
        }
        public SlmMatchBind(SlmMatch match, long msgNum)
        {
            _match = match;
            this.MsgNum = msgNum;
        }
        public int InitDate
        {
            get
            {
                return _match.InitDate;
            }
        }

        public string AccountNo
        {
            get
            {
                return _match.AccountNo;
            }
        }


        public int OrderDate
        {
            get
            {
                return _match.OrderDate;
            }
        }

        public int OrderTime
        {
            get
            {
                return _match.OrderTime;
            }
        }

        public Int64 OrderNo
        {
            get
            {
                return _match.OrderNo;
            }
        }

        public int TradeDate
        {
            get
            {
                return _match.TradeDate;
            }
        }

        public Int64 TradeNo
        {
            get
            {
                return _match.TradeNo;
            }
        }

        public int TradeTime
        {
            get
            {
                return _match.TradeTime;
            }
        }

        public Decimal TradePrice
        {
            get
            {
                return _match.TradePrice;
            }
        }

        public int TradeVol
        {
            get
            {
                return _match.TradeVol;
            }
        }

        public string CurrencyCode
        {
            get
            {
                return _match.CurrencyCode;
            }
        }

        public short ProxyId
        {
            get
            {
                return _match.ProxyId;
            }
        }

        public string BrokerTradeNo
        {
            get
            {
                return _match.BrokerTradeNo;
            }
        }

        public string ExchangeTradeNo
        {
            get
            {
                return _match.ExchangeTradeNo;
            }
        }

        public string ExchangeCode
        {
            get
            {
                return _match.ExchangeCode;
            }
        }

        public string CommodityCode
        {
            get
            {
                return _match.CommodityCode;
            }
        }

        public string ContractCode
        {
            get
            {
                return _match.ContractCode;
            }
        }

        public Decimal TradeFee
        {
            get
            {
                return _match.TradeFee;
            }
        }

        public string OperatorNo
        {
            get { return _match.OperatorNo; }
        }

        public string Info1
        {
            get { return _match.Info1; }
        }

        public string Info2
        {
            get { return _match.Info2; }
        }

        public string Info3
        {
            get { return _match.Info3; }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public string BuySellBinding
        {
            get
            {
                return _match.BuySell == DictionaryHelper.ORDER_DERECTION.BUY ? "买入" : "卖出";
            }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public string ProductTypeBinding
        {
            get
            {
                string s = string.Empty;
                if (_match.ProductType == DictionaryHelper.COMMODITY_TYPE.FUTURE)
                {
                    s = "期货";
                }
                else if (_match.ProductType == DictionaryHelper.COMMODITY_TYPE.SPEARD)
                {
                    s = "价差";
                }
                else if (_match.ProductType == DictionaryHelper.COMMODITY_TYPE.OPTION)
                {
                    s = "期权";
                }
                return s;
            }
        }

        /// <summary>
        /// T+1
        /// </summary>
        public string OrderRemainBinding
        {
            get
            {
                string s = string.Empty;
                if (_match.IsTplusone == 0)
                {
                    s = "否";
                }
                else if (_match.IsTplusone == 1)
                {
                    s = "是";
                }
                return s;
            }
        }

        /// <summary>
        /// 开平仓
        /// </summary>
        public string OpenCloseBinding
        {
            get
            {
                string s = string.Empty;
                if (_match.OpenClose == 0)
                {
                    s = "开仓";
                }
                else if (_match.OpenClose == 1)
                {
                    s = "平仓";
                }
                else if (_match.OpenClose == 2)
                {
                    s = "开平仓";
                }
                return s;
            }
        }
    }

    public class SlmMatchSms
    {
        public int Init_date { get; set; }
        public string Account_no { get; set; }
        public int Order_date { get; set; }
        public int Order_time { get; set; }
        public int Order_no { get; set; }
        public int Trade_date { get; set; }
        public int Trade_no { get; set; }
        public int Trade_time { get; set; }
        public Decimal Trade_price { get; set; }
        public int Trade_vol { get; set; }
        public string Currency_code { get; set; }
        public sbyte Is_tplusone { get; set; }
        public sbyte Open_close { get; set; }
        public short Proxy_id { get; set; }
        public string Broker_trade_no { get; set; }
        public string Exchange_trade_no { get; set; }
        public sbyte Product_type { get; set; }
        public string Exchange_code { get; set; }
        public string Commodity_code { get; set; }
        public string Contract_code { get; set; }
        public sbyte Buy_sell { get; set; }
        public Decimal Trade_fee { get; set; }
        public string Operator_no { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }
        public sbyte Order_insert_type { get; set; }
        public string Order_from { get; set; }
        public string Sms_type { get; set; }
        public string Send_Status { get; set; }
    }

    #endregion

    #region 持仓结构
    public class Position
    {
        public string init_date { get; set; }
        public string account_no { get; set; }
        public string open_date { get; set; }
        public string open_no { get; set; }
        public string open_price { get; set; }
        public string open_num { get; set; }
        public string currency_code { get; set; }
        public string proxy_id { get; set; }
        public string product_type { get; set; }
        public string exchange_code { get; set; }
        public string commodity_code { get; set; }
        public string contract_code { get; set; }
        public string buy_sell { get; set; }
        public string expiry_date { get; set; }
        public string last_trade_date { get; set; }
    }

    public class SlmPosition : SolomonType
    {
        private int _initData;
        private string _accountNo = string.Empty;
        private int _openDate;
        private Int64 _openNo;
        private Decimal _openPrice;
        private int _openNum;
        private string _currencyCode = string.Empty;
        private short _proxyId;
        private sbyte _productType;
        private string _exchangeCode = string.Empty;
        private string _commodityCode = string.Empty;
        private string _contractCode = string.Empty;
        private sbyte _buySell;
        private int _expiryDate;
        private int _lastTradeDate;
        private Decimal _marketValue;

        //以下两字段刷持仓时发送给前置使用，不存入数据库 ljz 2021.01.05
        private decimal _float;
        private decimal _margin;

        private string[] _arrayData = new string[18];

        public int InitDate
        {
            get { return _initData; }
            set
            {
                _initData = value;
                //_arrayData[0] = value.ToString();
            }
        }

        public string AccountNo
        {
            get { return _accountNo; }
            set
            {
                _accountNo = value;
                //_arrayData[1] = value;
            }
        }

        public int OpenDate
        {
            get { return _openDate; }
            set
            {
                _openDate = value;
                //_arrayData[2] = value.ToString();
            }
        }

        public Int64 OpenNo
        {
            get { return _openNo; }
            set
            {
                _openNo = value;
                //_arrayData[3] = value.ToString();
            }
        }

        public Decimal OpenPrice
        {
            get { return _openPrice; }
            set
            {
                _openPrice = value;
                //_arrayData[4] = value.ToString();
            }
        }

        public int OpenNum
        {
            get { return _openNum; }
            set
            {
                _openNum = value;
                //_arrayData[5] = value.ToString();
            }
        }

        public string CurrencyCode
        {
            get { return _currencyCode; }
            set
            {
                _currencyCode = value;
                //_arrayData[6] = value;
            }
        }

        public short ProxyId
        {
            get { return _proxyId; }
            set
            {
                _proxyId = value;
                //_arrayData[7] = value.ToString();
            }
        }

        public sbyte ProductType
        {
            get { return _productType; }
            set
            {
                _productType = value;
                //_arrayData[8] = value.ToString();
            }
        }

        public string ExchangeCode
        {
            get { return _exchangeCode; }
            set
            {
                _exchangeCode = value;
                //_arrayData[9] = value;
            }
        }

        public string CommodityCode
        {
            get { return _commodityCode; }
            set
            {
                _commodityCode = value;
                //_arrayData[10] = value;
            }
        }

        public string ContractCode
        {
            get { return _contractCode; }
            set
            {
                _contractCode = value;
                //_arrayData[11] = value;
            }
        }

        public sbyte BuySell
        {
            get { return _buySell; }
            set
            {
                _buySell = value;
                //_arrayData[12] = value.ToString();
            }
        }

        public int ExpiryDate
        {
            get { return _expiryDate; }
            set
            {
                _expiryDate = value;
                //_arrayData[13] = value.ToString();
            }
        }

        public int LastTradeDate
        {
            get { return _lastTradeDate; }
            set
            {
                _lastTradeDate = value;
                //_arrayData[14] = value.ToString();
            }
        }

        public Decimal MarketValue
        {
            get { return _marketValue; }
            set
            {
                _marketValue = value;
                //_arrayData[15] = value.ToString();
            }
        }

        public Decimal GetFloatValue()
        {
            return _float;
        }

        public void SetFloatValue(Decimal value)
        {
            _float = value;
        }

        public Decimal GetMargin()
        {
            return _margin;
        }

        public void SetMargin(Decimal value)
        {
            _margin = value;
        }

        public string[] GetDataArray()
        {
            _arrayData[0] = _initData.ToString();
            _arrayData[1] = _accountNo;
            _arrayData[2] = _openDate.ToString();
            _arrayData[3] = _openNo.ToString();
            _arrayData[4] = _openPrice.ToString();
            _arrayData[5] = _openNum.ToString();
            _arrayData[6] = _currencyCode;
            _arrayData[7] = _proxyId.ToString();
            _arrayData[8] = _productType.ToString();
            _arrayData[9] = _exchangeCode;
            _arrayData[10] = _commodityCode;
            _arrayData[11] = _contractCode;
            _arrayData[12] = _buySell.ToString();
            _arrayData[13] = _expiryDate.ToString();
            _arrayData[14] = _lastTradeDate.ToString();
            _arrayData[15] = _marketValue.ToString();

            _arrayData[16] = _float.ToString();
            _arrayData[17] = _margin.ToString();
            return _arrayData;
        }

        public SlmPosition GetCopy()
        {
            SlmPosition copyData = new SlmPosition();
            copyData.AccountNo = _accountNo;
            copyData.BuySell = _buySell;
            copyData.CommodityCode = _commodityCode;
            copyData.ContractCode = _contractCode;
            copyData.CurrencyCode = _currencyCode;
            copyData.ExchangeCode = _exchangeCode;
            copyData.ExpiryDate = _expiryDate;
            copyData.InitDate = _initData;
            copyData.LastTradeDate = _lastTradeDate;
            copyData.MarketValue = _marketValue;
            copyData.OpenDate = _openDate;
            copyData.OpenNo = _openNo;
            copyData.OpenNum = _openNum;
            copyData.OpenPrice = _openPrice;
            copyData.ProductType = _productType;
            copyData.ProxyId = _proxyId;
            copyData.SetFloatValue(_float);
            copyData.SetMargin(_margin);
            return copyData;
        }
    }

    public class SlmPositionRisk
    {
        private SlmPosition _pos;
        private decimal _float;
        private decimal _margin;

        public SlmPositionRisk(SlmPosition pos)
        {
            _pos = pos;
        }

        public int InitDate
        {
            get { return _pos.InitDate; }
        }

        public string AccountNo
        {
            get { return _pos.AccountNo; }
        }

        public int OpenDate
        {
            get { return _pos.OpenDate; }
        }

        public Int64 OpenNo
        {
            get { return _pos.OpenNo; }
        }

        public Decimal OpenPrice
        {
            get { return _pos.OpenPrice; }
        }

        public int OpenNum
        {
            get { return _pos.OpenNum; }
        }

        public string CurrencyCode
        {
            get { return _pos.CurrencyCode; }
        }

        public short ProxyId
        {
            get { return _pos.ProxyId; }
        }

        public sbyte ProductType
        {
            get { return _pos.ProductType; }
        }

        public string ExchangeCode
        {
            get { return _pos.ExchangeCode; }
        }

        public string CommodityCode
        {
            get { return _pos.CommodityCode; }
        }

        public string ContractCode
        {
            get { return _pos.ContractCode; }
        }

        public sbyte BuySell
        {
            get { return _pos.BuySell; }
        }

        public int ExpiryDate
        {
            get { return _pos.ExpiryDate; }
        }

        public int LastTradeDate
        {
            get { return _pos.LastTradeDate; }
        }

        public Decimal MarketValue
        {
            get { return _pos.MarketValue; }
        }

        public Decimal FloatValue
        {
            set { _float = value; }
            get { return _float; }
        }

        public Decimal Margin
        {
            set { _margin = value; }
            get { return _margin; }
        }
        private string[] _arrayData = new string[18];
        public string[] GetDataArray()
        {
            _arrayData[0] = _pos.InitDate.ToString();
            _arrayData[1] = _pos.AccountNo;
            _arrayData[2] = _pos.OpenDate.ToString();
            _arrayData[3] = _pos.OpenNo.ToString();
            _arrayData[4] = _pos.OpenPrice.ToString();
            _arrayData[5] = _pos.OpenNum.ToString();
            _arrayData[6] = _pos.CurrencyCode;
            _arrayData[7] = _pos.ProxyId.ToString();
            _arrayData[8] = _pos.ProductType.ToString();
            _arrayData[9] = _pos.ExchangeCode;
            _arrayData[10] = _pos.CommodityCode;
            _arrayData[11] = _pos.ContractCode;
            _arrayData[12] = _pos.BuySell.ToString();
            _arrayData[13] = _pos.ExpiryDate.ToString();
            _arrayData[14] = _pos.LastTradeDate.ToString();
            _arrayData[15] = _pos.MarketValue.ToString();
            _arrayData[16] = _float.ToString();
            _arrayData[17] = _margin.ToString();

            return _arrayData;
        }
    }

    public class SlmPositionBind : ShowDto
    {
        private SlmPosition _pos;

        public SlmPositionBind(SlmPosition pos)
        {
            _pos = pos;
        }

        public SlmPositionBind(SlmPosition pos, long msgnum)
        {
            _pos = pos;
            this.MsgNum = msgnum;
        }

        public int InitDate
        {
            get { return _pos.InitDate; }
        }

        public string AccountNo
        {
            get { return _pos.AccountNo; }
        }

        public int OpenDate
        {
            get { return _pos.OpenDate; }
        }

        public Int64 OpenNo
        {
            get { return _pos.OpenNo; }
        }

        public Decimal OpenPrice
        {
            get { return _pos.OpenPrice; }
        }

        public int OpenNum
        {
            get { return _pos.OpenNum; }
        }

        public string CurrencyCode
        {
            get { return _pos.CurrencyCode; }
        }

        public short ProxyId
        {
            get { return _pos.ProxyId; }
        }

        public string ExchangeCode
        {
            get { return _pos.ExchangeCode; }
        }

        public string CommodityCode
        {
            get { return _pos.CommodityCode; }
        }

        public string ContractCode
        {
            get { return _pos.ContractCode; }
        }

        public int ExpiryDate
        {
            get { return _pos.ExpiryDate; }
        }

        public int LastTradeDate
        {
            get { return _pos.LastTradeDate; }
        }

        public Decimal MarketValue
        {
            get { return _pos.MarketValue; }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public string ProductTypeBinding
        {
            get
            {
                string s = string.Empty;
                if (_pos.ProductType == DictionaryHelper.COMMODITY_TYPE.FUTURE)
                {
                    s = "期货";
                }
                else if (_pos.ProductType == DictionaryHelper.COMMODITY_TYPE.SPEARD)
                {
                    s = "价差";
                }
                else if (_pos.ProductType == DictionaryHelper.COMMODITY_TYPE.OPTION)
                {
                    s = "期权";
                }
                return s;
            }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public string BuySellBinding
        {
            get
            {
                sbyte c = Convert.ToSByte(_pos.BuySell);
                return c == 0 ? "买入" : "卖出";
            }
        }
    }
    #endregion

    #region 平仓结构
    public class SlmClose : SolomonType
    {
        private int _init_date;
        private string _account_no;
        private int _open_date;
        private Int64 _open_no;
        private Decimal _open_price;
        private int _close_date;
        private Int64 _close_no;
        private Decimal _close_price;
        private int _open_num;
        private int _close_num;
        private string _currency_code;
        private int _open_proxy_id;
        private int _close_proxy_id;
        private sbyte _product_type;
        private string _exchange_code;
        private string _commodity_code;
        private string _contract_code;
        private sbyte _buy_sell;
        private Decimal _profit_loss;
        private string[] _arrayData = new string[19];


        public int InitDate
        {
            get
            {
                return _init_date;
            }
            set
            {
                _init_date = value;
                //_arrayData[0] = value.ToString();
            }
        }

        public string AccountNo
        {
            get
            {
                return _account_no;
            }
            set
            {
                _account_no = value;
                //_arrayData[1] = value;
            }
        }

        public int OpenDate
        {
            get
            {
                return _open_date;
            }
            set
            {
                _open_date = value;
                //_arrayData[2] = value.ToString();
            }
        }

        public Int64 OpenNo
        {
            get
            {
                return _open_no;
            }
            set
            {
                _open_no = value;
                //_arrayData[3] = value.ToString();
            }
        }

        public Decimal OpenPrice
        {
            get
            {
                return _open_price;
            }
            set
            {
                _open_price = value;
                //_arrayData[4] = value.ToString();
            }
        }

        public int CloseDate
        {
            get
            {
                return _close_date;
            }
            set
            {
                _close_date = value;
                //_arrayData[5] = value.ToString();
            }
        }

        public Int64 CloseNo
        {
            get
            {
                return _close_no;
            }
            set
            {
                _close_no = value;
                //_arrayData[6] = value.ToString();
            }
        }

        public Decimal ClosePrice
        {
            get
            {
                return _close_price;
            }
            set
            {
                _close_price = value;
                //_arrayData[7] = value.ToString();
            }
        }

        public int OpenNum
        {
            get
            {
                return _open_num;
            }
            set
            {
                _open_num = value;
                //_arrayData[8] = value.ToString();
            }
        }

        public int CloseNum
        {
            get
            {
                return _close_num;
            }
            set
            {
                _close_num = value;
                //_arrayData[9] = value.ToString();
            }
        }

        public string CurrencyCode
        {
            get
            {
                return _currency_code;
            }
            set
            {
                _currency_code = value;
                //_arrayData[10] = value;
            }
        }

        public int OpenProxyId
        {
            get
            {
                return _open_proxy_id;
            }
            set
            {
                _open_proxy_id = value;
                //_arrayData[11] = value.ToString();
            }
        }

        public int CloseProxyId
        {
            get
            {
                return _close_proxy_id;
            }
            set
            {
                _close_proxy_id = value;
                //_arrayData[12] = value.ToString();
            }
        }

        public sbyte ProductType
        {
            get
            {
                return _product_type;
            }
            set
            {
                _product_type = value;
                //_arrayData[13] = value.ToString();
            }
        }

        public string ExchangeCode
        {
            get
            {
                return _exchange_code;
            }
            set
            {
                _exchange_code = value;
                //_arrayData[14] = value;
            }
        }

        public string CommodityCode
        {
            get
            {
                return _commodity_code;
            }
            set
            {
                _commodity_code = value;
                //_arrayData[15] = value;
            }
        }


        public string ContractCode
        {
            get
            {
                return _contract_code;
            }
            set
            {
                _contract_code = value;
                //_arrayData[16] = value;
            }
        }

        public sbyte BuySell
        {
            get
            {
                return _buy_sell;
            }
            set
            {
                _buy_sell = value;
                //_arrayData[17] = value.ToString();
            }
        }

        public Decimal ProfitLoss
        {
            get
            {
                return _profit_loss;
            }
            set
            {
                _profit_loss = value;
                //_arrayData[18] = value.ToString();
            }
        }

        public string[] GetDataArray()
        {
            _arrayData[0] = _init_date.ToString();
            _arrayData[1] = _account_no;
            _arrayData[2] = _open_date.ToString();
            _arrayData[3] = _open_no.ToString();
            _arrayData[4] = _open_price.ToString();
            _arrayData[5] = _close_date.ToString();
            _arrayData[6] = _close_no.ToString();
            _arrayData[7] = _close_price.ToString();
            _arrayData[8] = _open_num.ToString();
            _arrayData[9] = _close_num.ToString();
            _arrayData[10] = _currency_code;
            _arrayData[11] = _open_proxy_id.ToString();
            _arrayData[12] = _close_proxy_id.ToString();
            _arrayData[13] = _product_type.ToString();
            _arrayData[14] = _exchange_code;
            _arrayData[15] = _commodity_code;
            _arrayData[16] = _contract_code;
            _arrayData[17] = _buy_sell.ToString();
            _arrayData[18] = _profit_loss.ToString();

            return _arrayData;
        }
    }

    public class SlmCloseBind
    {
        private SlmClose _close;

        public SlmCloseBind(SlmClose close)
        {
            _close = close;
        }

        public int InitDate
        {
            get
            {
                return _close.InitDate;
            }
        }

        public string AccountNo
        {
            get
            {
                return _close.AccountNo;
            }
        }

        public int OpenDate
        {
            get
            {
                return _close.OpenDate;
            }
        }

        public Int64 OpenNo
        {
            get
            {
                return _close.OpenNo;
            }
        }

        public Decimal OpenPrice
        {
            get
            {
                return _close.OpenPrice;
            }
        }

        public int CloseDate
        {
            get
            {
                return _close.CloseDate;
            }
        }

        public Int64 CloseNo
        {
            get
            {
                return _close.CloseNo;
            }
        }

        public Decimal ClosePrice
        {
            get
            {
                return _close.ClosePrice;
            }
        }

        public int OpenNum
        {
            get
            {
                return _close.OpenNum;
            }
        }

        public int CloseNum
        {
            get
            {
                return _close.CloseNum;
            }
        }

        public string CurrencyCode
        {
            get
            {
                return _close.CurrencyCode;
            }
        }

        public int OpenProxyId
        {
            get
            {
                return _close.OpenProxyId;
            }
        }

        public int CloseProxyId
        {
            get
            {
                return _close.CloseProxyId;
            }
        }

        public string ExchangeCode
        {
            get
            {
                return _close.ExchangeCode;
            }
        }

        public string CommodityCode
        {
            get
            {
                return _close.CommodityCode;
            }
        }


        public string ContractCode
        {
            get
            {
                return _close.ContractCode;
            }
        }

        public Decimal ProfitLoss
        {
            get
            {
                return _close.ProfitLoss;
            }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public string BuySellBinding
        {
            get
            {
                return _close.BuySell == DictionaryHelper.ORDER_DERECTION.BUY ? "买入" : "卖出";
            }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public string ProductTypeBinding
        {
            get
            {
                string s = string.Empty;
                if (_close.ProductType == DictionaryHelper.COMMODITY_TYPE.FUTURE)
                {
                    s = "期货";
                }
                else if (_close.ProductType == DictionaryHelper.COMMODITY_TYPE.SPEARD)
                {
                    s = "价差";
                }
                else if (_close.ProductType == DictionaryHelper.COMMODITY_TYPE.OPTION)
                {
                    s = "期权";
                }
                return s;
            }
        }
    }
    #endregion

    #region 资金结构
    public class SlmFund : SolomonType
    {
        private string _accountNo = string.Empty;
        private string _operatorNo = string.Empty;
        private string _currencyCode = string.Empty;
        private Decimal _avaliableCash = 0;
        private Decimal _withDrawCash = 0;
        private Decimal _balance = 0;
        private Decimal _profitLoss = 0;
        private Decimal _dateProfitLoss = 0;
        private Decimal _floatProfitLoss = 0;
        private Decimal _premium = 0;
        private Decimal _preBalance = 0;
        private Decimal _creditLimit = 0;
        private Decimal _dayCreditLimit = 0;
        private Decimal _deposit = 0;
        private Decimal _lmeProfitLoss = 0;
        private Decimal _fare = 0;
        private Decimal _initFee = 0;
        private Decimal _keepFee = 0;
        private Decimal _freezeFee = 0;
        private Decimal _marketCall = 0;
        private Decimal _martketPut = 0;
        private Decimal _martketTotal = 0;
        private Decimal _equity = 0;
        private string[] _arrayData = new string[23];

        public string AccountNo
        {
            get { return _accountNo; }
            set
            {
                _accountNo = value;
                //_arrayData[0] = value;
            }
        }

        public string OperatorNo
        {
            get { return _operatorNo; }
            set
            {
                _operatorNo = value;
                //_arrayData[1] = value;
            }
        }

        public string CurrencyCode
        {
            get { return _currencyCode; }
            set
            {
                _currencyCode = value;
                //_arrayData[2] = value;
            }
        }

        public Decimal AvaliableCash
        {
            get { return _avaliableCash; }
            set
            {
                _avaliableCash = value;
                //_arrayData[3] = value.ToString();
            }
        }

        public Decimal WithDrawCash
        {
            get { return _withDrawCash; }
            set
            {
                _withDrawCash = value;
                //_arrayData[4] = value.ToString();
            }
        }

        public Decimal Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                //_arrayData[5] = value.ToString();
            }
        }

        public Decimal ProfitLoss
        {
            get { return _profitLoss; }
            set
            {
                _profitLoss = value;
                //_arrayData[6] = value.ToString();
            }
        }

        public Decimal DateProfitLoss
        {
            get { return _dateProfitLoss; }
            set
            {
                _dateProfitLoss = value;
                //_arrayData[7] = value.ToString();
            }
        }

        public Decimal FloatProfitLoss
        {
            get { return _floatProfitLoss; }
            set
            {
                _floatProfitLoss = value;
                //_arrayData[8] = value.ToString();
            }
        }

        public Decimal Premium
        {
            get { return _premium; }
            set
            {
                _premium = value;
                //_arrayData[9] = value.ToString();
            }
        }

        public Decimal PreBalance
        {
            get { return _preBalance; }
            set
            {
                _preBalance = value;
                //_arrayData[10] = value.ToString();
            }
        }

        public Decimal CreditLimit
        {
            get { return _creditLimit; }
            set
            {
                _creditLimit = value;
                //_arrayData[11] = value.ToString();
            }
        }

        public Decimal DayCreditLimit
        {
            get { return _dayCreditLimit; }
            set
            {
                _dayCreditLimit = value;
                //_arrayData[12] = value.ToString();
            }
        }

        public Decimal Deposit
        {
            get { return _deposit; }
            set
            {
                _deposit = value;
                //_arrayData[13] = value.ToString();
            }
        }

        public Decimal LmeProfitLoss
        {
            get { return _lmeProfitLoss; }
            set
            {
                _lmeProfitLoss = value;
                //_arrayData[14] = value.ToString();
            }
        }
        public Decimal Fare
        {
            get { return _fare; }
            set
            {
                _fare = value;
                //_arrayData[15] = value.ToString();
            }
        }

        public Decimal InitFee
        {
            get { return _initFee; }
            set
            {
                _initFee = value;
                //_arrayData[16] = value.ToString();
            }
        }

        public Decimal KeepFee
        {
            get { return _keepFee; }
            set
            {
                _keepFee = value;
                //_arrayData[17] = value.ToString();
            }
        }

        public Decimal FreezeFee
        {
            get { return _freezeFee; }
            set
            {
                _freezeFee = value;
                //_arrayData[18] = value.ToString();
            }
        }

        public Decimal MarketCall
        {
            get { return _marketCall; }
            set
            {
                _marketCall = value;
                //_arrayData[19] = value.ToString();
            }
        }

        public Decimal MarketPut
        {
            get { return _martketPut; }
            set
            {
                _martketPut = value;
                //_arrayData[20] = value.ToString();
            }
        }

        public Decimal MarketTotal
        {
            get { return _martketTotal; }
            set
            {
                _martketTotal = value;
                //_arrayData[21] = value.ToString();
            }
        }

        public Decimal Equity
        {
            get { return _equity; }
            set
            {
                _equity = value;
                //_arrayData[22] = value.ToString();
            }
        }

        #region 方法
        public SlmFund SelfDataCopy()
        {
            SlmFund f = new SlmFund();
            f.AccountNo = _accountNo;
            f.AvaliableCash = _avaliableCash;
            f.Balance = _balance;
            f.CreditLimit = _creditLimit;
            f.CurrencyCode = _currencyCode;
            f.DateProfitLoss = _dateProfitLoss;
            f.DayCreditLimit = _dayCreditLimit;
            f.Deposit = _deposit;
            f.Equity = _equity;
            f.Fare = _fare;
            f.FloatProfitLoss = _floatProfitLoss;
            f.FreezeFee = _freezeFee;
            f.InitFee = _initFee;
            f.KeepFee = _keepFee;
            f.LmeProfitLoss = _lmeProfitLoss;
            f.MarketCall = _marketCall;
            f.MarketPut = _martketPut;
            f.MarketTotal = _martketTotal;
            f.OperatorNo = _operatorNo;
            f.PreBalance = _preBalance;
            f.Premium = _premium;
            f.ProfitLoss = _profitLoss;
            f.WithDrawCash = _withDrawCash;

            return f;
        }

        public void DataCopy(ref SlmFund f)
        {
            f.AccountNo = _accountNo;
            f.AvaliableCash = _avaliableCash;
            f.Balance = _balance;
            f.CreditLimit = _creditLimit;
            f.CurrencyCode = _currencyCode;
            f.DateProfitLoss = _dateProfitLoss;
            f.DayCreditLimit = _dayCreditLimit;
            f.Deposit = _deposit;
            f.Equity = _equity;
            f.Fare = _fare;
            f.FloatProfitLoss = _floatProfitLoss;
            f.FreezeFee = _freezeFee;
            f.InitFee = _initFee;
            f.KeepFee = _keepFee;
            f.LmeProfitLoss = _lmeProfitLoss;
            f.MarketCall = _marketCall;
            f.MarketPut = _martketPut;
            f.MarketTotal = _martketTotal;
            f.OperatorNo = _operatorNo;
            f.PreBalance = _preBalance;
            f.Premium = _premium;
            f.ProfitLoss = _profitLoss;
            f.WithDrawCash = _withDrawCash;
        }

        public void DataSet(SlmFund f)
        {
            _accountNo = f.AccountNo;
            _avaliableCash = f.AvaliableCash;
            _balance = f.Balance;
            _creditLimit = f.CreditLimit;
            _currencyCode = f.CurrencyCode;
            _dateProfitLoss = f.DateProfitLoss;
            _dayCreditLimit = f.DayCreditLimit;
            _deposit = f.Deposit;
            _equity = f.Equity;
            _fare = f.Fare;
            _floatProfitLoss = f.FloatProfitLoss;
            _freezeFee = f.FreezeFee;
            _initFee = f.InitFee;
            _keepFee = f.KeepFee;
            _lmeProfitLoss = f.LmeProfitLoss;
            _marketCall = f.MarketCall;
            _martketPut = f.MarketPut;
            _martketTotal = f.MarketTotal;
            _operatorNo = f.OperatorNo;
            _preBalance = f.PreBalance;
            _premium = f.Premium;
            _profitLoss = f.ProfitLoss;
            _withDrawCash = f.WithDrawCash;
        }

        public bool IsZero()
        {
            if (_balance == 0 && _equity == 0 && _martketTotal == 0 && _preBalance == 0 && _avaliableCash == 0)
            {
                return true;
            }
            return false;
        }

        public string[] GetArrayData()
        {
            _arrayData[0] = _accountNo;
            _arrayData[1] = _operatorNo;
            _arrayData[2] = _currencyCode;
            _arrayData[3] = _avaliableCash.ToString();
            _arrayData[4] = _withDrawCash.ToString();
            _arrayData[5] = _balance.ToString();
            _arrayData[6] = _profitLoss.ToString();
            _arrayData[7] = _dateProfitLoss.ToString();
            _arrayData[8] = _floatProfitLoss.ToString();
            _arrayData[9] = _premium.ToString();
            _arrayData[10] = _preBalance.ToString();
            _arrayData[11] = _creditLimit.ToString();
            _arrayData[12] = _dayCreditLimit.ToString();
            _arrayData[13] = _deposit.ToString();
            _arrayData[14] = _lmeProfitLoss.ToString();
            _arrayData[15] = _fare.ToString();
            _arrayData[16] = _initFee.ToString();
            _arrayData[17] = _keepFee.ToString();
            _arrayData[18] = _freezeFee.ToString();
            _arrayData[19] = _marketCall.ToString();
            _arrayData[20] = _martketPut.ToString();
            _arrayData[21] = _martketTotal.ToString();
            _arrayData[22] = _equity.ToString();
            return _arrayData;
        }
        #endregion
    }

    public class SlmFund4DB
    {
        public string account_no { get; set; }
        public string currency_code { get; set; }
        public Decimal avaliable_cash { get; set; }
        public Decimal withdraw_cash { get; set; }
        public Decimal balance { get; set; }
        public Decimal profit_loss { get; set; }
        public Decimal date_profit_loss { get; set; }
        public Decimal float_profit_loss { get; set; }
        public Decimal premium { get; set; }
        public Decimal pre_balance { get; set; }
        public Decimal credit_limit { get; set; }
        public Decimal day_credit_limit { get; set; }
        public Decimal deposit { get; set; }
        public Decimal lme_profit_loss { get; set; }
        public Decimal fare { get; set; }
        public Decimal market_call { get; set; }
        public Decimal market_put { get; set; }
        public Decimal market_total { get; set; }
        public Decimal equity { get; set; }
    }
    /// <summary>
    /// 冻结资金
    /// </summary>
    public class FreezeFund
    {
        //账户
        public string account_no { get; set; }
        //币种
        public string currency_code { get; set; }
        //金额
        public Decimal amount { get; set; }
    }
    #endregion

    #region 订单流水结构
    public class SlmOrderJour : SolomonType
    {
        private int _init_date;
        private string _router_no;
        private string _operator_no;
        private string _order_person;
        private string _update_person;
        private short _proxy_id;
        private string _account_no;
        private sbyte _product_type;
        private string _exchange_code;
        private string _commodity_code;
        private string _contract_code;
        private sbyte _call_put;
        private string _strike_price;
        private sbyte _buy_sell;
        private Decimal _order_price;
        private short _order_num;
        private sbyte _order_type;
        private sbyte _order_life;
        //private sbyte _orderInsertType;
        private sbyte _order_from;
        private Decimal _trigger_price;
        private char _order_status;
        private int _filled_num;
        private int _order_date;
        private int _order_time;
        private int _update_date;
        private int _update_time;
        private int _exchange_date;
        private int _exchange_time;
        private Int64 _order_no;
        private short _order_jour_no;
        private string _broker_order_no;
        private string _broker_order_jour_no;
        private string _exchange_order_no;
        private short _error_no;
        private string _error_info;
        private string _remark_cus;
        private string _info_s1;
        private string _info_s2;

        #region 推送前置新增字段，服务API
        private sbyte _action { get; set; }//动作：1下单成功，2下单失败；3改单成功，4改单失败；5撤单成功，6撤单失败；7其他
        #endregion

        private long _msgNum;//此字段必须在最末尾，加字段在前面插入

        private string[] _arrayData = new string[40];


        private byte[] orderJourIceByte;
        public int InitDate
        {
            get { return _init_date; }
            set
            {
                _init_date = value;
                //_arrayData[0] = value.ToString();
            }
        }

        public string RouterNo
        {
            get { return _router_no; }
            set
            {
                _router_no = value;
                //_arrayData[1] = value;
            }
        }

        public string OperatorNo
        {
            get { return _operator_no; }
            set
            {
                _operator_no = value;
                //_arrayData[2] = value;
            }
        }

        public string OrderPerson
        {
            get { return _order_person; }
            set
            {
                _order_person = value;
                //_arrayData[3] = value;
            }
        }

        public string UpdatePerson
        {
            get { return _update_person; }
            set
            {
                _update_person = value;
                //_arrayData[4] = value;
            }
        }

        public short ProxyId
        {
            get { return _proxy_id; }
            set
            {
                _proxy_id = value;
                //_arrayData[5] = value.ToString();
            }
        }

        public string AccountNo
        {
            get { return _account_no; }
            set
            {
                _account_no = value;
                //_arrayData[6] = value;
            }
        }

        public sbyte ProductType
        {
            get { return _product_type; }
            set
            {
                _product_type = value;
                //_arrayData[7] = value.ToString();
            }
        }

        public string ExchangeCode
        {
            get { return _exchange_code; }
            set
            {
                _exchange_code = value;
                //_arrayData[8] = value;
            }
        }

        public string CommodityCode
        {
            get { return _commodity_code; }
            set
            {
                _commodity_code = value;
                //_arrayData[9] = value;
            }
        }

        public string ContractCode
        {
            get { return _contract_code; }
            set
            {
                _contract_code = value;
                //_arrayData[10] = value;
            }
        }

        public sbyte CallPut
        {
            get { return _call_put; }
            set
            {
                _call_put = value;
                //_arrayData[11] = value.ToString();
            }
        }

        public string StrikePrice
        {
            get { return _strike_price; }
            set
            {
                _strike_price = value;
                //_arrayData[12] = value;
            }
        }

        public sbyte BuySell
        {
            get { return _buy_sell; }
            set
            {
                _buy_sell = value;
                //_arrayData[13] = value.ToString();
            }
        }

        public Decimal OrderPrice
        {
            get { return _order_price; }
            set
            {
                _order_price = value;
                //_arrayData[14] = value.ToString();
            }
        }

        public short OrderNum
        {
            get { return _order_num; }
            set
            {
                _order_num = value;
                //_arrayData[15] = value.ToString();
            }
        }

        public sbyte OrderType
        {
            get { return _order_type; }
            set
            {
                _order_type = value;
                //_arrayData[16] = value.ToString();
            }
        }

        public sbyte OrderLife
        {
            get { return _order_life; }
            set
            {
                _order_life = value;
                //_arrayData[17] = value.ToString();
            }
        }

        /// <summary>
        /// 人工录入单，程序传入单
        /// </summary>
        //public sbyte OrderInsertType
        //{
        //    get { return _orderInsertType; }
        //    set
        //    {
        //        _orderInsertType = value;
        //        //_arrayData[15] = value.ToString();
        //    }
        //}


        public sbyte OrderFrom
        {
            get { return _order_from; }
            set
            {
                _order_from = value;
                //_arrayData[18] = value.ToString();
            }
        }

        public Decimal TriggerPrice
        {
            get { return _trigger_price; }
            set
            {
                _trigger_price = value;
                //_arrayData[19] = value.ToString();
            }
        }

        public char OrderStatus
        {
            get { return _order_status; }
            set
            {
                _order_status = value;
                //_arrayData[20] = value.ToString();
            }
        }

        public int FilledNum
        {
            get { return _filled_num; }
            set
            {
                _filled_num = value;
                //_arrayData[21] = value.ToString();
            }
        }

        public int OrderDate
        {
            get { return _order_date; }
            set
            {
                _order_date = value;
                //_arrayData[22] = value.ToString();
            }
        }

        public int OrderTime
        {
            get { return _order_time; }
            set
            {
                _order_time = value;
                //_arrayData[23] = value.ToString();
            }
        }

        public int UpdateDate
        {
            get { return _update_date; }
            set
            {
                _update_date = value;
                //_arrayData[24] = value.ToString();
            }
        }

        public int UpdateTime
        {
            get { return _update_time; }
            set
            {
                _update_time = value;
                //_arrayData[25] = value.ToString();
            }
        }

        public int ExchangeDate
        {
            get { return _exchange_date; }
            set
            {
                _exchange_date = value;
                //_arrayData[26] = value.ToString();
            }
        }

        public int ExchangeTime
        {
            get { return _exchange_time; }
            set
            {
                _exchange_time = value;
                //_arrayData[27] = value.ToString();
            }
        }

        public Int64 OrderNo
        {
            get { return _order_no; }
            set
            {
                _order_no = value;
                //_arrayData[28] = value.ToString();
            }
        }

        public short OrderJourNo
        {
            get { return _order_jour_no; }
            set
            {
                _order_jour_no = value;
                //_arrayData[29] = value.ToString();
            }
        }

        public string BrokerOrderNo
        {
            get { return _broker_order_no; }
            set
            {
                _broker_order_no = value;
                //_arrayData[30] = value;
            }
        }

        public string BrokerOrderJourNo
        {
            get { return _broker_order_jour_no; }
            set
            {
                _broker_order_jour_no = value;
                //_arrayData[31] = value;
            }
        }

        public string ExchangeOrderNo
        {
            get { return _exchange_order_no; }
            set
            {
                _exchange_order_no = value;
                //_arrayData[32] = value;
            }
        }

        public short ErrorNo
        {
            get { return _error_no; }
            set
            {
                _error_no = value;
                //_arrayData[33] = value.ToString();
            }
        }

        public string ErrorInfo
        {
            get { return _error_info; }
            set
            {
                _error_info = value;
                //_arrayData[34] = value;
            }
        }

        public string RemarkCus
        {
            get { return _remark_cus; }
            set
            {
                _remark_cus = value;
                //_arrayData[35] = value;
            }
        }

        public string InfoS1
        {
            get { return _info_s1; }
            set
            {
                _info_s1 = value;
                //_arrayData[36] = value;
            }
        }

        public string InfoS2
        {
            get { return _info_s2; }
            set
            {
                _info_s2 = value;
                //_arrayData[37] = value;
            }
        }

        public sbyte Action
        {
            get { return _action; }
            set
            {
                _action = value;
                //_arrayData[38] = value;
            }
        }

        public long MsgNum
        {
            get { return _msgNum; }
            set
            {
                _msgNum = value;
            }
        }

        public string[] GetDataArray()
        {
            _arrayData[0] = _init_date.ToString();
            _arrayData[1] = _router_no;
            _arrayData[2] = _operator_no;
            _arrayData[3] = _order_person;
            _arrayData[4] = _update_person;
            _arrayData[5] = _proxy_id.ToString();
            _arrayData[6] = _account_no;
            _arrayData[7] = _product_type.ToString();
            _arrayData[8] = _exchange_code;
            _arrayData[9] = _commodity_code;
            _arrayData[10] = _contract_code;
            _arrayData[11] = _call_put.ToString();
            _arrayData[12] = _strike_price;
            _arrayData[13] = _buy_sell.ToString();
            _arrayData[14] = _order_price.ToString();
            _arrayData[15] = _order_num.ToString();
            _arrayData[16] = _order_type.ToString();
            _arrayData[17] = _order_life.ToString();
            _arrayData[18] = _order_from.ToString();
            _arrayData[19] = _trigger_price.ToString();
            _arrayData[20] = _order_status.ToString();
            _arrayData[21] = _filled_num.ToString();
            _arrayData[22] = _order_date.ToString();
            _arrayData[23] = _order_time.ToString();
            _arrayData[24] = _update_date.ToString();
            _arrayData[25] = _update_time.ToString();
            _arrayData[26] = _exchange_date.ToString();
            _arrayData[27] = _exchange_time.ToString();
            _arrayData[28] = _order_no.ToString();
            _arrayData[29] = _order_jour_no.ToString();
            _arrayData[30] = _broker_order_no;
            _arrayData[31] = _broker_order_jour_no;
            _arrayData[32] = _exchange_order_no;
            _arrayData[33] = _error_no.ToString();
            _arrayData[34] = _error_info;
            _arrayData[35] = _remark_cus;
            _arrayData[36] = _info_s1;
            _arrayData[37] = _info_s2;
            _arrayData[38] = _action.ToString();
            _arrayData[39] = _msgNum.ToString();

            return _arrayData;
        }

        public byte[] GetBytes()
        {
            return orderJourIceByte;
        }

        public void SetIceBytes(byte[] iceBytes)
        {
            orderJourIceByte = iceBytes;
        }

        public SlmOrderJour Clone()
        {
            return (SlmOrderJour)this.MemberwiseClone();

        }
    }

    public class SlmOrderJourBind : ShowDto
    {
        private SlmOrderJour _jour;

        public SlmOrderJourBind(SlmOrderJour jour)
        {
            _jour = jour;
        }

        public SlmOrderJourBind(SlmOrderJour jour, long msgnum)
        {
            _jour = jour;
            this.MsgNum = msgnum;
        }


        public int InitDate
        {
            get { return _jour.InitDate; }
        }

        public string RouterNo
        {
            get { return _jour.RouterNo; }
        }

        public string OperatorNo
        {
            get { return _jour.OperatorNo; }
        }

        public string OrderPerson
        {
            get { return _jour.OrderPerson; }
        }

        public string UpdatePerson
        {
            get { return _jour.UpdatePerson; }
        }

        public short ProxyId
        {
            get { return _jour.ProxyId; }
        }

        public string AccountNo
        {
            get { return _jour.AccountNo; }
        }

        public sbyte ProductType
        {
            get { return _jour.ProductType; }
        }

        public string ExchangeCode
        {
            get { return _jour.ExchangeCode; }
        }

        public string CommodityCode
        {
            get { return _jour.CommodityCode; }
        }

        public string ContractCode
        {
            get { return _jour.ContractCode; }
        }

        public string StrikePrice
        {
            get { return _jour.StrikePrice; }
        }

        public Decimal OrderPrice
        {
            get { return _jour.OrderPrice; }
        }

        public short OrderNum
        {
            get { return _jour.OrderNum; }
        }

        public Decimal TriggerPrice
        {
            get { return _jour.TriggerPrice; }
        }

        public int FilledNum
        {
            get { return _jour.FilledNum; }
        }

        public int OrderDate
        {
            get { return _jour.OrderDate; }
        }

        public int OrderTime
        {
            get { return _jour.OrderTime; }
        }

        public int UpdateDate
        {
            get { return _jour.UpdateDate; }
        }

        public int UpdateTime
        {
            get { return _jour.UpdateTime; }
        }

        public int ExchangeDate
        {
            get { return _jour.ExchangeDate; }
        }

        public int ExchangeTime
        {
            get { return _jour.ExchangeTime; }
        }

        public Int64 OrderNo
        {
            get { return _jour.OrderNo; }
        }

        public short OrderJourNo
        {
            get { return _jour.OrderJourNo; }
        }

        public string BrokerOrderNo
        {
            get { return _jour.BrokerOrderNo; }
        }

        public string BrokerOrderJourNo
        {
            get { return _jour.BrokerOrderJourNo; }
        }

        public string ExchangeOrderNo
        {
            get { return _jour.ExchangeOrderNo; }
        }

        public short ErrorNo
        {
            get { return _jour.ErrorNo; }
        }

        public string ErrorInfo
        {
            get { return _jour.ErrorInfo; }
        }

        public string RemarkCus
        {
            get { return _jour.RemarkCus; }
        }

        public string InfoS1
        {
            get { return _jour.InfoS1; }
        }

        public string InfoS2
        {
            get { return _jour.InfoS2; }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public string ProductTypeBinding
        {
            get
            {
                string s = string.Empty;
                if (_jour.ProductType == DictionaryHelper.COMMODITY_TYPE.FUTURE)
                {
                    s = "期货";
                }
                else if (_jour.ProductType == DictionaryHelper.COMMODITY_TYPE.SPEARD)
                {
                    s = "价差";
                }
                else if (_jour.ProductType == DictionaryHelper.COMMODITY_TYPE.OPTION)
                {
                    s = "期权";
                }
                return s;
            }
        }

        /// <summary>
        /// 看涨看跌属性
        /// </summary>
        public string CallPutBinding
        {
            get
            {
                string s = string.Empty;
                if (_jour.CallPut == 0)
                {
                    s = "涨";
                }
                else if (_jour.CallPut == 1)
                {
                    s = "跌";
                }
                return s;
            }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public string BuySellBinding
        {
            get
            {
                return _jour.BuySell == 0 ? "买入" : "卖出";
            }
        }

        /// <summary>
        /// 委托类型 市价，限价等
        /// </summary>
        //public string OrderTypeBinding
        //{
        //    get
        //    {
        //        string s = string.Empty;
        //        if (_jour.OrderType == DictionaryHelper.ORDER_TYPE.LIMIT_ORDER)
        //        {
        //            s = "限价";
        //        }
        //        else if (_jour.OrderType == DictionaryHelper.ORDER_TYPE.MARKET_ORDER)
        //        {
        //            s = "市价";
        //        }
        //        else if (_jour.OrderType == DictionaryHelper.ORDER_TYPE.LIMIT_STOP_ORDER)
        //        {
        //            s = "限价止损";
        //        }
        //        else if (_jour.OrderType == DictionaryHelper.ORDER_TYPE.STOP_ORDER)
        //        {
        //            s = "止损";
        //        }
        //        return s;
        //    }
        //}

        /// <summary>
        /// 有效时间
        /// </summary>
        public string OrderLifeBinding
        {
            get
            {
                string s = string.Empty;
                if (_jour.OrderLife == DictionaryHelper.ORDER_LIFE.DAY_ORDER)
                {
                    s = "当日有效";
                }
                else if (_jour.OrderLife == DictionaryHelper.ORDER_LIFE.GTC_ORDER)
                {
                    s = "长期有效";
                }
                else if (_jour.OrderLife == DictionaryHelper.ORDER_LIFE.FOK_ORDER)
                {
                    s = "FOK";
                }
                else if (_jour.OrderLife == DictionaryHelper.ORDER_LIFE.FAK_ORDER)
                {
                    s = "FAK";
                }
                return s;
            }
        }

        /// <summary>
        /// 委托状态
        /// </summary>
        public string OrderStateBinding
        {
            get
            {
                string s = string.Empty;
                if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_SENDED)
                {
                    s = "已发出";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_RECEIVED)
                {
                    s = "已接收";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_RECEPT)
                {
                    s = "已受理";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_ACTIVED)
                {
                    s = "已报入";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_PART_FILLED)
                {
                    s = "部分成交";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_ALL_FILLED)
                {
                    s = "完全成交";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_WAIT_UPDATE)
                {
                    s = "待改";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_WAIT_DEL)
                {
                    s = "待撤";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_DELED)
                {
                    s = "已撤单";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_FAILED)
                {
                    s = "委托失败";
                }
                else if (_jour.OrderStatus == DictionaryHelper.ORDER_STATUS.ORDER_PART_DELETE)
                {
                    s = "部分撤单";
                }
                return s;
            }
        }

    }
    #endregion

    #region 网关推送下单状态结构
    public class PushOrderState : SolomonType
    {
        private Int64 _orderNo;
        private char _orderState;
        private string _orderJourNo = string.Empty;
        private int _updateDate;
        private int _updateTime;
        private short _errNo;
        private string _errInfo = string.Empty;
        private Decimal _orderPrice;
        private short _orderVol;
        private Decimal _triggerPrice;
        private string _exchangeOrderNo = string.Empty;
        private string _exchangeDateTime = string.Empty;
        private short _matchVol;//累计成交量
        private string _brokerOrderNo = string.Empty;
        private string _brokerOrderJourNo = string.Empty;
        private sbyte _orderType;//ljz 2020.10.13 订单类型，开发竞价市价单衍生的新字段

        #region 改单新增字段
        private short _curMatchVol;//本次成交量
        private sbyte _action;//动作：1下单成功，2下单失败；3改单成功，4改单失败；5撤单成功，6撤单失败；7其他
        #endregion

        #region 属性
        public Int64 OrderNo
        {
            get { return _orderNo; }
            set { _orderNo = value; }
        }

        public char OrderState
        {
            get { return _orderState; }
            set { _orderState = value; }
        }

        public string OrderJourNo
        {
            get { return _orderJourNo; }
            set { _orderJourNo = value; }
        }

        public int UpdateDate
        {
            get { return _updateDate; }
            set { _updateDate = value; }
        }

        public int UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        public short ErrNo
        {
            get { return _errNo; }
            set { _errNo = value; }
        }

        public string ErrInfo
        {
            get { return _errInfo; }
            set { _errInfo = value; }
        }

        public Decimal OrderPrice
        {
            get { return _orderPrice; }
            set { _orderPrice = value; }
        }

        public short OrderVol
        {
            get { return _orderVol; }
            set { _orderVol = value; }
        }

        public Decimal TriggerPrice
        {
            get { return _triggerPrice; }
            set { _triggerPrice = value; }
        }

        public string ExchangeOrderNo
        {
            get { return _exchangeOrderNo; }
            set { _exchangeOrderNo = value; }
        }

        public string ExchangeDateTime
        {
            get { return _exchangeDateTime; }
            set { _exchangeDateTime = value; }
        }

        public short MatchVol
        {
            get { return _matchVol; }
            set { _matchVol = value; }
        }

        public string BrokerOrderNo
        {
            get { return _brokerOrderNo; }
            set { _brokerOrderNo = value; }
        }

        public string BrokerOrderJourNo
        {
            get { return _brokerOrderJourNo; }
            set { _brokerOrderJourNo = value; }
        }
        public sbyte OrderType
        {
            get { return _orderType; }
            set { _orderType = value; }
        }
        public short CurMatchVol
        {
            get { return _curMatchVol; }
            set { _curMatchVol = value; }
        }
        public sbyte Action
        {
            get { return _action; }
            set { _action = value; }
        }

        #endregion

        public PushOrderState()
        {

        }

        public PushOrderState(string[] data)
        {
            Int64.TryParse(data[0], out _orderNo);
            Char.TryParse(data[1], out _orderState);
            _orderJourNo = data[2];
            Int32.TryParse(data[3], out _updateDate);
            Int32.TryParse(data[4], out _updateTime);
            Int16.TryParse(data[5], out _errNo);
            _errInfo = data[6];
            Decimal.TryParse(data[7], out _orderPrice);
            Int16.TryParse(data[8], out _orderVol);
            Decimal.TryParse(data[9], out _triggerPrice);
            _exchangeOrderNo = data[10];
            _exchangeDateTime = data[11];
            Int16.TryParse(data[12], out _matchVol);
            _brokerOrderNo = data[13];
            _brokerOrderJourNo = data[14];
            sbyte.TryParse(data[15], out _orderType);
        }
    }
    #endregion

    #region 网关推送成交结构
    public class PushMatchState : SolomonType
    {
        private string _account_no = string.Empty;
        private Int64 _order_no;
        private int _trade_date;
        private string _trade_no; //上手成交编号
        private int _trade_time;
        private Decimal _trade_price;
        private int _trade_vol;
        private short _proxy_id;
        private string _broker_trade_no = string.Empty;
        private string _exchange_trade_no = string.Empty;
        private string _operator_no = string.Empty;
        private string _info1 = string.Empty;
        private string _info2 = string.Empty;
        private string _info3 = string.Empty;
        private sbyte _order_insert_type;
        private string _order_from = string.Empty;
        private string _commodity_code = string.Empty;
        private string _contract_code = string.Empty;
        private string _buy_sell = string.Empty;

        public string AccountNo
        {
            get
            {
                return _account_no;
            }
            set
            {
                _account_no = value;
            }
        }

        public Int64 OrderNo
        {
            get
            {
                return _order_no;
            }
            set
            {
                _order_no = value;
            }
        }
        public int TradeDate
        {
            get
            {
                return _trade_date;
            }
            set
            {
                _trade_date = value;
            }
        }
        public string TradeNo
        {
            get
            {
                return _trade_no;
            }
            set
            {
                _trade_no = value;
            }
        }
        public int TradeTime
        {
            get
            {
                return _trade_time;
            }
            set
            {
                _trade_time = value;
            }
        }
        public Decimal TradePrice
        {
            get
            {
                return _trade_price;
            }
            set
            {
                _trade_price = value;
            }
        }
        public int TradeVol
        {
            get
            {
                return _trade_vol;
            }
            set
            {
                _trade_vol = value;
            }
        }

        public short ProxyId
        {
            get
            {
                return _proxy_id;
            }
            set
            {
                _proxy_id = value;
            }
        }
        public string BrokerTradeNo
        {
            get
            {
                return _broker_trade_no;
            }
            set
            {
                _broker_trade_no = value;
            }
        }
        public string ExchangeTradeNo
        {
            get
            {
                return _exchange_trade_no;
            }
            set
            {
                _exchange_trade_no = value;
            }
        }

        public string OperatorNo
        {
            get
            {
                return _operator_no;
            }
            set
            {
                _operator_no = value;
            }
        }

        public string Info1
        {
            get
            {
                return _info1;
            }
            set
            {
                _info1 = value;
            }
        }

        public string Info2
        {
            get
            {
                return _info2;
            }
            set
            {
                _info2 = value;
            }
        }

        public string Info3
        {
            get
            {
                return _info3;
            }
            set
            {
                _info3 = value;
            }
        }

        public sbyte OrderInsertType
        {
            get
            {
                return _order_insert_type;
            }
            set
            {
                _order_insert_type = value;
            }
        }

        public string OrderFrom
        {
            get
            {
                return _order_from;
            }
            set
            {
                _order_from = value;
            }
        }

        public string CommodityCode
        {
            get
            {
                return _commodity_code;
            }
            set
            {
                _commodity_code = value;
            }
        }

        public string ContractCode
        {
            get
            {
                return _contract_code;
            }
            set
            {
                _contract_code = value;
            }
        }

        public string BuySell
        {
            get
            {
                return _buy_sell;
            }
            set
            {
                _buy_sell = value;
            }
        }

    }
    #endregion

    #region 推送交易前置订单状态变化,成交，string[][]改object
    /// <summary>
    /// 推送交易前置订单状态变化 TradeServerCallBack.SE_ORDER_STATUS
    /// </summary>
    public class SendOrderStatus
    {
        public string AccountNo { get; set; }//账户号码
        public string OperatorNo { get; set; }//交易员编号
        public long OrderNo { get; set; }//orderId
        public char OrderState { get; set; }//委托状态
        public short OrderJourNo { get; set; }//内部委托流水号
        public int UpdateDate { get; set; }//更新日期
        public int UpdateTime { get; set; }//更新时间
        public short ErrorNo { get; set; }//返回错误代码
        public string ErrorInfo { get; set; }//返回错误信息
        public decimal OrderPrice { get; set; }//委托价格
        public short OrderNum { get; set; }//委托数量
        public decimal TriggerPrice { get; set; }//触发价格
        public int ExchangeDate { get; set; }//交易所日期
        public int ExchangeTime { get; set; }//交易所时间
        public string ExchangeOrderNo { get; set; }//交易所委托号
        public short FilledNum { get; set; }//成交数量
        public string BrokerOrderNo { get; set; }//上手委托号
        public string BrokerOrderJourNo { get; set; }//上手委托流号
        public string InfoS1 { get; set; }//用户保留
        public string InfoS2 { get; set; }//用户保留
        public int InitDate { get; set; }//交易日期
        public sbyte OrderType { get; set; }//委托类型，开发竞价市价单衍生的新字段
        public sbyte OrderFrom { get; set; }//委托来源，成交修复更新APP使用
        public string Remarks { get; set; }//备注，成交修复

        #region 推送前置新增字段，服务API
        public sbyte Action { get; set; } = DictionaryHelper.GateWayAction.OTHER;//动作：1下单成功，2下单失败；3改单成功，4改单失败；5撤单成功，6撤单失败；7其他

        public Int64 MsgNum { get; set; }
        #endregion
    }

    #endregion

    #region 主备双活新增数据类
    /// <summary>
    /// 主备双活，新增的唯一消息号，所有消息体的基类
    /// </summary>
    public class TradeStandbyTransDataBase
    {
        private Int64 msgNumber;
        public Int64 MsgNumber
        {
            get
            {
                return msgNumber;
            }
        }

        public TradeStandbyTransDataBase()
        {
            //msgNumber = TradeTools.GetUniqueMsgNumber();
        }
    }

    /// <summary>
    /// 主发送给备的数据类型
    /// </summary>
    public enum TradeServersDataType
    {
        /// <summary>
        /// 对应dataObj是SlmOrder, 交易收到前置下单
        /// </summary>
        InsertOrder = 1,
        /// <summary>
        /// 对应dataObj是string[] Params, 交易收到前置改单
        /// </summary>
        UpdateOrder = 2,
        /// <summary>
        /// 对应dataObj是string[] Params, 交易收到前置撤单
        /// </summary>
        DelOrder = 3,
        /// <summary>
        /// 对应dataObj是PushOrderState, 交易收到网关委托状态变化
        /// </summary>
        OrderStatus = 4,
        /// <summary>
        /// 对应dataObj是PushMatchState, 交易收到网关成交
        /// </summary>
        Trade = 5,

        //下阶段补缺： 人工录单，人工修改委托状态等
    }

    /// <summary>
    /// 主交易服发送给备交易服的消息结构
    /// </summary>
    public class TradeToStandbyData : TradeStandbyTransDataBase
    {
        public TradeServersDataType dataType;
        public object dataObj;
    }

    /// <summary>
    /// 交易服与内存服通信的消息体
    /// </summary>
    public class TradeToMemSerData : TradeStandbyTransDataBase
    {
    }

    /// <summary>
    /// 交易服与仲裁服通信的消息体
    /// </summary>
    public class TradeToArbSerData
    {

    }

    public class MsgPackage
    {
        public int serviceNo { get; set; }
        public object data { get; set; }
        public string MsgNum { get; set; }
    }
    #endregion

    /// <summary>
    /// DB
    /// k v 
    /// </summary>
    public class keyvalueDB
    {
       public string k { get; set; }
       public string v { get; set; }
    }
}
