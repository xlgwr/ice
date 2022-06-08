using Solomon.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.PackageServer.Model;

namespace Test.PackageServer
{
    public class CommunicationClientInfo : ClientInfo
    {

        public ClientType ClientType { get; set; }
        public ServerIdentify ServerIdentify { get; set; }
        public bool IsLogin { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public string ConfigKeyName { get; set; }
        /// <summary>
        /// 是否宕机
        /// </summary>
        public IsCrash IsCrash { get; set; }
        /// <summary>
        /// $"{ClientId}:{IPInfo}:{Port}";
        /// </summary>
        public string Key
        {
            get
            {
                return $"{ClientId}:{IPInfo}:{Port}";
            }
        }
        /// <summary>
        /// $"{ClientId}:{ClientType}";
        /// </summary>
        public string KeyIDAndType
        {
            get
            {
                return $"{ClientId}:{ClientType}";
            }
        }
    }
}
