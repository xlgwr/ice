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
		string BusinessNo; 
		string AccountNo; 
		int BusinessFlag; 
		int OccurTime; 
		string OperatorNo; 
		string CurrencyCode; 
		double OccurBalance; 
		string Remark; 
		byte AuditFlag; 
		int OccurDate; 
		int TplusFlag; 
	};
 //end
};

