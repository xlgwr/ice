// **********************************************************************
//
// Copyright (c) 2021 Solomon, Inc. All rights reserved.
//
// **********************************************************************
#pragma once
 

module SlmCommunicator
{
	//成交表
	struct TradeCacheDB
	{
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
		int ProxyId; 
		string BrokerTradeNo; 
		string ExchangeTradeNo; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte BuySell; 
		float TradeFee; 
		string OperatorNo; 
		string Info1; 
		string Info2; 
		string Info3; 
		byte OrderInsertType; 
		string OrderFrom;
	};
 //end
};

