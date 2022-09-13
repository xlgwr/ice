using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.DataType
{
    public class RequestPackageActiveSendEx
    {
        public int ServiceNo { get; set; }

        public string[] Head { get; set; }

        public string ClientId { get; set; }

        public string UUID { get; set; }

        public string[] DataBody { get; set; }
    }
}
