using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Log
{
    class LogFormat : BaseLogClass
    {
        private List<ServerLogClass> servers = new List<ServerLogClass>();

        public LogFormat()
        {

            ServerLogClass slc = new ServerLogClass();
            slc.ServerNo = 100001;
            slc.ServerName = "TE_DEPT_ADD";
            slc.Keys = new string[] { "dept_id",
                               "dept_name",
                               "dept_en_name" ,
                               "remark",
                               "dept_parent_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100002;
            slc.ServerName = "TE_DEPT_ADD";
            slc.Keys = new string[] { "dept_id",
                               "dept_name",
                               "dept_en_name" ,
                               "remark"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100004;
            slc.ServerName = "TE_OPER_ADD";
            slc.Keys = new string[] { "oper_id",
                               "dept_id",
                               "oper_pwd" ,
                               "oper_name",
                               "oper_tel",
                               "oper_status",
                               "oper_create_time",
                               "remark",
                               "login_num_limit",
                               "roles"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100005;
            slc.ServerName = "TE_OPER_UPD";
            slc.Keys = new string[] { "oper_id",
                               "dept_id",
                               "oper_pwd" ,
                               "oper_name",
                               "oper_tel",
                               "oper_status",
                               "oper_create_time",
                               "remark",
                               "login_num_limit",
                               "roles"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100007;
            slc.ServerName = "TE_OPER_LOGIN";
            slc.Keys = new string[] { "oper_id",
                               "oper_pwd"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100009;
            slc.ServerName = "TE_OPERSTATUS_UPD";
            slc.Keys = new string[] { "oper_id",
                               "oper_status"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100010;
            slc.ServerName = "TE_OPERROLE_UPD";
            slc.Keys = new string[] { "oper_id",
                               "oper_role"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100011;
            slc.ServerName = "TE_ROLE_ADD";
            slc.Keys = new string[] { "role_id",
                               "role_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100012;
            slc.ServerName = "TE_ROLE_UPD";
            slc.Keys = new string[] { "role_id",
                               "role_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100017;
            slc.ServerName = "TE_OPER_PWD_UPD";
            slc.Keys = new string[] { "oper_id",
                               "oper_old_pwd",
                               "oper_pwd"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100018;
            slc.ServerName = "TE_ROLE_RIGHT_UPD";
            slc.Keys = new string[] { "role_id",
                               "rights"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100030;
            slc.ServerName = "TE_ROLE_RIGHT_HIDE_ADD";
            slc.Keys = new string[] { "role_id",
                               "right_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100031;
            slc.ServerName = "TE_ROLE_RIGHT_HIDE_DEL";
            slc.Keys = new string[] { "role_id",
                               "right_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100036;
            slc.ServerName = "TE_CURRENCY_ADD";
            slc.Keys = new string[] { "currency_group",
                               "currency_code",
                               "currency_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100037;
            slc.ServerName = "TE_CURRENCY_UPD";
            slc.Keys = new string[] { "currency_group",
                               "currency_code",
                               "currency_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100041;
            slc.ServerName = "TE_ER_ADD";
            slc.Keys = new string[] { "currency_code",
                               "to_currency_code",
                               "exchange_rate",
                               "a_exchange_rate",
                               "dps_exchangerate",
                               "a_dps_exchangerate"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100042;
            slc.ServerName = "TE_ER_UPD";
            slc.Keys = new string[] { "currency_code",
                               "to_currency_code",
                               "exchange_rate",
                               "a_exchange_rate",
                               "dps_exchangerate",
                               "a_dps_exchangerate"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100044;
            slc.ServerName = "TE_EXCHANGE_ADD";
            slc.Keys = new string[] { "exchange_code",
                               "exchange_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100045;
            slc.ServerName = "TE_EXCHANGE_UPD";
            slc.Keys = new string[] { "exchange_code",
                               "exchange_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100048;
            slc.ServerName = "TE_COMMODITY_ADD";
            slc.Keys = new string[] { "exchange_code",
                               "product_type",
                               "commodity_code",
                               "commodity_en_name",
                               "commodity_cn_name",
                               "proxy_code",
                               "currency_code",
                               "commodity_size",
                               "base_commodity_code",
                               "status",
                               "tick_size",
                               "numerator",
                               "denominator",
                               "delivery_mode",
                               "is_tplusone",
                               "div_time",
                               "max_pos_num",
                               "max_order_num",
                               "value_date_mode",
                               "margin_mode",
                               "spread_seq"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100049;
            slc.ServerName = "TE_COMMODITY_UPD";
            slc.Keys = new string[] { "exchange_code",
                               "product_type",
                               "commodity_code",
                               "commodity_en_name",
                               "commodity_cn_name",
                               "proxy_code",
                               "currency_code",
                               "commodity_size",
                               "base_commodity_code",
                               "status",
                               "tick_size",
                               "numerator",
                               "denominator",
                               "delivery_mode",
                               "is_tplusone",
                               "div_time",
                               "max_pos_num",
                               "max_order_num",
                               "value_date_mode",
                               "margin_mode",
                               "spread_seq"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100052;
            slc.ServerName = "TE_MARGIN_ADD";
            slc.Keys = new string[] { "margin_class",
                               "margin_description",
                               "exchange_code",
                               "product_type",
                               "commodity_code",
                               "contract_code",
                               "currency_code",
                               "calculate_type",
                               "init_margin",
                               "main_margin",
                               "lockup_ratio",
                               "operation_time"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100053;
            slc.ServerName = "TE_MARGIN_UPD";
            slc.Keys = new string[] { "margin_class",
                               "margin_description",
                               "exchange_code",
                               "product_type",
                               "commodity_code",
                               "contract_code",
                               "currency_code",
                               "calculate_type",
                               "init_margin",
                               "main_margin",
                               "lockup_ratio",
                               "operation_time"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100055;
            slc.ServerName = "TE_FEE_ADD";
            slc.Keys = new string[] { "fee_class",
                               "exchange_code",
                               "product_type",
                               "commodity_code",
                               "fee",
                               "currency_code",
                               "fee_currency_code",
                               "operation_time"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100056;
            slc.ServerName = "TE_FEE_UPD";
            slc.Keys = new string[] { "fee_class",
                               "exchange_code",
                               "product_type",
                               "commodity_code",
                               "fee",
                               "currency_code",
                               "fee_currency_code",
                               "operation_time"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100058;
            slc.ServerName = "TE_PROXY_ADD";
            slc.Keys = new string[] { "proxy_id",
                               "en_name",
                               "cn_name",
                               "status",
                               "remark"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100059;
            slc.ServerName = "TE_PROXY_UPD";
            slc.Keys = new string[] { "proxy_id",
                               "en_name",
                               "cn_name",
                               "status",
                               "remark"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100062;
            slc.ServerName = "TE_CP_ADD";
            slc.Keys = new string[] { "exchange_code",
                               "product_type",
                               "commodity_code",
                               "contract_code",
                               "settle_price",
                               "en_name",
                               "cn_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100063;
            slc.ServerName = "TE_CP_UPD";
            slc.Keys = new string[] { "exchange_code",
                               "product_type",
                               "commodity_code",
                               "contract_code",
                               "settle_price",
                               "en_name",
                               "cn_name"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100066;
            slc.ServerName = "TE_FC_ADD";
            slc.Keys = new string[] { "exchange_code",
                               "commodity_code",
                               "contract_code",
                               "status",
                               "expiry_date",
                               "last_trade_date",
                               "first_notice_day",
                               "en_name",
                               "cn_name",
                               "value_date_mode"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100067;
            slc.ServerName = "TE_FC_UPD";
            slc.Keys = new string[] { "exchange_code",
                               "commodity_code",
                               "contract_code",
                               "status",
                               "expiry_date",
                               "last_trade_date",
                               "first_notice_day",
                               "en_name",
                               "cn_name",
                               "value_date_mode"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100069;
            slc.ServerName = "TE_OC_ADD";
            slc.Keys = new string[] { "exchange_code",
                               "commodity_code",
                               "contract_code",
                               "currency_code",
                               "base_commodity_code",
                               "call_put",
                               "strike_price",
                               "en_name",
                               "cn_name",
                               "status",
                               "tick_size",
                               "delivery_mode",
                               "expiry_date",
                               "last_trade_date",
                               "first_notice_day",
                               "value_date_mode"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100070;
            slc.ServerName = "TE_OC_UPD";
            slc.Keys = new string[] { "exchange_code",
                               "commodity_code",
                               "contract_code",
                               "currency_code",
                               "base_commodity_code",
                               "call_put",
                               "strike_price",
                               "en_name",
                               "cn_name",
                               "status",
                               "tick_size",
                               "delivery_mode",
                               "expiry_date",
                               "last_trade_date",
                               "first_notice_day",
                               "value_date_mode"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100075;
            slc.ServerName = "TE_ACCOUNT_ADD";
            slc.Keys = new string[] { "account_no",
                               "margin_class",
                               "fee_class",
                               "trading_status",
                               "data_status",
                               "trading_num_limit",
                               "data_num_limit",
                               "trading_pwd",
                               "data_pwd",
                               "commodity_limit_flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 910025;
            slc.ServerName = "TE_CLIENT_ADD";
            slc.Keys = new string[] { "account_no",
                               "margin_class",
                               "fee_class",
                               "trading_status",
                               "data_status",
                               "trading_num_limit",
                               "data_num_limit",
                               "trading_pwd",
                               "data_pwd",
                               "account_email",
                               "account_tel",
                               "account_en_name",
                               "account_cn_name",
                               "account_id_no",
                               "account_id_type",
                               "open_date",
                               "account_type",
                               "account_type",
                               "commodity_limit_flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 910029;
            slc.ServerName = "TE_CLIENT_INFO_CHANGE";
            slc.Keys = new string[] { "account_no",
                               "margin_class",
                               "fee_class",
                               "trading_status",
                               "data_status",
                               "trading_num_limit",
                               "data_num_limit",
                               "trading_pwd",
                               "data_pwd",
                               "account_email",
                               "account_tel",
                               "account_en_name",
                               "account_cn_name",
                               "account_id_no",
                               "account_id_type",
                               "open_date",
                               "account_type",
                               "account_type",
                               "commodity_limit_flag"
            };
            servers.Add(slc);


            slc = new ServerLogClass();
            slc.ServerNo = 100076;
            slc.ServerName = "TE_ACCOUNT_UPD";
            slc.Keys = new string[] { "account_no",
                               "margin_class",
                               "fee_class",
                               "trading_status",
                               "data_status",
                               "trading_num_limit",
                               "data_num_limit",
                               "trading_pwd",
                               "data_pwd",
                               "commodity_limit_flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100078;
            slc.ServerName = "TE_AI_ADD";
            slc.Keys = new string[] { "account_no",
                               "account_email",
                               "account_tel",
                               "account_en_name",
                               "account_cn_name",
                               "account_id_no",
                               "account_id_type",
                               "open_date",
                               "account_type",
                               "account_birthday"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100079;
            slc.ServerName = "TE_AI_UPD";
            slc.Keys = new string[] { "account_no",
                               "account_email",
                               "account_tel",
                               "account_en_name",
                               "account_cn_name",
                               "account_id_no",
                               "account_id_type",
                               "open_date",
                               "account_type",
                               "account_birthday"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100087;
            slc.ServerName = "TE_DEPOSIT_ADD";
            slc.Keys = new string[] { "init_date",
                               "business_flag",
                               "business_no",
                               "occur_time",
                               "operator_no",
                               "account_no",
                               "currency_code",
                               "occur_balance",
                               "remark",
                               "audit_flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100088;
            slc.ServerName = "TE_DEPOSIT_UPD";
            slc.Keys = new string[] { "init_date",
                               "business_flag",
                               "business_no",
                               "occur_time",
                               "operator_no",
                               "account_no",
                               "currency_code",
                               "occur_balance",
                               "remark",
                               "audit_flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100165;
            slc.ServerName = "TE_LME_DATE_SET";
            slc.Keys = new string[] { "init_date",
                               "value_date",
                               "oper_id",
                               "trade_time"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100166;
            slc.ServerName = "TE_LME_DATE_SET_HIS";
            slc.Keys = new string[] { "init_date",
                               "value_date",
                               "oper_id",
                               "trade_time"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100105;
            slc.ServerName = "TE_RATE_INFO_ADD";
            slc.Keys = new string[] { "account_no",
                               "own_balance",
                               "day_credit_limt",
                               "credit_limt",
                               "own_line",
                               "own_init_rate",
                               "own_main_rate",
                               "close_condition",
                               "is_open",
                               "is_open_warning",
                               "warning_line",
                               "tracing_line",
                               "is_open_max_balance",
                               "max_balance",
                               "is_open_max_open_rate",
                               "max_open_rate",
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100108;
            slc.ServerName = "TE_RATE_INFO_UPD";
            slc.Keys = new string[] { "account_no",
                               "own_balance",
                               "day_credit_limt",
                               "credit_limt",
                               "own_line",
                               "own_init_rate",
                               "own_main_rate",
                               "close_condition",
                               "is_open",
                               "is_open_warning",
                               "warning_line",
                               "tracing_line",
                               "is_open_max_balance",
                               "max_balance",
                               "is_open_max_open_rate",
                               "max_open_rate",
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100141;
            slc.ServerName = "TE_COMMODITY_PROXY_ADD";
            slc.Keys = new string[] { "account_no",
                               "exchange_code",
                               "commodity_code",
                               "product_type",
                               "proxy_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100144;
            slc.ServerName = "TE_COMMODITY_PROXY_UPD";
            slc.Keys = new string[] { "account_no",
                               "exchange_code",
                               "commodity_code",
                               "product_type",
                               "proxy_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100142;
            slc.ServerName = "TE_COMMODITY_STATUS_ADD";
            slc.Keys = new string[] { "commodity_status_info"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100145;
            slc.ServerName = "TE_COMMODITY_STATUS_UPD";
            slc.Keys = new string[] { "account_no",
                               "exchange_code",
                               "commodity_code",
                               "product_type",
                               "commodity_status"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100143;
            slc.ServerName = "TE_COMMODITY_COUNT_ADD";
            slc.Keys = new string[] { "account_no",
                               "exchange_code",
                               "commodity_code",
                               "product_type",
                               "max_count"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100146;
            slc.ServerName = "TE_COMMODITY_COUNT_UPD";
            slc.Keys = new string[] { "account_no",
                               "exchange_code",
                               "commodity_code",
                               "product_type",
                               "max_count"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100180;
            slc.ServerName = "TE_ACCOUNT_RIGHTS_RELATION_ADD";
            slc.Keys = new string[] { "account_no",
                               "right_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100181;
            slc.ServerName = "TE_ACCOUNT_RIGHTS_RELATION_DEL";
            slc.Keys = new string[] { "account_no",
                               "right_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100183;
            slc.ServerName = "TE_ACCOUNT_QUOT_INFO_ADD";
            slc.Keys = new string[] { "account_no",
                               "exchange_code"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100184;
            slc.ServerName = "TE_ACCOUNT_QUOT_INFO_DEL";
            slc.Keys = new string[] { "account_no",
                               "exchange_code"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100186;
            slc.ServerName = "TE_ACCOUNT_QUOT_CHANGE_LOG_ADD";
            slc.Keys = new string[] { "init_date",
                               "business_no",
                               "operator_no",
                               "account_no",
                               "exchange_code",
                               "flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 910043;
            slc.ServerName = "TE_ORDER_IN";
            slc.Keys = new string[] { "account_no",
                               "exchange_code",
                               "product_type",
                               "commodity_code",
                               "contract_code",
                               "oreder_num",
                               "order_price",
                               "order_type",
                               "buy_sell",
                               "eff_type",
                               "proxy_id",
                               "fee_class",
                               "currency",
                               "oper_id"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100182;
            slc.ServerName = "TE_CD_ORDER_VERIFY";
            slc.Keys = new string[] { "orderNO",
                               "operatorNo",
                               "status",
                               "remark"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100189;
            slc.ServerName = "TE_SET_PRE_BALANCE";
            slc.Keys = new string[] { "pre_fund_list"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100190;
            slc.ServerName = "TE_SET_TN_DATE";
            slc.Keys = new string[] { "order_no",
                               "date"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100098;
            slc.ServerName = "TE_NEWS_ADD";
            slc.Keys = new string[] { "news_no",
                               "account_no",
                               "news_type",
                               "news_title",
                               "news_content",
                               "news_time",
                               "accounts"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 910029;
            slc.ServerName = "TE_CLIENT_INFO_CHANGE";
            slc.Keys = new string[] {"account_no",
                               "margin_class",
                               "fee_class",
                               "trading_status",
                               "data_status",
                               "trading_num_limit",
                               "data_num_limit",
                               "trading_pwd",
                               "data_pwd",
                               "account_email",
                               "account_tel",
                               "account_en_name",
                               "account_cn_name",
                               "account_id_no",
                               "account_id_type",
                               "open_date",
                               "account_type",
                               "account_birthday",
                               "commodity_limit_flag"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 100120;
            slc.ServerName = "TE_RISK_CLOSE";
            slc.Keys = new string[] { "oper_no",
                               "account_no",
                               "trading_status"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 912066;
            slc.ServerName = "TE_MOVE_TRADE";
            slc.Keys = new string[] { "orderNos",
                               "toAccount",
                               "smsMsg",
                               "remark"
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 610022;
            slc.ServerName = "TE_API_CLIENT_INFO_CHANGE";
            slc.Keys = new string[] { "oper_no",
                               "account_tel",
                               "account_email"
            };
            servers.Add(slc);

        }

        override
        public List<ServerLogClass> GetAllServerClass()
        {
            return servers;
        }
    }
}
