using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.PackageServer.Model;

namespace Test.PackageServer.EventHandler
{
    public class ActionAll
    {
        public Action<string, int, int, string, string[][], int> SendDataToFrontEventHandler { get; set; }
        public Action<bool, CommunicationClientInfo> ClientConnectedStateEvent { get; set; }

        public Action<bool> OnFrontServerStateEvent { get; set; }

        public Action<string, short> OnSystemMessageEvent { get; set; }

        public Action ExitEvent { get; set; }
        public Action<bool> OnQuotConnectionStatusEvent { get; set; }
    }
    /// <summary>
    /// 错误代码类
    /// </summary>
    public static class ErrCode
    {
        public const int OK = 0;
        public const int PARAM_ERROR = 1;
        public const int SERVICE_NOT_EXIST = 2;

        public const int NO_CURRENCY_EXIST = 20001;
        public const int NO_ACCOUNT_EXIST = 20002;
        public const int ALREADY_VERIFY = 20003;
        public const int NO_DEPOSIT_RECORD = 20004;
        public const int MAGIN_INFO_EXIST = 20005;
        public const int MAGIN_INFO_NOT_EXIST = 20006;
        public const int OPER_ID_ALREADY_EXIST = 20007;
        public const int OPER_ID_NOT_EXIST = 20008;
        public const int ERR_SAVE_DB = 20009;
        public const int DEPOSIT_NOT_ENOUGH = 20010;
    }
}
