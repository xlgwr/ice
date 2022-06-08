using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Logger
{

    public enum PushMode
    {

        All = 0,
        ByClientType = 1,
        ByUuid = 2

    }


    public class PushData
    {
        public string ClientId { get; set; }
        public int ErrorNo { get; set; }
        public string ErrorInfo { get; set; }
        public int ServiceNo { get; set; }
        //public string[][] DataBody { get; set; }
        public string[][] DataBody { get; set; }
        public string[] DataHead { get; set; }
        public ClientType ClientType { get; set; }

        public bool IsSync { get; set; } = false;

        public string MsgID { get; set; }

        public string UUID { get; set; }

        public PushMode PushMode { get; set; } = PushMode.ByClientType;
    }
}
