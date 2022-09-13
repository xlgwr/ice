using Solomon.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trade.DataType
{
    public class PackageCacheDBEx
    {
        public int ServiceNo;

        public string ClientId;

        public string UUID;

        public string MsgId;

        public int ErrorNo;

        public string ErrorInfo;

        public PagingInfo PagingInfo;

        public Dictionary<string, string> DatasDic;

        public string[] Head;

        public string[][] DataBody;

    }
}
