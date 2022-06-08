using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmCommunicator
{
    /// <summary>
    /// 主要用ice 对象扩展
    /// </summary>
    public static class IceExts
    {
        public static void setCallback(this Ice.Connection con, SlmCommunicator.Server.ConnectionServerEvent conn)
        {
            con.setCloseCallback(conn.closed);
            con.setHeartbeatCallback(conn.heartbeat);
        }
        public static void setCallback(this Ice.Connection con, SlmCommunicator.Client.ConnectionClientEvent conn)
        {
            con.setCloseCallback(conn.closed);
            con.setHeartbeatCallback(conn.heartbeat);
        }
        /// <summary>
        /// con.setCloseCallback(null); con.setHeartbeatCallback(null);
        /// </summary>
        /// <param name="con"></param>
        public static void setCallbackNull(this Ice.Connection con)
        {
            con.setCloseCallback(null);
            con.setHeartbeatCallback(null);
        }
        /// <summary>
        /// 总页数计算
        /// </summary>
        /// <returns></returns>
        public static long GetPageCount(this PagingInfoBase paging)
        {
            var pageCount = 0L;

            paging.pSize = paging.pSize <= 0 ? 1000 : paging.pSize;

            pageCount = (paging.Count / paging.pSize);

            if (paging.Count % paging.pSize != 0)
            {
                pageCount += 1;//不为0，所需页数加1
            }
            return pageCount;
        }
        /// <summary>
        /// 跳过条数
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public static int GetSkipCount(this PagingInfoBase paging)
        {
            return paging.pSize * (paging.pIndex - 1);
        }
    }
}
