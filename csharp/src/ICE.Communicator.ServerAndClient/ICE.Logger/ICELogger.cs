using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICE.Communicator.ServerAndClient.ICE.Logger
{
    public class ICELogger : Ice.Logger
    {
        private string Prefix { get; set; } = string.Empty;

        public ICELogger(string prefix)
        {
            Prefix = prefix;
        }

        public Ice.Logger cloneWithPrefix(string prefix)
        {
            Prefix = prefix;
            return this;
        }

        public void error(string message)
        {
            LogHelper.WriteLog($"{Prefix}Error", message);
        }

        public string getPrefix()
        {
            return Prefix;
        }

        public void print(string message)
        {
            LogHelper.WriteLog($"{Prefix}Print", message);
        }

        public void trace(string category, string message)
        {
            LogHelper.WriteLog($"{Prefix}Trace", $"{category}:{message}");
        }

        public void warning(string message)
        {
            LogHelper.WriteLog($"{Prefix}Warning", message);
        }
    }
}
