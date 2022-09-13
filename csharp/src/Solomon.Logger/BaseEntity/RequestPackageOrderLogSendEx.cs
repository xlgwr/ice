using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.DataType;

namespace Trade.DataType
{
    /// <summary>
    /// RequestPackageOrderLog 中间对象
    /// </summary>
    public class RequestPackageOrderLogEx
    {

        public RequestPackageOrderLogEx() { }


        public RequestPackageOrderLogEx(int serviceNo, object dataBody, DBCommand dBCommand = null)
        {
            ServiceNo = serviceNo;
            DBCommand = dBCommand;
            switch (serviceNo)
            {
                case (int)GatewayServerAvailable.TE_ORDER_ADD:
                case (int)TradeServerAvailable.TE_ORDER_ADD:
                case (int)TradeServerCallBack.SE_ORDER_INSERT:
                case (int)TradeServerAvailable.DB_ADD_ORDER://新建委托
                case (int)TradeServerAvailable.DB_UPD_ORDER://更新委托状态
                    SlmOrderBody = dataBody as SlmOrder;
                    break;
                case (int)TradeServerCallBack.SE_ORDERJOUR:
                case (int)TradeServerAvailable.DB_ADD_ORDER_JOUR://插入委托流水
                    SlmOrderJourBody = dataBody as SlmOrderJour;
                    break;
                case (int)TradeServerCallBack.SE_ORDER_STATUS:
                    SendOrderStatusBody = dataBody as SendOrderStatus;
                    break;
                case (int)TradeServerCallBack.SE_MATCH_INFO:
                case (int)TradeServerAvailable.DB_ADD_MATCH://插入成交
                    SlmMatchBody = dataBody as SlmMatch;
                    break;
                case (int)GatewayServerCallBack.SE_ORDER_STATUS:
                    PushOrderStateBody = dataBody as PushOrderState;
                    break;
                case (int)GatewayServerCallBack.SE_MATCH_INFO:
                    PushMatchStateBody = dataBody as PushMatchState;
                    break;
                case (int)TradeServerAvailable.DB_ADD_POSITION:
                case (int)TradeServerAvailable.DB_DEL_POSITION:
                    SlmPositionBody = dataBody as SlmPosition;
                    break;
                case (int)TradeServerAvailable.DB_ADD_CLOSE:
                    SlmCloseBody = dataBody as SlmClose;
                    break;
                default:
                    LogMsg = dataBody.toJsonStr();
                    break;
            }
        }

        public string ClientId { get; set; }
        public int ServiceNo { get; set; }
        public string UUID { get; set; }
        public string MsgId { get; set; }
        public string LogFileName { get; set; }
        public byte LogLevel { get; set; }
        public string LogTitle { get; set; }
        public string LogFormat { get; set; }
        public string LogMsg { get; set; }

        public string[] DataBody { get; set; }

        public SlmOrder SlmOrderBody { get; set; }


        public PushMatchState PushMatchStateBody { get; set; }


        public PushOrderState PushOrderStateBody { get; set; }


        public SendOrderStatus SendOrderStatusBody { get; set; }


        public SlmOrderJour SlmOrderJourBody { get; set; }


        public SlmMatch SlmMatchBody { get; set; }

        public SlmPosition SlmPositionBody { get; set; }

        public SlmClose SlmCloseBody { get; set; }
        public DBCommand DBCommand { get; set; }


    }
}
