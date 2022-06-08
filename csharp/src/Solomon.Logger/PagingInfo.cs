using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solomon.Logger
{
    public class PagingInfoDicHelper
    {
        /// <summary>
        /// 常量，表头分面标记,注意要为小写
        /// </summary>
        public const string HeaderName = "paginginfo111";
    }
    /// <summary>
    /// 分页辅助类
    /// </summary>
    [Serializable]
    public class PagingInfo
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int Count { get; set; } = 0;
        /// <summary>
        /// 一页显示条数
        /// </summary>
        public int pSize { get; set; } = 1000;
        /// <summary>
        /// 当前页
        /// </summary>
        public int pIndex { get; set; } = 1;

        /// <summary>
        /// 总页数计算
        /// </summary>
        /// <returns></returns>
        public int GetPageCount()
        {
            int pageCount = 0;
            pSize = pSize <= 0 ? 1000 : pSize;
            if (Count % pSize == 0)//取余为0
            {
                pageCount = Count / pSize;
            }
            else
            {
                pageCount = (Count / pSize) + 1;//不为0，所需页数加1
            }
            return pageCount;
        }
        public int GetSkipCount()
        {
            return pSize * (pIndex - 1);
        }
    }
}
