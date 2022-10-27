// **********************************************************************
//
// Copyright (c) 2021 Solomon, Inc. All rights reserved.
//
// **********************************************************************
#pragma once
 

module SlmCommunicator
{
	//委托表
	struct OrderCacheDB
	{	
		long MsgNum; // 增量序列号		
		int InitDate; // 交易日期		
		long OrderNo; // 定单号		
		byte OrderState; // 委托状态
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
		int OrderDate; // 定单日期
		int OrderTime; // 定单时间
		string RouterNo; // 前置号
		string UpdatePerson; // 最后一次修改委托的人 
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
 //end
};

