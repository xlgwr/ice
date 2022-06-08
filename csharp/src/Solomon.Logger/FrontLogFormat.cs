using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Log
{
    class FrontLogFormat:BaseLogClass
    {

        private List<ServerLogClass> servers = new List<ServerLogClass>();



        public FrontLogFormat ()
        {

            ServerLogClass slc = new ServerLogClass();
            slc.ServerNo = 910001;
            slc.ServerName = "TE_ORDER_ADD";
            slc.Keys = new string[] {
                "account_no",
                "operator_no",
                "order_person" ,
                "product_type" ,
                "exchange_code" ,
                "commodity_code",
                "contract_code" ,
                "call_put" ,
                "strike_price" ,
                "open_close" ,
                "buy_sell" ,
                "order_price",
                "order_num" ,
                "order_type",
                "order_life" ,
                "order_insert_type",
                "del_tag",
                "order_from" ,
                "order_sub_num_min" ,
                "order_sub_num_max" ,
                "order_expiry_date" ,
                "trigger_price" ,
                "order_remain" ,
                "remark_cus" ,
                "info_s1" ,
                "info_s2",
                "local_no",
                "proxy_id",
                "trade_fee",
                "currency_code",
                "front_id"
            };
            servers.Add(slc);


            slc = new ServerLogClass();
            slc.ServerNo = 910002;
            slc.ServerName = "TE_ORDER_UPD";
            slc.Keys = new string[] {
                "order_no",
                "order_price",
                "trigger_price",
                "order_num",
                "machine_code",
            };
            servers.Add(slc);


            slc = new ServerLogClass();
            slc.ServerNo = 910004;
            slc.ServerName = "TE_ORDER_DEL";
            slc.Keys = new string[] {
                "order_no",
                "machine_code",
            };
            servers.Add(slc);



            slc = new ServerLogClass();
            slc.ServerNo = 920001;
            slc.ServerName = "SE_ORDER_STATUS";  
            slc.Keys = new string[] {
                "account_no",
                "operator_no",
                "order_no",
                "order_status",
                "order_jour_no",
                "update_date",
                "update_time",
                "error_no",
                "error_info",
                "order_price",
                "order_num",
                "trigger_price",
                "exchange_date",
                "exchange_time",
                "exchange_order_no",
                "match_vol",
                "broker_order_no",
                "broker_order_jour_no",
                "info_s1",
                "info_s2",
                "init_date",
                "order_Type",
                "order_from",
                "remarks",
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 920002;
            slc.ServerName = "SE_MATCH_INFO";
            slc.Keys = new string[] {
                "init_date",
                "account_no",
                "order_date",
                "order_time",
                "order_no",
                "trade_date",
                "trade_no",
                "trade_time",
                "trade_price",
                "trade_vol",
                "currency_code",
                "is_tplusone",
                "open_close",
                "proxy_id",
                "broker_trade_no",
                "exchange_trade_no",
                "product_type",
                "exchange_code",
                "commodity_code",
                "contract_code",
                "buy_sell",
                "trade_fee",
                "operator_no"

            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 920019;
            slc.ServerName = "SE_PUSH_DILEVERY_MATCH";
            slc.Keys = new string[] {
                "init_date",
                "account_no",
                "order_date",
                "order_time",
                "order_no",
                "trade_date",
                "trade_no",
                "trade_time",
                "trade_price",
                "trade_vol",
                "currency_code",
                "is_tplusone",
                "open_close",
                "proxy_id",
                "broker_trade_no",
                "exchange_trade_no",
                "product_type",
                "exchange_code",
                "commodity_code",
                "contract_code",
                "buy_sell",
                "trade_fee",
                "operator_no"


            };
            servers.Add(slc);


            slc = new ServerLogClass();
            slc.ServerNo = 920008;
            slc.ServerName = "SE_ORDER_INSERT";
            slc.Keys = new string[] {
                "account_no",
                "operator_no",
                "order_person",
                "product_type",
                "exchange_code",
                "commodity_code",
                "contract_code",
                "call_put",
                "strike_price",
                "open_close",
                "buy_sell",
                "order_price",
                "order_num",
                "order_type",
                "order_life",
                "order_insert_type",
                "del_tag",
                "order_from",
                "order_sub_num_min",
                "order_sub_num_max",
                "order_expiry_date",
                "trigger_price",
                "order_remain",
                "remark_cus",
                "info_s1",
                "info_s2",
                "local_no",
                "order_no",
                "order_date",
                "order_time",
                "strategy_reference",
                "strategy_type",
                "strategy_content",
                "proxy_id",
                "front_id",
                "init_date",
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 910050;
            slc.ServerName = "TE_CD_VERIFY";
            slc.Keys = new string[] {
                "order_id",
                "operator_no",
                "verify_result",
                "remark_info",
            };
            servers.Add(slc);


            slc = new ServerLogClass();
            slc.ServerNo = 910052;
            slc.ServerName = "TE_ORDER_STATUS_VERIFY";
            slc.Keys = new string[] {
                "order_id",
                "operator_no",
                "new_state",
                "remark_info",
            };
            servers.Add(slc);

            slc = new ServerLogClass();
            slc.ServerNo = 610022;
            slc.ServerName = "SE_ACCOUNT_INFO_SYNC";
            slc.Keys = new string[] {
                "accountNo",
                "fixed_phone",
                "cell_phone",
                "email",
                "address",
                "bank_op_type",
                "bank1_bank_code",
                "bank1_bank_acc",
                "bank1_currency_code",
                "bank1_bankname",
                "bank2_bank_code",
                "bank2_bank_acc",
                "bank2_currency_code",
                "bank2_bankname",
                "residence_country",
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
