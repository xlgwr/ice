using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.CommonTools;

namespace Trade.DataType
{
    public class SlmOrder : SolomonType
    {
        private string _accountNo = string.Empty;//0
        private string _operatorNo = string.Empty;//1
        private string _orderPerson = string.Empty;//2
        private sbyte _productType;//3
        private string _exchangeCode = string.Empty;//4
        private string _commodityCode = string.Empty;//5
        private string _contractCode = string.Empty;//6
        private sbyte _callPut;//7
        private string _strikePrice = string.Empty;//8
        private sbyte _openClose;//9
        private sbyte _buySell;//10
        private Decimal _orderPrice;//11
        private short _orderNum;//12
        private sbyte _orderType;//13
        private sbyte _orderLife;//14
        private sbyte _orderInsertType;//15
        private sbyte _delTag;//16
        private sbyte _orderFrom;//17
        private short _orderSubNumMin;//18
        private short _orderSubNumMax;//19
        private int _orderExpiryDate;//20
        private Decimal _triggerPrice;//21
        private sbyte _orderRemain;//22
        private string _remarkCus = string.Empty;//23
        private string _infoS1 = string.Empty;//24
        private string _infoS2 = string.Empty;//25
        private string _localNo = string.Empty;//26
        private Int64 _orderNo;//27
        private int _orderDate;//28
        private int _orderTime;//29
        private string _routerNo = string.Empty;//30
        private int _initDate;//31
        private string _updatePerson = string.Empty;//32
        private short _brokerNo;//33
        private char _orderState;//34
        private short _filledNum;//35
        private int _updateDate;//36
        private int _updateTime;//37
        private int _exchangeDate;//38
        private int _exchangeTime;//39
        private short _orderJourNo;//40
        private string _brokerOrderNo = string.Empty;//41
        private string _brokerOrderJourNo = string.Empty;//42
        private string _exchangeOrderNo = string.Empty;//43
        private string _carryNo = string.Empty;//44
        private short _errorNo;//45
        private string _errorInfo = string.Empty;//46
        private short _proxyId;//47
        private int _closeNum; //48 平仓数量
        private int _openNum; //49 开仓数量

        private sbyte _action;//50
        private long _msgNum;//51 此字段必须在最末尾，加字段在前面插入

        private string[] _arrayData = new string[52];
        //BrokerNo  和_proxyId 重复 _closeNum _openNum 数据库没有，前置没有。
        /// <summary>
        /// 序列化字节流
        /// </summary>
        private Dictionary<long, byte[]> orderBytesDic = new Dictionary<long, byte[]>();
        #region public
        /// <summary>
        /// 账户号码
        /// </summary>
        public string AccountNo
        {
            get { return _accountNo; }
            set
            {
                _accountNo = value;
                //_arrayData[0] = value;
            }
        }

        /// <summary>
        /// 交易员编号
        /// </summary>
        public string OperatorNo
        {
            get { return _operatorNo; }
            set
            {
                _operatorNo = value;
                //_arrayData[1] = value;
            }
        }

        /// <summary>
        /// 委托人
        /// </summary>
        public string OrderPerson
        {
            get { return _orderPerson; }
            set
            {
                _orderPerson = value;
                //_arrayData[2] = value;
            }
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public sbyte ProductType
        {
            get { return _productType; }
            set
            {
                _productType = value;
                //_arrayData[3] = value.ToString();
            }
        }

        /// <summary>
        /// 市场代码
        /// </summary>
        public string ExchangeCode
        {
            get { return _exchangeCode; }
            set
            {
                _exchangeCode = value;
                //_arrayData[4] = value;
            }
        }

        /// <summary>
        /// 商品代码
        /// </summary>
        public string CommodityCode
        {
            get { return _commodityCode; }
            set
            {
                _commodityCode = value;
                //_arrayData[5] = value;
            }
        }

        /// <summary>
        /// 合约代码
        /// </summary>
        public string ContractCode
        {
            get { return _contractCode; }
            set
            {
                _contractCode = value;
                //_arrayData[6] = value;
            }
        }

        /// <summary>
        /// 看涨看跌属性
        /// </summary>
        public sbyte CallPut
        {
            get { return _callPut; }
            set
            {
                _callPut = value;
                //_arrayData[7] = value.ToString();
            }
        }

        /// <summary>
        /// 行权价
        /// </summary>
        public string StrikePrice
        {
            get { return _strikePrice; }
            set
            {
                _strikePrice = value;
                //_arrayData[8] = value;
            }
        }

        /// <summary>
        /// 开平仓
        /// </summary>
        public sbyte OpenClose
        {
            get { return _openClose; }
            set
            {
                _openClose = value;
                //_arrayData[9] = value.ToString();
            }
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public sbyte BuySell
        {
            get { return _buySell; }
            set
            {
                _buySell = value;
                //_arrayData[10] = value.ToString();
            }
        }

        /// <summary>
        /// 委托价格
        /// </summary>
        public Decimal OrderPrice
        {
            get { return _orderPrice; }
            set
            {
                _orderPrice = value;
                //_arrayData[11] = value.ToString();
            }
        }

        /// <summary>
        /// 委托数量
        /// </summary>
        public short OrderNum
        {
            get { return _orderNum; }
            set
            {
                _orderNum = value;
                //_arrayData[12] = value.ToString();
            }
        }

        /// <summary>
        /// 委托类型 市价，限价等
        /// </summary>
        public sbyte OrderType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
                //_arrayData[13] = value.ToString();
            }
        }

        /// <summary>
        /// GTD,DTC,FOK,FAK等
        /// </summary>
        public sbyte OrderLife
        {
            get { return _orderLife; }
            set
            {
                _orderLife = value;
                //_arrayData[14] = value.ToString();
            }
        }

        /// <summary>
        /// 人工录入单，程序传入单
        /// </summary>
        public sbyte OrderInsertType
        {
            get { return _orderInsertType; }
            set
            {
                _orderInsertType = value;
                //_arrayData[15] = value.ToString();
            }
        }

        /// <summary>
        /// 删除标志
        /// </summary>
        public sbyte DelTag
        {
            get { return _delTag; }
            set
            {
                _delTag = value;
                //_arrayData[16] = value.ToString();
            }
        }

        /// <summary>
        /// 委托来源
        /// </summary>
        public sbyte OrderFrom
        {
            get { return _orderFrom; }
            set
            {
                _orderFrom = value;
                //_arrayData[17] = value.ToString();
            }
        }

        /// <summary>
        /// 子委托数量(针对IceBerg)
        /// </summary>
        public short OrderSubNumMin
        {
            get { return _orderSubNumMin; }
            set
            {
                _orderSubNumMin = value;
                //_arrayData[18] = value.ToString();
            }
        }

        /// <summary>
        /// 子委托数量(针对IceBerg)
        /// 用来改单操作，改后的中间缓存大小
        /// </summary>
        public short OrderSubNumMax
        {
            get { return _orderSubNumMax; }
            set
            {
                _orderSubNumMax = value;
                //_arrayData[19] = value.ToString();
            }
        }

        /// <summary>
        /// 用于GTD等
        /// </summary>
        public int OrderExpiryDate
        {
            get { return _orderExpiryDate; }
            set
            {
                _orderExpiryDate = value;
                //_arrayData[20] = value.ToString();
            }
        }


        /// <summary>
        /// 止损触发价
        /// </summary>
        public Decimal TriggerPrice
        {
            get { return _triggerPrice; }
            set
            {
                _triggerPrice = value;
                //_arrayData[21] = value.ToString();
            }
        }

        /// <summary>
        /// 是否T+1
        /// </summary>
        public sbyte OrderRemain
        {
            get { return _orderRemain; }
            set
            {
                _orderRemain = value;
                //_arrayData[22] = value.ToString();
            }
        }

        /// <summary>
        /// 用户输入信息
        /// </summary>
        public string RemarkCus
        {
            get { return _remarkCus; }
            set
            {
                _remarkCus = value;
                //_arrayData[23] = value;
            }
        }

        /// <summary>
        /// 用户交易关联字段1
        /// </summary>
        public string InfoS1
        {
            get { return _infoS1; }
            set
            {
                _infoS1 = value;
                //_arrayData[24] = value;
            }
        }

        /// <summary>
        /// 用户交易关联字段2
        /// </summary>
        public string InfoS2
        {
            get { return _infoS2; }
            set
            {
                _infoS2 = value;
                //_arrayData[25] = value;
            }
        }

        /// <summary>
        /// 本地号
        /// </summary>
        public string LocalNo
        {
            get { return _localNo; }
            set
            {
                _localNo = value;
                //_arrayData[26] = value;
            }
        }

        /// <summary>
        /// 定单号
        /// </summary>
        public Int64 OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
                //_arrayData[27] = value.ToString();
            }
        }


        /// <summary>
        /// 定单日期
        /// </summary>
        public int OrderDate
        {
            get { return _orderDate; }
            set
            {
                _orderDate = value;
                //_arrayData[28] = value.ToString();
            }
        }


        /// <summary>
        /// 定单时间
        /// </summary>
        public int OrderTime
        {
            get { return _orderTime; }
            set
            {
                _orderTime = value;
                //_arrayData[29] = value.ToString();
            }
        }

        /// <summary>
        /// 前置号
        /// </summary>
        public string RouterNo
        {
            get { return _routerNo; }
            set
            {
                _routerNo = value;
                //_arrayData[30] = value.ToString();
            }
        }

        /// <summary>
        /// 交易日期
        /// </summary>
        public int InitDate
        {
            get { return _initDate; }
            set
            {
                _initDate = value;
                //_arrayData[31] = value.ToString();
            }
        }

        /// <summary>
        /// 最后一次修改委托的人
        /// </summary>
        public string UpdatePerson
        {
            get { return _updatePerson; }
            set
            {
                _updatePerson = value;
                //_arrayData[32] = value;
            }
        }

        /// <summary>
        /// 上手代码
        /// </summary>
        public short BrokerNo
        {
            get { return _brokerNo; }
            set
            {
                _brokerNo = value;
                //_arrayData[33] = value.ToString();
            }
        }

        /// <summary>
        /// 委托状态
        /// </summary>
        public char OrderState
        {
            get { return _orderState; }
            set
            {
                _orderState = value;
                //_arrayData[34] = value.ToString();
            }
        }

        /// <summary>
        /// 已成交数量
        /// </summary>
        public short FilledNum
        {
            get { return _filledNum; }
            set
            {
                _filledNum = value;
                //_arrayData[35] = value.ToString();
            }
        }

        /// <summary>
        /// 订单修改日期（本地日期）
        /// </summary>
        public int UpdateDate
        {
            get { return _updateDate; }
            set
            {
                _updateDate = value;
                //_arrayData[36] = value.ToString();
            }
        }

        /// <summary>
        /// 订单修改时间（本地时间）
        /// </summary>
        public int UpdateTime
        {
            get { return _updateTime; }
            set
            {
                _updateTime = value;
                //_arrayData[37] = value.ToString();
            }
        }

        /// <summary>
        /// 交易所日期
        /// </summary>
        public int ExchangeDate
        {
            get { return _exchangeDate; }
            set
            {
                _exchangeDate = value;
                //_arrayData[38] = value.ToString();
            }
        }

        /// <summary>
        /// 交易所时间
        /// </summary>
        public int ExchangeTime
        {
            get { return _exchangeTime; }
            set
            {
                _exchangeTime = value;
                //_arrayData[39] = value.ToString();
            }
        }

        /// <summary>
        /// 内部委托最后流水号
        /// </summary>
        public short OrderJourNo
        {
            get { return _orderJourNo; }
            set
            {
                _orderJourNo = value;
                //_arrayData[40] = value.ToString();
            }
        }

        /// <summary>
        /// 上级委托编号
        /// </summary>
        public string BrokerOrderNo
        {
            get { return _brokerOrderNo; }
            set
            {
                _brokerOrderNo = value;
                //_arrayData[41] = value;
            }
        }

        /// <summary>
        /// 上级委托最后流水号
        /// </summary>
        public string BrokerOrderJourNo
        {
            get { return _brokerOrderJourNo; }
            set
            {
                _brokerOrderJourNo = value;
                //_arrayData[42] = value;
            }
        }

        /// <summary>
        /// 交易所编号
        /// </summary>
        public string ExchangeOrderNo
        {
            get { return _exchangeOrderNo; }
            set
            {
                _exchangeOrderNo = value;
                //_arrayData[43] = value;
            }
        }

        /// <summary>
        /// 用于LME Carry录入后系统自动配对，carryNo由系统自动生成
        /// </summary>
        public string CarryNo
        {
            get { return _carryNo; }
            set
            {
                _carryNo = value;
                //_arrayData[44] = value;
            }
        }

        /// <summary>
        /// 错误编码
        /// </summary>
        public short ErrorNo
        {
            get { return _errorNo; }
            set
            {
                _errorNo = value;
                //_arrayData[45] = value.ToString();
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo
        {
            get { return _errorInfo; }
            set
            {
                _errorInfo = value;
                //_arrayData[46] = value;
            }
        }

        /// <summary>
        /// 上级代码
        /// </summary>
        public short ProxyId
        {
            get { return _proxyId; }
            set
            {
                _proxyId = value;
                //_arrayData[47] = value.ToString();
            }
        }

        /// <summary>
        /// 平仓数量
        /// </summary>
        public int CloseNum
        {
            get { return _closeNum; }
            set
            {
                _closeNum = value;
                //_arrayData[48] = value.ToString();
            }
        }

        /// <summary>
        /// 开仓数量
        /// </summary>
        public int OpenNum
        {
            get { return _openNum; }
            set
            {
                _openNum = value;
                //_arrayData[49] = value.ToString();
            }
        }

        public sbyte Action
        {
            get { return _action; }
            set
            {
                _action = value;
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
            _arrayData[0] = _accountNo;
            _arrayData[1] = _operatorNo;
            _arrayData[2] = _orderPerson;
            _arrayData[3] = _productType.ToString();
            _arrayData[4] = _exchangeCode;
            _arrayData[5] = _commodityCode;
            _arrayData[6] = _contractCode;
            _arrayData[7] = _callPut.ToString();
            _arrayData[8] = _strikePrice;
            _arrayData[9] = _openClose.ToString();
            _arrayData[10] = _buySell.ToString();
            _arrayData[11] = _orderPrice.ToString();
            _arrayData[12] = _orderNum.ToString();
            _arrayData[13] = _orderType.ToString();
            _arrayData[14] = _orderLife.ToString();
            _arrayData[15] = _orderInsertType.ToString();
            _arrayData[16] = _delTag.ToString();
            _arrayData[17] = _orderFrom.ToString();
            _arrayData[18] = _orderSubNumMin.ToString();
            _arrayData[19] = _orderSubNumMax.ToString();
            _arrayData[20] = _orderExpiryDate.ToString();
            _arrayData[21] = _triggerPrice.ToString();
            _arrayData[22] = _orderRemain.ToString();
            _arrayData[23] = _remarkCus;
            _arrayData[24] = _infoS1;
            _arrayData[25] = _infoS2;
            _arrayData[26] = _localNo;
            _arrayData[27] = _orderNo.ToString();
            _arrayData[28] = _orderDate.ToString();
            _arrayData[29] = _orderTime.ToString();
            _arrayData[30] = _routerNo.ToString();
            _arrayData[31] = _initDate.ToString();
            _arrayData[32] = _updatePerson;
            _arrayData[33] = _brokerNo.ToString();
            _arrayData[34] = _orderState.ToString();
            _arrayData[35] = _filledNum.ToString();
            _arrayData[36] = _updateDate.ToString();
            _arrayData[37] = _updateTime.ToString();
            _arrayData[38] = _exchangeDate.ToString();
            _arrayData[39] = _exchangeTime.ToString();
            _arrayData[40] = _orderJourNo.ToString();
            _arrayData[41] = _brokerOrderNo;
            _arrayData[42] = _brokerOrderJourNo;
            _arrayData[43] = _exchangeOrderNo;
            _arrayData[44] = _carryNo;
            _arrayData[45] = _errorNo.ToString();
            _arrayData[46] = _errorInfo;
            _arrayData[47] = _proxyId.ToString();
            _arrayData[48] = _closeNum.ToString();
            _arrayData[49] = _openNum.ToString();
            _arrayData[50] = _action.ToString();
            _arrayData[51] = _msgNum.ToString();

            return _arrayData;
        }

        public bool IsHKFEOption()
        {
            if (ProductType == DictionaryHelper.PRODUCT_TYPE.PRODUCT_TYPE_OPTION && ExchangeCode == DictionaryHelper.Exchange.HKFE)
            {
                return true;
            }
            return false;
        }

        public bool IsNotHKFEOption()
        {
            if (ProductType == DictionaryHelper.PRODUCT_TYPE.PRODUCT_TYPE_OPTION && ExchangeCode != DictionaryHelper.Exchange.HKFE)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 非港交所期权买入
        /// </summary>
        /// <returns></returns>
        public bool IsOptionBuyNotHKFE()
        {
            if (ExchangeCode != DictionaryHelper.Exchange.HKFE && ProductType == DictionaryHelper.PRODUCT_TYPE.PRODUCT_TYPE_OPTION && BuySell == DictionaryHelper.ORDER_DERECTION.BUY)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 非港交所期权卖
        /// </summary>
        /// <returns></returns>
        public bool IsOptionSaleNotHKFE()
        {
            if (ExchangeCode != DictionaryHelper.Exchange.HKFE && ProductType == DictionaryHelper.PRODUCT_TYPE.PRODUCT_TYPE_OPTION && BuySell == DictionaryHelper.ORDER_DERECTION.SALE)
            {
                return true;
            }
            return false;
        }

        public bool IsNotHKFEAndSGX()
        {
            if (ExchangeCode != DictionaryHelper.Exchange.HKFE && ExchangeCode != DictionaryHelper.Exchange.SGX)
            {
                return true;
            }
            return false;
        }

        public bool IsHKFEOrSGX()
        {
            if (ExchangeCode == DictionaryHelper.Exchange.HKFE || ExchangeCode == DictionaryHelper.Exchange.SGX)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 期货 价差或者是港交所期权
        /// </summary>
        /// <returns></returns>
        public bool IsFutSpdBuyOpt()
        {

            return (ProductType == DictionaryHelper.PRODUCT_TYPE.PRODUCT_TYPE_FUTURE || ProductType == DictionaryHelper.PRODUCT_TYPE.PRODUCT_TYPE_SPEARD) || IsHKFEOption();
        }

        public SlmOrder Clone()
        {
            return (SlmOrder)this.MemberwiseClone();

        }

        public void UpdataOrder(SlmOrder order)
        {
            _accountNo = order._accountNo;//0
            _operatorNo = order._operatorNo;//1
            _orderPerson = order._orderPerson;//2
            _productType = order._productType;//3
            _exchangeCode = order._exchangeCode;//4
            _commodityCode = order._commodityCode;//5
            _contractCode = order._contractCode;//6
            _callPut = order._callPut;//7
            _strikePrice = order._strikePrice;//8
            _openClose = order._openClose;//9
            _buySell = order._buySell;//10
            _orderPrice = order._orderPrice;//11
            _orderNum = order._orderNum;//12
            _orderType = order._orderType;//13
            _orderLife = order._orderLife;//14
            _orderInsertType = order._orderInsertType;//15
            _delTag = order._delTag;//16
            _orderFrom = order._orderFrom;//17
            _orderSubNumMin = order._orderSubNumMin;//18
            _orderSubNumMax = order._orderSubNumMax;//19
            _orderExpiryDate = order._orderExpiryDate;//20
            _triggerPrice = order._triggerPrice;//21
            _orderRemain = order._orderRemain;//22
            _remarkCus = order._remarkCus;//23
            _infoS1 = order._infoS1;//24
            _infoS2 = order._infoS2;//25
            _localNo = order._localNo;//26
            _orderNo = order._orderNo;//27
            _orderDate = order._orderDate;//28
            _orderTime = order._orderTime;//29
            _routerNo = order._routerNo;//30
            _initDate = order._initDate;//31
            _updatePerson = order._updatePerson;
            _brokerNo = order._brokerNo;
            _orderState = order._orderState;
            _filledNum = order._filledNum;
            _updateDate = order._updateDate;
            _updateTime = order._updateTime;
            _exchangeDate = order._exchangeDate;
            _exchangeTime = order._exchangeTime;
            _orderJourNo = order._orderJourNo;
            _brokerOrderNo = order._brokerOrderNo;
            _brokerOrderJourNo = order._brokerOrderJourNo;
            _exchangeOrderNo = order._exchangeOrderNo;
            _carryNo = order._carryNo;
            _errorNo = order._errorNo;
            _errorInfo = order._errorInfo;
            _proxyId = order._proxyId;
            _closeNum = order._closeNum; //平仓数量
            _openNum = order._openNum; //开仓数量

        }

        public byte[] GetByteByMsgNum(long msgNum)
        {
            if (orderBytesDic.ContainsKey(MsgNum))
            {
                return orderBytesDic[msgNum];
            }
            return null;
        }

        public void SetByteByMsgNum(long msgNum, byte[] orderByte)
        {
            if (orderBytesDic.ContainsKey(MsgNum))
            {
                return;
            }
            orderBytesDic.Add(msgNum, orderByte);
        }
        #endregion
    }

}
