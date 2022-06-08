using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Logger.BaseEntity
{
    public static class GlobalErrorNo
    {
        /// <summary>
        /// 当前增量号重复
        /// </summary>
        public const int MsgNumDouble = -8888;
        /// <summary>
        /// 当前包不需要推送
        /// </summary>
        public const int EndRspErrorNo = -9999;
        /// <summary>
        /// 客户端增量，终结包，无增量更新
        /// </summary>
        public const int EndNoDataOffSetQuery = 9999;
    }
}
