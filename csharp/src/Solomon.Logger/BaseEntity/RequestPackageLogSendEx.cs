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
    public class RequestPackageLogEx
    {

        public string ClientId { get; set; }
        public int ServiceNo { get; set; }
        public string UUID { get; set; }
        public string MsgId { get; set; }
        public string LogFileName { get; set; }
        public byte LogLevel { get; set; }
        public string LogTitle { get; set; }
        public string LogFormat { get; set; }
        public string LogMsg { get; set; }       
    }
}
