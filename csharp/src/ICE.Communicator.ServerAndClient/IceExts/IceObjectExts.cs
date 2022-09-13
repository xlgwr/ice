using SlmCommunicator;
using SlmCommunicator.Server;
using Solomon.Log;
using Solomon.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trade.CommonTools;
using Trade.DataType;

namespace SlmCommunicator
{
    /// <summary>
    /// 基础类型转换处理
    /// </summary>
    public static class IceClientObjectExts
    {
        /// <summary>
        /// 通用反射进行类型转换
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static R Mapping<R, T>(T model)
        {
            R result = Activator.CreateInstance<R>();
            foreach (PropertyInfo info in typeof(R).GetProperties())
            {
                PropertyInfo pro = typeof(T).GetProperty(info.Name);
                if (pro != null)
                    info.SetValue(result, pro.GetValue(model));
            }
            return result;
        }

        public static RequestPackageEx GetRequestPackageEx(this RequestPackageActiveSendEx curr)
        {
            RequestPackageEx requestPackage = new RequestPackageEx()
            {
                PackageData = new RequestPackage()
                {
                    ClientId = curr.ClientId,
                    DataBody = curr.DataBody,
                    Head = curr.Head,
                    ServiceNo = curr.ServiceNo,
                    UUID = curr.UUID
                },
                ServiceNo = curr.ServiceNo,
            };
            return requestPackage;
        }

        public static CacheTradeDB GetResponseCacheTradeDB(this CacheTradeDBEx curr)
        {
            CacheTradeDB requestPackage = new CacheTradeDB();

            requestPackage.PackageData = curr.PackageData.GetResponsePackageCacheDB();

            if (curr.Orders != null)
            {
                requestPackage.Orders = new OrderCacheDB[curr.Orders.Length];
                for (int i = 0; i < curr.Orders.Length; i++)
                {
                    requestPackage.Orders[i] = curr.Orders[i].GetOrderCacheDB();
                }
            }

            if (curr.Orderjours != null)
            {
                requestPackage.Orderjours = new OrderjourCacheDB[curr.Orderjours.Length];
                for (int i = 0; i < curr.Orderjours.Length; i++)
                {
                    requestPackage.Orderjours[i] = curr.Orderjours[i].GetOrderjourCacheDB();
                }
            }

            if (curr.Trades != null)
            {
                requestPackage.Trades = new TradeCacheDB[curr.Trades.Length];
                for (int i = 0; i < curr.Trades.Length; i++)
                {
                    requestPackage.Trades[i] = curr.Trades[i].GetTradeCacheDB();
                }
            }

            if (curr.Deposits != null)
            {
                requestPackage.Deposits = new DepositCacheDB[curr.Deposits.Length];
                for (int i = 0; i < curr.Deposits.Length; i++)
                {
                    requestPackage.Deposits[i] = curr.Deposits[i].GetDepositCacheDB();
                }
            }

            if (curr.PrePositions != null)
            {
                requestPackage.PrePositions = new PositionCacheDB[curr.PrePositions.Length];
                for (int i = 0; i < curr.PrePositions.Length; i++)
                {
                    requestPackage.PrePositions[i] = curr.PrePositions[i].GetPositionCacheDB();
                }
            }

            if (curr.Positions != null)
            {
                requestPackage.Positions = new PositionCacheDB[curr.Positions.Length];
                for (int i = 0; i < curr.Positions.Length; i++)
                {
                    requestPackage.Positions[i] = curr.Positions[i].GetPositionCacheDB();
                }
            }

            if (curr.PackageData.DatasDic != null)
            {
                requestPackage.PackageData.DatasDic = new Dictionary<string, string>();
                foreach (var item in curr.PackageData.DatasDic)
                {
                    requestPackage.PackageData.DatasDic.Add(item.Key, item.Value);
                }
            }
            return requestPackage;
        }

        public static PackageCacheDBEx GetResponsePackageCacheDBEx(this PackageCacheDB curr)
        {
            PackageCacheDBEx requestPackage = new PackageCacheDBEx()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                MsgId = curr.MsgId,
                DataBody = curr.DataBody,
                Head = curr.Head,
                ErrorInfo = curr.ErrorInfo,
                ErrorNo = curr.ErrorNo,
                PagingInfo = new PagingInfo()
                {
                }
            };
            return requestPackage;
        }

        public static RequestPackageActiveSendEx GetRequestPackageActiveSendEx(this ResponsePackage curr)
        {
            RequestPackageActiveSendEx requestPackageEx = new RequestPackageActiveSendEx()
            {
                ServiceNo = curr.ServiceNo,
                ClientId = curr.ClientId,
                Head = curr.Head,
                UUID = curr.UUID,
                DataBody = curr.DataBody[0]
            };
            return requestPackageEx;
        }

        public static OrderCacheDB GetOrderCacheDB(this SlmOrder m)

        {
            OrderCacheDB curr = new OrderCacheDB();

            curr.InitDate = m.InitDate;

            curr.RouterNo = m.RouterNo;

            curr.OperatorNo = m.OperatorNo;

            curr.OrderPerson = m.OrderPerson;

            curr.UpdatePerson = m.UpdatePerson;

            curr.ProxyId = (short)m.ProxyId;

            curr.AccountNo = m.AccountNo;

            curr.ProductType = (byte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.CallPut = (byte)m.CallPut;

            curr.StrikePrice = m.StrikePrice;

            curr.OpenClose = (byte)m.OpenClose;

            curr.BuySell = (byte)m.BuySell;

            curr.OrderPrice = m.OrderPrice.TryToDouble();

            curr.OrderNum = m.OrderNum;

            curr.OrderType = (byte)m.OrderType;

            curr.OrderLife = (byte)m.OrderLife;

            curr.OrderInsertType = (byte)m.OrderInsertType;

            curr.DelTag = (byte)m.DelTag;

            curr.OrderFrom = (byte)m.OrderFrom;

            curr.OrderSubNumMin = m.OrderSubNumMin;

            curr.OrderSubNumMax = m.OrderSubNumMax;

            curr.OrderExpiryDate = m.OrderExpiryDate;

            curr.TriggerPrice = m.TriggerPrice.TryToDouble();

            curr.OrderState = m.OrderState.ToByte();

            curr.FilledNum = m.FilledNum;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.UpdateDate = m.UpdateDate;

            curr.UpdateTime = m.UpdateTime;

            curr.ExchangeDate = m.ExchangeDate;

            curr.ExchangeTime = m.ExchangeTime;

            curr.OrderRemain = (byte)m.OrderRemain;

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

            curr.MsgNum = m.MsgNum;

            curr.Action = (byte)m.Action;

            return curr;
        }
        public static OrderjourCacheDB GetOrderjourCacheDB(this SlmOrderJour m)

        {

            OrderjourCacheDB curr = new OrderjourCacheDB();

            curr.InitDate = m.InitDate;

            curr.RouterNo = m.RouterNo;

            curr.OperatorNo = m.OperatorNo;

            curr.OrderPerson = m.OrderPerson;

            curr.UpdatePerson = m.UpdatePerson;

            curr.ProxyId = (short)m.ProxyId;

            curr.AccountNo = m.AccountNo;

            curr.ProductType = (byte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.CallPut = (byte)m.CallPut;

            curr.StrikePrice = m.StrikePrice;


            curr.BuySell = (byte)m.BuySell;

            curr.OrderPrice = m.OrderPrice.TryToDouble();

            curr.OrderNum = (short)m.OrderNum;

            curr.OrderType = (byte)m.OrderType;

            curr.OrderLife = (byte)m.OrderLife;



            curr.OrderFrom = m.OrderFrom.TryToByte();



            curr.TriggerPrice = m.TriggerPrice.TryToDouble();

            curr.OrderStatus = m.OrderStatus.ToByte();

            curr.FilledNum = m.FilledNum;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.UpdateDate = m.UpdateDate;

            curr.UpdateTime = m.UpdateTime;

            curr.ExchangeDate = m.ExchangeDate;

            curr.ExchangeTime = m.ExchangeTime;


            curr.OrderNo = m.OrderNo;

            curr.OrderJourNo = (short)m.OrderJourNo;

            curr.BrokerOrderNo = m.BrokerOrderNo;

            curr.BrokerOrderJourNo = m.BrokerOrderJourNo;

            curr.ExchangeOrderNo = m.ExchangeOrderNo;



            curr.ErrorNo = (short)m.ErrorNo;

            curr.ErrorInfo = m.ErrorInfo;

            curr.RemarkCus = m.RemarkCus;

            curr.InfoS1 = m.InfoS1;

            curr.InfoS2 = m.InfoS2;

            curr.MsgNum = m.MsgNum;

            curr.Action = (byte)m.Action;

            return curr;

        }
        public static PositionCacheDB GetPositionCacheDB(this Position m)
        {

            PositionCacheDB curr = new PositionCacheDB();

            curr.InitDate = m.init_date.TryToInt();

            curr.AccountNo = m.account_no;

            curr.OpenDate = m.open_date.TryToInt();

            curr.OpenNo = m.open_no.TryToLong();

            curr.OpenPrice = m.open_price.TryToDouble();

            curr.OpenNum = m.open_num.TryToInt();

            curr.CurrencyCode = m.currency_code;

            curr.ProxyId = m.proxy_id.TryToInt();

            curr.ProductType = m.product_type.TryToByte();

            curr.ExchangeCode = m.exchange_code;

            curr.CommodityCode = m.commodity_code;

            curr.ContractCode = m.contract_code;

            curr.BuySell = m.buy_sell.TryToByte();

            curr.ExpiryDate = m.expiry_date.TryToInt();

            curr.LastTradeDate = m.last_trade_date.TryToInt();

            //curr.MarketValue = m.market_value.TryToDecimal();

            return curr;

        }
        public static PositionCacheDB GetPositionCacheDB(this SlmPosition m)
        {

            PositionCacheDB curr = new PositionCacheDB();
            curr.InitDate = m.InitDate;

            curr.AccountNo = m.AccountNo;

            curr.OpenDate = m.OpenDate;

            curr.OpenNo = m.OpenNo;

            curr.OpenPrice = m.OpenPrice.TryToDouble();

            curr.OpenNum = m.OpenNum;

            curr.CurrencyCode = m.CurrencyCode;

            curr.ProxyId = (short)m.ProxyId;

            curr.ProductType = (byte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.BuySell = (byte)m.BuySell;

            curr.ExpiryDate = m.ExpiryDate;

            curr.LastTradeDate = m.LastTradeDate;

            curr.MarketValue = m.MarketValue.TryToDouble();
            return curr;

        }

        public static TradeCacheDB GetTradeCacheDB(this SlmMatch m)
        {
            TradeCacheDB curr = new TradeCacheDB();
            curr.InitDate = m.InitDate;

            curr.AccountNo = m.AccountNo;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.OrderNo = m.OrderNo;

            curr.TradeDate = m.TradeDate;

            curr.TradeNo = m.TradeNo;

            curr.TradeTime = m.TradeTime;

            curr.TradePrice = m.TradePrice.TryToDouble();

            curr.TradeVol = m.TradeVol;

            curr.CurrencyCode = m.CurrencyCode;

            curr.IsTplusone = (byte)m.IsTplusone;

            curr.OpenClose = (byte)m.OpenClose;

            curr.ProxyId = (short)m.ProxyId;

            curr.BrokerTradeNo = m.BrokerTradeNo;

            curr.ExchangeTradeNo = m.ExchangeTradeNo;

            curr.ProductType = (byte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.BuySell = (byte)m.BuySell;

            curr.TradeFee = m.TradeFee.ToString().TryToFloat();

            curr.OperatorNo = m.OperatorNo;

            curr.Info1 = m.Info1;

            curr.Info2 = m.Info2;

            curr.Info3 = m.Info3;

            curr.OrderInsertType = (byte)m.OrderInsertType;

            curr.OrderFrom = m.OrderFrom.TryToByte();

            curr.MsgNum = m.MsgNum;

            return curr;

        }
        public static CacheTradeDBEx GetResponseCacheTradeDBEx(this CacheTradeDB curr)
        {
            CacheTradeDBEx requestPackage = new CacheTradeDBEx();

            requestPackage.PackageData = curr.PackageData.GetResponsePackageCacheDBEx();

            if (curr.Orders != null)
            {
                requestPackage.Orders = new SlmOrder[curr.Orders.Length];
                for (int i = 0; i < curr.Orders.Length; i++)
                {
                    requestPackage.Orders[i] = curr.Orders[i].GetSlmOrder();
                }
            }

            if (curr.Orderjours != null)
            {
                requestPackage.Orderjours = new SlmOrderJour[curr.Orderjours.Length];
                for (int i = 0; i < curr.Orderjours.Length; i++)
                {
                    requestPackage.Orderjours[i] = curr.Orderjours[i].GetSlmOrderJour();
                }
            }

            if (curr.Trades != null)
            {
                requestPackage.Trades = new SlmMatch[curr.Trades.Length];
                for (int i = 0; i < curr.Trades.Length; i++)
                {
                    requestPackage.Trades[i] = curr.Trades[i].GetSlmMatch();
                }
            }

            if (curr.PrePositions != null)
            {
                requestPackage.PrePositions = new SlmPosition[curr.PrePositions.Length];
                for (int i = 0; i < curr.PrePositions.Length; i++)
                {
                    requestPackage.PrePositions[i] = curr.PrePositions[i].GetSlmPosition();
                }
            }

            if (curr.Positions != null)
            {
                requestPackage.Positions = new SlmPosition[curr.Positions.Length];
                for (int i = 0; i < curr.Positions.Length; i++)
                {
                    requestPackage.Positions[i] = curr.Positions[i].GetSlmPosition();
                }
            }

            if (curr.PackageData.DatasDic != null)
            {
                requestPackage.PackageData.DatasDic = new Dictionary<string, string>();
                foreach (var item in curr.PackageData.DatasDic)
                {
                    requestPackage.PackageData.DatasDic.Add(item.Key,item.Value);
                }
            }


            return requestPackage;
        }
        /// <summary>
        /// 类型转换，后期需转换byte
        /// </summary>
        /// <param name="curr"></param>
        /// <returns></returns>
        public static SlmOrder GetSlmOrder(this SlmOrderRow curr)
        {
            SlmOrder result = new SlmOrder();
            result.AccountNo = curr.AccountNo;
            result.OperatorNo = curr.OperatorNo;
            result.OrderPerson = curr.OrderPerson;
            result.ProductType = (sbyte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.CallPut = (sbyte)curr.CallPut;
            result.StrikePrice = curr.StrikePrice;
            result.OpenClose = (sbyte)curr.OpenClose;
            result.BuySell = (sbyte)curr.BuySell;
            result.OrderPrice = (decimal)curr.OrderPrice;
            result.OrderNum = curr.OrderNum;
            result.OrderType = (sbyte)curr.OrderType;
            result.OrderLife = (sbyte)curr.OrderLife;
            result.OrderInsertType = (sbyte)curr.OrderInsertType;
            result.DelTag = (sbyte)curr.DelTag;
            result.OrderFrom = (sbyte)curr.OrderFrom;
            result.OrderSubNumMin = curr.OrderSubNumMin;
            result.OrderSubNumMax = curr.OrderSubNumMax;
            result.OrderExpiryDate = curr.OrderExpiryDate;
            result.TriggerPrice = (decimal)curr.TriggerPrice;
            result.OrderRemain = (sbyte)curr.OrderRemain;
            result.RemarkCus = curr.RemarkCus;
            result.InfoS1 = curr.InfoS1;
            result.InfoS2 = curr.InfoS2;
            result.LocalNo = curr.LocalNo;
            result.OrderNo = curr.OrderNo;
            result.OrderDate = curr.OrderDate;
            result.OrderTime = curr.OrderTime;
            result.RouterNo = curr.RouterNo;
            result.InitDate = curr.InitDate;
            result.UpdatePerson = curr.UpdatePerson;
            result.OrderState = curr.OrderState.ToChar();
            result.FilledNum = curr.FilledNum;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ExchangeDate = curr.ExchangeDate;
            result.ExchangeTime = curr.ExchangeTime;
            result.OrderJourNo = curr.OrderJourNo;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.CarryNo = curr.CarryNo;
            result.ErrorNo = curr.ErrorNo;
            result.ErrorInfo = curr.ErrorInfo;
            result.ProxyId = curr.ProxyId;
            result.Action = (sbyte)curr.Action;
            result.MsgNum = curr.MsgNum;
            return result;
        }
        /// <summary>
        /// 类型转换，后期需转换byte
        /// </summary>
        /// <param name="curr"></param>
        /// <returns></returns>
        public static SlmOrderRow GetSlmOrderRow(this SlmOrder curr)
        {
            SlmOrderRow result = new SlmOrderRow();
            result.AccountNo = curr.AccountNo;
            result.OperatorNo = curr.OperatorNo;
            result.OrderPerson = curr.OrderPerson;
            result.ProductType = (byte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.CallPut = (byte)curr.CallPut;
            result.StrikePrice = curr.StrikePrice;
            result.OpenClose = (byte)curr.OpenClose;
            result.BuySell = (byte)curr.BuySell;
            result.OrderPrice = (double)curr.OrderPrice;
            result.OrderNum = curr.OrderNum;
            result.OrderType = (byte)curr.OrderType;
            result.OrderLife = (byte)curr.OrderLife;
            result.OrderInsertType = (byte)curr.OrderInsertType;
            result.DelTag = (byte)curr.DelTag;
            result.OrderFrom = (byte)curr.OrderFrom;
            result.OrderSubNumMin = curr.OrderSubNumMin;
            result.OrderSubNumMax = curr.OrderSubNumMax;
            result.OrderExpiryDate = curr.OrderExpiryDate;
            result.TriggerPrice = (double)curr.TriggerPrice;
            result.OrderRemain = (byte)curr.OrderRemain;
            result.RemarkCus = curr.RemarkCus;
            result.InfoS1 = curr.InfoS1;
            result.InfoS2 = curr.InfoS2;
            result.LocalNo = curr.LocalNo;
            result.OrderNo = curr.OrderNo;
            result.OrderDate = curr.OrderDate;
            result.OrderTime = curr.OrderTime;
            result.RouterNo = curr.RouterNo;
            result.InitDate = curr.InitDate;
            result.UpdatePerson = curr.UpdatePerson;
            result.OrderState = curr.OrderState.ToByte();
            result.FilledNum = curr.FilledNum;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ExchangeDate = curr.ExchangeDate;
            result.ExchangeTime = curr.ExchangeTime;
            result.OrderJourNo = curr.OrderJourNo;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.CarryNo = curr.CarryNo;
            result.ErrorNo = curr.ErrorNo;
            result.ErrorInfo = curr.ErrorInfo;
            result.ProxyId = curr.ProxyId;
            result.Action = (byte)curr.Action;
            result.MsgNum = curr.MsgNum;

            return result;
        }

        public static PushMatchState GetPushMatchState(this PushMatchStateRow curr)
        {
            var result = new PushMatchState();
            result.AccountNo = curr.AccountNo;
            result.OrderNo = curr.OrderNo;
            result.TradeDate = curr.TradeDate;
            result.TradeNo = curr.TradeNo;
            result.TradeTime = curr.TradeTime;
            result.TradePrice = (decimal)curr.TradePrice;
            result.TradeVol = curr.TradeVol;
            result.ProxyId = curr.ProxyId;
            result.BrokerTradeNo = curr.BrokerTradeNo;
            result.ExchangeTradeNo = curr.ExchangeTradeNo;
            result.OperatorNo = curr.OperatorNo;
            result.Info1 = curr.Info1;
            result.Info2 = curr.Info2;
            result.Info3 = curr.Info3;
            result.OrderInsertType = (sbyte)curr.OrderInsertType;
            result.OrderFrom = curr.OrderFrom.ToString();
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = curr.BuySell;
            return result;
        }
        public static PushMatchStateRow GetPushMatchStateRow(this PushMatchState curr)
        {
            var result = new PushMatchStateRow();
            result.AccountNo = curr.AccountNo;
            result.OrderNo = curr.OrderNo;
            result.TradeDate = curr.TradeDate;
            result.TradeNo = curr.TradeNo;
            result.TradeTime = curr.TradeTime;
            result.TradePrice = (double)curr.TradePrice;
            result.TradeVol = curr.TradeVol;
            result.ProxyId = curr.ProxyId;
            result.BrokerTradeNo = curr.BrokerTradeNo;
            result.ExchangeTradeNo = curr.ExchangeTradeNo;
            result.OperatorNo = curr.OperatorNo;
            result.Info1 = curr.Info1;
            result.Info2 = curr.Info2;
            result.Info3 = curr.Info3;
            result.OrderInsertType = (byte)curr.OrderInsertType;
            result.OrderFrom = curr.OrderFrom.TryToByte();
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = curr.BuySell;

            return result;
        }
        public static PushOrderState GetPushOrderState(this PushOrderStateRow curr)
        {
            var result = new PushOrderState();
            result.OrderNo = curr.OrderNo;
            result.OrderState = curr.OrderState.ToChar();
            result.OrderJourNo = curr.OrderJourNo;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ErrNo = curr.ErrNo;
            result.ErrInfo = curr.ErrInfo;
            result.OrderPrice = (decimal)curr.OrderPrice;
            result.OrderVol = curr.OrderVol;
            result.TriggerPrice = (decimal)curr.TriggerPrice;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.ExchangeDateTime = curr.ExchangeDateTime;
            result.MatchVol = curr.MatchVol;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.OrderType = curr.OrderType.TryToSByte();
            result.CurMatchVol = curr.CurMatchVol;
            result.Action = curr.Action.TryToSByte();

            return result;
        }
        public static PushOrderStateRow GetPushOrderStateRow(this PushOrderState curr)
        {
            var result = new PushOrderStateRow();
            result.OrderNo = curr.OrderNo;
            result.OrderState = curr.OrderState.ToByte();
            result.OrderJourNo = curr.OrderJourNo;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ErrNo = curr.ErrNo;
            result.ErrInfo = curr.ErrInfo;
            result.OrderPrice = (double)curr.OrderPrice;
            result.OrderVol = curr.OrderVol;
            result.TriggerPrice = (double)curr.TriggerPrice;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.ExchangeDateTime = curr.ExchangeDateTime;
            result.MatchVol = curr.MatchVol;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.OrderType = (byte)curr.OrderType;
            result.CurMatchVol = curr.CurMatchVol;
            result.Action = (byte)curr.Action;

            return result;
        }

        public static SendOrderStatus GetSendOrderStatus(this SendOrderStatusRow curr)
        {
            var result = new SendOrderStatus();
            result.AccountNo = curr.AccountNo;
            result.OperatorNo = curr.OperatorNo;
            result.OrderNo = curr.OrderNo;
            result.OrderState = curr.OrderState.ToChar();
            result.OrderJourNo = curr.OrderJourNo;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ErrorNo = curr.ErrorNo;
            result.ErrorInfo = curr.ErrorInfo;
            result.OrderPrice = (decimal)curr.OrderPrice;
            result.OrderNum = curr.OrderNum;
            result.TriggerPrice = (decimal)curr.TriggerPrice;
            result.ExchangeDate = curr.ExchangeDate;
            result.ExchangeTime = curr.ExchangeTime;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.FilledNum = curr.FilledNum;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.InfoS1 = curr.InfoS1;
            result.InfoS2 = curr.InfoS2;
            result.InitDate = curr.InitDate;
            result.OrderType = (sbyte)curr.OrderType;
            result.OrderFrom = (sbyte)curr.OrderFrom;
            result.Remarks = curr.Remarks;
            result.Action = (sbyte)curr.Action;


            return result;
        }
        public static SendOrderStatusRow GetSendOrderStatusRow(this SendOrderStatus curr)
        {
            var result = new SendOrderStatusRow();
            result.AccountNo = curr.AccountNo;
            result.OperatorNo = curr.OperatorNo;
            result.OrderNo = curr.OrderNo;
            result.OrderState = curr.OrderState.ToByte();
            result.OrderJourNo = curr.OrderJourNo;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ErrorNo = curr.ErrorNo;
            result.ErrorInfo = curr.ErrorInfo;
            result.OrderPrice = (double)curr.OrderPrice;
            result.OrderNum = curr.OrderNum;
            result.TriggerPrice = (double)curr.TriggerPrice;
            result.ExchangeDate = curr.ExchangeDate;
            result.ExchangeTime = curr.ExchangeTime;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.FilledNum = curr.FilledNum;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.InfoS1 = curr.InfoS1;
            result.InfoS2 = curr.InfoS2;
            result.InitDate = curr.InitDate;
            result.OrderType = (byte)curr.OrderType;
            result.OrderFrom = (byte)curr.OrderFrom;
            result.Remarks = curr.Remarks;
            result.Action = (byte)curr.Action;

            return result;
        }


        public static SlmOrderJour GetSlmOrderJour(this SlmOrderJourRow curr)
        {
            var result = new SlmOrderJour();
            result.InitDate = curr.InitDate;
            result.RouterNo = curr.RouterNo;
            result.OperatorNo = curr.OperatorNo;
            result.OrderPerson = curr.OrderPerson;
            result.UpdatePerson = curr.UpdatePerson;
            result.ProxyId = curr.ProxyId;
            result.AccountNo = curr.AccountNo;
            result.ProductType = (sbyte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.CallPut = (sbyte)curr.CallPut;
            result.StrikePrice = curr.StrikePrice;
            result.BuySell = (sbyte)curr.BuySell;
            result.OrderPrice = (decimal)curr.OrderPrice;
            result.OrderNum = curr.OrderNum;
            result.OrderType = (sbyte)curr.OrderType;
            result.OrderLife = (sbyte)curr.OrderLife;
            result.OrderFrom = (sbyte)curr.OrderFrom;
            result.TriggerPrice = (decimal)curr.TriggerPrice;
            result.OrderStatus = curr.OrderStatus.ToChar();
            result.FilledNum = curr.FilledNum;
            result.OrderDate = curr.OrderDate;
            result.OrderTime = curr.OrderTime;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ExchangeDate = curr.ExchangeDate;
            result.ExchangeTime = curr.ExchangeTime;
            result.OrderNo = curr.OrderNo;
            result.OrderJourNo = curr.OrderJourNo;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.ErrorNo = curr.ErrorNo;
            result.ErrorInfo = curr.ErrorInfo;
            result.RemarkCus = curr.RemarkCus;
            result.InfoS1 = curr.InfoS1;
            result.InfoS2 = curr.InfoS2;

            result.Action = (sbyte)curr.Action;
            result.MsgNum = curr.MsgNum;

            return result;
        }
        public static SlmOrderJourRow GetSlmOrderJourRow(this SlmOrderJour curr)
        {
            var result = new SlmOrderJourRow();
            result.InitDate = curr.InitDate;
            result.RouterNo = curr.RouterNo;
            result.OperatorNo = curr.OperatorNo;
            result.OrderPerson = curr.OrderPerson;
            result.UpdatePerson = curr.UpdatePerson;
            result.ProxyId = curr.ProxyId;
            result.AccountNo = curr.AccountNo;
            result.ProductType = (byte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.CallPut = (byte)curr.CallPut;
            result.StrikePrice = curr.StrikePrice;
            result.BuySell = (byte)curr.BuySell;
            result.OrderPrice = (double)curr.OrderPrice;
            result.OrderNum = curr.OrderNum;
            result.OrderType = (byte)curr.OrderType;
            result.OrderLife = (byte)curr.OrderLife;
            result.OrderFrom = (byte)curr.OrderFrom;
            result.TriggerPrice = (double)curr.TriggerPrice;
            result.OrderStatus = (byte)curr.OrderStatus;
            result.FilledNum = curr.FilledNum;
            result.OrderDate = curr.OrderDate;
            result.OrderTime = curr.OrderTime;
            result.UpdateDate = curr.UpdateDate;
            result.UpdateTime = curr.UpdateTime;
            result.ExchangeDate = curr.ExchangeDate;
            result.ExchangeTime = curr.ExchangeTime;
            result.OrderNo = curr.OrderNo;
            result.OrderJourNo = curr.OrderJourNo;
            result.BrokerOrderNo = curr.BrokerOrderNo;
            result.BrokerOrderJourNo = curr.BrokerOrderJourNo;
            result.ExchangeOrderNo = curr.ExchangeOrderNo;
            result.ErrorNo = curr.ErrorNo;
            result.ErrorInfo = curr.ErrorInfo;
            result.RemarkCus = curr.RemarkCus;
            result.InfoS1 = curr.InfoS1;
            result.InfoS2 = curr.InfoS2;
            result.Action = (byte)curr.Action;
            result.MsgNum = curr.MsgNum;

            return result;
        }
        public static SlmMatch GetSlmMatch(this SlmMatchRow curr)
        {
            var result = new SlmMatch();
            result.InitDate = curr.InitDate;
            result.AccountNo = curr.AccountNo;
            result.OrderDate = curr.OrderDate;
            result.OrderTime = curr.OrderTime;
            result.OrderNo = curr.OrderNo;
            result.TradeDate = curr.TradeDate;
            result.TradeNo = curr.TradeNo;
            result.TradeTime = curr.TradeTime;
            result.TradePrice = (decimal)curr.TradePrice;
            result.TradeVol = curr.TradeVol;
            result.CurrencyCode = curr.CurrencyCode;
            result.IsTplusone = (sbyte)curr.IsTplusone;
            result.OpenClose = (sbyte)curr.OpenClose;
            result.ProxyId = curr.ProxyId;
            result.BrokerTradeNo = curr.BrokerTradeNo;
            result.ExchangeTradeNo = curr.ExchangeTradeNo;
            result.ProductType = (sbyte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = (sbyte)curr.BuySell;
            result.TradeFee = (decimal)curr.TradeFee;
            result.OperatorNo = curr.OperatorNo;
            result.Info1 = curr.Info1;
            result.Info2 = curr.Info2;
            result.Info3 = curr.Info3;
            result.OrderInsertType = (sbyte)curr.OrderInsertType;
            result.OrderFrom = curr.OrderFrom.ToString();
            result.MsgNum = curr.MsgNum;

            return result;
        }
        public static SlmMatchRow GetSlmMatchRow(this SlmMatch curr)
        {
            var result = new SlmMatchRow();
            result.InitDate = curr.InitDate;
            result.AccountNo = curr.AccountNo;
            result.OrderDate = curr.OrderDate;
            result.OrderTime = curr.OrderTime;
            result.OrderNo = curr.OrderNo;
            result.TradeDate = curr.TradeDate;
            result.TradeNo = curr.TradeNo;
            result.TradeTime = curr.TradeTime;
            result.TradePrice = (double)curr.TradePrice;
            result.TradeVol = curr.TradeVol;
            result.CurrencyCode = curr.CurrencyCode;
            result.IsTplusone = (byte)curr.IsTplusone;
            result.OpenClose = (byte)curr.OpenClose;
            result.ProxyId = curr.ProxyId;
            result.BrokerTradeNo = curr.BrokerTradeNo;
            result.ExchangeTradeNo = curr.ExchangeTradeNo;
            result.ProductType = (byte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = (byte)curr.BuySell;
            result.TradeFee = (double)curr.TradeFee;
            result.OperatorNo = curr.OperatorNo;
            result.Info1 = curr.Info1;
            result.Info2 = curr.Info2;
            result.Info3 = curr.Info3;
            result.OrderInsertType = (byte)curr.OrderInsertType;
            result.OrderFrom = curr.OrderFrom.TryToByte();
            result.MsgNum = curr.MsgNum;

            return result;
        }

        public static SlmMatch[] GetSlmMatchs(this SlmMatchRow[] curr)
        {
            if (curr == null || curr.Length <= 0)
            {
                return null;
            }
            var result = new SlmMatch[curr.Length];
            for (int i = 0; i < curr.Length; i++)
            {
                result[i] = curr[i].GetSlmMatch();
            }
            return result;
        }
        public static SlmMatchRow[] GetSlmMatchRows(this SlmMatch[] curr)
        {
            if (curr == null || curr.Length <= 0)
            {
                return null;
            }
            var result = new SlmMatchRow[curr.Length];
            for (int i = 0; i < curr.Length; i++)
            {
                result[i] = curr[i].GetSlmMatchRow();
            }
            return result;
        }
        public static SlmPositionRow GetSlmPositionRow(this SlmPosition curr)
        {
            var result = new SlmPositionRow();
            result.InitDate = curr.InitDate;
            result.AccountNo = curr.AccountNo;
            result.OpenDate = curr.OpenDate;
            result.OpenNo = curr.OpenNo;
            result.OpenPrice = (double)curr.OpenPrice;
            result.OpenNum = curr.OpenNum;
            result.CurrencyCode = curr.CurrencyCode;
            result.ProxyId = curr.ProxyId;
            result.ProductType = (byte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = (byte)curr.BuySell;
            result.ExpiryDate = curr.ExpiryDate;
            result.LastTradeDate = curr.LastTradeDate;
            result.MarketValue = (double)curr.MarketValue;

            return result;
        }
        public static SlmPosition GetSlmPosition(this SlmPositionRow curr)
        {
            var result = new SlmPosition();
            result.InitDate = curr.InitDate;
            result.AccountNo = curr.AccountNo;
            result.OpenDate = curr.OpenDate;
            result.OpenNo = curr.OpenNo;
            result.OpenPrice = (decimal)curr.OpenPrice;
            result.OpenNum = curr.OpenNum;
            result.CurrencyCode = curr.CurrencyCode;
            result.ProxyId = curr.ProxyId;
            result.ProductType = (sbyte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = (sbyte)curr.BuySell;
            result.ExpiryDate = curr.ExpiryDate;
            result.LastTradeDate = curr.LastTradeDate;
            result.MarketValue = (decimal)curr.MarketValue;
            return result;
        }
        public static SlmCloseRow GetSlmCloseRow(this SlmClose curr)
        {
            var result = new SlmCloseRow();

            result.InitDate = curr.InitDate;
            result.AccountNo = curr.AccountNo;
            result.OpenDate = curr.OpenDate;
            result.OpenNo = curr.OpenNo;
            result.OpenPrice = (double)curr.OpenPrice;
            result.CloseDate = curr.CloseDate;
            result.CloseNo = curr.CloseNo;
            result.ClosePrice = (double)curr.ClosePrice;
            result.OpenNum = curr.OpenNum;
            result.CloseNum = curr.CloseNum;
            result.CurrencyCode = curr.CurrencyCode;
            result.OpenProxyId = curr.OpenProxyId;
            result.CloseProxyId = curr.CloseProxyId;
            result.ProductType = (byte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = (byte)curr.BuySell;
            result.ProfitLoss = (double)curr.ProfitLoss;
            return result;
        }
        public static SlmClose GetSlmClose(this SlmCloseRow curr)
        {
            var result = new SlmClose();

            result.InitDate = curr.InitDate;
            result.AccountNo = curr.AccountNo;
            result.OpenDate = curr.OpenDate;
            result.OpenNo = curr.OpenNo;
            result.OpenPrice = (decimal)curr.OpenPrice;
            result.CloseDate = curr.CloseDate;
            result.CloseNo = curr.CloseNo;
            result.ClosePrice = (decimal)curr.ClosePrice;
            result.OpenNum = curr.OpenNum;
            result.CloseNum = curr.CloseNum;
            result.CurrencyCode = curr.CurrencyCode;
            result.OpenProxyId = curr.OpenProxyId;
            result.CloseProxyId = curr.CloseProxyId;
            result.ProductType = (sbyte)curr.ProductType;
            result.ExchangeCode = curr.ExchangeCode;
            result.CommodityCode = curr.CommodityCode;
            result.ContractCode = curr.ContractCode;
            result.BuySell = (sbyte)curr.BuySell;
            result.ProfitLoss = (decimal)curr.ProfitLoss;
            return result;
        }
        public static RequestPackageOrder GetRequestPackage(this RequestPackagePushEx curr)
        {
            RequestPackageOrder requestPackage = new RequestPackageOrder()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                MsgId = curr.MsgId,
                DataBody = curr.DataBody,
                Head = curr.Head,
                PushMatchStateBody = curr.PushMatchStateBody?.GetPushMatchStateRow(),
                PushOrderStateBody = curr.PushOrderStateBody?.GetPushOrderStateRow(),
                SendOrderStatusBody = curr.SendOrderStatusBody?.GetSendOrderStatusRow(),
                SlmMatchBody = curr.SlmMatchBody?.GetSlmMatchRows(),
                SlmOrderBody = curr.SlmOrderBody?.GetSlmOrderRow(),
                SlmOrderJourBody = curr.SlmOrderJourBody?.GetSlmOrderJourRow()
            };
            return requestPackage;
        }

        public static RequestPackageOrderLog GetRequestPackageLog(this RequestPackageOrder curr, string fileName, string title = "")
        {
            RequestPackageOrderLog requestPackage = new RequestPackageOrderLog()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                DataBody = curr.DataBody[1],
                LogFileName = fileName,
                LogTitle = title,
                PushMatchStateBody = (PushMatchStateRow)curr.PushMatchStateBody?.Clone(),
                PushOrderStateBody = (PushOrderStateRow)curr.PushOrderStateBody?.Clone(),
                SendOrderStatusBody = (SendOrderStatusRow)curr.SendOrderStatusBody?.Clone(),
                SlmMatchBody = (SlmMatchRow)curr.SlmMatchBody?.Clone(),
                SlmOrderBody = (SlmOrderRow)curr.SlmOrderBody?.Clone(),
                SlmOrderJourBody = (SlmOrderJourRow)curr.SlmOrderJourBody?.Clone()
            };
            return requestPackage;
        }

        /// <summary>
        /// no SlmCloseBody SlmPositionBody 
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="fileName"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static RequestPackageOrderLog GetRequestPackageLog(this RequestPackageOrderLogEx curr, string fileName, string title = "")
        {
            RequestPackageOrderLog requestPackage = new RequestPackageOrderLog()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                DataBody = curr.DataBody,
                LogFileName = fileName,
                LogTitle = title,
                PushMatchStateBody = curr.PushMatchStateBody?.GetPushMatchStateRow(),
                PushOrderStateBody = curr.PushOrderStateBody?.GetPushOrderStateRow(),
                SendOrderStatusBody = curr.SendOrderStatusBody?.GetSendOrderStatusRow(),
                SlmMatchBody = curr.SlmMatchBody?.GetSlmMatchRow(),
                SlmOrderBody = curr.SlmOrderBody?.GetSlmOrderRow(),
                SlmOrderJourBody = curr.SlmOrderJourBody?.GetSlmOrderJourRow()
            };
            return requestPackage;
        }

        public static RequestPackageOrderLog GetRequestPackage(this RequestPackageOrderLogEx curr)
        {
            RequestPackageOrderLog requestPackage = new RequestPackageOrderLog()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                DataBody = curr.DataBody,
                LogFileName = curr.LogFileName,
                LogFormat = curr.LogFormat,
                LogLevel = curr.LogLevel,
                LogMsg = curr.LogMsg,
                LogTitle = curr.LogTitle,
                MsgId = curr.MsgId,
                PushMatchStateBody = curr.PushMatchStateBody?.GetPushMatchStateRow(),
                PushOrderStateBody = curr.PushOrderStateBody?.GetPushOrderStateRow(),
                SendOrderStatusBody = curr.SendOrderStatusBody?.GetSendOrderStatusRow(),
                SlmMatchBody = curr.SlmMatchBody?.GetSlmMatchRow(),
                SlmOrderBody = curr.SlmOrderBody?.GetSlmOrderRow(),
                SlmOrderJourBody = curr.SlmOrderJourBody?.GetSlmOrderJourRow(),
                SlmCloseBody = curr.SlmCloseBody?.GetSlmCloseRow(),
                SlmPositionBody = curr.SlmPositionBody?.GetSlmPositionRow(),
            };
            return requestPackage;
        }
        public static RequestPackageLog GetRequestPackage(this RequestPackageLogEx curr)
        {
            RequestPackageLog requestPackage = new RequestPackageLog()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                LogFileName = curr.LogFileName,
                LogFormat = curr.LogFormat,
                LogLevel = curr.LogLevel,
                LogMsg = curr.LogMsg,
                LogTitle = curr.LogTitle,
                MsgId = curr.MsgId
            };
            return requestPackage;
        }

        public static ResponsePackageOrder[] GetResponsePackageOrders(this CacheTradeDBEx data)
        {
            int nCount = 1;//没数据，至少返回一条
            if (data.PackageData.ServiceNo == (int)CacheSeverServiceNo.TE_ORDER_OFFSET)
            {
                if (data.Orders != null && data.Orders.Length > 0)
                {
                    nCount = data.Orders.Length;
                }
            }
            else if (data.PackageData.ServiceNo == (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET)
            {
                if (data.Orderjours != null && data.Orderjours.Length > 0)
                {
                    nCount = data.Orderjours.Length;
                }
            }
            else if (data.PackageData.ServiceNo == (int)CacheSeverServiceNo.TE_TRADE_OFFSET)
            {
                if (data.Trades != null && data.Trades.Length > 0)
                {
                    nCount = data.Trades.Length;
                }
            }
            else if (data.PackageData.ServiceNo == (int)ActiveStandbyFrontServiceNo.TE_SWITCH_READY)
            {
                nCount = 1;
            }
            else
            {
                LogHelper.WriteLog("ErrorLog", $"[FrontCommunicationHelper.GetResponsePackageOrderFromCacheTradeDBEx]错误的服务号：{data.PackageData.ServiceNo}");
            }

            ResponsePackageOrder[] objAry = new ResponsePackageOrder[nCount];
            for (int i = 0; i < nCount; i++)
            {
                objAry[i] = new ResponsePackageOrder();
            }
            foreach (var v in objAry)
            {
                v.ErrorNo = data.PackageData.ErrorNo;
                v.ErrorInfo = data.PackageData.ErrorInfo;
                //if (data.PackageData.ServiceNo == (int)ActiveStandbyFrontServiceNo.TE_SWITCH_READY)
                //{
                //    v.ServiceNo = data.PackageData.ServiceNo;
                //}
                //else
                //{
                //    v.ServiceNo = data.PackageData.ServiceNo;
                //}
                v.ServiceNo = data.PackageData.ServiceNo;
                v.Head = data.PackageData.Head;
                v.ClientId = data.PackageData.ClientId;
                v.UUID = data.PackageData.UUID;
                v.MsgId = data.PackageData.MsgId;
                v.DataBody = data.PackageData.DataBody;
            }

            for (int i = 0; i < objAry.Length; i++)
            {
                if (data.PackageData.ServiceNo == (int)CacheSeverServiceNo.TE_ORDER_OFFSET)
                {
                    if (data.Orders != null && data.Orders.Length > 0)
                    {
                        objAry[i].SlmOrderBody = data.Orders[i].GetSlmOrderRow();
                    }
                }
                else if (data.PackageData.ServiceNo == (int)CacheSeverServiceNo.TE_ORDERJOUR_OFFSET)
                {
                    if (data.Orderjours != null && data.Orderjours.Length > 0)
                    {
                        objAry[i].SlmOrderJourBody = data.Orderjours[i].GetSlmOrderJourRow();
                    }
                }
                else if (data.PackageData.ServiceNo == (int)CacheSeverServiceNo.TE_TRADE_OFFSET)
                {
                    if (data.Trades != null && data.Trades.Length > 0)
                    {
                        objAry[i].SlmMatchBody = new SlmMatchRow[1];
                        objAry[i].SlmMatchBody[0] = data.Trades[i].GetSlmMatchRow();
                    }
                }
            }
            return objAry;
        }


        #region 主备双活新增
        public static SlmOrder GetSlmOrder(this OrderCacheDB m)

        {
            SlmOrder curr = new SlmOrder();

            curr.InitDate = m.InitDate;

            curr.RouterNo = m.RouterNo;

            curr.OperatorNo = m.OperatorNo;

            curr.OrderPerson = m.OrderPerson;

            curr.UpdatePerson = m.UpdatePerson;

            curr.ProxyId = (short)m.ProxyId;

            curr.AccountNo = m.AccountNo;

            curr.ProductType = (sbyte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.CallPut = (sbyte)m.CallPut;

            curr.StrikePrice = m.StrikePrice;

            curr.OpenClose = (sbyte)m.OpenClose;

            curr.BuySell = (sbyte)m.BuySell;

            curr.OrderPrice = m.OrderPrice.TryToDecimal();

            curr.OrderNum = m.OrderNum;

            curr.OrderType = (sbyte)m.OrderType;

            curr.OrderLife = (sbyte)m.OrderLife;

            curr.OrderInsertType = (sbyte)m.OrderInsertType;

            curr.DelTag = (sbyte)m.DelTag;

            curr.OrderFrom = m.OrderFrom.TryToSByte();

            curr.OrderSubNumMin = m.OrderSubNumMin;

            curr.OrderSubNumMax = m.OrderSubNumMax;

            curr.OrderExpiryDate = m.OrderExpiryDate;

            curr.TriggerPrice = m.TriggerPrice.TryToDecimal();

            curr.OrderState = m.OrderState.ToChar();

            curr.FilledNum = m.FilledNum;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.UpdateDate = m.UpdateDate;

            curr.UpdateTime = m.UpdateTime;

            curr.ExchangeDate = m.ExchangeDate;

            curr.ExchangeTime = m.ExchangeTime;

            curr.OrderRemain = (sbyte)m.OrderRemain;

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

            curr.MsgNum = m.MsgNum;

            curr.Action = (sbyte)m.Action;

            return curr;
        }
        public static SlmOrderJour GetSlmOrderJour(this OrderjourCacheDB m)

        {

            SlmOrderJour curr = new SlmOrderJour();

            curr.InitDate = m.InitDate;

            curr.RouterNo = m.RouterNo;

            curr.OperatorNo = m.OperatorNo;

            curr.OrderPerson = m.OrderPerson;

            curr.UpdatePerson = m.UpdatePerson;

            curr.ProxyId = (short)m.ProxyId;

            curr.AccountNo = m.AccountNo;

            curr.ProductType = (sbyte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.CallPut = (sbyte)m.CallPut;

            curr.StrikePrice = m.StrikePrice;


            curr.BuySell = (sbyte)m.BuySell;

            curr.OrderPrice = m.OrderPrice.TryToDecimal();

            curr.OrderNum = (short)m.OrderNum;

            curr.OrderType = (sbyte)m.OrderType;

            curr.OrderLife = (sbyte)m.OrderLife;



            curr.OrderFrom = m.OrderFrom.TryToSByte();



            curr.TriggerPrice = m.TriggerPrice.TryToDecimal();

            curr.OrderStatus = m.OrderStatus.ToChar();

            curr.FilledNum = m.FilledNum;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.UpdateDate = m.UpdateDate;

            curr.UpdateTime = m.UpdateTime;

            curr.ExchangeDate = m.ExchangeDate;

            curr.ExchangeTime = m.ExchangeTime;


            curr.OrderNo = m.OrderNo;

            curr.OrderJourNo = (short)m.OrderJourNo;

            curr.BrokerOrderNo = m.BrokerOrderNo;

            curr.BrokerOrderJourNo = m.BrokerOrderJourNo;

            curr.ExchangeOrderNo = m.ExchangeOrderNo;



            curr.ErrorNo = (short)m.ErrorNo;

            curr.ErrorInfo = m.ErrorInfo;

            curr.RemarkCus = m.RemarkCus;

            curr.InfoS1 = m.InfoS1;

            curr.InfoS2 = m.InfoS2;

            curr.MsgNum = m.MsgNum;

            curr.Action = (sbyte)m.Action;

            return curr;

        }
        public static SlmMatch GetSlmMatch(this TradeCacheDB m)
        {
            SlmMatch curr = new SlmMatch();
            curr.InitDate = m.InitDate;

            curr.AccountNo = m.AccountNo;

            curr.OrderDate = m.OrderDate;

            curr.OrderTime = m.OrderTime;

            curr.OrderNo = m.OrderNo;

            curr.TradeDate = m.TradeDate;

            curr.TradeNo = m.TradeNo;

            curr.TradeTime = m.TradeTime;

            curr.TradePrice = m.TradePrice.TryToDecimal();

            curr.TradeVol = m.TradeVol;

            curr.CurrencyCode = m.CurrencyCode;

            curr.IsTplusone = (sbyte)m.IsTplusone;

            curr.OpenClose = (sbyte)m.OpenClose;

            curr.ProxyId = (short)m.ProxyId;

            curr.BrokerTradeNo = m.BrokerTradeNo;

            curr.ExchangeTradeNo = m.ExchangeTradeNo;

            curr.ProductType = (sbyte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.BuySell = (sbyte)m.BuySell;

            curr.TradeFee = m.TradeFee.TryToDecimal();

            curr.OperatorNo = m.OperatorNo;

            curr.Info1 = m.Info1;

            curr.Info2 = m.Info2;

            curr.Info3 = m.Info3;

            curr.OrderInsertType = (sbyte)m.OrderInsertType;

            curr.OrderFrom = m.OrderFrom.ToString();

            curr.MsgNum = m.MsgNum;

            return curr;

        }
        public static SlmPosition GetSlmPosition(this PositionCacheDB m)
        {

            SlmPosition curr = new SlmPosition();
            curr.InitDate = m.InitDate;

            curr.AccountNo = m.AccountNo;

            curr.OpenDate = m.OpenDate;

            curr.OpenNo = m.OpenNo;

            curr.OpenPrice = m.OpenPrice.TryToDecimal();

            curr.OpenNum = m.OpenNum;

            curr.CurrencyCode = m.CurrencyCode;

            curr.ProxyId = (short)m.ProxyId;

            curr.ProductType = (sbyte)m.ProductType;

            curr.ExchangeCode = m.ExchangeCode;

            curr.CommodityCode = m.CommodityCode;

            curr.ContractCode = m.ContractCode;

            curr.BuySell = (sbyte)m.BuySell;

            curr.ExpiryDate = m.ExpiryDate;

            curr.LastTradeDate = m.LastTradeDate;

            curr.MarketValue = m.MarketValue.TryToDecimal();
            return curr;

        }




        public static DepositCacheDB GetDepositCacheDB(this Deposit m)
        {
            DepositCacheDB curr = new DepositCacheDB();
            curr.InitDate = m.init_date.TryToInt();
            curr.BusinessFlag = m.business_flag.TryToInt();
            curr.BusinessNo = m.business_no;
            curr.OccurTime = m.occur_time.TryToInt();
            curr.OperatorNo = m.operator_no;
            curr.AccountNo = m.account_no;
            curr.CurrencyCode = m.currency_code;
            curr.OccurBalance = m.occur_balance.TryToDouble();
            curr.Remark = m.remark;
            curr.AuditFlag = m.audit_flag.TryToByte();
            curr.OccurDate = m.occur_date.TryToInt();
            curr.TplusFlag = m.tplus_flag.TryToInt();
            curr.MsgNum = m.msg_num.TryToLong();
            return curr;
        }

        public static Deposit GetDeposit(this DepositCacheDB curr)
        {
            Deposit m = new Deposit();
            m.init_date = curr.InitDate.ToString();
            m.business_flag = curr.BusinessFlag.ToString();
            m.business_no = curr.BusinessNo;
            m.occur_time = curr.OccurTime.ToString();
            m.operator_no = curr.OperatorNo;
            m.account_no = curr.AccountNo;
            m.currency_code = curr.CurrencyCode;
            m.occur_balance = curr.OccurBalance.ToString();
            m.remark = curr.Remark;
            m.audit_flag = curr.AuditFlag.ToString();
            m.occur_date = curr.OccurDate.ToString();
            m.tplus_flag = curr.TplusFlag.ToString();
            m.msg_num = curr.MsgNum.ToString();
            return m;
        }

        public static PackageCacheDB GetResponsePackageCacheDB(this PackageCacheDBEx curr)
        {
            PackageCacheDB requestPackage = new PackageCacheDB()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                MsgId = curr.MsgId,
                DataBody = curr.DataBody,
                Head = curr.Head,
                ErrorInfo = curr.ErrorInfo,
                ErrorNo = curr.ErrorNo,
                PagingInfo = new PagingInfoBase()
                {
                }
            };
            return requestPackage;
        }

        public static RequestPackageActiveSendEx GetRequestPackageActiveSendEx(this RequestPackageEx curr)
        {
            RequestPackageActiveSendEx requestPackage = new RequestPackageActiveSendEx()
            {
                ServiceNo = curr.PackageData.ServiceNo,
                UUID = curr.PackageData.UUID,
                ClientId = curr.PackageData.ClientId,
                DataBody = curr.PackageData.DataBody,
                Head = curr.PackageData.Head
            };
            return requestPackage;
        }



        public static ResponsePackage GetResponsePackage(this RequestPackageEx curr)
        {

            var data = new string[1][];
            data[0] = curr.PackageData.DataBody;
            ResponsePackage responsePackage = new ResponsePackage()
            {
                ServiceNo = curr.PackageData.ServiceNo,
                UUID = curr.PackageData.UUID,
                ClientId = curr.ClientId,
                DataBody = data,
                Head = curr.PackageData.Head
            };
            return responsePackage;
        }

        #endregion

        public static ResponsePackageOrder GetResponsePackage(this ResponsePackagePushEx curr)
        {
            ResponsePackageOrder requestPackage = new ResponsePackageOrder()
            {
                ServiceNo = curr.ServiceNo,
                UUID = curr.UUID,
                ClientId = curr.ClientId,
                MsgId = curr.MsgId,
                DataBody = curr.DataBody,
                Head = curr.Head,
                ErrorInfo = curr.ErrorInfo,
                ErrorNo = curr.ErrorNo,
                PushMatchStateBody = curr.PushMatchStateBody?.GetPushMatchStateRow(),
                PushOrderStateBody = curr.PushOrderStateBody?.GetPushOrderStateRow(),
                SendOrderStatusBody = curr.SendOrderStatusBody?.GetSendOrderStatusRow(),
                SlmMatchBody = curr.SlmMatchBody?.GetSlmMatchRows(),
                SlmOrderBody = curr.SlmOrderBody?.GetSlmOrderRow(),
                SlmOrderJourBody = curr.SlmOrderJourBody?.GetSlmOrderJourRow()
            };
            return requestPackage;
        }
    }
}
