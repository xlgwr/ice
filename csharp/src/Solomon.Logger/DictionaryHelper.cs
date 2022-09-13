using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.CommonTools
{
    public class DictionaryHelper
    {
        public class Exchange
        {
            public const string HKFE = "HKFE";
            /// <summary>
            /// 新加坡交易所
            /// </summary>
            public const string SGX = "SGX";
        }
        public class YesOrNo
        {
            public const string YES = "1";
            public const string No = "0";
        }
        public class TimesInfo
        {
            public const int SpreadMarginSize = 2;
        }
        /// <summary>
        /// 商品类别
        /// </summary>
        public class PRODUCT_TYPE
        {
            /// <summary>
            /// 期货
            /// </summary>
            public const sbyte PRODUCT_TYPE_FUTURE = 0;

            /// <summary>
            /// 价差
            /// </summary>
            public const sbyte PRODUCT_TYPE_SPEARD = 2;

            /// <summary>
            /// 期权
            /// </summary>
            public const sbyte PRODUCT_TYPE_OPTION = 1;
        }

        /// <summary>
        /// 商品状态
        /// </summary>
        public class PRODUCT_STATUS
        {
            /// <summary>
            /// 正常交易
            /// </summary>
            public const int PRODUCT_STATUS_NORMAL = 0;

            /// <summary>
            /// 只可平仓
            /// </summary>
            public const int PRODUCT_STATUS_CLOSEONLY = 1;

            /// <summary>
            /// 禁止交易
            /// </summary>
            public const int PRODUCT_STATUS_FORBIDEN = 2;
        }

        /// <summary>
        /// 登录身份
        /// </summary>
        public class LOGIN_TYPE
        {
            /// <summary>
            /// 客户
            /// </summary>
            public const sbyte LOGIN_TYPE_CUS = (sbyte)'C';

            /// <summary>
            /// AE
            /// </summary>
            public const sbyte LOGIN_TYPE_AE = (sbyte)'A';

            /// <summary>
            /// 交易员
            /// </summary>
            public const sbyte LOGIN_TYPE_TRADER = (sbyte)'T';
        }

        /// <summary>
        /// 买卖方向
        /// </summary>
        public class ORDER_DERECTION
        {
            /// <summary>
            /// 买
            /// </summary>
            public const sbyte BUY = 0;

            /// <summary>
            /// 卖
            /// </summary>
            public const sbyte SALE = 1;
        }

        /// <summary>
        /// 商品交割方式
        /// </summary>
        public class DELIVERY_TYPE
        {
            /// <summary>
            /// 现金交割
            /// </summary>
            public const sbyte CASH_DELIVERY = 0;

            /// <summary>
            /// 实物交割
            /// </summary>
            public const sbyte COMMODITY_DELIVERY = 1;
        }

        /// <summary>
        /// 是否T+1
        /// </summary>
        public class IS_T_NEXT_DAY
        {
            /// <summary>
            /// 非T+1交易
            /// </summary>
            public const sbyte T_NOT_NEXT_DAY = 0;

            /// <summary>
            /// T+1交易
            /// </summary>
            public const sbyte T_NEXT_DAY = 1;
        }

        /// <summary>
        /// 委托类型
        /// </summary>
        public class ORDER_TYPE
        {
            /// <summary>
            /// 限价单
            /// </summary>
            public const sbyte LIMIT_ORDER = 0;

            /// <summary>
            /// 市价单
            /// </summary>
            public const sbyte MARKET_ORDER = 1;

            /// <summary>
            /// 止损单
            /// </summary>
            public const sbyte STOP_ORDER = 2;

            /// <summary>
            /// 限价止损单
            /// </summary>
            public const sbyte LIMIT_STOP_ORDER = 3;

            /// <summary>
            /// 止盈止损单
            /// </summary>
            public const sbyte OCO_ORDER = 4;

            /// 竞价市价单
            public const sbyte AUCTION_MARKET_ORDER = 5;
        }

        /// <summary>
        /// 委托时限
        /// </summary>
        public class ORDER_LIFE
        {
            /// <summary>
            /// 当日有效
            /// </summary>
            public const sbyte DAY_ORDER = 0;

            /// <summary>
            /// 长期有效
            /// </summary>
            public const sbyte GTC_ORDER = 1;

            /// <summary>
            /// 截至日期有效
            /// </summary>
            public const sbyte GTD_ORDER = 2;

            /// <summary>
            /// fill or kill
            /// </summary>
            public const sbyte FOK_ORDER = 3;

            /// <summary>
            /// fill and kill
            /// </summary>
            public const sbyte FAK_ORDER = 4;

        }

        /// <summary>
        /// 委托方式
        /// </summary>
        public class ORDER_FROM
        {
            /// <summary>
            /// 客户端传入
            /// </summary>
            public const sbyte CLIENT_IN = 0;

            /// <summary>
            /// API传入
            /// </summary>
            public const sbyte API_IN = 2;

            /// <summary>
            /// Risk
            /// </summary>
            public const sbyte RISK_IN = 3;

            /// <summary>
            /// IOS
            /// </summary>
            public const sbyte IOS_IN = 4;

            /// <summary>
            /// Android
            /// </summary>
            public const sbyte ANDROID_IN = 5;

            /// <summary>
            /// 柜台录单
            /// </summary>
            public const sbyte COUNTER_IN = 6;

            /// <summary>
            /// 人工强平单
            /// </summary>
            public const sbyte MANUAL_CLOSE = 7;

            /// <summary>
            /// 人工修改订单状态
            /// </summary>
            public const sbyte MANUAL_STATUS_MODIFY = 8;

            /// <summary>
            /// Ftrade自动休市撤单  
            /// </summary>
            public const sbyte FtradeAuto = 9;

            /// <summary>
            /// 修复，恢复客户仓位的委托设置为Modify， 给客户录单 (app 不显示本标记的委托和成交)
            /// </summary>
            public const sbyte Modify = 10;

            /// <summary>
            /// 强平修复，将错误的强平成交，修改为ForceCloseModify，不显示给客户 (app 不显示本标记的委托和成交)
            /// </summary>
            public const sbyte ForceCloseModify = 11;

            /// <summary>
            /// 客户端下单修复，客户端下单给错误的用户，使用ClientModify修改该成交记录
            /// </summary>
            public const sbyte ClientModify = 12;

            /// <summary>
            /// (成交转移--录单给自营)
            /// </summary>
            public const sbyte ManuInSelf = 13;

            /// <summary>
            /// (成交转移--下单给自营)
            /// </summary>
            public const sbyte OrderInSelf = 14;

        }

        /// <summary>
        /// 委托状态
        /// </summary>
        public class ORDER_STATUS
        {
            /// <summary>
            /// 已发出
            /// </summary>
            public const char ORDER_SENDED = '0';
            /// <summary>
            /// 已接收
            /// </summary>
            public const char ORDER_RECEIVED = '1';
            /// <summary>
            /// 已受理
            /// </summary>
            public const char ORDER_RECEPT = '2';
            /// <summary>
            /// 已报入
            /// </summary>			
            public const char ORDER_ACTIVED = '3';
            /// <summary>
            /// 部分成交
            /// </summary>
            public const char ORDER_PART_FILLED = '4';
            /// <summary>
            /// 完全成交
            /// </summary>
            public const char ORDER_ALL_FILLED = '5';
            /// <summary>
            /// 待改
            /// </summary>
            public const char ORDER_WAIT_UPDATE = '6';
            /// <summary>
            /// 待撤
            /// </summary>
            public const char ORDER_WAIT_DEL = '7';
            /// <summary>
            /// 已撤单
            /// </summary>
            public const char ORDER_DELED = '8';
            /// <summary>
            /// 委托失败
            /// </summary>
            public const char ORDER_FAILED = '9';

            /// <summary>
            /// 部分撤单
            /// </summary>
            public const char ORDER_PART_DELETE = 'a';

        }

        /// <summary>
        /// TT网关返回委托状态action字段取值范围
        /// </summary>
        public class GateWayAction
        {
            /// <summary>
            /// 下单
            /// </summary>
            public const sbyte ADD_ORDER = 10;
            /// <summary>
            /// 改单
            /// </summary>
            public const sbyte UPDATE_ORDER = 20;
            /// <summary>
            /// 撤单
            /// </summary>
            public const sbyte DELETE_ORDER = 30;
            /// <summary>
            /// 下单成功
            /// </summary>
            public const sbyte ORDER_SUCCESSED = 1;
            /// <summary>
            /// 下单失败
            /// </summary>
            public const SByte ORDER_FAILED = 2;
            /// <summary>
            /// 改单成功
            /// </summary>
            public const sbyte UPDATE_SUCCESSED = 3;
            /// <summary>
            /// 改单失败
            /// </summary>
            public const sbyte UPDATE_FAILED = 4;
            /// <summary>
            /// 撤单成功
            /// </summary>
            public const sbyte DELETE_SUCCESSED = 5;
            /// <summary>
            /// 撤单失败
            /// </summary>
            public const sbyte DELETE_FAILED = 6;
            /// <summary>
            /// 其他情况
            /// </summary>
            public const sbyte OTHER = 7;
        }

        public class OrderOrTradeInsert
        {

            public const string OrderInsert = "OrderInsert";
            public const string TradeInsert = "TradeInsert";
        }

        /// <summary>
        /// 开平仓方式
        /// </summary>
        public class OPEN_CLOSE
        {
            /// <summary>
            /// 开仓
            /// </summary>
            public const sbyte OPEN = 0;

            /// <summary>
            /// 平仓
            /// </summary>
            public const sbyte CLOSE = 1;

            /// <summary>
            /// 开平仓
            /// </summary>
            public const sbyte OPENCLOSE = 2;

            /// <summary>
            /// 无
            /// </summary>
            public const sbyte UNKNOW = 3;
        }

        /// <summary>
        /// 商品类型
        /// </summary>
        public class COMMODITY_TYPE
        {
            /// <summary>
            /// 期货
            /// </summary>
            public const sbyte FUTURE = 0;

            /// <summary>
            /// 价差
            /// </summary>
            public const sbyte SPEARD = 2;

            /// <summary>
            /// 期权
            /// </summary>
            public const sbyte OPTION = 1;
        }

        public class ERROR_INFO
        {
            /// <summary>
            /// 开仓资金不足
            /// </summary>
            public const int NO_FUND = 10000;

            /// <summary>
            /// 基币设置错误
            /// </summary>
            public const int ERR_BASE_FUND = 10001;

            /// <summary>
            /// 客户可用资金不足
            /// </summary>
            public const int NO_AVALIABLE_FUND = 10002;

            /// <summary>
            /// 客户只可查询
            /// </summary>
            public const int NO_TRADE = 10003;

            /// <summary>
            /// 客户只可平仓
            /// </summary>
            public const int CLOSE_ONLY = 10004;

            /// <summary>
            /// 商品禁止交易
            /// </summary>
            public const int COMMODITY_DISABLE = 10005;

            /// <summary>
            /// 查找上手失败
            /// </summary>
            public const int PROXY_ERROR = 10006;

            /// <summary>
            /// 商品禁止开仓
            /// </summary>
            public const int NO_OPEN = 10007;

            /// <summary>
            /// 无效委托
            /// </summary>
            public const int NO_ORDER = 10008;

            /// <summary>
            /// 该委托已经成交
            /// </summary>
            public const int ORDER_FILLED = 10009;

            /// <summary>
            /// 保证金错误
            /// </summary>
            public const int NO_MARGIN = 10010;

            /// <summary>
            /// 发送数据到上手失败
            /// </summary>
            public const int SEND_PROXY_ERROR = 10011;

            /// <summary>
            /// 参数错误
            /// </summary>
            public const int PARAM_ERROR = 10012;

            /// <summary>
            /// 客户不能交易该商品
            /// </summary>
            public const int USER_COMMODITY_TRADE_ERROR = 10013;

            /// <summary>
            /// 查找商品信息失败
            /// </summary>
            public const int GET_COMMODITY_ERROR = 10014;

            /// <summary>
            /// 超过商品的最大下单量
            /// </summary>
            public const int MAX_ORDER_COUNT_ERROR = 10015;

            /// <summary>
            /// 超过商品的最大持仓量
            /// </summary>
            public const int MAX_POSITION_COUNT_ERROR = 10016;

            /// <summary>
            /// 超过客户设置商品的最大持仓量
            /// </summary>
            public const int USER_MAX_POSITION_COUNT_ERROR = 10017;

            /// <summary>
            /// 本系统不能交易期权
            /// </summary>
            public const int NO_OPITION_TRADE = 10018;

            /// <summary>
            /// 客户交易信息不存在，无法进行交易
            /// </summary>
            public const int NO_ACCOUNT_TRADE_INFO = 10019;

            /// <summary>
            /// 商品不支持市价单
            /// </summary>
            public const int NO_MARKET_PRICE = 10020;

            /// <summary>
            /// 可用资金小于权利金
            /// </summary>
            public const int NO_PREMIUM_MONEY = 10021;

            /// <summary>
            /// LME到期日未设置
            /// </summary>
            public const int NO_LME_DATE = 10022;

            /// <summary>
            /// 下单合约不存在
            /// </summary>
            public const int NO_CONTRACT = 10023;

            /// <summary>
            /// 无客户资金信息
            /// </summary>
            public const int NO_ACCOUNT_FUND = 10024;

            /// <summary>
            /// 超过最大交易金额
            /// </summary>
            public const int MAX_BALANCE = 10025;

            /// <summary>
            /// 超过最大开仓比例
            /// </summary>
            public const int MAX_OPEN_RATE = 10026;

            /// <summary>
            /// 改单数据错误
            /// </summary>
            public const int MODIFY_DATA_ERROR = 10027;

            /// <summary>
            /// 合约禁止交易
            /// </summary>
            public const int CONTRACT_BANNED = 10028;

            /// <summary>
            /// 合约只可平仓
            /// </summary>
            public const int CONTRACT_CLOSE_ONLY = 10029;

            /// <summary>
            /// 风控委托无法修改
            /// </summary>
            public const int RISK_DATA = 10030;

            /// <summary>
            /// T+1 Tag Error
            /// </summary>
            public const int T_DAY_ERR = 10031;

            /// <summary>
            /// 委托数量错误
            /// </summary>
            public const int ORDER_COUNT_ERR = 10032;

            /// <summary>
            /// OCO价格错误
            /// </summary>
            public const int OCO_PRICE_ERR = 10033;
            /// <summary>
            /// 市场关闭操作不支持
            /// 只有撤单才能使用这个错误码，不可以再使用
            /// </summary>
            public const int Market_Closed = 10034;

            /// <summary>
            /// 状态冻结
            /// </summary>
            public const int ACCOUNT_LOCKED = 12001;

            public const int ERR_STATUS_MODIFY_ORDER = 12002;

        }


        /// <summary>
        /// 删除标记
        /// </summary>
        public class IS_DELETE_TAG
        {
            /// <summary>
            /// 是
            /// </summary>
            public const sbyte YES = 1;
            /// <summary>
            /// 否
            /// </summary>
            public const sbyte NO = 0;
        }

        /// <summary>
        /// 下单类型
        /// </summary>
        public class ORDER_INSERT_TYPE
        {
            /// <summary>
            /// 程序单
            /// </summary>
            public const sbyte CLIENT_IN = 0;
            /// <summary>
            /// 人工录入单
            /// </summary>
            public const sbyte MANU_IN = 1;
            /// <summary>
            /// cd单
            /// </summary>
            public const sbyte CD_IN = 2;
        }

        public class CALL_PUT
        {
            /// <summary>
            /// 看涨
            /// </summary>
            public const sbyte CALL = 0;
            /// <summary>
            /// 看跌
            /// </summary>
            public const sbyte PUT = 1;
            /// <summary>
            /// 无
            /// </summary>
            public const sbyte NONE = 3;
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public class AUDIT_FLAG
        {
            /// <summary>
            /// 未审核
            /// </summary>
            public const string AUDIT_NULL = "0";

            /// <summary>
            /// 审核通过
            /// </summary>
            public const string AUDIT_YES = "1";

            /// <summary>
            /// 审核不通过
            /// </summary>
            public const string AUDIT_NO = "2";
        }
        /// <summary>
        /// 客户账号状态
        /// </summary>
        public class AccountStatus
        {

            // 0正常  1 只可平仓  2只可查询  3禁止登录 4冻结

            public const int Noramrl = 0;
            public const int OnlyClose = 1;
            public const int OnlyQuery = 2;
            public const int NoLogin = 3;
            public const int Frozen = 4;

        }
    }
    public enum OderAction
    {
        NewOrder = 0,
        CancelOrder = 1,
        ChangeOrder = 2
    }
}
