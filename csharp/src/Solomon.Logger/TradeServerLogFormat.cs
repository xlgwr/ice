using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Log
{
    class TradeServerLogFormat : BaseLogClass
    {

        private List<ServerLogClass> servers = new List<ServerLogClass>();



        public TradeServerLogFormat()
        {

            #region 940001 TE_ORDER_ADD
            ServerLogClass slc = new ServerLogClass();
            slc.ServerNo = 940001;
            slc.ServerName = "TE_ORDER_ADD";
            slc.Keys = new string[]
            {
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
                "router_no",
                "init_date",
            };
            servers.Add(slc);
            #endregion

            #region 940002 TE_ORDER_UPD
            slc = new ServerLogClass();
            slc.ServerNo = 940002;
            slc.ServerName = "TE_ORDER_ADD";
            slc.Keys = new string[]
            {
                "order_no",
                "order_price",
                "trigger_price",
                "order_num",
            };
            servers.Add(slc);
            #endregion

            #region 940004 TE_ORDER_DEL
            slc = new ServerLogClass();
            slc.ServerNo = 940004;
            slc.ServerName = "TE_ORDER_DEL";
            slc.Keys = new string[]
            {
                "order_no",
            };
            servers.Add(slc);
            #endregion

            #region 930001 SE_ORDER_STATUS
            slc = new ServerLogClass();
            slc.ServerNo = 930001;
            slc.ServerName = "SE_ORDER_STATUS";
            slc.Keys = new string[]
            {
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
                "exchange_order_no",
                "exchange_date_time",
                "match_vol",
                "broker_order_no",
                "broker_order_jour_no",
                "order_type",//2020.11.09 策略单，新增
                "info1",
                "info2",
                "initDate",
                "order_from",
                "remarks",
            };
            servers.Add(slc);
            #endregion

            #region 930002 SE_MATCH_INFO
            slc = new ServerLogClass();
            slc.ServerNo = 930002;
            slc.ServerName = "SE_MATCH_INFO";
            slc.Keys = new string[]
            {
                "account_no",
                "order_no",
                "trade_date",
                "trade_no",
                "trade_time",
                "trade_price",
                "trade_vol",
                "proxy_id",
                "broker_trade_no",
                "exchange_trade_no",
                "operator_no",
                "info1",
                "info2",
                "info3",
                "order_insert_type",
                "order_from",
                "commodity_code",
                "contract_code",
                "buy_sell",
            };
            servers.Add(slc);
            #endregion

            #region 920008 SE_ORDER_INSERT
            slc = new ServerLogClass();
            slc.ServerNo = 920008;
            slc.ServerName = "SE_ORDER_INSERT";
            slc.Keys = new string[]
            {
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
            #endregion

            #region 920005 SE_ORDERJOUR
            slc = new ServerLogClass();
            slc.ServerNo = 920005;
            slc.ServerName = "SE_ORDERJOUR";
            slc.Keys = new string[]
            {
                "init_date",
                "front_id",
                "operator_no",
                "order_person",
                "update_person",
                "proxy_id",
                "account_no",
                "product_type",
                "exchange_code",
                "commodity_code",
                "contract_code",
                "call_put",
                "strike_price",
                "buy_sell",
                "order_price",
                "order_num",
                "order_type",
                "order_life",
                "order_from",
                "trigger_price",
                "order_status",
                "filled_num",
                "order_date",
                "order_time",
                "update_date",
                "update_time",
                "exchange_date",
                "exchange_time",
                "order_no",
                "order_jour_no",
                "broker_order_no",
                "broker_order_jour_no",
                "exchange_order_no",
                "error_no",
                "error_info",
                "remark_cus",
                "info_s1",
                "info_s2",
            };
            servers.Add(slc);
            #endregion

        }

        override
        public List<ServerLogClass> GetAllServerClass()
        {
            return servers;
        }
    }

}
