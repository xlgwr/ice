// **********************************************************************
//
// Copyright (c) 2021 Solomon, Inc. All rights reserved.
//
// **********************************************************************
#pragma once
 

module SlmCommunicator
{
	//出入金
	struct DepositCacheDB
	{ 	
		long MsgNum; 
		int InitDate; 
		int BusinessFlag; 
		string BusinessNo; 
		int OccurTime; 
		string OperatorNo; 
		string AccountNo; 
		string CurrencyCode; 
		double OccurBalance; 
		string Remark; 
		byte AuditFlag; 
		int OccurDate; 
		int TplusFlag; 
	};
 //end
};

