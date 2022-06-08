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
		byte OpenClose; 
		byte BuySell; 
		double OrderPrice; 
		short OrderNum; 
		byte OrderType; 
		byte OrderLife; 
		byte OrderInsertType; 
		byte DelTag; 
		string OrderFrom; 
		short OrderSubNumMin; 
		short OrderSubNumMax; 
		int OrderExpiryDate; 
		double TriggerPrice; 
		byte OrderStatus; //not
		short FilledNum; 
		int OrderDate; 
		int OrderTime; 
		int UpdateDate; 
		int UpdateTime; 
		int ExchangeDate; 
		int ExchangeTime; 
		byte OrderRemain; 
		long OrderNo; 
		short OrderJourNo; 
		string BrokerOrderNo; 
		string BrokerOrderJourNo; 
		string ExchangeOrderNo; 
		string CarryNo; 
		short ErrorNo; 
		string ErrorInfo; 
		string RemarkCus; 
		string InfoS1; 
		string InfoS2; 
		string LocalNo;
		byte Action;
	};
 //end
};

