using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageServer
{
    /// <summary>
    /// 入参数类
    /// </summary>
    public class ResponsePackageDTO : ComDto
    {
        public string ServiceName { get; set; }
        public string ClientId { get; set; }
        public string UUId { get; set; }
        public List<string> Head { get; set; }
        public Dictionary<int, List<string>> DataBody { get; set; }
        public int ErrorNo { get; set; }
        public string ErrorInfo { get; set; }

        public ResponsePackageDTO Copy()
        {
            var result = new ResponsePackageDTO()
            {
                ClientId = this.ClientId,
                UUId = this.UUId,
                DataBody = this.DataBody,
                Head = this.Head,
                ServiceName = this.ServiceName,
                ServiceNo = this.ServiceNo,
                ErrorInfo = this.ErrorInfo,
                ErrorNo = this.ErrorNo
            };
            return result;
        }

    }
}
