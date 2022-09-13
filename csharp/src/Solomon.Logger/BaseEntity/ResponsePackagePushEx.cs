using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.DataType;

namespace Trade.DataType

{
    public class ResponsePackagePushEx
    {

        public int ErrorNo { get; set; }

        public string ErrorInfo { get; set; }

        public int ServiceNo { get; set; }

        public string MsgId { get; set; }

        public string[] Head { get; set; }

        public string ClientId { get; set; }


        public string UUID { get; set; }


        public string[][] DataBody { get; set; }


        public SlmOrder SlmOrderBody { get; set; }


        public PushMatchState PushMatchStateBody { get; set; }


        public PushOrderState PushOrderStateBody { get; set; }


        public SendOrderStatus SendOrderStatusBody { get; set; }


        public SlmOrderJour SlmOrderJourBody { get; set; }


        public SlmMatch[] SlmMatchBody { get; set; }
    }
}
