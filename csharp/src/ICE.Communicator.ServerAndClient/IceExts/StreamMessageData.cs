using Ice;
using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmCommunicator
{
    /// <summary>
    /// ice二制流基础类
    /// 主要用于业务对象与二制流转换
    /// </summary>
    [Serializable]
    public class StreamMessageData
    {
        #region 参数
        #region private
        #endregion
        public StreamPackage _StreamPackage { get; private set; }
        /// <summary>
        /// 对象类型
        /// </summary>
        public PackageOrderType dataType { get; private set; }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType">消息类型</param>
        /// <param name="dataStream">字节流</param>
        /// <param name="i">IceInternal.Instance</param>
        /// <param name="v">Ice.EncodingVersion</param>
        /// <param name="initAttUseDefault">是否使用默认取值，否：通过计算pos，来取值</param>
        public StreamMessageData(PackageOrderType dataType, byte[] dataStream)
        {
            this.DateTime = DateTime.Now;
            this.dataType = dataType;
            _StreamPackage = new StreamPackage() { DataStreamBody = new byte[1][] { dataStream } };
        }

        public StreamMessageData(PackageOrderType dataType, StreamPackage dataStream)
        {
            this.DateTime = DateTime.Now;
            this.dataType = dataType;
            _StreamPackage = dataStream;
        }

        public StreamMessageData(PackageOrderType dataType, byte[] dataStream, InputStream bs)
        {
            this.DateTime = DateTime.Now;
            this.dataType = dataType;
            _StreamPackage = new StreamPackage() { DataStreamBody = new byte[1][] { dataStream } };
            //初启化属性
            InitAttr(bs);
        }

        public StreamMessageData(PackageOrderType dataType, StreamPackage dataStream, InputStream bs)
        {
            this.DateTime = DateTime.Now;
            this.dataType = dataType;
            _StreamPackage = dataStream;
            //初启化属性
            InitAttr(bs);
        }

        #region 主要属性
        #region uuid,ServiceNo
        public int CacheIndex { get; set; }
        public DateTime DateTime { get; private set; }
        /// <summary>
        /// 来此StreamPackage
        /// </summary>
        public int ServiceNo
        {
            get
            {
                if (_StreamPackage != null)
                {
                    return _StreamPackage.ServiceNo;
                }
                return 0;
            }
        }
        public string UUID
        {
            get
            {
                if (_StreamPackage != null)
                {
                    return _StreamPackage.UUID;
                }
                return "";
            }
        }
        public string MsgId
        {
            get
            {
                if (_StreamPackage != null)
                {
                    return _StreamPackage.MsgId;
                }
                return "";
            }
        }
        #endregion
        /// <summary>
        /// 主键类
        /// 通过 InputStream(_io, _v) 字节计算
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 消息号，通用
        /// </summary>
        public long MsgNum { get; private set; }
        public int MsgNumLeftNum { get; private set; }
        public string MsgNumStr { get; private set; }

        #endregion
        #region 初始化属性值
        public void InitAttr(InputStream bs, bool initAttUseDefault = true)
        {
            lock (bs)
            {
                //刷新字节流
                bs.UpdateBuffer(_StreamPackage.DataStreamBody[0]);
                //计算相关属性
                this.MsgNum = GetMsgNum(bs);
                this.MsgNumStr = this.MsgNum.ToString();
                this.MsgNumLeftNum = this.MsgNumStr.GetLeftStr(8).TryToInt();

                if (initAttUseDefault)
                {
                    this.Key = GetKeyByDefault(bs);
                }
            }
        }
        /// <summary>
        /// MsgNum,排在字节流最前面
        /// </summary>
        /// <returns></returns>
        public long GetMsgNum(InputStream bs)
        {
            lock (bs)
            {
                if (bs.getBuffer().b.limit() > 0)
                {
                    bs.pos(0);
                    return bs.readLong();
                }
                return 0;
            }
        }
        /// <summary>
        /// 使用默认方法取值,Slice struct Package.ice 主键要放到最前面
        /// </summary>
        /// <returns></returns>
        public string GetKeyByDefault(InputStream bs)
        {
            lock (this)
            {
                switch (dataType)
                {
                    case PackageOrderType.Orders:
                        //case PackageOrderType.SlmOrderBody:
                        return bs.GetKeyOrder();
                        break;
                    case PackageOrderType.Orderjours:
                        //case PackageOrderType.SlmOrderJourBody:
                        return bs.GetKeyOrderJour();
                        break;
                    case PackageOrderType.Trades:
                        //case PackageOrderType.SlmMatchBody:
                        return bs.GetKeyTrade();
                        break;
                    case PackageOrderType.Deposits:
                        return bs.GetKeyDeposit();
                        break;
                    default:
                        throw new System.Exception($"{dataType}:类型对象不支持GetKeyByDefault");
                        break;
                };
                return string.Empty;
            }
        }
        #endregion
        #region ICE对象属性
        /// <summary>
        /// ICE对象
        /// </summary>
        object _IceObjec;

        public T GetICEObject<T>(InputStream bs, PackageOrderType orderType = PackageOrderType.Unkow)
           where T : class
        {
            lock (bs)
            {
                if (_IceObjec != null)
                {
                    return _IceObjec as T;
                }

                bs.UpdateBuffer(_StreamPackage.DataStreamBody[0]);

                if (orderType != PackageOrderType.Unkow && orderType != dataType)
                {
                    return GetICEObjectByBytes(orderType, bs) as T;
                }

                _IceObjec = GetICEObjectByBytes(dataType, bs);

                return _IceObjec as T;
            }
        }

        private object GetICEObjectByBytes(PackageOrderType orderType, InputStream bs)
        {
            if (bs == null)
            {
                throw new System.Exception("_basicStream参数为空");
            }
            try
            {
                lock (this)
                {
                    bs.pos(0);
                    switch (orderType)
                    {
                        case PackageOrderType.Orders:
                            return bs.GetOrderCacheDB();
                            break;
                        case PackageOrderType.Orderjours:
                            return bs.GetOrderjourCacheDB();
                            break;
                        case PackageOrderType.Trades:
                            return bs.GetTradeCacheDB();
                            break;
                        case PackageOrderType.Deposits:
                            return bs.GetDepositCacheDB();
                            break;
                        case PackageOrderType.SlmOrderBody:
                            return bs.GetSlmOrderRow();
                            break;
                        case PackageOrderType.SlmMatchBody:
                            return bs.GetSlmMatchRow();
                            break;
                        case PackageOrderType.SlmOrderJourBody:
                            return bs.GetSlmOrderJourRow();
                            break;
                        default:
                            throw new System.Exception($"{dataType}:类型对象不支持UpdateReadBufferPos");
                            break;
                    }
                    return null;
                }
            }
            catch (Ice.Exception ex)
            {
                LogHelper.IceError(ex, "GetICEObjectByBytes");
                throw ex;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #region ICEObjectByBytes

        #endregion
        #endregion
        #region 功能方法

        #endregion
    }
    #region ICE对象属性enum
    /// <summary>
    /// 主要用于ICE相关基础参数
    /// </summary>
    public static class ICEParamBase
    {
        #region ICE相关参数
        /// <summary>
        /// 可变类型
        /// </summary>
        public const string sliceVariablLengthType = "string";
        public const int sliceVariablLengthStringType = 9999;
        /// <summary>
        /// https://doc.zeroc.com/ice/3.6/the-slice-language/basic-types
        /// 基础类型，对应byte长度
        /// </summary>
        public static Dictionary<IceSliceTypeEnum, int> sliceSizeDic = new Dictionary<IceSliceTypeEnum, int>()
        {
            {IceSliceTypeEnum.bbool,1 },
            {IceSliceTypeEnum.bbyte,1  },
            {IceSliceTypeEnum.sshort,2  },
            {IceSliceTypeEnum.iint,4  },
            {IceSliceTypeEnum.ffloat,4  },
            {IceSliceTypeEnum.llong,8  },
            {IceSliceTypeEnum.ddouble,8  },//IEEE double-precision
            {IceSliceTypeEnum.sstring,sliceVariablLengthStringType  }//Variable-length
        };
        #endregion
    }

    #region ICE对象枚举
    public enum IceSliceTypeEnum
    {
        /// <summary>
        /// 1 byte
        /// </summary>
        bbool = 0,
        /// <summary>
        /// 1 byte
        /// </summary>
        bbyte = 1,
        /// <summary>
        /// 2 byte
        /// </summary>
        sshort = 2,
        /// <summary>
        /// 4 byte
        /// </summary>
        iint = 3,
        /// <summary>
        /// 4 byte
        /// </summary>
        ffloat = 4,
        /// <summary>
        /// 8 byte
        /// </summary>
        llong = 5,
        /// <summary>
        /// IEEE double-precision 8 byte
        /// </summary>
        ddouble = 6,
        /// <summary>
        /// Variable-length 可变 字符串字节>255 5+n | 1+n
        /// </summary>
        sstring = 7
    }
    public enum SlmOrderRowEnum
    {
        MsgNum = 0,
        InitDate = 1,
        OrderNo = 2,
        OrderState = 3,
        AccountNo = 4,
        OperatorNo = 5,
        OrderPerson = 6,
        ProductType = 7,
        ExchangeCode = 8,
        CommodityCode = 9,
        ContractCode = 10,
        CallPut = 11,
        StrikePrice = 12,
        OpenClose = 13,
        BuySell = 14,
        OrderPrice = 15,
        OrderNum = 16,
        OrderType = 17,
        OrderLife = 18,
        OrderInsertType = 19,
        DelTag = 20,
        OrderFrom = 21,
        OrderSubNumMin = 22,
        OrderSubNumMax = 23,
        OrderExpiryDate = 24,
        TriggerPrice = 25,
        OrderRemain = 26,
        RemarkCus = 27,
        InfoS1 = 28,
        InfoS2 = 29,
        LocalNo = 30,
        OrderDate = 31,
        OrderTime = 32,
        RouterNo = 33,
        UpdatePerson = 34,
        FilledNum = 35,
        UpdateDate = 36,
        UpdateTime = 37,
        ExchangeDate = 38,
        ExchangeTime = 39,
        OrderJourNo = 40,
        BrokerOrderNo = 41,
        BrokerOrderJourNo = 42,
        ExchangeOrderNo = 43,
        CarryNo = 44,
        ErrorNo = 45,
        ErrorInfo = 46,
        ProxyId = 47,
        Action = 48,
    }

    public enum PushMatchStateRowEnum
    {
        MsgNum = 0,
        OrderNo = 1,
        AccountNo = 2,
        TradeDate = 3,
        TradeNo = 4,
        TradeTime = 5,
        TradePrice = 6,
        TradeVol = 7,
        ProxyId = 8,
        BrokerTradeNo = 9,
        ExchangeTradeNo = 10,
        OperatorNo = 11,
        Info1 = 12,
        Info2 = 13,
        Info3 = 14,
        OrderInsertType = 15,
        OrderFrom = 16,
        CommodityCode = 17,
        ContractCode = 18,
        BuySell = 19,
    }

    public enum PushOrderStateRowEnum
    {
        MsgNum = 0,
        OrderNo = 1,
        OrderState = 2,
        OrderJourNo = 3,
        UpdateDate = 4,
        UpdateTime = 5,
        ErrNo = 6,
        ErrInfo = 7,
        OrderPrice = 8,
        OrderVol = 9,
        TriggerPrice = 10,
        ExchangeOrderNo = 11,
        ExchangeDateTime = 12,
        MatchVol = 13,
        BrokerOrderNo = 14,
        BrokerOrderJourNo = 15,
        OrderType = 16,
        OrderFrom = 17,
        Remarks = 18,
        CurMatchVol = 19,
        Action = 20,
    }

    public enum SendOrderStatusRowEnum
    {
        MsgNum = 0,
        OrderNo = 1,
        AccountNo = 2,
        OperatorNo = 3,
        OrderState = 4,
        OrderJourNo = 5,
        UpdateDate = 6,
        UpdateTime = 7,
        ErrorNo = 8,
        ErrorInfo = 9,
        OrderPrice = 10,
        OrderNum = 11,
        TriggerPrice = 12,
        ExchangeDate = 13,
        ExchangeTime = 14,
        ExchangeOrderNo = 15,
        FilledNum = 16,
        BrokerOrderNo = 17,
        BrokerOrderJourNo = 18,
        InfoS1 = 19,
        InfoS2 = 20,
        InitDate = 21,
        OrderType = 22,
        OrderFrom = 23,
        Remarks = 24,
        Action = 25,
    }

    public enum SlmOrderJourRowEnum
    {
        MsgNum = 0,
        InitDate = 1,
        OrderNo = 2,
        OrderJourNo = 3,
        RouterNo = 4,
        OperatorNo = 5,
        OrderPerson = 6,
        UpdatePerson = 7,
        ProxyId = 8,
        AccountNo = 9,
        ProductType = 10,
        ExchangeCode = 11,
        CommodityCode = 12,
        ContractCode = 13,
        CallPut = 14,
        StrikePrice = 15,
        BuySell = 16,
        OrderPrice = 17,
        OrderNum = 18,
        OrderType = 19,
        OrderLife = 20,
        OrderFrom = 21,
        TriggerPrice = 22,
        OrderStatus = 23,
        FilledNum = 24,
        OrderDate = 25,
        OrderTime = 26,
        UpdateDate = 27,
        UpdateTime = 28,
        ExchangeDate = 29,
        ExchangeTime = 30,
        BrokerOrderNo = 31,
        BrokerOrderJourNo = 32,
        ExchangeOrderNo = 33,
        ErrorNo = 34,
        ErrorInfo = 35,
        RemarkCus = 36,
        InfoS1 = 37,
        InfoS2 = 38,
        Action = 39,
    }

    public enum SlmMatchRowEnum
    {
        MsgNum = 0,
        InitDate = 1,
        TradeNo = 2,
        OrderNo = 3,
        AccountNo = 4,
        OrderDate = 5,
        OrderTime = 6,
        TradeDate = 7,
        TradeTime = 8,
        TradePrice = 9,
        TradeVol = 10,
        CurrencyCode = 11,
        IsTplusone = 12,
        OpenClose = 13,
        ProxyId = 14,
        BrokerTradeNo = 15,
        ExchangeTradeNo = 16,
        ProductType = 17,
        ExchangeCode = 18,
        CommodityCode = 19,
        ContractCode = 20,
        BuySell = 21,
        TradeFee = 22,
        OperatorNo = 23,
        Info1 = 24,
        Info2 = 25,
        Info3 = 26,
        OrderInsertType = 27,
        OrderFrom = 28,
    }

    public enum DepositCacheDBEnum
    {
        MsgNum = 0,
        InitDate = 1,
        BusinessNo = 2,
        AccountNo = 3,
        BusinessFlag = 4,
        OccurTime = 5,
        OperatorNo = 6,
        CurrencyCode = 7,
        OccurBalance = 8,
        Remark = 9,
        AuditFlag = 10,
        OccurDate = 11,
        TplusFlag = 12,
    }


    #endregion
    #endregion

    #region StreamMessageDataExt
    public static class StreamMessageDataExt
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="iceObjType">1:SlmOrderBody 2:SlmOrderJourBody 3:SendOrderStatusBody 4:SlmMatchBody 5:PushOrderStateBody 6:PushMatchStateBody</param>
        /// <param name="iOs"></param>
        /// <returns></returns>
        public static StreamMessageData GetStreamMessageData(this StreamPackage p, PackageOrderType iceObjType, InputStream bs)
        {

            if (!p.DataStreamBody.HasItem())
            {
                throw new System.Exception($"{iceObjType}:参数StreamPackage.DataStreamBody为空");
            }

            StreamMessageData r;

            switch (iceObjType)
            {
                case PackageOrderType.Orders:
                    //case PackageOrderType.SlmOrderBody:
                    r = p.GetStreamMessageOrderData(bs);
                    break;
                case PackageOrderType.Orderjours:
                    //case PackageOrderType.SlmOrderJourBody:
                    r = p.GetStreamMessageOrderJourData(bs);
                    break;
                case PackageOrderType.Trades:
                    //case PackageOrderType.SlmMatchBody:
                    r = p.GetStreamMessageTradeData(bs);
                    break;
                case PackageOrderType.Deposits:
                    r = p.GetStreamMessageDepositData(bs);
                    break;
                default:
                    throw new System.Exception($"{iceObjType}:类型对象不支持转换");
                    break;
            }
            return r;
        }
        #region readByteSeq to StreamMessageOrder

        //order
        public static StreamMessageData GetStreamMessageOrderData(this byte[] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Orders, readByteSeq, bs);
            return result;
        }
        public static StreamMessageData GetStreamMessageOrderData(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Orders, sp, bs);
            return result;
        }
        public static StreamMessageData[] GetStreamMessageOrderData(this byte[][] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetStreamMessageOrderData(bs);
            }
            return result;
        }
        public static StreamMessageData[] GetStreamMessageOrderDatas(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData[sp.DataStreamBody.Length];
            for (int i = 0; i < sp.DataStreamBody.Length; i++)
            {
                result[i] = sp.DataStreamBody[i].GetStreamMessageOrderData(bs);
            }
            return result;
        }
        //jour
        public static StreamMessageData GetStreamMessageOrderJourData(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Orderjours, sp, bs);
            return result;
        }
        public static StreamMessageData GetStreamMessageOrderJourData(this byte[] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Orderjours, readByteSeq, bs);
            return result;
        }
        public static StreamMessageData[] GetStreamMessageOrderJourData(this byte[][] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetStreamMessageOrderJourData(bs);
            }
            return result;
        }
        public static StreamMessageData[] GetStreamMessageOrderJourDatas(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData[sp.DataStreamBody.Length];
            for (int i = 0; i < sp.DataStreamBody.Length; i++)
            {
                result[i] = sp.DataStreamBody[i].GetStreamMessageOrderJourData(bs);
            }
            return result;
        }
        //trade
        public static StreamMessageData GetStreamMessageTradeData(this byte[] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Trades, readByteSeq, bs);
            return result;
        }
        public static StreamMessageData GetStreamMessageTradeData(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Trades, sp, bs);
            return result;
        }
        public static StreamMessageData[] GetStreamMessageTradeData(this byte[][] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetStreamMessageTradeData(bs);
            }
            return result;
        }
        public static StreamMessageData[] GetStreamMessageTradeDatas(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData[sp.DataStreamBody.Length];
            for (int i = 0; i < sp.DataStreamBody.Length; i++)
            {
                result[i] = sp.DataStreamBody[i].GetStreamMessageTradeData(bs);
            }
            return result;
        }
        //Deposits
        public static StreamMessageData GetStreamMessageDepositData(this StreamPackage sp, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Deposits, sp, bs);
            return result;
        }
        public static StreamMessageData GetStreamMessageDepositData(this byte[] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData(PackageOrderType.Deposits, readByteSeq, bs);
            return result;
        }
        public static StreamMessageData[] GetStreamMessageDepositDatas(this byte[][] readByteSeq, InputStream bs)
        {
            var result = new StreamMessageData[readByteSeq.Length];
            for (int i = 0; i < readByteSeq.Length; i++)
            {
                result[i] = readByteSeq[i].GetStreamMessageDepositData(bs);
            }
            return result;
        }
        #endregion

        #region stream to ice object
        public static SlmOrderRow GetSlmOrderRow(this InputStream os__)
        {
            var result = SlmCommunicator.SlmOrderRow.ice_read(os__);
            return result;
        }
        public static OrderCacheDB GetOrderCacheDB(this InputStream os__)
        {
            var result = SlmCommunicator.OrderCacheDB.ice_read(os__);
            return result;
        }
        public static SlmOrderJourRow GetSlmOrderJourRow(this InputStream os__)
        {
            var result = SlmCommunicator.SlmOrderJourRow.ice_read(os__);
            return result;
        }
        public static OrderjourCacheDB GetOrderjourCacheDB(this InputStream os__)
        {
            var result = SlmCommunicator.OrderjourCacheDB.ice_read(os__);
            return result;
        }
        public static SlmMatchRow GetSlmMatchRow(this InputStream os__)
        {
            var result = SlmCommunicator.SlmMatchRow.ice_read(os__);
            return result;
        }
        public static TradeCacheDB GetTradeCacheDB(this InputStream os__)
        {
            var result = SlmCommunicator.TradeCacheDB.ice_read(os__);
            return result;
        }
        public static DepositCacheDB GetDepositCacheDB(this InputStream os__)
        {
            var result = SlmCommunicator.DepositCacheDB.ice_read(os__);
            return result;
        }
        #endregion
        #region basicStream基础方法
        /// <summary>
        /// 更新字节流
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="byteList"></param>
        public static void UpdateBuffer(this InputStream bs, byte[] byteList, IceInternal.ByteBuffer.ByteOrder order = IceInternal.ByteBuffer.ByteOrder.LITTLE_ENDIAN)
        {
            if (bs == null)
            {
                return;
            }
            bs.getBuffer().b = IceInternal.ByteBuffer.wrap(byteList);
            bs.getBuffer().b.order(order);
        }

        public static bool readBoolPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readBool();

        }
        public static byte readBytePos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readByte();
        }
        public static int readIntPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readInt();
        }
        public static short readShortPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readShort();
        }
        public static float readFloatPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readFloat();
        }

        public static long readLongPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readLong();
        }
        public static double readDoublePos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readDouble();
        }
        public static string readStringPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            return _basicStream.readString();
        }
        /// <summary>
        /// 获取string字节流对应大小
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static int ReadSizeStrPos(this InputStream _basicStream, int pos)
        {
            _basicStream.pos(pos);
            //string计算可变值
            int tmpPos = _basicStream.readSize();
            if (tmpPos > byte.MaxValue)
            {
                tmpPos += 5;
            }
            else
            {
                tmpPos += 1;
            }
            return tmpPos;
        }
        #endregion
        #region BasicStream GetKey Ext
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicStream"></param>
        /// <param name="PosDic"></param>
        /// <returns>$"o_xxx_xxx"</returns>
        public static string GetKeyOrder(this InputStream basicStream, Dictionary<int, int> PosDic)
        {
            if (basicStream.size() > 0)
            {
                var tmpinitdate = basicStream.readIntPos(PosDic[(int)SlmOrderRowEnum.InitDate]);
                var tmporder_no = basicStream.readLongPos(PosDic[(int)SlmOrderRowEnum.OrderNo]);

                return $"{tmpinitdate}_{tmporder_no}";
            }

            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicStream"></param>
        /// <param name="PosDic"></param>
        /// <returns>$"oj_xxx_xxx_x"</returns>
        public static string GetKeyOrderJour(this InputStream basicStream, Dictionary<int, int> PosDic)
        {
            if (basicStream.size() > 0)
            {
                var tmpinitdate = basicStream.readIntPos(PosDic[(int)SlmOrderJourRowEnum.InitDate]);
                var tmporder_no = basicStream.readLongPos(PosDic[(int)SlmOrderJourRowEnum.OrderNo]);
                var tmporder_noJour = basicStream.readShortPos(PosDic[(int)SlmOrderJourRowEnum.OrderJourNo]);

                return $"{tmpinitdate}_{tmporder_no}_{tmporder_noJour}";
            }

            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicStream"></param>
        /// <param name="PosDic"></param>
        /// <returns>$"t_xxx_xxx"</returns>
        public static string GetKeyTrade(this InputStream basicStream, Dictionary<int, int> PosDic)
        {
            if (basicStream.size() > 0)
            {
                var tmpinitdate = basicStream.readIntPos(PosDic[(int)SlmMatchRowEnum.InitDate]);
                var tmpotrade_no = basicStream.readLongPos(PosDic[(int)SlmMatchRowEnum.TradeNo]);

                return $"{tmpinitdate}_{tmpotrade_no}";
            }

            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicStream"></param>
        /// <param name="PosDic"></param>
        /// <returns>$"d_xxx_xxx"</returns>
        public static string GetKeyDeposit(this InputStream basicStream, Dictionary<int, int> PosDic)
        {
            if (basicStream.size() > 0)
            {
                var tmpinitdate = basicStream.readIntPos(PosDic[(int)DepositCacheDBEnum.InitDate]);
                var tmpobusinessNo = basicStream.readStringPos(PosDic[(int)DepositCacheDBEnum.BusinessNo]);

                return $"{tmpinitdate}_{tmpobusinessNo}";
            }

            return string.Empty;
        }
        #endregion

        #region BasicStream GetKey default Ext
        /// <summary>
        /// 使用默认方法取值,Slice struct Package.ice 主键要放到最前面
        /// </summary>
        /// <param name="bs"></param>
        /// <returns>$"xxx_xxx"</returns>
        public static string GetKeyOrder(this InputStream bs)
        {
            if (bs.getBuffer().b.limit() > 0)
            {
                bs.pos(8);
                var tmpinitdate = bs.readInt();
                var tmporder_no = bs.readLong();

                return $"{tmpinitdate}_{tmporder_no}";
            }

            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bs"></param>
        /// <returns>$"oj_xxx_xxx_x"</returns>
        public static string GetKeyOrderJour(this InputStream bs)
        {
            if (bs.getBuffer().b.limit() > 0)
            {
                bs.pos(8);
                var tmpinitdate = bs.readInt();
                var tmporder_no = bs.readLong();
                var tmporder_noJour = bs.readShort();

                return $"{tmpinitdate}_{tmporder_no}_{tmporder_noJour}";
            }

            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bs"></param>
        /// <returns>$"t_xxx_xxx"</returns>
        public static string GetKeyTrade(this InputStream bs)
        {
            if (bs.getBuffer().b.limit() > 0)
            {
                bs.pos(8);
                var tmpinitdate = bs.readInt();
                var tmpotrade_no = bs.readLong();

                return $"{tmpinitdate}_{tmpotrade_no}";
            }

            return string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicStream"></param>
        /// <returns>$"d_xxx_xxx"</returns>
        public static string GetKeyDeposit(this InputStream basicStream)
        {
            if (basicStream.size() > 0)
            {
                basicStream.pos(8);
                var tmpinitdate = basicStream.readInt();
                var tmpobusinessNo = basicStream.readString();

                return $"{tmpinitdate}_{tmpobusinessNo}";
            }

            return string.Empty;
        }
        #endregion
    }
    #endregion
}
