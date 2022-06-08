// **********************************************************************
//
// Copyright (c) 2016 Solomon, Inc. All rights reserved.
//
// **********************************************************************

#pragma once

#include <Ice/Identity.ice>
#include "PackageCacheDB.ice"

module SlmCommunicator
{
    
	/**
	*  交易服 模拟交易 SlmOrder  940001
	*  交易前置 交易服 SlmOrder  910001
	**/
	struct SlmOrderRow{
		long MsgNum; // 增量序列号
		string AccountNo; // 账户号码
		string OperatorNo; // 交易员编号
		string OrderPerson; // 委托人
		byte ProductType; // 商品类型
		string ExchangeCode; // 市场代码
		string CommodityCode; // 商品代码
		string ContractCode; // 合约代码
		byte CallPut; // 看涨看跌属性
		string StrikePrice; // 行权价
		byte OpenClose; // 开平仓
		byte BuySell; // 买卖方向
		double OrderPrice; // 委托价格
		short OrderNum; // 委托数量
		byte OrderType; // 委托类型 市价，限价等
		byte OrderLife; // GTD,DTC,FOK,FAK等
		byte OrderInsertType; // 人工录入单，程序传入单
		byte DelTag; // 删除标志
		byte OrderFrom; // 委托来源
		short OrderSubNumMin; // 子委托数量(针对IceBerg)
		short OrderSubNumMax; // 子委托数量(针对IceBerg)
		int OrderExpiryDate; // 用于GTD等
		double TriggerPrice; // 止损触发价
		byte OrderRemain; // 是否T+1
		string RemarkCus; // 用户输入信息
		string InfoS1; // 用户交易关联字段1
		string InfoS2; // 用户交易关联字段2
		string LocalNo; // 本地号
		long OrderNo; // 定单号
		int OrderDate; // 定单日期
		int OrderTime; // 定单时间
		string RouterNo; // 前置号
		int InitDate; // 交易日期
		string UpdatePerson; // 最后一次修改委托的人 
		byte OrderState; // 委托状态
		short FilledNum; // 已成交数量
		int UpdateDate; // 订单修改日期（本地日期）
		int UpdateTime; // 订单修改时间（本地时间）
		int ExchangeDate; // 交易所日期
		int ExchangeTime; // 交易所时间
		short OrderJourNo; // 内部委托最后流水号
		string BrokerOrderNo; // 上级委托编号
		string BrokerOrderJourNo; // 上级委托最后流水号
		string ExchangeOrderNo; // 交易所编号
		string CarryNo; // 用于LME Carry录入后系统自动配对，carryNo由系统自动生成
		short ErrorNo; // 错误编码
		string ErrorInfo; // 错误信息
		short ProxyId; // 上级代码 
		byte Action;
	};
	/**
	* 网关推送成交结构
	* 模拟交易 交易服推送成交 PushMatchState 930002
	**/
	struct PushMatchStateRow {
		long MsgNum; // 增量序列号
		string AccountNo; 
		long OrderNo; 
		int TradeDate; 
		string TradeNo; 
		int TradeTime; 
		double TradePrice; 
		int TradeVol; 
		short ProxyId; 
		string BrokerTradeNo; 
		string ExchangeTradeNo; 
		string OperatorNo; 
		string Info1; 
		string Info2; 
		string Info3; 
		byte OrderInsertType; 
		byte OrderFrom; 
		string CommodityCode; 
		string ContractCode; 
		string BuySell; 
	};
	/**
	* 网关推送下单状态结构
	* 模拟交易->交易服 推送委托状态 PushOrderState 930001 
	**/
	struct PushOrderStateRow{	 
		long MsgNum; // 增量序列号
		long OrderNo; 
		byte OrderState; 
		string OrderJourNo; 
		int UpdateDate; 
		int UpdateTime; 
		short ErrNo; 
		string ErrInfo; 
		double OrderPrice; 
		short OrderVol; 
		double TriggerPrice; 
		string ExchangeOrderNo; 
		string ExchangeDateTime; 
		short MatchVol; 
		string BrokerOrderNo; 
		string BrokerOrderJourNo; 
		byte OrderType;
		byte OrderFrom; // 委托来源		
		string Remarks; 
		short CurMatchVol;
		byte Action;
	};

	/**
	* 交易服->交易前置 SendOrderStatus
	* 920001 委托状态变化
	**/
	struct SendOrderStatusRow{
		long MsgNum; // 增量序列号
		string AccountNo; //账户号码
		string OperatorNo; //交易员编号
		long OrderNo; //orderId
		byte OrderState; //委托状态
		short OrderJourNo; //内部委托流水号
		int UpdateDate;//更新日期
		int UpdateTime;//更新时间
		short ErrorNo; //返回错误代码
		string ErrorInfo; //返回错误信息			
		double OrderPrice; //委托价格
		short OrderNum; //委托数量
		double TriggerPrice; //触发价格
		int ExchangeDate; //交易所日期
		int ExchangeTime; //交易所时间
		string ExchangeOrderNo; //交易所委托号
		short FilledNum; //成交数量
		string BrokerOrderNo; //上手委托号
		string BrokerOrderJourNo; //上手委托流号
		string InfoS1; //用户保留
		string InfoS2; //用户保留
		int InitDate; //交易日期
		byte OrderType;
		byte OrderFrom; // 委托来源
		string Remarks; 
		byte Action;
	};

	/**
	* 交易服->交易前置 SlmOrderJour
	* 920005 委托流水变化
	**/
	struct SlmOrderJourRow{	
		long MsgNum; // 增量序列号
		int InitDate; 
		string RouterNo; 
		string OperatorNo; 
		string OrderPerson; 
		string UpdatePerson; 
		short ProxyId; 
		string AccountNo; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte CallPut; 
		string StrikePrice; 
		byte BuySell; 
		double OrderPrice; 
		short OrderNum; 
		byte OrderType; 
		byte OrderLife; 
		byte OrderFrom; 
		double TriggerPrice; 
		byte OrderStatus; 
		int FilledNum; 
		int OrderDate; 
		int OrderTime; 
		int UpdateDate; 
		int UpdateTime; 
		int ExchangeDate; 
		int ExchangeTime; 
		long OrderNo; 
		short OrderJourNo; 
		string BrokerOrderNo; 
		string BrokerOrderJourNo; 
		string ExchangeOrderNo; 
		short ErrorNo; 
		string ErrorInfo; 
		string RemarkCus; 
		string InfoS1; 
		string InfoS2; 
		byte Action;
	};

	/**
	* 交易服->交易前置 SlmMatch[]
	* 920002 成交变化
	**/
	struct SlmMatchRow{  
		long MsgNum; // 增量序列号
		int InitDate; 
		string AccountNo; 
		int OrderDate; 
		int OrderTime; 
		long OrderNo; 
		int TradeDate; 
		long TradeNo; 
		int TradeTime; 
		double TradePrice; 
		int TradeVol; 
		string CurrencyCode; 
		byte IsTplusone; 
		byte OpenClose; 
		short ProxyId; 
		string BrokerTradeNo; 
		string ExchangeTradeNo; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte BuySell; 
		double TradeFee; 
		string OperatorNo; 
		string Info1; 
		string Info2; 
		string Info3; 
		byte OrderInsertType; 
		byte OrderFrom; 
	};
	/**
	* 持仓
	**/
	struct SlmPositionRow{
		long MsgNum; // 增量序列号
		int InitDate; 
		string AccountNo; 
		int OpenDate; 
		long OpenNo; 
		double OpenPrice; 
		int OpenNum; 
		string CurrencyCode; 
		short ProxyId; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte BuySell; 
		int ExpiryDate; 
		int LastTradeDate; 
		double MarketValue; 
	};
	/**
	* 平仓结构
	**/
	struct SlmCloseRow{ 
		long MsgNum; // 增量序列号
		int InitDate; 
		string AccountNo; 
		int OpenDate; 
		long OpenNo; 
		double OpenPrice; 
		int CloseDate; 
		long CloseNo; 
		double ClosePrice; 
		int OpenNum; 
		int CloseNum; 
		string CurrencyCode; 
		int OpenProxyId; 
		int CloseProxyId; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte BuySell; 
		double ProfitLoss;
	};
	/**
	* 通用类型数组
	**/
	sequence<string> DataRow;
	sequence<DataRow> DataGroup;
	/**
	* 交易用类型数组
	**/
	sequence<SlmOrderRow> SlmOrderRows; 
	
	sequence<PushMatchStateRow> PushMatchStateRows; 

	sequence<PushOrderStateRow> PushOrderStateRows; 

	sequence<SlmMatchRow> SlmMatchRows; 

	struct RequestPackage {
        int ServiceNo;
        DataRow Head;
		string ClientId;
		string UUID;
        DataRow DataBody;
	};
	struct RequestPackageOrder {
        int ServiceNo;
        DataRow Head;
		string ClientId;
		string UUID;
		string MsgId;
        DataGroup DataBody;
        SlmOrderRow SlmOrderBody;
        PushMatchStateRow PushMatchStateBody;
        PushOrderStateRow PushOrderStateBody;
		SendOrderStatusRow SendOrderStatusBody;
		SlmOrderJourRow SlmOrderJourBody;
		SlmMatchRows SlmMatchBody;
	};
	sequence<RequestPackageOrder> RequestPackageList;

	struct ResponsePackage {
        int ErrorNo;
        string ErrorInfo;
		int ServiceNo;
		DataRow Head;
		string ClientId;
		string UUID;
        DataGroup DataBody;
	};
	struct ResponsePackageOrder {
        int ErrorNo;
        string ErrorInfo;
		int ServiceNo;
		DataRow Head;
		string ClientId;
		string UUID;
		string MsgId;
        DataGroup DataBody;
        SlmOrderRow SlmOrderBody;
        PushMatchStateRow PushMatchStateBody;
        PushOrderStateRow PushOrderStateBody;		
		SendOrderStatusRow SendOrderStatusBody;
		SlmOrderJourRow SlmOrderJourBody;
		SlmMatchRows SlmMatchBody;
	};
	struct ResponsePackageOrderBackMsg {
        int ErrorNo;
        string ErrorInfo;
		int ServiceNo;
		DataRow Head;
		string ClientId;
		string UUID;
		string MsgId;
        DataGroup DataBody;
	};
	sequence<ResponsePackageOrder> ResponsePackageList;

	/**
	* 推送更新数据
	**/
	struct NewFlashData {
        string ContractNo; //合约号
        float NPrice;  //最新价
		float HPrice;  //最高价
		float LPrice;  //最低价
		float OPrice;  //开盘价
		float CPrice;  //收盘价
		float YSettle; //昨结算价
		int   Amount;  //成交量
		int   NVol;    //最新成交量
		byte  Status;  //合约交易状态
		float NetChg;  //涨跌
		float Markup;  //涨跌幅
		float BestBPrice; //最优买价
		int   BestBVol;   //最优买量
		float BestSPrice; //最优卖价
		int   BestSVol;   //最优卖量
		float BOPrice; //买一价
		int   BOVol;   //买一量
		float SOPrice; //卖一价
		int   SOVol;   //卖一量
		int   UpdateTime; //更新时间 
		byte  Type;  //类型
	};

	sequence<float> FloatRow;
	sequence<int> IntRow;
	/**
	* 推送深度数据
	**/
	struct DeepData {
        string ContractNo; //合约号
        FloatRow Ask;      //申卖价
		IntRow   AskVol;   //申卖量
		FloatRow Bid;      //申买价
		IntRow   BidVol;   //申买量
	};

	sequence<NewFlashData> NewFlashArray;
	sequence<DeepData> DeepDataArray;

	/**
	* 日志消息结构
	**/
	struct RequestPackageLog {
		string ClientId;
      	int ServiceNo;
		string UUID;
		string MsgId;
		string LogFileName;
		byte LogLevel;
		string LogFormat;  		
		string LogTitle;  
		string LogMsg;  
	};
	/**
	* 日志消息结构 list
	**/
	struct RequestPackageOrderLog {
		string ClientId;
      	int ServiceNo;
		string UUID;
		string MsgId;
		string LogFileName;
		byte LogLevel; 
		string LogTitle;  
		string LogFormat;  
		string LogMsg;  
		DataRow DataBody;
        SlmOrderRow SlmOrderBody;
        PushMatchStateRow PushMatchStateBody;
        PushOrderStateRow PushOrderStateBody;
		SendOrderStatusRow SendOrderStatusBody;
		SlmOrderJourRow SlmOrderJourBody;
		SlmMatchRow SlmMatchBody;
		SlmPositionRow SlmPositionBody;
		SlmCloseRow SlmCloseBody;

	}; 
	sequence<RequestPackageOrderLog> RequestPackageLogList;

	interface PackageReceiver extends PackageReceiverCacheDB
	{
		/**
		 * 接收数据包
		 **/
		int ReceivePackage(ResponsePackage responseField);
		/**
		 * 接收数据包list
		 **/
		int ReceivePackageList(ResponsePackageList responseFields);

		/**
		 * 注册回调对象
		 **/
		void RegCallbackClient();
	};

	interface PackageSender extends PackageSenderCacheDB
	{
		/**
		 * 发送数据包
		 **/
		ResponsePackage SendPackage(RequestPackage requestField);
		/**
		 * 发送数据包list
		 **/
		ResponsePackageOrderBackMsg SendPackageList(RequestPackageList requestFields);


		/**
		 * 发送数据包日志
		 **/
		int SendLogPackage(RequestPackageLog responseField);
		/**
		 * 发送数据包日志 list
		 **/
		int SendLogPackageList(RequestPackageLogList responseFields);
		/**
		 * 推送数据包日志 list 
		 * mstype: 0 Receive 1 Send
		 **/
		int SendLogResponsePackageList(ResponsePackageList responseFields, int mstype);
		
		/**
		 * 接收数据包日志 list 
		 * mstype: 0 Receive 1 Send
		 **/
		int SendLogRequestPackageList(RequestPackageList responseFields, int mstype);

		/**
		 * 注册回调对象(one way)
		 **/
		int RegClient(Ice::Identity ident, int type);

		/**
		 * 注册回调对象(two way)
		 **/
		int RegClientExt(PackageReceiver* proxy, Ice::Identity ident, int type);

		/**
		 * 注册行情回调对象
		 **/
		int RegQuotClient(Ice::Identity ident, Ice::Identity quotIdent, int type);
	};

	interface QuotPackage
	{
		/**
		 * 推送数据包
		 **/
		void PushPackage(byte type, NewFlashArray nfArray, DeepDataArray dArray);
	};
	 

};
