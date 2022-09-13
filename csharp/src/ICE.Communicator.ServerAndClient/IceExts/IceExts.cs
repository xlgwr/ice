using Solomon.Log;
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
        public static Task<bool> ice_pingasync(this PackageSenderPrx packageSenderPrx, string notice = "")
        {
            return Task.Factory.StartNew(() =>
            {
                var msg = notice;
                try
                {
                    packageSenderPrx.ice_ping();
                    return true;
                }
                catch (Ice.ConnectTimeoutException ex)
                {
                    LogHelper.WriteLog("ICE_Ping", msg + $",ICE.Error:ConnectTimeoutException:{ex.Message}");
                    return false;
                }
                catch (Ice.InvocationTimeoutException ex)
                {
                    LogHelper.WriteLog("ICE_Ping", msg + $",ICE.Error:InvocationTimeoutException:{ex.Message}");
                    return false;
                }
                catch (Ice.SocketException ex)
                {
                    LogHelper.WriteLog("ICE_Ping", msg + $",ICE.Error:SocketException:{ex.Message}");
                    return false;
                }
                catch (Ice.Exception ex)
                {
                    LogHelper.WriteLog("ICE_Ping", msg + $",ICE.Error:{ex.toJsonStr()}");
                    return false;
                }
                catch (System.Exception ex)
                {
                    LogHelper.WriteLog("ICE_Ping", msg + $",Error:{ex.toJsonStr()}");
                    return false;
                }
            });
        }
        public static bool ice_ping_WaitTimeOut(this PackageSenderPrx packageSenderPrx, int millisecondsTimeout = 2000, string notice = "")
        {
            try
            {
                bool? result = packageSenderPrx?.ice_pingasync(notice).Wait(millisecondsTimeout);
                return result.GetValueOrDefault();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
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
        /// <summary>
        /// 把一个基本类型转成2维数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string[][] MakeStringArray(this object obj)
        {
            string[][] newData = new string[1][];
            string[] tmp = new string[1];
            tmp[0] = obj.ToString();
            newData[0] = tmp;
            return newData;
        }
    }
}
