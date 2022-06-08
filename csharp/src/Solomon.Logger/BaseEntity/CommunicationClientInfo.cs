using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public enum ServerIdentify
    {
        Unknown = 0,
        Active = 1,
        Standby = 2,
        Normal = 3,

    }
    public enum IsCrash
    {
        Unknown = 0,
        Crashed = 1,
        NotCrash = 2
    }


    public class CommunicationBaseInfo
    {
        public string ClientId { get; set; }
        public string IPInfo { get; set; }

        public ClientType ClientType { get; set; }

        public ServerIdentify ServerIdentify { get; set; }
        /// <summary> 
        /// 是否宕机
        /// </summary>
        public IsCrash IsCrash { get; set; }

        public string Remark { get; set; }


        public CommunicationBaseInfoForICE ConvertToIceObject()
        {
            return new CommunicationBaseInfoForICE()
            {
                ClientId = this.ClientId,
                ClientType = (int)this.ClientType,
                IPInfo = this.IPInfo,
                IsCrash = (int)this.IsCrash,
                Remark = this.Remark,
                ServerIdentify = (int)this.ServerIdentify
            };
        }
    }

    public class CommunicationBaseInfoForICE
    {

        public string ClientId { get; set; }
        public string IPInfo { get; set; }

        public int ClientType { get; set; }

        public int ServerIdentify { get; set; }
        /// <summary> 
        /// 是否宕机
        /// </summary>
        public int IsCrash { get; set; }

        public string Remark { get; set; }

        public CommunicationBaseInfo ConvertToObject()
        {
            return new CommunicationBaseInfo()
            {
                ClientId = this.ClientId,
                ClientType = (ClientType)this.ClientType,
                IPInfo = this.IPInfo,
                IsCrash = (IsCrash)this.IsCrash,
                Remark = this.Remark,
                ServerIdentify = (ServerIdentify)this.ServerIdentify
            };
        }
    }

    public class CommunicationClientInfo : CommunicationBaseInfo
    {
        public string Port { get; set; }
        public string LoginDateTime { get; set; }

        public string ConnectionId { get; set; }

        public bool IsLogin { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public string ConfigKeyName { get; set; }

    }




    public enum ArbittrationSeverServiceNo
    {

        /// <summary>
        /// 请求心跳
        /// </summary>
        RequestHeartBeat = 28000000,

        /// <summary>
        /// 请求发起投票，客户端发送给仲裁使用
        /// </summary>
        RequestStartVote = 28000001,

        /// <summary>
        /// 请求投票服务，客户端接收发起投票检测
        /// </summary>
        RequestVoteService = 28000002,

        /// <summary>
        ///主服务失效--备服务启用
        /// </summary>
        RequestActiveStandbySwitch = 28000003,


        /// <summary>
        ///  请求身份，客户端连接仲裁服务发送
        /// </summary>
        RequestServerIdentify = 28000006,

    }
    /// <summary>
    /// 内存分类对象
    /// </summary>
    public enum CacheSeverType
    {
        /// <summary>
        /// 其他类
        /// </summary>
        Other = 0,
        /// <summary>
        /// 委托
        /// </summary>
        Order = 1,
        /// <summary>
        /// 委托流水
        /// </summary>
        OrderJour = 2,
        /// <summary>
        /// 成交
        /// </summary>
        Trade = 3,
        /// <summary>
        /// 出入金
        /// </summary>
        Deposit = 4
    }
    /// <summary>
    /// 内存服务器服务号
    /// </summary>
    public enum CacheSeverServiceNo
    {

        /// <summary>
        /// 内存服是否 准备就绪
        /// 1：就绪  2：忙
        /// </summary>
        Cache_IS_Ready = 38000000,
        /// <summary>
        /// 备内存服是否 新增/更新 确认OK
        /// 1：新增/更新确认OK 2:失败
        /// 用于备内存服回报二次确认OK
        /// </summary>
        Cache_Save_Ready = 38100000,
        /// <summary>
        /// 恢复文件相关检测及恢复
        /// 备向主发送本地恢复文件清单
        /// 主向备推送差异文件更新
        /// </summary>
        Cache_Recover_Check = 38200000,
        /// <summary>
        /// 获取全量 委托数据缓存
        /// </summary>
        TE_ORDER_QRY = 38000001,
        /// <summary>
        /// 新增 委托数据缓存
        /// </summary>
        TE_ORDER_ADD = 38000002,
        /// <summary>
        /// 更新 委托数据缓存
        /// </summary>
        TE_ORDER_UPD = 38000003,
        /// <summary>
        /// 增量消息 数据同步
        /// </summary>
        TE_ORDER_OFFSET = 38000004,
        /// <summary>
        /// 获取全量 委托流程数据缓存
        /// </summary>
        TE_ORDERJOUR_QRY = 38000005,
        /// <summary>
        /// 新增 委托流程数据缓存
        /// </summary>
        TE_ORDERJOUR_ADD = 38000006,
        /// <summary>
        /// 待定，流水无修改功能
        /// </summary>
        TE_ORDERJOUR_UPD = 38000007,
        /// <summary>
        /// 增量消息 数据同步 委托流程数据缓存
        /// </summary>
        TE_ORDERJOUR_OFFSET = 38000008,
        /// <summary>
        /// 获取全量 昨天持仓信息
        /// </summary>
        TE_Pre_Position_QRY = 38000010,
        /// <summary>
        /// 获取全量 成交数据缓存
        /// </summary>
        TE_TRADE_QRY = 380000015,
        TE_TRADE_ADD = 380000016,
        /// <summary>
        /// 待定，成交无修改功能
        /// </summary>
        TE_TRADE_UPD = 380000017,
        /// <summary>
        /// 增量消息
        /// </summary>
        TE_TRADE_OFFSET = 380000018,
        /// <summary>
        /// 获取全量 持仓信息
        /// </summary>
        TE_Position_QRY = 38000020,
        TE_Position_ADD = 38000021,
        /// <summary>
        /// 待定，持仓无修改功能
        /// </summary>
        TE_Position_UPD = 38000022,
        /// <summary>
        /// 增量消息
        /// </summary>
        TE_Position_OFFSET = 38000023,
        TE_Position_DEL = 38000024,

        /// <summary>
        /// 交易服发起委托恢复
        /// </summary>
        TE_ORDER_RECOVERY = 38000025,

        /// <summary>
        /// 交易服发起委托流水恢复
        /// </summary>
        TE_ORDERJOUR_RECOVERY = 38000026,

        /// <summary>
        /// 交易服发起成交恢复
        /// </summary>
        TE_TRADE_RECOVERY = 38000027,

        /// <summary>
        /// 查询主内存服务器最大msgnum
        /// </summary>
        TE_Cache_MaxMessageNum = 38000028,
        /// <summary>
        /// 交易服发起出入金恢复
        /// </summary>
        TE_DEPOSIT_RECOVERY = 38000029,
        /// <summary>
        /// 获取全量 出入金数据缓存
        /// </summary>
        TE_DEPOSIT_QRY = 38000031,
        /// <summary>
        /// 新增 出入金数据缓存
        /// </summary>
        TE_DEPOSIT_ADD = 38000032,
        /// <summary>
        /// 更新 出入金数据缓存 
        /// </summary>
        TE_DEPOSIT_UPD = 38000033,
        /// <summary>
        /// 增量消息 数据同步
        /// </summary>
        TE_DEPOSIT_OFFSET = 38000034,
    }

    public enum ActiveStandbyFrontServiceNo
    {
        TE_SWITCH_READY = 48000000,
    }

    public enum ActiveStandbyTradeServiceNo
    {
        TE_ACTIVE_SENDPROXY_FAILED = 58000000,
    }

    public enum GatewayServiceNo
    {
        /// <summary>
        /// 网关推送增量消息
        /// </summary>
        TE_QUERY_ORDER_RETURN = 930010
    }
}
