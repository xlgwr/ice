using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageServer.Model
{
    public class ClientInfo
    {
        public string ClientId
        {
            get;
            set;
        }

        public string IPInfo
        {
            get;
            set;
        }

        public string LoginDateTime
        {
            get;
            set;
        }

        public string Port
        {
            get;
            set;
        }

        public string ConnectionId
        {
            get;
            set;
        }
    }

    public class SystemMessage
    {
        public string DateTime
        {
            get;
            set;
        }

        public string MessageInfo
        {
            get;
            set;
        }

        public short Type
        {
            get;
            set;
        }
    }
}
