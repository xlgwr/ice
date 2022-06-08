using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
