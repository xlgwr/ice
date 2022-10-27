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
		long TradeNo; 
		long OrderNo; 
		string AccountNo; 
		int OrderDate; 
		int OrderTime; 
		int TradeDate; 
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
 //end
};

