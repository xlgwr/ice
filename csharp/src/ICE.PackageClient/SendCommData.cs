using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageClient
{
    public class SendCommData
    {
        public string ClientId { get; set; }
        public int ErrorNo { get; set; }
        public string ErrorInfo { get; set; }
        public int ServiceNo { get; set; }
        public string[] Head { get; set; }
        public string[][] DataBody { get; set; }
        public int Type { get; set; }
        public ComDto DtoData { get; set; } = new ComDto();
        public object PushData { get; set; }
    }
}
