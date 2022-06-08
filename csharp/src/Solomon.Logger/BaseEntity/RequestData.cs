using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Logger
{
    public class RequestData
    {
      
            public string ClientId { get; set; }
            public int ErrorNo { get; set; }
            public string ErrorInfo { get; set; }
            public int ServiceNo { get; set; }
            //public string[][] DataBody { get; set; }
            public string[][] DataBody { get; set; }
            public string[] DataHead { get; set; }
            public int ClientType { get; set; }

            public bool IsSync { get; set; } = false;

            public string MsgID { get; set; }

            public string UUID { get; set; }

        
    }
}
