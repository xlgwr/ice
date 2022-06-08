//  **********************************************************************
// 
//  Copyright (c) 2021 Solomon, Inc. All rights reserved.
// 
//  **********************************************************************
#pragma once
 
#include <Ice/BuiltinSequences.ice>
#include "DB/OrderCacheDB.ice" 
#include "DB/OrderjourCacheDB.ice" 
#include "DB/TradeCacheDB.ice" 
#include "DB/PositionCacheDB.ice" 
#include "DB/DepositCacheDB.ice" 

//  other ice #include "PackageCacheDB.ice"
//  interface extends
//  Definitions here... 

module SlmCommunicator
{
	/**
	* 通用类型数组
	**/
	sequence<Ice::StringSeq> DataStrGroup;	
	dictionary<string, string> CacheDBDic; // 示例字典

	/**
	* 分5类缓存 A.交易类 trade B.参数类 config C.账户信息及出入金信息  Account D.资金类：Fund E.交易控制信息  control 
	**/

	// 分页结构
	struct PagingInfoBase
	{
		long Count;// 总条数
		int pSize;// 一页显示条数
		int pIndex;// 当前页
	};

	/**
	* 缓存同步 请求包 通用结构参数
	**/
	struct PackageCacheDB
	{
		/**
		* 服务号
		**/		
		int ServiceNo;
		string ClientId; // 客户端id
		string UUID; // ice 对应uuid
		string MsgId; // 消息标记guid
		int ErrorNo; // 通用错误码 主要用于返回确认
		string ErrorInfo; // 通用错误码信息 主要用于返回确认
		PagingInfoBase PagingInfo;// 分页相关信息
		CacheDBDic DatasDic;// <key,value> string,string 字典 用于其它参数设置
		Ice::StringSeq Head; // 一维数组字符备用
		DataStrGroup DataBody; // 二维数组字符备用
	};
	
	// A.交易类 列表相关
	sequence<OrderCacheDB> OrderCacheDBs; // 委托表
	sequence<OrderjourCacheDB> OrderjourCacheDBs; // 委托流水
	sequence<TradeCacheDB> TradeCacheDBs; // 成交表
	sequence<PositionCacheDB> PositionCacheDBs; // 持仓，昨持仓
	sequence<DepositCacheDB> DepositCacheDBs; // 出入金

	/**
	* 缓存同步 A.交易类 trade
	**/
	struct CacheTradeDB
	{
		PackageCacheDB PackageData; // 头信息相关
		OrderCacheDBs Orders; // 委托表
		OrderjourCacheDBs Orderjours; // 委托流水
		TradeCacheDBs Trades;// 成交表
		DepositCacheDBs Deposits;// 出入金表
		PositionCacheDBs PrePositions;// 昨持仓表
		PositionCacheDBs Positions;// 持仓表
	};
	
	/**
	* 表同步相关功能,服务端推送对应缓存信息 extends
	**/
	interface PackageReceiverCacheDB
	{ 	
	
		/**
		* 缓存同步 接收数据包 通用类
		**/
		PackageCacheDB ReceivePackageCacheBase(PackageCacheDB responseField);
		/**
		* 缓存同步 接收数据包 A.交易类 trade
		**/
		PackageCacheDB ReceivePackageCacheTrade(CacheTradeDB responseField);
	};

	/**
	* 表同步相关功能,获取服务端对应缓存信息 extends
	**/
	interface PackageSenderCacheDB
	{
	
		/**
		* 缓存同步 发送数据包 通用类
		**/
		PackageCacheDB SendPackageCacheBase(PackageCacheDB requestField);
		/**
		* 缓存同步 发送数据包 A.交易类 trade
		**/
		CacheTradeDB SendPackageCacheTrade(CacheTradeDB requestField);
	};

 // end
};

