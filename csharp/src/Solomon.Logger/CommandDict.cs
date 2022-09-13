using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.DataType
{
    #region DBCommand
    /// <summary>
    /// 主要用于sqlbulkcopy数据库表对表映射处理
    /// </summary>
    public interface IDBEntity
    {
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <returns></returns>
        string GetTableName();
        /// <summary>
        /// 获取主键
        /// </summary>
        /// <returns></returns>
        string GetKeys();
        /// <summary>
        /// 获取更新列
        /// </summary>
        /// <returns></returns>
        string GetAllUpdateCols();
        /// <summary>
        /// 获取所有列
        /// </summary>
        /// <returns></returns>
        string GetAllCols();
        /// <summary>
        /// 获取当前表结构
        /// </summary>
        /// <returns></returns>
        DataTable GetTable();
        /// <summary>
        /// 获取删除sql
        /// </summary>
        /// <param name="ServiceNo"></param>
        /// <returns></returns>
        string GetSqlDelete(int ServiceNo);

    }
    /// <summary>
    /// 用于集合sql或表table
    /// </summary>
    public class DBTable
    {
        /// <summary>
        /// 表table操作
        /// 1: 新增, 2:新增or更新, 3:sql语句（简单sql、删除）
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string SqlStr { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string Keys { get; set; }
        /// <summary>
        /// 更新列
        /// </summary>
        public string ColUpdate { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表
        /// </summary>
        public DataTable Dt { get; set; }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="dBEntity"></param>
        public void InitData(IDBEntity dBEntity, int serverno)
        {
            if (dBEntity == null)
            {
                return;
            }
            this.TableName = dBEntity.GetTableName();
            this.Dt = dBEntity.GetTable();
            this.Keys = dBEntity.GetKeys();
            this.ColUpdate = dBEntity.GetAllUpdateCols();
            this.SqlStr = dBEntity.GetSqlDelete(serverno);
        }
    }
    public class DBCommand
    {
        public int ServiceNo { get; set; }
        public string MsgId { get; set; }
        public object CommandData { get; set; }
        public IDBEntity DBEntity { get; set; }
        /// <summary>
        /// sql语句及表相关数据
        /// </summary>
        public DBTable DbTable { get; set; } = new DBTable();
        /// <summary>
        /// 记数
        /// </summary>
        public Int64 Num { get; set; } = -1;
        /// <summary>
        /// 处理类型：0本地处理，1持久化处理
        /// </summary>
        public DBCommandType Type { get; set; } = DBCommandType.local;
        /// <summary>
        /// 用于，DB保存成功后，写计数文件
        /// </summary>
        public string SuccessDBlogFilePath
        {
            get
            {
                string result = ConstantSUtil.DBlogSuccessFlagLocal;
                switch (Type)
                {
                    case DBCommandType.Persistent:
                        return ConstantSUtil.DBlogSuccessFlagPersistent;
                     
                    default:
                        return result;
                       
                }
              
            }
        }
        /// <summary>
        /// 用于，DB恢复文件
        /// </summary>
        public string RecoverDataFilePath
        {
            get
            {
                string result = ConstantSUtil.RecoverData;
                switch (Type)
                {
                    case DBCommandType.Persistent:
                        return ConstantSUtil.RecoverDataPersistent;
                       
                    default:
                        return result;
                       
                }
               
            }
        }

        public DBCommand CopyNoCommandData()
        {
            return new DBCommand()
            {
                Num = this.Num,
                ServiceNo = this.ServiceNo,
                Type = this.Type
            };
        }
    }
    public enum DBCommandType
    {
        /// <summary>
        /// 0本地处理
        /// </summary>
        local = 0,
        /// <summary>
        /// 1持久化处理
        /// </summary>
        Persistent = 1
    }
    /// <summary>
    /// 记数类型
    /// </summary>
    public enum DBCommandNumType
    {
        /// <summary>
        /// 0第一行记数
        /// </summary>
        before = 0,
        /// <summary>
        /// 1最后一行记数
        /// </summary>
        end = 1
    }
    #endregion
    #region 交易服务号
    public enum TradeServerAvailable
    {
        TE_TRADE_ADD = 9100065,
        /// <summary>
        /// 服务_新增订单
        /// </summary>
        TE_ORDER_ADD = 910001,

        /// <summary>
        /// 服务_新增修改
        /// </summary>
        TE_ORDER_UPD = 910002,

        /// <summary>
        /// 服务_订单查询（当日）
        /// </summary>
        TE_ORDER_QRY = 910003,

        /// <summary>
        /// 服务_撤销订单
        /// </summary>
        TE_ORDER_DEL = 910004,

        /// <summary>
        /// 服务_查询成交信息（当日）
        /// </summary>
        TE_MATCH_QRY = 910005,

        /// <summary>
        /// 服务_资金信息查询
        /// </summary>
        TE_FUND_QRY = 910006,

        /// <summary>
        /// 服务_持仓汇总查询-没用
        /// </summary>
        TE_POSGROUP_QRY = 910007,

        /// <summary>
        /// 服务_昨持仓明细查询
        /// </summary>
        TE_POS_QRY = 910008,

        /// <summary>
        /// 服务_价差合约信息查询
        /// </summary>
        TE_SPD_CONTRACT_QRY = 910015,

        /// <summary>
        /// 服务_强制退出消息
        /// </summary>
        TE_LOGOUT = 910016,

        /// <summary>
        /// 服务_成交流水信息查询
        /// </summary>
        TE_ORDERJOUR_QRY = 910017,

        /// <summary>
        /// 服务_客户端登入登出消息
        /// </summary>
        TE_CLIENT_LOGIN = 910018,



        /// <summary>
        /// 服务_查询操作员分组数据
        /// </summary>
        TE_OPER_GROUP_QRY = 910022,

        /// <summary>
        /// 服务_管理前置增加客户
        /// </summary>
        TE_CLIENT_ADD = 910025,

        /// <summary>
        /// 批量导入开户，批量传参数(新增账户)
        /// </summary>
        TE_CLIENTS_ADD = 9100250,



        /// <summary>
        /// 服务_客户分组改变
        /// </summary>
        //TE_CLIENT_GROUP_CHANGE = 910027,

        /// <summary>
        /// 服务_操作员分组改变
        /// </summary>
        //TE_OPER_GROUP_CHANGE = 910028,

        /// <summary>
        /// 服务_客户信息改变
        /// </summary>
        TE_CLIENT_INFO_CHANGE = 910029,

        /// <summary>
        /// 服务_商品信息改变
        /// </summary>
        TE_COMMODITY_INFO_CHANGE = 910030,

        /// <summary>
        /// 服务_保证金信息改变
        /// </summary>
        TE_MARGIN_INFO_CHANGE = 910031,

        /// <summary>
        /// 服务_佣金信息改变
        /// </summary>
        TE_FEE_INFO_CHANGE = 910032,

        /// <summary>
        /// 服务_查询客户的信用额度(风控&强平)
        /// </summary>
        TE_CREDIT_QRY = 910035,

        /// <summary>
        /// 服务_修改客户商品状态-没用
        /// </summary>
        TE_USER_COMMODITY_STATUS = 910038,

        /// <summary>
        /// 服务_修改客户商品上手-没用
        /// </summary>
        TE_USER_COMMODITY_PROXY = 910039,

        /// <summary>
        /// 服务_修改客户商品持仓量-没用
        /// </summary>
        TE_USER_COMMODITY_COUNT = 910040,

        /// <summary>
        /// 服务_强平删除所有挂单委托
        /// </summary>
        TE_FORCE_CLOSE_ORDER_DEL = 910041,

        /// <summary>
        /// 服务_定时获取所有前置的所有客户信息
        /// </summary>
        TE_GET_CLIENT_LOGIN_STATUS = 910042,

        /// <summary>
        /// 服务_柜台的录单信息
        /// </summary>
        TE_COUNTER_ORDER_IN = 910043,

        /// <summary>
        /// 服务_导入结算价(柜台)
        /// </summary>
        TE_CONTRACT_PRICE_LOAD = 910044,

        /// <summary>
        /// 服务_设置LME到期日
        /// </summary>
        TE_SETTING_LME_VALUE_DATE = 910045,

        /// <summary>
        /// 服务_手动行权
        /// </summary>
        TE_MENU_STRIKE = 910046,

        /// <summary>
        /// 服务_查询期权最新价
        /// </summary>
        TE_OPTION_NEW_PRICE_QRY = 910047,

        /// <summary>
        /// 服务_查询当天交割信息
        /// </summary>
        TE_DELIVERY_MATCH_QRY = 910048,

        /// <summary>
        /// 服务_查询交易日期
        /// </summary>
        TE_TRADE_DAY_QRY = 910049,

        /// <summary>
        /// 服务_cd录单审核
        /// </summary>
        TE_CD_ORDER_VERIFY = 910050,

        /// <summary>
        /// 服务_查询上手
        /// </summary>
        TE_PROXY_QRY = 910051,

        /// <summary>
        /// 服务_修改委托状态
        /// </summary>
        TE_ORDER_STATUS_MODIFY = 910052,

        /// <summary>
        /// 服务_修改昨资金
        /// </summary>
        TE_SET_PRE_BALANCE = 910054,

        /// <summary>
        /// 服务_设置T+N日期 节假日
        /// </summary>
        TE_SET_T_PLUS_N_DATE = 910055,

        /// <summary>
        /// 服务_客户持仓明细查询
        /// </summary>
        TE_ACCOUNT_POS_QRY = 910056,

        /// <summary>
        /// 服务_单个持仓强平
        /// </summary>
        TE_FORCE_CLOSE_ONE_POS = 910057,

        /// <summary>
        /// 服务_客户基币资金查询
        /// </summary>
        TE_ACCOUNT_BASE_FUND_QRY = 910058,

        /// <summary>
        /// 服务_客户资金明细查询
        /// </summary>
        TE_ACCOUNT_FUND_QRY = 910059,

        /// <summary>
        /// 服务_查询支持发送短信的客户列表
        /// </summary>
        TE_SMS_ACCOUNTS_QRY = 910060,

        /// <summary>
        /// 服务_客户持仓汇总查询
        /// </summary>
        TE_ACCOUNT_POS_SUM_QRY = 910061,

        /// <summary>
        /// 服务_查询某个合约的所有持仓列表
        /// </summary>
        TE_ACCOUNT_WITH_CONTRACT_QRY = 910062,

        #region 保存数据库
        DB_ADD_ORDER = 100,
        DB_UPD_ORDER = 101,
        DB_ADD_ORDER_JOUR = 102,
        DB_ADD_MATCH = 103,

        DB_ADD_ORDER_ICE = 1000,
        DB_UPD_ORDER_ICE = 1010,
        DB_ADD_ORDER_JOUR_ICE = 1020,
        DB_ADD_MATCH_ICE = 1030,

        DB_ADD_POSITION = 104,
        DB_ADD_LME_DATE_PL = 105,
        DB_ADD_DELIVERY = 106,
        DB_ADD_MONEY_NOW = 107,
        DB_ADD_POS_NOW = 108,
        DB_PRE_SETTLE = 109,
        DB_CD_ORDER_ADD = 110,
        DB_CD_ORDER_UPD = 111,
        DB_UPD_MATCH = 112,
        DB_UPD_ORDER_EX = 113,
        DB_UPD_PRE_FUND = 114,
        DB_ADD_CLOSE = 115,
        DB_ADD_PRE_FUND = 116,
        DB_MATCH_SMS = 117,
        DB_DEL_POSITION = 118,
        DB_LOGIN_INFO_ADD = 119,
        DB_UPD_MATCH_ORDER_FROM = 120,
        DB_UPD_ORDER_ORDER_FROM = 121,
        DB_TRADE_CHANGE_RECORD_ADD = 122,
        DB_TRADE_CHANGE_SMS_ADD = 123,
        #endregion


        #region 管理前置（查询）*****
        #region 操作员管理
        /// <summary>
        /// 服务_查询所有角色信息
        /// </summary>
        TE_ROLE_QRY = 911000,

        /// <summary>
        /// 服务_查询所有操作员信息
        /// </summary>
        TE_OPER_QRY = 911001,

        /// <summary>
        /// 服务_查询所有权限信息
        /// </summary>
        TE_RIGHT_QRY = 911002,

        /// <summary>
        /// 服务_查询所有隐藏权限信息
        /// </summary>
        TE_RIGHT_HIDE_QRY = 911003,

        /// <summary>
        /// 服务_查询角色有哪些权限
        /// </summary>
        TE_ROLE_RIGHT_QRY = 911004,

        /// <summary>
        /// 服务_查询角色有哪些隐藏权限
        /// </summary>
        TE_ROLE_RIGHT_HIDE_QRY = 911005,

        /// <summary>
        /// 服务_查询操作员有哪些角色
        /// </summary>
        TE_OPER_ROLE_QRY = 911006,

        /// <summary>
        /// 服务_查询所有分组信息
        /// </summary>
        TE_GROUP_QRY = 911007,

        /// <summary>
        /// 服务_查询所有用户信息(部分)
        /// </summary>
        TE_ACCOUNT_QRY = 911008,

        /// <summary>
        /// 服务_查询操作员下面的分组_柜台用
        /// </summary>
        TE_OPER_GROUP_QRY_COUNTER = 911009,

        /// <summary>
        /// 服务_查询操作员下面的用户
        /// </summary>
        TE_OPER_ACCOUNT_QRY = 911010,

        /// <summary>
        /// 服务_新增角色
        /// </summary>
        TE_ROLE_ADD = 912000,

        /// <summary>
        /// 服务_修改角色
        /// </summary>
        TE_ROLE_UPD = 912001,

        /// <summary>
        /// 服务_修改角色权限
        /// </summary>
        TE_ROLE_RIGHTS_UPD = 912002,

        /// <summary>
        /// 服务_新增操作员
        /// </summary>
        TE_OPER_ADD = 910026,

        /// <summary>
        /// 服务_修改操作员
        /// </summary>
        TE_OPER_UPD = 912003,

        /// <summary>
        /// 服务_修改操作员角色
        /// </summary>
        TE_OPER_ROLE_UPD = 912004,

        /// <summary>
        /// 服务_操作员增加分组
        /// </summary>
        TE_OPER_GROUP_ADD = 912005,

        /// <summary>
        /// 服务_操作员删除分组
        /// </summary>
        TE_OPER_GROUP_DEL = 912006,

        /// <summary>
        /// 服务_增加角色隐藏权限
        /// </summary>
        TE_ROLE_RIGHTS_HIDE_ADD = 912007,

        /// <summary>
        /// 服务_修改角色隐藏权限
        /// </summary>
        TE_ROLE_RIGHTS_HIDE_DEL = 912008,
        #endregion

        #region 参数管理
        /// <summary>
        /// 服务_查询币种
        /// </summary>
        TE_CURRENCY_QRY = 912009,

        /// <summary>
        /// 服务_增加币种
        /// </summary>
        TE_CURRENCY_ADD = 912010,

        /// <summary>
        /// 服务_修改币种
        /// </summary>
        TE_CURRENCY_UPD = 912011,

        /// <summary>
        /// 服务_查询汇率数据
        /// </summary>
        TE_RATE_QRY = 910021,

        /// <summary>
        /// 服务_增加汇率
        /// </summary>
        TE_RATE_ADD = 912012,

        /// <summary>
        /// 服务_修改汇率
        /// </summary>
        TE_RATE_UPD = 912013,

        /// <summary>
        /// 服务_市场信息查询
        /// </summary>
        TE_EXCHANGE_QRY = 910011,

        /// <summary>
        /// 服务_增加市场
        /// </summary>
        TE_EXCHANGE_ADD = 912014,

        /// <summary>
        /// 服务_修改市场
        /// </summary>
        TE_EXCHANGE_UPD = 912015,

        /// <summary>
        /// 服务_商品信息查询
        /// </summary>
        TE_COMMODITY_QRY = 910012,

        /// <summary>
        /// 服务_增加商品
        /// </summary>
        TE_COMMODITY_ADD = 912016,

        /// <summary>
        /// 服务_修改商品
        /// </summary>
        TE_COMMODITY_UPD = 912017,

        /// <summary>
        /// 服务_期货合约信息查询
        /// </summary>
        TE_FUT_CONTRACT_QRY = 910013,

        /// <summary>
        /// 服务_增加期货合约
        /// </summary>
        TE_FUT_CONTRACT_ADD = 912018,

        /// <summary>
        /// 服务_修改期货合约
        /// </summary>
        TE_FUT_CONTRACT_UPD = 912019,

        /// <summary>
        /// 服务_期权合约信息查询
        /// </summary>
        TE_OPT_CONTRACT_QRY = 910014,

        /// <summary>
        /// 服务_增加期权合约
        /// </summary>
        TE_OPT_CONTRACT_ADD = 912020,

        /// <summary>
        /// 服务_修改期权合约
        /// </summary>
        TE_OPT_CONTRACT_UPD = 912021,

        /// <summary>
        /// 服务_查询保证金
        /// </summary>
        TE_MARGIN_QRY = 910020,

        /// <summary>
        /// 服务_期货合约信息查询-api用
        /// </summary>
        TE_API_FUT_CONTRACT_QRY = 910063,

        /// <summary>
        /// 服务_修改客户密码登录设置
        /// </summary>
        TE_MODIFY_ACCOUNT_PASS_LOGIN = 910064,

        /// <summary>
        /// 服务_查询出金未审核金额
        /// </summary>
        TE_ACCOUNT_WITHDRAWAL_UNREVIEWED_QRY = 910065,

        /// <summary>
        /// 服务_增加保证金
        /// </summary>
        TE_MARGIN_ADD = 912022,

        /// <summary>
        /// 服务_修改保证金
        /// </summary>
        TE_MARGIN_UPD = 912023,

        /// <summary>
        /// 服务_查询佣金
        /// </summary>
        TE_FEE_QRY = 912024,

        /// <summary>
        /// 服务_增加佣金-没有用，佣金不是实时生效，由管理前置写数据库
        /// </summary>
        TE_FEE_ADD = 912025,

        /// <summary>
        /// 服务_修改佣金-没有用，佣金不是实时生效，由管理前置写数据库
        /// </summary>
        TE_FEE_UPD = 912026,

        /// <summary>
        /// 服务_查询结算价
        /// </summary>
        TE_CP_QRY = 912027,

        /// <summary>
        /// 服务_增加结算价
        /// </summary>
        TE_CP_ADD = 912028,

        /// <summary>
        /// 服务_修改结算价
        /// </summary>
        TE_CP_UPD = 912029,

        /// <summary>
        /// 服务_查询所有佣金分组
        /// </summary>
        TE_FEE_CLASS_QRY = 912030,

        /// <summary>
        /// 服务_批量修改账号所在分组
        /// </summary>
        TE_FEE_CLASS_UPD = 912031,

        /// <summary>
        /// 服务_增加价差合约
        /// </summary>
        TE_SPD_CONTRACT_ADD = 912032,

        /// <summary>
        /// 服务_修改价差合约
        /// </summary>
        TE_SPD_CONTRACT_UPD = 912033,

        /// <summary>
        /// 服务_查询币种-API
        /// </summary>
        TE_CURRENCY_QRY_EX = 912034,

        /// <summary>
        /// 服务_删除保证金
        /// </summary>
        TE_MARGIN_DEL_ALL = 912035,

        /// <summary>
        /// 服务_查询商品的浮动价格
        /// </summary>
        TE_COMMODITY_TICK_SIZE_QRY = 912036,
        /// <summary>
        /// 查询单用户信息
        /// </summary>
        TE_ACCOUNTINFO_QRY = 912037,

        /// <summary>
        /// 服务_查询撤单信息设置
        /// </summary>
        TE_ORDER_DELED_INFO_QRY = 912052,

        /// <summary>
        /// 服务_增加撤单信息设置
        /// </summary>
        TE_ORDER_DELED_INFO_ADD = 912053,

        /// <summary>
        /// 服务_修改撤单信息设置
        /// </summary>
        TE_ORDER_DELED_INFO_UPD = 912054,
        /// <summary>
        /// 服务_删除撤单信息设置
        /// </summary>
        TE_ORDER_DELED_INFO_DEL = 912055,


        /// <summary>
        /// 查询休市时间信息设置
        /// </summary>
        TE_MARKETCLOSEDTIME_QRY= 912061,

        /// <summary>
        /// 新增休市时间信息设置
        /// </summary>
        TE_MARKETCLOSEDTIME_ADD = 912062,

        /// <summary>
        /// 修改休市时间信息设置
        /// </summary>
        TE_MARKETCLOSEDTIME_UPD = 912063,

        /// <summary>
        /// 删除休市时间信息设置
        /// </summary>
        TE_MARKETCLOSEDTIME_DEL = 912064,


        /// <summary>
        /// 服务_风控_账户_撤单
        /// </summary>
        TE_RISK_ORDER_DELED = 912056,

        #endregion

        #region 账户管理
        /// <summary>
        /// 服务_增加账户交易信息
        /// </summary>
        TE_ACCOUNT_ADD = 913001,

        /// <summary>
        /// 服务_增加账户信息
        /// </summary>
        TE_ACCOUNT_INFO_ADD = 913002,

        /// <summary>
        /// 服务_修改账户交易信息
        /// </summary>
        TE_ACCOUNT_UPD = 913003,

        /// <summary>
        /// 服务_修改账户信息
        /// </summary>
        TE_ACCOUNT_INFO_UPD = 913004,

        /// <summary>
        /// 服务_增加账户分组
        /// </summary>
        TE_GROUP_ADD = 913005,

        /// <summary>
        /// 服务_修改账户分组
        /// </summary>
        TE_GROUP_UPD = 913006,

        /// <summary>
        /// 服务_加入分组
        /// </summary>
        TE_GROUP_IN = 913007,

        /// <summary>
        /// 服务_移出分组
        /// </summary>
        TE_GROUP_OUT = 913008,

        /// <summary>
        /// 服务_查询分组账号
        /// </summary>
        TE_GROUP_ACCOUNT_QRY = 913009,

        /// <summary>
        /// 服务_新增出入金
        /// </summary>
        TE_DEPOSIT_ADD = 910019,
        /// <summary>
        /// 服务_更新出入金
        /// </summary>
        TE_DEPOSIT_UPD_CACHE = 9100191,
        /// <summary>
        /// 服务_新增出入金-无需审核
        /// </summary>
        TE_DEPOSIT_ADD_AUTO = 100209,

        /// <summary>
        /// 服务_出入金审核
        /// </summary>
        TE_DEPOSIT_VERIFY = 913031,

        /// <summary>
        /// 服务_出入金记录查询
        /// </summary>
        TE_DEPOSIT_QRY = 913032,

        /// <summary>
        /// 服务_信用额度信息改变
        /// </summary>
        TE_CREDIT_CHANGE = 910033,

        /// <summary>
        /// 服务_删除客户的风险率
        /// </summary>
        TE_DEL_RISK = 910037,

        /// <summary>
        /// 服务_风险率信息改变
        /// </summary>
        TE_RISK_CHANGE = 910034,

        /// <summary>
        /// 批量导入开户，批量传参数(新增风险率)
        /// </summary>
        TE_RISKS_CHANGE = 1001050,

        /// <summary>
        /// 服务_查询所有客户的风险率
        /// </summary>
        TE_ALL_RISK_QRY = 910036,

        /// <summary>
        /// 服务_查询客户品种上手列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_PROXY_QRY = 913010,

        /// <summary>
        /// 服务_增加客户品种上手列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_PROXY_ADD = 913011,

        /// <summary>
        /// 服务_删除客户品种上手列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_PROXY_DEL = 913012,

        /// <summary>
        /// 服务_查询客户品种状态列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_STATUS_QRY = 913013,

        /// <summary>
        /// 服务_增加客户品种状态列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_STATUS_ADD = 913014,

        /// <summary>
        /// 服务_删除客户品种状态列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_STATUS_DEL = 913015,

        /// <summary>
        /// 服务_查询客户品种数量列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_COUNT_QRY = 913016,

        /// <summary>
        /// 服务_增加客户品种数量列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_COUNT_ADD = 913017,

        /// <summary>
        /// 服务_删除客户品种数量列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_COUNT_DEL = 913018,

        /// <summary>
        /// 服务_修改客户品种数量列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_COUNT_UPD = 913019,

        /// <summary>
        /// 服务_修改客户品种状态列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_STATUS_UPD = 913020,

        /// <summary>
        /// 服务_修改客户品种上手列表
        /// </summary>
        TE_ACCOUNT_COMMODITY_PROXY_UPD = 913021,

        /// <summary>
        /// 服务_查询用户权限列表
        /// </summary>
        TE_ACCOUNT_RIGHTS_QRY = 913022,

        /// <summary>
        /// 服务_查询用户权限关系
        /// </summary>
        TE_ACCOUNT_RIGHTS_RELATION_QRY = 913023,

        /// <summary>
        /// 服务_增加用户权限关系
        /// </summary>
        TE_ACCOUNT_RIGHTS_RELATION_ADD = 913024,

        /// <summary>
        /// 服务_删除用户权限关系
        /// </summary>
        TE_ACCOUNT_RIGHTS_RELATION_DEL = 913025,

        /// <summary>
        /// 服务_修改账户交易状态
        /// </summary>
        TE_ACCOUNT_TRADE_STATUS_MODIFY = 913026,

        /// <summary>
        /// 服务_修改账户行情状态
        /// </summary>
        TE_ACCOUNT_QUOT_STATUS_MODIFY = 913027,

        /// <summary>
        /// 服务_修改账户商品受限状态
        /// </summary>
        TE_ACCOUNT_COMMODITY_STATUS_MODIFY = 913028,

        /// <summary>
        /// 服务_修改账户风险状态
        /// </summary>
        TE_ACCOUNT_RISK_STATUS_MODIFY = 913029,

        /// <summary>
        /// 服务_修改操作员状态
        /// </summary>
        TE_OPER_TRADE_STATUS_MODIFY = 913030,

        /// <summary>
        /// 服务_修改客户保证金分组
        /// </summary>
        TE_ACCOUNT_MARGIN_MODIFY = 913033,
        #endregion

        #region 交易查询
        /// <summary>
        /// 服务_柜台查询实时持仓
        /// </summary>
        TE_CONTER_POS_QRY = 916000,

        /// <summary>
        /// 服务_柜台查询实时资金-强平用
        /// </summary>
        TE_CONTER_MONEY_QRY = 916001,

        /// <summary>
        /// 服务_柜台查询实时委托
        /// </summary> 912052
        TE_CONTER_ORDER_QRY = 916002,
        /// <summary>
        /// 查询客户登录数
        /// </summary>
        TE_CLIENT_COUNT_LOGIN_QRY = 916003,
        #endregion

        #endregion

        TE_TRADE_FRONT_WRITEDB = 912065,


        /// <summary>
        /// 服务_柜台发送成交转移命令
        /// </summary>
        TE_MOVE_TRADE = 912066,

        /// <summary>
        /// api通过前置修改用户手机和邮箱
        /// </summary>
        TE_API_CLIENT_INFO_CHANGE = 610022,
        /// <summary>
        /// 查询经纪关系设置
        /// </summary>
        TE_AGENT_ACCOUNT_QRY = 914001,
        /// <summary>
        /// 查询返佣设置
        /// </summary>
        TE_AGENT_QRY = 914002,

        /// <summary>
        /// 开户新增客户经纪关系
        /// </summary>
        TE_AGENT_ACCOUNT_ASSOCIATE_ADD = 914003,

        /// <summary>
        /// 成交转移更新委托，投入队列产生消息号
        /// </summary>
        TE_TRADECHANGE_UPDATEORDER = 915001,
        /// <summary>
        /// 网关委托恢复，更新委托和流水，投入队列产生消息号
        /// </summary>
        TE_GATEWAY_RECOVERY_ORDER = 915002
    }

    /// <summary>
    /// 推给前置的服务号
    /// </summary>
    public enum TradeServerCallBack
    {
        /// <summary>
        /// 消息_订单状态变化信息
        /// </summary>
        SE_ORDER_STATUS = 920001,

        /// <summary>
        /// 消息_成交变化信息
        /// </summary>
        SE_MATCH_INFO = 920002,

        /// <summary>
        /// 消息_资金变化信息
        /// </summary>
        SE_FUND_INFO = 920003,

        /// <summary>
        /// 消息_持仓变化信息-没有用
        /// </summary>
        SE_POS_INFO = 920004,

        /// <summary>
        /// 消息_订单流水变化信息
        /// </summary>
        SE_ORDERJOUR = 920005,

        /// <summary>
        /// 消息_交易通知信息（没用）
        /// </summary>
        SE_TRADING_MSG = 920006,

        /// <summary>
        /// 消息_强制登出信息
        /// </summary>
        SE_LOGOUT = 920007,

        /// <summary>
        /// 消息_下单同步
        /// </summary>
        SE_ORDER_INSERT = 920008,

        /// <summary>
        /// 消息_请求客户列表
        /// </summary>
        SE_REQ_CLIENT_INFO = 920009,

        /// <summary>
        /// 消息_给管理前置推送在线客户信息
        /// </summary>
        SE_PUSH_CLIENT_INFO = 920012,

        /// <summary>
        /// 消息_分组关系改变消息
        /// </summary>
        SE_PUSH_GROUP_CHANGE = 920013,

        /// <summary>
        /// 消息_给管理前置推送客户风险率信息
        /// </summary>
        SE_PUSH_RISK_RATE_INFO = 920014,

        /// <summary>
        /// 消息_给管理前置推送保证金变化
        /// </summary>
        SE_PUSH_MARGIN_INFO = 920015,

        /// <summary>
        /// 消息_给交易前置推送客户信息变化
        /// </summary>
        SE_PUSH_ACCOUNT_INFO_CHANGE = 920016,

        /// <summary>
        /// 消息_给交易前置推送LME结算价变化
        /// </summary>
        SE_PUSH_CONTRACT_PRICE_LME = 920017,

        /// <summary>
        /// 消息_推送手动行权的交割信息
        /// </summary>
        SE_PUSH_DILEVERY_MATCH = 920018,

        /// <summary>
        /// 消息_推送客户风险率变化给交易前置
        /// </summary>
        SE_PUSH_RISK_CHANGE = 920019,

        /// <summary>
        /// 消息_推送删除客户风险率给交易前置
        /// </summary>
        SE_PUSH_RISK_DEL = 920020,

        /// <summary>
        /// 消息_给管理前置推送客户交易状态变化
        /// </summary>
        SE_PUSH_ACCOUNT_TRADE_STATUS = 920021,

        /// <summary>
        /// 消息_给交易前置推送LME结算价变化
        /// </summary>
        SE_PUSH_CONTRACT_PRICE_ALL = 920022,

        /// <summary>
        /// 推送交易前置新增客户经纪关系（新增开户）
        /// </summary>
        SE_PUSH_AGENT_ASSOCIATE = 920024,

    }

    public enum TradeServerMonitor
    {
        /// <summary>
        /// 服务_查询交易服务器信息
        /// </summary>
        TE_TRADE_SERVER_INFO_QRY = 610001,

        /// <summary>
        /// 服务_查询前置列表
        /// </summary>
        TE_FRONT_CLIENT_LIST_QRY = 610002,

        /// <summary>
        /// 服务_查询委托队列数量
        /// </summary>
        TE_ORDER_COUNT_QRY = 610003,

        /// <summary>
        /// 服务_查询状态队列数量
        /// </summary>
        TE_STATUS_QUEUE_COUNT_QRY = 610004,

        /// <summary>
        /// 服务_查询成交队列数量
        /// </summary>
        TE_MATCH_QUEUE_COUNT_QRY = 610005,

        /// <summary>
        /// 服务_查询日志列表
        /// </summary>
        TE_LOG_QRY = 610006,

        /// <summary>
        /// 服务_查询所有队列数量
        /// </summary>
        TE_QUEUE_NUM_ALL_QRY = 610007,

        /// <summary>
        /// 服务_查询上手信息
        /// </summary>
        TE_PROXY_INFO_QRY = 610008,

        /// <summary>
        /// 服务_查询错误日志信息
        /// </summary>
        TE_ERR_LOG_QRY = 610009,

        /// <summary>
        /// 服务_查询连接行情状态信息
        /// </summary>
        TE_QUOT_STATUS_QRY = 610010,
    }

    #endregion

    #region 网关服务号
    public enum GatewayServerAvailable
    {
        /// <summary>
        /// 服务_新增订单
        /// </summary>
        TE_ORDER_ADD = 940001,

        /// <summary>
        /// 服务_新增修改
        /// </summary>
        TE_ORDER_UPD = 940002,

        /// <summary>
        /// 服务_订单查询（当日）
        /// </summary>
        TE_ORDER_QRY = 940003,

        /// <summary>
        /// 服务_撤销订单
        /// </summary>
        TE_ORDER_DEL = 940004,

        /// <summary>
        /// 服务_查询成交信息（当日）
        /// </summary>
        TE_MATCH_QRY = 940005,

        /// <summary>
        /// 服务_资金信息查询
        /// </summary>
        TE_FUND_QRY = 940006,

        /// <summary>
        /// 服务_持仓汇总查询
        /// </summary>
        TE_POSGROUP_QRY = 940007,

        /// <summary>
        /// 服务_持仓明细查询
        /// </summary>
        TE_POS_QRY = 940008,

        /// <summary>
        /// 服务_历史委托查询
        /// </summary>
        TE_HISORDER_QRY = 940009,

        /// <summary>
        /// 服务_历史成交信息查询
        /// </summary>
        TE_HISMATCH_QRY = 940010,

        /// <summary>
        /// 服务_市场信息查询
        /// </summary>
        TE_EXCHANGE_QRY = 940011,

        /// <summary>
        /// 服务_商品信息查询
        /// </summary>
        TE_COMMODITY_QRY = 940012,

        /// <summary>
        /// 服务_期货合约信息查询
        /// </summary>
        TE_FUT_CONTRACT_QRY = 940013,

        /// <summary>
        /// 服务_期权合约信息查询
        /// </summary>
        TE_OPT_CONTRACT_QRY = 940014,

        /// <summary>
        /// 服务_价差合约信息查询
        /// </summary>
        TE_SPD_CONTRACT_QRY = 940015,
    }

    public enum GatewayServerCallBack
    {
        /// <summary>
        /// 消息_订单状态变化信息
        /// </summary>
        SE_ORDER_STATUS = 930001,

        /// <summary>
        /// 消息_成交变化信息
        /// </summary>
        SE_MATCH_INFO = 930002,

        /// <summary>
        /// 消息_资金变化信息
        /// </summary>
        SE_FUND_INFO = 930003,

        /// <summary>
        /// 消息_持仓变化信息
        /// </summary>
        SE_POS_INFO = 930004,

        /// <summary>
        /// 消息_订单流水变化信息
        /// </summary>
        SE_ORDERJOUR = 930005,

        /// <summary>
        /// 消息_交易通知信息
        /// </summary>
        SE_TRADING_MSG = 930006,

        /// <summary>
        /// 消息_强制登出信息
        /// </summary>
        SE_LOGOUT = 930007,

        /// <summary>
        /// 消息_下单同步
        /// </summary>
        SE_ORDER_INSERT_PROXY = 930008,

        /// <summary>
        /// 消息_委托恢复
        /// </summary>
        SE_ORDER_RECOVERY_PROXY = 930009,

        /// 消息_新增期货合约信息
        /// </summary>
        SE_NEW_FUT_CONTRACT = 930013,

        /// <summary>
        /// 消息_新增期权合约信息
        /// </summary>
        SE_NEW_OPT_CONTRACT = 930014,

        /// <summary>
        /// 消息_新增价差合约信息
        /// </summary>
        SE_NEW_SPD_CONTRACT = 930015,

        /// <summary>
        /// 委托恢复，TT网关模拟改单命令
        /// </summary>
        SE_CHANGE_ORDER_PROXY = 930020,

        /// <summary>
        /// TT网关恢复阶段完成，可以接收新的订单服务
        /// </summary>
        SE_ORDER_RECOVERY_DONE = 930021,

        /// <summary>
        /// 给主TT网关发送TT处理过的最大EXEc_id
        /// </summary>
        TE_ORDER_MAX_EXECID_RETURN = 930011
    }
    #endregion

    #region 风控服务
    public enum RiskService
    {
        /// <summary>
        /// 消息_推送客户的基币资金
        /// </summary>
        SE_BASE_FUND = 300001,

        /// <summary>
        /// 消息_推送客户的分币种资金的追保情况
        /// </summary>
        SE_MARGIN_ADD = 300002,

        /// <summary>
        /// 消息_推送客户的即将到期的持仓情况
        /// </summary>
        SE_POSITION_EXPIRE = 921001, //交易前置//300003,

        /// <summary>
        /// 消息_注册风控数据
        /// </summary>
        SE_RISK_REG = 300004,

        /// <summary>
        /// 消息_推送强平成交
        /// </summary>
        SE_FORCE_CLOSE_MATCH = 300005,

        /// <summary>
        /// 消息_推送强平资金
        /// </summary>
        SE_FORCE_CLOSE_FUND = 300006,

        /// <summary>
        /// 消息_推送强平委托
        /// </summary>
        SE_FORCE_CLOSE_ORDER = 300007,

        /// <summary>
        /// 消息_推送精简资金信息
        /// </summary>
        SE_PUSH_FUND_MIN = 300008,
    }
    #endregion

    #region 行情服务
    public enum QuotService
    {
        /// <summary>
        /// 服务_交易服务订阅
        /// </summary>
        TE_QUOT_TRADE_REG = 930005,
    }
    #endregion


    public enum MonitorService
    {
        TE_SVR_CFG_QRY = 70005,
        TE_SVR_CFG_UPD = 70006,
        TE_SVR_CMD = 70008

    }
    /// <summary>
    /// 持久化服务推送
    /// </summary>
    public enum PersistentService
    {
        SE_PUSH_DBFLAG = 950001
    }

    public enum PersistentServiceLogLevelType
    {
        log = 0,
        db = 1,
        debut = 4,
        info = 5,
        war = 6,
        error = 7,
        notice = 8
    }

    #region 主交易服推送消息给备交易服 服务号
    public enum ActiveTrade_To_Standby
    {
        /// <summary>
        /// 推送下单给备交易服
        /// </summary>
        PushInsertOrderToStandby = 27000000,
        /// <summary>
        /// 推送改单给备交易服
        /// </summary>
        PushUpdateOrderToStandby = 27000001,
        /// <summary>
        /// 推送撤单给备交易服
        /// </summary>
        PushDeleteOrderToStandby = 27000002,
        /// <summary>
        /// 推送委托状态变化给备交易服
        /// </summary>
        PushOrderStatusToStandby = 27000003,
        /// <summary>
        /// 推送成交给备交易服
        /// </summary>
        PushTradeToStandby = 27000004,
        
        ///下阶段补缺： 人工录单，人工修改委托状态等
    }

    /// <summary>
    /// 备交易服推送给主的消息
    /// </summary>
    public enum StandbyTrade_To_Active
    {
        /// <summary>
        /// 对主发送给备的下单消息进行确认
        /// </summary>
        //ConfirmMsgToActive = 29000000
    }
    #endregion
}
