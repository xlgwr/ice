using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trade.DataType;

namespace System
{
    public static partial class ExtModels
    {
        public static List<CommunicationBaseInfo> GetListObj(this ICollection<CommunicationBaseInfoForICE> list)
        {
            var result = new List<CommunicationBaseInfo>();
            if (list.HasItem())
            {
                foreach (var item in list)
                {
                    result.Add(item.ConvertToObject());
                }
            }
            return result;
        }
        public static List<CommunicationBaseInfoForICE> GetListObj(this ICollection<CommunicationBaseInfo> list)
        {
            var result = new List<CommunicationBaseInfoForICE>();
            if (list.HasItem())
            {
                foreach (var item in list)
                {
                    result.Add(item.ConvertToIceObject());
                }
            }
            return result;
        }
        public static Dictionary<CacheSeverType, long> GetCacheSeverTypeDic(this List<keyvalueDB> dBs)
        {
            var result = new Dictionary<CacheSeverType, long>();
            foreach (var item in dBs)
            {
                result.Add((CacheSeverType)item.k.TryToInt(), item.v.TryToLong());
            }
            return result;
        }
    }
}
