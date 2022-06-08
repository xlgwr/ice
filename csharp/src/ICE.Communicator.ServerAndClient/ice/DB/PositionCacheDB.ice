// **********************************************************************
//
// Copyright (c) 2021 Solomon, Inc. All rights reserved.
//
// **********************************************************************
#pragma once
 

module SlmCommunicator
{
	//持仓
	struct PositionCacheDB
	{
		long MsgNum; // 增量序列号
		int InitDate; 
		string AccountNo; 
		int OpenDate; 
		long OpenNo; 
		double OpenPrice; 
		int OpenNum; 
		string CurrencyCode; 
		int ProxyId; 
		byte ProductType; 
		string ExchangeCode; 
		string CommodityCode; 
		string ContractCode; 
		byte BuySell; 
		int ExpiryDate; 
		int LastTradeDate; 
		double MarketValue;
	};
 //end
};

