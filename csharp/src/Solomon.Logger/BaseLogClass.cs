using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Log
{
     class ServerLogClass
    {

        public int ServerNo { get; set; }

        public string ServerName { get; set; }
        /// <summary>
        /// "acount={0},orderNo={1}"
        /// </summary>
        public string[] Keys { get; set; }
    }
    class BaseLogClass
    {

        public virtual List<ServerLogClass> GetAllServerClass()
        {
            return null;
        }
    }
    /// <summary>
    /// 用于测试用
    /// </summary>
    public class PackageRequestDTO
    {
        public int ServiceNo { get; set; }
        public string ServiceName { get; set; }
        public List<string> Head { get; set; }
        public List<string> DataBody { get; set; }

    }
}
