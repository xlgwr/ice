using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.PackageClient.Exts
{
    public static class exts
    {
        /// <summary>
        /// 字符包含
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m"></param>
        /// <param name="tovaule"></param>
        /// <returns></returns>
        public static bool ContainsReAll<T>(this ICollection<T> m, string allvaule)
        {
            if (m == null || !m.Any() || string.IsNullOrEmpty(allvaule))
            {
                return false;
            }
            var toLower = allvaule.ToLower();
            foreach (var item in m)
            {
                if (item == null)
                {
                    continue;
                }
                if (toLower.Contains(item.ToString().ToLower()) || toLower == item.NullToStr().ToLower()) return true;
            }
            return false;
        }
        /// <summary>
        /// 目录新增
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool FileDirectoryCreateDirectory(this string m)
        {
            try
            {
                if (m.isNull())
                {
                    return false;
                }
                if (!Directory.Exists(m))
                {
                    Directory.CreateDirectory(m);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
