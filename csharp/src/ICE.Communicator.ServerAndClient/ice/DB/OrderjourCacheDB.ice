// **********************************************************************
//
// Copyright (c) 2021 Solomon, Inc. All rights reserved.
//
// **********************************************************************
#pragma once
 

module SlmCommunicator
{
	//委托流水
	struct OrderjourCacheDB
	{
		long MsgNum; // 增量序列号
		int InitDate; 
		string RouterNo; 
		string OperatorNo; 
		string OrderPerson; 
		string UpdatePerson; 
		int ProxyId; 
		string AccountNo; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte CallPut; 
		string StrikePrice; 
		byte BuySell; 
		double OrderPrice; 
		int OrderNum; 
		byte OrderType; 
		byte OrderLife; 
		string OrderFrom; 
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
		int OrderJourNo; 
		string BrokerOrderNo; 
		string BrokerOrderJourNo; 
		string ExchangeOrderNo; 
		int ErrorNo; 
		string ErrorInfo; 
		string RemarkCus; 
		string InfoS1; 
		string InfoS2;
		byte Action;
	};
 //end
};

