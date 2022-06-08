using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public static class ExtensionClass
    {

        public static decimal ToDecimal(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return 0;
            }
            return decimal.Parse(str);
        }
        public static bool IntToBool(this int i)
        {
            if (i == 1)
            {
                return true;
            }
            return false;
        }
        public static bool IntToBool(this int? i)
        {
            if (i == 1)
            {
                return true;
            }
            return false;
        }


        public static string BoolToIntString(this bool b)
        {
            if (b)
            {
                return "1";
            }
            return "0";

        }
        public static string BoolToIntString(this bool? b)
        {
            if (b == true)
            {
                return "1";
            }
            return "0";

        }
        public static string ToJsonString(this object obj)
        {

            return JsonConvert.SerializeObject(obj);

        }

        public static T JsonStringToObj<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static object JsonStringToObj(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }
        public static object JsonStringToObj(string value, Type type, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject(value, type, settings);
        }

        public static object JsonStringToObj(this string str)
        {
            return JsonConvert.DeserializeObject(str);
        }
        public static bool IsNullOrWhiteSpace(this string str)
        {

            return string.IsNullOrWhiteSpace(str);

        }
        public static string NullOrWhiteSpaceToDefault(this string valueSource, string valueDist)
        {
            if (valueSource.IsNullOrWhiteSpace())
            {
                return valueDist;
            }
            return valueSource;
        }

        public static bool IsNotNullOrWhiteSpace(this string str)
        {

            return !string.IsNullOrWhiteSpace(str);

        }

        public static decimal StringToDecimal(this string str)
        {
            return decimal.Parse(str);
        }
        public static int StringToInt(this string str)
        {
            return int.Parse(str);
        }

        public static string ToNotNullString(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }
        public static DateTime ToSettleDT(this string str)
        {
            return DateTime.ParseExact(str, ConstantSUtil.DateTimeFormatSettle, System.Globalization.CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// 时间补0
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string TimeConverter(this string time)
        {
            if (time == null) return string.Empty;
            if (time.Equals("0")) return string.Empty;

            if (time != null && time.Length != 6)
            {
                time = $"0{time}";
            }
            return time;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="str">20190101</param>
        /// <param name="formatstr">ConstantSUtil</param>
        /// <returns></returns>
        public static string ToStrDateTime(this string str, string formatstr = "yyyyMMdd HHmmss", string toformatstr = "yyyy-MM-dd HH:mm:ss")
        {
            if (str.isNull())
            {
                return "";
            }
            try
            {
                var dd = DateTime.ParseExact(str, formatstr, System.Globalization.CultureInfo.CurrentCulture);
                return dd.ToString(toformatstr);

            }
            catch //(Exception ex)
            {
                return str;
            }
        }
        /// <summary>
        ///  "en-us", "zh-cn"
        /// CultureInfo culture = CultureInfo.GetCultureInfo("zh-cn");  
        /// yyyy-MM-dd HH:mm:ss.ffff
        /// </summary>
        /// <param name="str"></param>
        /// <param name="cultureInfo"></param>
        /// <param name="formatstr"></param>
        /// <param name="toformatstr"></param>
        /// <returns></returns>
        public static DateTime? ToStrDateTime(this string str, System.Globalization.CultureInfo cultureInfo, string formatstr = "yyyy-MM-dd HH:mm:ss.ffff")
        {
            if (str.isNull())
            {
                return null;
            }
            try
            {
                var dd = DateTime.ParseExact(str, formatstr, cultureInfo);
                return dd;

            }
            catch //(Exception ex)
            {
                return null;
            }
        }
        public static DateTime? StrToDateTime(this string str, string formatstr = "yyyyMMdd HHmmss")
        {
            if (str.isNull())
            {
                return null;
            }
            try
            {
                var dd = DateTime.ParseExact(str, formatstr, System.Globalization.CultureInfo.CurrentCulture);
                return dd;

            }
            catch //(Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        ///  日期加时间转换标准模式
        /// </summary>
        /// <param name="strDate">20190101</param>
        /// <param name="strTime">080808</param>
        /// <returns></returns>
        public static string ToStrDateAddTime(this string strDate, string strTime, string formatstr = "yyyyMMdd HHmmss", string toformatstr = "yyyy-MM-dd HH:mm:ss")
        {
            return $"{strDate} {strTime.TimeConverter()}".ToStrDateTime(formatstr, toformatstr);
        }
        /// <summary>
        /// int32
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int TryToInt(this string str)
        {
            int result = default;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            int.TryParse(str, out result);
            return result;
        }
        /// <summary>
        /// int16
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static short TryToShort(this string str)
        {
            short result = default;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            short.TryParse(str, out result);
            return result;
        }
        /// <summary>
        /// int64
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long TryToLong(this string str)
        {
            long result = default;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            long.TryParse(str, out result);
            return result;
        }
        /// <summary>
        /// float
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float TryToFloat(this string str)
        {
            float result = default;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            float.TryParse(str, out result);
            return result;
        }
        /// <summary>
        /// decimal
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal TryToDecimal(this string str)
        {
            decimal result = default;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            decimal.TryParse(str, out result);
            return result;
        }
        public static double TryToDouble(this string str)
        {
            double result = default;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            double.TryParse(str, out result);
            return result;
        }
        public static int TryToInt(this string str, int defaultvalue = 0)
        {
            int result = defaultvalue;
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }
            int.TryParse(str, out result);
            return result;
        }

        public static string TryGetArrayByIndex(this string[] strArr, int index)
        {
            if (strArr != null && strArr.Length > index)
            {
                return strArr[index];
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 补空后面数组
        /// </summary>
        /// <param name="strArr"></param>
        /// <param name="allCount">总长度</param>
        /// <param name="defaultstr">默认值</param>
        /// <returns></returns>
        public static string[] AddItemByCount(this string[] strArr, int allCount, string defaultstr = "", bool toUTF8 = true)
        {
            if (strArr != null && strArr.Length < allCount)
            {
                var getAlllist = strArr.ToList();
                for (int i = strArr.Length; i < allCount; i++)
                {
                    getAlllist.Add(defaultstr);
                }
                //if (toUTF8)
                //{
                //    var getStr = string.Join(",", getAlllist).get_uft8();
                //    return getStr.Split(',');
                //}
                return getAlllist.ToArray();
            }
            return strArr;
        }
        public static string[] AddItemToEnd(this string[] strArr, string addtoEnd = "")
        {
            if (strArr != null)
            {
                var getAlllist = strArr.ToList();
                getAlllist.Add(addtoEnd);
                return getAlllist.ToArray();
            }
            return strArr;
        }

        public static string TryToString(this object obj)
        {
            if (obj == null)
                return string.Empty;
            else
                return obj.ToString();
        }

        public static double TryToDouble(this object obj, double defaultvalue = 0)
        {
            if (obj == null)
                return default(double);
            else
            {
                double tmp = 0;
                double.TryParse(obj.TryToString(), out tmp);
                return tmp;
            }
        }
        public static decimal TryToDecimal(this object obj)
        {
            if (obj == null)
                return default(decimal);
            else
            {
                decimal tmp;
                decimal.TryParse(obj.TryToString(), out tmp);
                return tmp;
            }
        }

        public static string TryToInterStr(this string obj)
        {
            var deci = obj.TryToDecimal();
            return ((int)deci).ToString();
        }


        public static bool HasItem<TSource>(this IEnumerable<TSource> source)
        {

            if (source == null)
            {
                return false;
            }
            else
            {
                return source.Count() > 0;
            }
        }

        public static ObservableCollection<TSource> AddToObservableCollection<TSource>(this IEnumerable<TSource> source, ObservableCollection<TSource> target)
        {
            if (target == null || source == null)
            {
                return target;
            }
            foreach (var item in source)
            {
                target.Add(item);
            }
            return target;
        }


        public static DateTime ToDateTime(this string str)
        {
            return DateTime.Parse(str);
        }


        public static DateTime AddWorkDays(this DateTime dt, decimal value)
        {
            if (value > 0)
            {
                for (int i = 0; i < value;)
                {
                    dt = dt.AddDays(1);
                    if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                    {
                        i++;
                    }
                }
            }
            else
            {
                for (int i = 0; i > value;)
                {
                    dt = dt.AddDays(-1);
                    if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                    {
                        i--;
                    }
                }
            }
            return dt;
        }

        public static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };
        #region 是否判断
        public static string getIfStr(this int m, string other = "", int isYesI = 1, int isNoI = 0, string isyes = "是", string isNo = "否")
        {
            return m == isYesI ? isyes : m == isNoI ? isNo : other;
        }
        #endregion

        #region 字符串
        public static double ToDouble(this string m, double defaultV = 0)
        {
            double result = 0.00;
            if (double.TryParse(m, out result))
            {
                return result;
            };
            return defaultV;
        }
        public static int ToInt(this string m, int defaultV = 0)
        {
            int result = 0;
            if (int.TryParse(m, out result))
            {
                return result;
            };
            return defaultV;
        }

        public static Int64 ToInt64(this string m, Int64 defaultV = 0)
        {
            Int64 result = 0;
            if (Int64.TryParse(m, out result))
            {
                return result;
            };
            return defaultV;
        }

        public static bool Contains(this string m, List<string> checkV)
        {
            if (m.isNull() || checkV == null)
            {
                return false;
            }
            foreach (var item in checkV)
            {
                if (m.ContainsWithLower(item))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 字符串
        /// </summary>
        /// <param name="m"></param>
        /// <param name="checkV"></param>
        /// <returns></returns>
        public static bool ContainsWithLower(this string m, string checkV)
        {
            if (m.isNull() || checkV.isNull())
            {
                return false;
            }
            if (m.ToLower().Contains(checkV.ToLower()))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 文件路径获取内容，（文件）
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string FilePathGetContent(this string m, Encoding encoder)
        {
            string currcontent = string.Empty;
            try
            {
                if (!File.Exists(m))
                {
                    return currcontent;
                }
                using (FileStream fs = File.Open(m, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoder))
                {
                    currcontent = sr.ReadToEnd();
                }
            }
            catch //(Exception ex)
            {
                return currcontent;
            }
            return currcontent;
        }
        /// <summary>
        /// 文件路径获取内容，（文件）
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string[] FilePathGetAllLine(this string m, int SkipLine, Encoding encoder)
        {
            var currcontent = new string[0];
            try
            {
                if (!File.Exists(m))
                {
                    return currcontent;
                }
                var listAll = new List<string>();
                int i = 0;
                using (FileStream fs = new FileStream(m, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoder))
                {
                    while (sr.Peek() > -1)
                    {
                        i++;
                        var currLine = sr.ReadLine();
                        if (i <= SkipLine)
                        {
                            continue;
                        }
                        listAll.Add(currLine);

                    }
                }
                return listAll.ToArray();
            }
            catch //(Exception ex)
            {
                return currcontent;
            }
        }
        /// <summary>
        /// 获取文件行数
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static int FilePathGetAllLineNumber(this string m, Encoding encoder)
        {
            int currcontent = 0;
            try
            {
                if (!File.Exists(m))
                {
                    return currcontent;
                }
                var currLine = string.Empty;
                int i = 0;
                using (FileStream fs = new FileStream(m, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoder))
                {
                    while (sr.Peek() > -1)
                    {
                        currLine = sr.ReadLine();
                        i++;
                    }
                }
                return i;
            }
            catch //(Exception ex)
            {
                return currcontent;
            }
        }
        /// <summary>
        /// 文件路径获取内容，（文件）
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static IEnumerable<string> FilePathReadAllLines(this string m, int skipRow, Encoding encoder)
        {
            IEnumerable<string> currcontent = new List<string>();
            try
            {
                if (!File.Exists(m))
                {
                    return currcontent;
                }
                currcontent = File.ReadAllLines(m, encoder).Skip(skipRow);

                return currcontent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool FilePathSaveContent(this string m, string content, Encoding encoder)
        {
            try
            {
                File.WriteAllText(m, content, encoder);
            }
            catch //(Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool FilePathSaveContentStreamWriter(this string m, string content, Encoding encoder, bool append = false)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(m, append, encoder))
                {
                    sw.AutoFlush = true;
                    sw.Write(content);
                    sw.Flush();
                }
            }
            catch //(Exception ex)
            {
                return false;
            }
            return true;
        }
        public static bool FileExists(this string m)
        {
            try
            {
                if (m.isNull())
                {
                    return false;
                }
                return File.Exists(m);
            }
            catch //(Exception ex)
            {
                return false;
            }
        }
        public static bool FileDelete(this string m)
        {
            try
            {
                if (m.isNull())
                {
                    return false;
                }
                if (m.FileExists())
                {
                    File.Delete(m);
                    return true;
                }
            }
            catch //(Exception ex)
            {
                return false;
            }
            return false;
        }
        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="strText">待加密字符串</param>
        ///   <returns>加密后的字符串</returns>
        public static string MD5Encrypt(this string strText)
        {
            if (strText.isNull())
            {
                return strText;
            }
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] hashedDataBytes;
            hashedDataBytes = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(strText));
            StringBuilder tmp = new StringBuilder();
            foreach (byte i in hashedDataBytes)
            {
                tmp.Append(i.ToString("x2"));
            }
            return tmp.ToString();
        }

        /// <summary>
        /// 编码：Base64编码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string ToBase64String(this string m)
        {
            if (m.isNull())
            {
                return "";
            }
            byte[] bytes = Encoding.Default.GetBytes(m);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// 解码：Base64编码
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string Base64GetString(this string m)
        {
            if (m.isNull())
            {
                return "";
            }
            byte[] outputb = Convert.FromBase64String(m);
            return Encoding.Default.GetString(outputb);
        }
        public static void SaveLogFile(this byte[] bytes, string logPath)
        {
            using (FileStream fs = new FileStream(logPath, FileMode.Append))
            {
                //将byte数组写入文件中
                fs.Seek(0, System.IO.SeekOrigin.End);
                fs.Write(bytes, 0, bytes.Length);
                //所有流类型都要关闭流，否则会出现内存泄露问题
                fs.Close();
            }
        }
        public static byte[] GetbytesByFilePath(this string logPath)
        {
            byte[] array;
            using (FileStream fs = new FileStream(logPath, FileMode.Open))
            {
                //获取文件大小
                long size = fs.Length;
                array = new byte[size];
                //将文件读到byte数组中
                fs.Read(array, 0, array.Length);
                fs.Close();
            }
            return array;
        }
        public static string Replace(this string m, List<string> toReplace, string foValue = "")
        {
            if (m.isNull())
            {
                return "";
            }
            foreach (var item in toReplace)
            {
                m.Replace(item, foValue);
            }
            return m;
        }
        public static bool isNull(this string m)
        {
            if (string.IsNullOrEmpty(m) || string.IsNullOrWhiteSpace(m))
            {
                return true;
            }
            return false;
        }
        public static string NullToStr(this object m, string defaultvalue = "", char trimChar = '、')
        {
            if (m == null)
            {
                return defaultvalue;
            }
            return m.ToString().Trim(trimChar);
        }
        /// <summary>
        /// 重复生成记录空白
        /// </summary>
        /// <param name="m"></param>
        /// <param name="genNum"></param>
        /// <param name="formatStr"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        public static string StrFormatNum(this string m, int genNum = 3, string formatStr = "   {0}   {1}", char splitChar = '、')
        {
            if (m == null)
            {
                m = m.NullToStr();
            }
            var splitResult = m.SplitTrim(splitChar);
            string result = "";
            for (int i = 0; i < genNum; i++)
            {
                string nameValue = "      ";
                if (i < splitResult.Count)
                {
                    nameValue = splitResult[i];
                }
                result += string.Format(formatStr, nameValue, splitChar);
            }
            return result.Trim(splitChar);
        }
        /// <summary>
        /// 重复生成字符
        /// </summary>
        /// <param name="m"></param>
        /// <param name="genNum"></param>
        /// <param name="formatStr"></param>
        /// <returns></returns>
        public static string StrFormatNum(this string m, double genNum = 3, string formatStr = " ")
        {
            if (m == null)
            {
                m = m.NullToStr();
            }
            string result = "";
            for (int i = 0; i < genNum; i++)
            {
                result += formatStr;
            }
            return result;
        }
        /// <summary>
        /// 去空，去空格
        /// </summary>
        /// <param name="m"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<string> SplitTrim(this string m, params char[] separator)
        {
            if (m == null)
            {
                return null;
            }
            var result = new List<string>();
            var resultSplit = m.Split(separator);
            foreach (var item in resultSplit)
            {
                if (!item.isNull())
                {
                    result.Add(item.Trim());
                }
            }
            return result;
        }

        #endregion

        #region 字典相关
        /// <summary>
        /// ConcurrentDictionary 修改
        /// </summary>
        /// <param name="keyValues"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool UpdateNewValue(this ConcurrentDictionary<string, int> keyValues, string key, int newvalue)
        {
            if (!keyValues.ContainsKey(key))
            {
                return false;
            }
            var getvalue = keyValues[key];
            return keyValues.TryUpdate(key, newvalue, getvalue);
        }
        /// <summary>
        /// 尝试将键和值添加到字典中：如果不存在，才添加；存在，不添加也不抛导常
        /// </summary>
        public static Dictionary<TKey, TValue> TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key) == false) dict.Add(key, value);
            return dict;
        }
        /// <summary>
        /// 将键和值添加或替换到字典中：如果不存在，则添加；存在，则替换
        /// </summary>
        public static Dictionary<TKey, TValue> AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
            return dict;
        }
        /// <summary>
        /// 获取与指定的键相关联的值，如果没有则返回输入的默认值
        /// </summary>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            return !key.NullToStr().isNull() && dict.ContainsKey(key) ? dict[key] : defaultValue;
        }

        public static void TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, List<TKey> keys)
        {
            if (dict.HasItem())
            {
                foreach (TKey item in keys)
                {
                    dict.Remove(item);
                }
            }
        }
        public static void TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, List<TKey> keys)
        {
            if (dict.HasItem())
            {
                TValue value;
                foreach (TKey item in keys)
                {
                    dict.TryRemove(item, out value);
                }
            }
        }

        #endregion
        #region 时间相关
        public static string toFormat(this DateTime m, string format = "yyyy-MM-dd HH:mm:ss")
        {
            if (m == null)
            {
                return "";
            }
            return m.ToString(format);
        }
        #endregion

        #region 数据转换
        public static string toEmptyStr(this double m, double defaultV = 0)
        {
            if (m <= defaultV)
            {
                return "";
            }
            return m.ToString();
        }
        #endregion

        #region SQL Getn
        /// <summary>
        /// 生成like sql 语句
        /// </summary>
        /// <param name="m"></param>
        /// <param name="isAnd">0: ,1:and,2:or</param>
        /// <param name="flag">0: XX%,1:%XX,other:%XX%</param>
        /// <returns></returns>
        public static string toLikeSql(this string m, string vaule, int isAnd = 0, int flag = 0)
        {
            if (m.isNull() || vaule.isNull())
            {
                return "";
            }
            string isOr = isAnd == 0 ? "" : isAnd == 1 ? "AND" : "OR";
            switch (flag)
            {
                case 0:
                    return string.Format(" {0} {1} like N'{2}%' ", isOr, m, vaule);
                case 1:
                    return string.Format(" {0} {1} like N'%{2}' ", isOr, m, vaule);
                default:
                    return string.Format(" {0} {1} like N'%{2}%' ", isOr, m, vaule);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="vaule"></param>
        /// <param name="isAnd">0:空,1:and,2:or</param>
        /// <param name="isEquleGeLt">3:=,4:>,5:<,6:!=<,7: is null,8:is NOT NULL /param>
        /// <returns></returns>
        public static string toAndOrSql(this string m, string vaule, int isAnd = 0, int isEquleGeLt = 3)
        {
            if (m.isNull())
            {
                return "";
            }
            string isOr = isAnd == 0 ? "" : isAnd == 1 ? "AND" : "OR";
            var strResult = "";
            switch (isEquleGeLt)
            {
                case 3:
                    strResult = string.Format(" {0} {1} = N'{2}' ", isOr, m, vaule);
                    break;
                case 4:
                    strResult = string.Format(" {0} {1} > '{2}' ", isOr, m, vaule);
                    break;
                case 5:
                    strResult = string.Format(" {0} {1} < '{2}' ", isOr, m, vaule);
                    break;
                case 6:
                    strResult = string.Format(" {0} {1} != N'{2}' ", isOr, m, vaule);
                    break;
                case 7:
                    strResult = string.Format(" {0} {1} is NULL ", isOr, m, vaule);
                    break;
                case 8:
                    strResult = string.Format(" {0} {1} is NOT NULL ", isOr, m, vaule);
                    break;
                default:
                    strResult = string.Format(" {0} {1} = N'{2}' ", isOr, m, vaule);
                    break;
            }

            return strResult;

        }
        #endregion
        public static T JsonTo<T>(this object m)
        {
            if (m == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(m.NullToStr(), settings);
        }
        public static string toJsonStr(this object m)
        {
            if (m == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(m, settings);
        }
        /// <summary>
        ///  将Object类型对象(注：必须是可序列化的对象)转换为二进制序列字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeBinaryObject(this object obj)
        {
            IFormatter formatter = new BinaryFormatter();
            string result = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                //byte[] byt = new byte[stream.Length];
                //byt = stream.ToArray();
                //result = Encoding.UTF8.GetString(byt, 0, byt.Length);
                //result = Convert.ToBase64String(byt);
                stream.Flush();
            }
            return result;
        }

        /// <summary>
        /// 将二进制序列字符串转换为Object类型对象
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static object DeserializeBinaryObject(this string str)
        {
            IFormatter formatter = new BinaryFormatter();
            //byte[] byt = Encoding.UTF8.GetBytes(str);
            byte[] byt = Convert.FromBase64String(str);
            object obj = null;
            using (Stream stream = new MemoryStream(byt, 0, byt.Length))
            {
                obj = formatter.Deserialize(stream);
            }
            return obj;
        }
        public static byte ToByte(this sbyte value)
        {

            return Convert.ToByte(value);
        }
        public static byte ToByte(this char value)
        {

            return Convert.ToByte(value);
        }

        public static sbyte ToSByte(this byte value)
        {

            return Convert.ToSByte(value);
        }
        public static char ToChar(this byte value)
        {
            return Convert.ToChar(value);
        }
        /// <summary>
        /// 获取字符串，第一个字母对应char 转为 byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetStringFirstCharToByte(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return 0;
            }
            return value[0].ToByte();
        }
        public static char ToChar(this sbyte value)
        {

            return Convert.ToChar(value);
        }
        public static int TryToInt(this object obj)
        {
            int tmp = default(int);
            if (obj == null)
                return tmp;
            else
            {
                int.TryParse(obj.TryToString(), out tmp);
            }
            return tmp;
        }

        public static int TryToInt(this object obj, int defavlue = 0)
        {
            int tmp = defavlue;
            if (obj == null)
                return tmp;
            else
            {
                int.TryParse(obj.TryToString(), out tmp);
            }
            return tmp;
        }

        public static decimal? TryToDecimalOrNull(this object obj)
        {
            if (obj == null)
                return null;
            else
            {
                decimal tmp;
                if (decimal.TryParse(obj.TryToString(), out tmp))
                    return tmp;
                else
                    return null;
            }
        }
        public static decimal TryToDecimal(this object obj, decimal defaultvalue = 0)
        {
            if (obj == null)
                return defaultvalue;
            else
            {
                decimal tmp;
                if (decimal.TryParse(obj.TryToString(), out tmp))
                    return tmp;
                else
                    return defaultvalue;
            }
        }

        public static sbyte TryToSByte(this object obj)
        {
            if (obj == null)
                return default(sbyte);
            else
            {
                sbyte tmp;
                sbyte.TryParse(obj.TryToString(), out tmp);
                return tmp;
            }
        }
        public static byte TryToByte(this object obj)
        {
            if (obj == null)
                return default(byte);
            else
            {
                byte tmp;
                byte.TryParse(obj.TryToString(), out tmp);
                return tmp;
            }
        }
        public static DateTime? TryToDetaTime(this object obj)
        {
            if (obj == null)
                return null;
            else
            {
                try
                {
                    DateTime tmp;
                    tmp = DateTime.ParseExact(obj.TryToString(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    return tmp;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static DataTable CopyToDataTableExt(this DataRow[] rows, DataTable defaultTable)
        {
            if (rows == null || rows.Length == 0)
            {
                return defaultTable.Clone();
            }
            return rows.CopyToDataTable();
        }
        public static Dictionary<string, string> GetColumnNames(this DataColumnCollection dataColumns)
        {
            var result = new Dictionary<string, string>();
            if (dataColumns == null || dataColumns.Count == 0)
            {
                return result;
            }
            foreach (Data.DataColumn item in dataColumns)
            {
                result.TryAdd(item.ColumnName, item.Caption);
            }
            return result;
        }
        /// <summary>
        /// 少于0时,返回0;
        /// a < 0 ? 0 : a
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int CheckToZero(this int a)
        {
            return a < 0 ? 0 : a;
        }
        /// <summary>
        /// 是否判定返回
        /// 1：是
        /// 其它：否
        /// a == 1 ? isOK : isNot
        /// </summary>
        /// <param name="a"></param>
        /// <param name="isOK"></param>
        /// <param name="isNot"></param>
        /// <returns></returns>
        public static string ToIsOkOrNot(this int a, string isOK = "是", string isNot = "否")
        {
            return a == 1 ? isOK : isNot;
        }
        public static string ToIsOkOrNot(this bool a, string isOK = "是", string isNot = "否")
        {
            return a ? isOK : isNot;
        }
        /// <summary>
        /// 取左边几位
        /// 超过长度，取全部
        /// </summary>
        /// <param name="a"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetLeftStr(this string a, int length = 2)
        {
            if (a.IsNullOrWhiteSpace())
            {
                return a;
            }
            if (a.Length > length)
            {
                return a.Substring(a.Length - length);
            }
            return a;
        }
        /// <summary>
        /// 初启化为默认值
        /// </summary>
        /// <param name=""></param>
        /// <param name="datas"></param>
        public static void AddOrUpdateDatas(this ConcurrentDictionary<long, bool> dics, IEnumerable<long> datas, bool defaultvalue = false)
        {
            if (dics == null || !datas.HasItem())
            {
                return;
            }
            foreach (var item in datas)
            {
                dics[item] = defaultvalue;
            }
        }
        /// <summary>
        /// 初启化为默认值
        /// </summary>
        /// <param name=""></param>
        /// <param name="datas"></param>
        public static void TryAddDatas(this ConcurrentDictionary<long, bool> dics, IEnumerable<long> datas, bool defaultvalue = false)
        {
            if (dics == null || !datas.HasItem())
            {
                return;
            }
            foreach (var item in datas)
            {
                dics.TryAdd(item, defaultvalue);
            }
        }
    }

    public class ConstantSUtil
    {
        /// <summary>
        /// 时间格式
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        public const string DateTimeFileFormat = "yyyy-MM-dd HHmmss";
        public const string DateTimeFormatShot = "yyyy-MM-dd";
        public const string DateTimeFormatSettle = "yyyyMMdd";
        public const string UserID = "UserID";
        public const string RecoverData = "RecoverData";
        public const string RecoverDataPersistent = "RecoverDataPersistent";
        public const string DBlogSuccessFlagLocal = "DBlogSuccessFlagLocal";
        public const string DBlogSuccessFlagPersistent = "DBlogSuccessFlagPersistent";
        public const decimal DEFAULT_PRICE = -999999;
        public const string NegativePrice = "NegativePrice";
        public static string NYMEX_CL { get; set; }


    }

    /// <summary>
    /// 时间拓展累
    /// </summary>
    public static class DateTimeExtend
    {
        /// <summary>
        /// 判断是否为工作日
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsWorkDay(this DateTime dt)
        {
            //先从日期表中，查找不是上班时间，如果不是直接返回 false ，如果是，直接返回 true。
            //如果在日期表中，找不到，则查找定义的日历，依据日历定义的周末时间来定义是否为工作日。
            //获取日历中不上班的标准周末时间,判断是不是上班时间
            if (dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday)
                return false;
            else
                return true;
        }
        public static string ToyyyyMMddExt(this DateTime obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString("yyyyMMdd");
        }

        public static string ToyyyyMMddExt(this DateTime? obj)
        {
            if (obj == null)
                return string.Empty;
            else
            {
                DateTime dt = (DateTime)obj;
                return dt.ToString("yyyyMMdd");
            }
        }

        public static string ToHHmmssExt(this DateTime obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString("HHmmss");
        }

        public static string ToHHmmssfffExt(this DateTime obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString("HHmmssfff");
        }

        public static string ToyyyyMMddHHmmssExt(this DateTime obj)
        {
            if (obj == null) return string.Empty;
            return obj.ToString("yyyyMMddHHmmss");
        }

        public static DateTime? TryParseExt(this string dateStr)
        {
            DateTime dateTime = DateTime.Now;
            if (DateTime.TryParse(dateStr, out dateTime))
            {
                return dateTime;
            }
            return null;
        }

        /// <summary>
        /// 时分秒时间补零  传入数据为93000等格式,即为9点30，返回093000
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回补零后的字符串</returns>
        public static string TimeStrAddZeroExt(this string str)
        {
            if (str != null || str.Length < 6)
            {
                StringBuilder sb = new StringBuilder();
                var length = str.Length;
                for (int i = length; i < 6; i++)
                {
                    sb.Append("0");
                }

                return $"{sb.ToString()}{str}";
            }
            return str;
        }
        public static string get_uft82(this string unicodeString)
        {
            if (unicodeString.isNull())
            {
                return "";
            }
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        /// <summary>
        /// 以UTF-8带BOM格式返回utf8编码
        /// </summary>
        /// <param name="unicodeString"></param>
        /// <returns></returns>
        public static string get_uft8(this string text)
        {

            if (text.isNull())
            {
                return "";
            }
            //声明字符集   
            System.Text.Encoding utf8, utf8bom;

            //Default   //以UTF-8带BOM格式读取文件内容
            utf8bom = new UTF8Encoding(true);
            //utf8   
            utf8 = new UTF8Encoding(false);
            byte[] gb;
            gb = utf8bom.GetBytes(text);
            gb = System.Text.Encoding.Convert(utf8bom, utf8, gb);
            //返回转换后的字符   
            var result = utf8.GetString(gb);

            //using (var sink = new StreamWriter("FoobarNoBom.txt", false, utf8WithoutBom))
            //{
            //    sink.WriteLine(result);
            //}
            //using (var sink = new StreamWriter("FoobarBom.txt", false, utf8bom))
            //{
            //    sink.WriteLine(result);
            //}

            return result;
        }
        /// <summary>
        /// GB2312转换成UTF8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string gb2312_utf8(this string text)
        {
            if (text.isNull())
            {
                return "";
            }
            //声明字符集   
            System.Text.Encoding utf8, gb2312;

            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string utf8_gb2312(this string text)
        {
            if (text.isNull())
            {
                return "";
            }
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }

        /// <summary>
        /// 将自定义对象序列化为XML字符串
        /// </summary>
        /// <param name="myObject">自定义对象实体</param>
        /// <returns>序列化后的XML字符串</returns>
        public static string SerializeToXml<T>(this T myObject)
        {
            if (myObject != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));

                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Xml.Formatting.None;//缩进
                xs.Serialize(writer, myObject);

                stream.Position = 0;
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                    reader.Close();
                }
                writer.Close();
                return sb.ToString();
            }
            return string.Empty;
        }

        //xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"

        public static string SerializeToXmlWithNoHead<T>(this T myObject)
        {
            if (myObject != null)
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));

                MemoryStream stream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Formatting = Xml.Formatting.None;//缩进
                xs.Serialize(writer, myObject);

                stream.Position = 0;
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        sb.Append(line);
                    }
                    reader.Close();
                }
                writer.Close();
                return sb.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf - 8\"?>", "").Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "").Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            }
            return string.Empty;
        }

        /// <summary>
        /// 将XML字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">XML字符</param>
        /// <returns></returns>
        public static T DeserializeToObject<T>(this string xml)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }
        /// <summary>
        /// HH:mm
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool NowIsInSpanHM(this string timeSpan)
        {
            var timeSpans = timeSpan.Split('-');
            var now = DateTime.Now.ToString("HH:mm").Replace(":", "").ToInt();
            var start = timeSpans[0].Replace(":", "").ToInt();
            var end = timeSpans[1].Replace(":", "").ToInt();
            if (start <= end)
            {
                if (now >= start && now <= end)
                {
                    return true;
                }
            }
            else
            {
                if (now >= start || now <= end)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// HH:mm:ss
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool NowIsInSpanHMS(this string timeSpan)
        {

            var timeSpans = timeSpan.Split('-');
            var now = DateTime.Now.ToString("HH:mm:ss").Replace(":", "").ToInt();
            var start = timeSpans[0].Replace(":", "").ToInt();
            var end = timeSpans[1].Replace(":", "").ToInt();
            if (start <= end)
            {
                if (now >= start && now <= end)
                {
                    return true;
                }
            }
            else
            {
                if (now >= start || now <= end)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsInTimeSpan(this string timeSpan, int time)
        {
            var timeSpans = timeSpan.Split('-');
            var now = time;
            var start = timeSpans[0].Replace(":", "").ToInt();
            var end = timeSpans[1].Replace(":", "").ToInt();
            if (start <= end)
            {
                if (now >= start && now <= end)
                {
                    return true;
                }
            }
            else
            {
                if (now >= start || now <= end)
                {
                    return true;
                }
            }
            return false;
        }

    }

    /// <summary>
    /// 时间拓展2
    /// </summary>
    public static class DateTimeExts
    {
        /// <summary>
        /// 时间戳计时开始时间
        /// </summary>
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// DateTime转换为10位时间戳（单位：秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>10位时间戳（单位：秒）</returns>
        public static long DateTimeToTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalSeconds;
        }

        /// <summary>
        /// DateTime转换为13位时间戳（单位：毫秒）
        /// </summary>
        /// <param name="dateTime"> DateTime</param>
        /// <returns>13位时间戳（单位：毫秒）</returns>
        public static long DateTimeToLongTimeStamp(this DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 10位时间戳（单位：秒）转换为DateTime
        /// </summary>
        /// <param name="timeStamp">10位时间戳（单位：秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime TimeStampToDateTime(this long timeStamp)
        {
            return timeStampStartTime.AddSeconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// 13位时间戳（单位：毫秒）转换为DateTime
        /// </summary>
        /// <param name="longTimeStamp">13位时间戳（单位：毫秒）</param>
        /// <returns>DateTime</returns>
        public static DateTime LongTimeStampToDateTime(this long longTimeStamp)
        {
            return timeStampStartTime.AddMilliseconds(longTimeStamp).ToLocalTime();
        }
    }
    public static class FilePathIOExtend
    {
        /// <summary>
        /// temp目录下生成临时文件目录
        /// </summary>
        /// <returns></returns>
        public static string CreateTempPath(this string pathname)
        {
            string temPath = System.IO.Path.GetTempPath();
            while (!temPath.ToLower().EndsWith("temp"))
            {
                temPath = System.IO.Directory.GetParent(temPath).FullName;
            }
            string path = Path.Combine(temPath, pathname);
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }
        public static bool DirectoryExists(this string path)
        {
            return System.IO.Directory.Exists(path);
        }
        public static bool DirectoryCreate(this string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            };
            return true;
        }
        /// <summary>
        /// 文件路径获取内容，（文件）,获取前N行
        /// -1 取所有行
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<string> FilePathBeforeLine(this string m, int beforeLine, Encoding encoder)
        {
            var result = new List<string>();
            try
            {
                if (!File.Exists(m))
                {
                    return result;
                }

                int i = 0;
                FileStream fs = new FileStream(m, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                string currLinestr = string.Empty;
                using (StreamReader sr = new StreamReader(fs, encoder))
                {
                    while (sr.Peek() > -1)
                    {
                        currLinestr = sr.ReadLine();
                        if (i < beforeLine || beforeLine == -1)
                        {
                            result.Add(currLinestr);
                        }
                        else
                        {
                            break;
                        }
                        i++;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        // <summary>
        /// 倒序读取文本文件
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <param name="position">读取流的起始位置</param>
        /// <returns></returns>
        public static List<string> FilePathLastLine(this string file, int endNline, Encoding encoder, string searchStr = "")
        {

            var result = new List<string>();

            if (!File.Exists(file))
            {
                return result;
            }
            long ps = 0;
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (ps <= 0)
                    ps = fs.Length;
                //读取n行
                for (int i = 0; i <= endNline; i++)
                {
                    var getresult = InverseReadRow2(fs, ps, ref result, searchStr);

                    if (!result.HasItem())
                    {
                        endNline++;
                    }
                    ps = getresult.Item1;

                    if (ps <= 0)
                        break;

                    if (result.Count == endNline)
                    {
                        break;
                    }

                    if (getresult.Item2.IsNotNullOrWhiteSpace())
                    {
                        break;
                    }
                    else
                    {
                        if (searchStr.IsNotNullOrWhiteSpace())
                        {
                            endNline++;
                        }
                    }


                }
            }
            return result;
        }
        /// <summary>
        /// 从后向前按行读取文本文件，最多读取 100kb
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="position"></param>
        /// <param name="s"></param>
        /// <param name="maxRead">默认每次最多读取100kb数据</param>
        /// <returns>返回读取位置</returns>
        public static Tuple<long, string> InverseReadRow2(FileStream fs, long position
             , ref List<string> s
             , string findstr = ""
             , int maxRead = 1024000)
        {
            //byte n = 0xD;//回车符
            byte r = 0xA;//换行符
            if (fs.Length == 0)
                return new Tuple<long, string>(0, "");
            var newPos = position - 1;
            int curVal = 0;
            var readLength = 0;
            var str = "";
            do
            {
                readLength++;
                if (newPos <= 0)
                    newPos = 0;

                fs.Position = newPos;
                curVal = fs.ReadByte();
                if (curVal == -1)
                    break;
                if (curVal != r)
                    newPos--;

                if (newPos <= 0)
                    break;
                //
                if (readLength == maxRead)
                    break;

            } while (curVal != r);

            byte[] arr = new byte[readLength];
            fs.Position = position - readLength;
            fs.Read(arr, 0, readLength);
            str = Encoding.UTF8.GetString(arr);

            string findStr = string.Empty;
            if (!str.IsNullOrWhiteSpace() && readLength > 1)
            {
                s.Insert(0, str);
                if (!findstr.IsNullOrWhiteSpace())
                {
                    if (str.Contains(findstr))
                    {
                        findStr = str;
                    }
                }
            }
            return new Tuple<long, string>(newPos, findStr);
        }

        public static List<string> GetDirectoryOfFullName(this string path, List<string> nameFilter, string extensionFilte = ".csv")
        {

            var result = new List<string>();
            if (path.IsNullOrWhiteSpace() || !path.DirectoryExists())
            {
                return result;
            }
            try
            {
                DirectoryInfo fdir = new DirectoryInfo(path);//文件列表   
                var file = fdir.GetFiles().Select(a => a.FullName).ToList();
                fdir = null;
                if (file.Count == 0) //当前目录文件或文件夹不为空                   
                {
                    return result;
                }
                var currname = string.Empty;
                var currnameext = string.Empty;
                foreach (var f in file) //显示当前目录所有文件   
                {
                    currname = Path.GetFileNameWithoutExtension(f);
                    currnameext = Path.GetExtension(f).NullToStr().ToLower();
                    if (currname.IsNullOrWhiteSpace())
                    {
                        continue;
                    }
                    if (nameFilter.HasItem())
                    {
                        if (!currname.Contains(nameFilter))
                        {
                            continue;
                        };
                    }
                    if (extensionFilte.IsNullOrWhiteSpace())
                    {
                        result.Add(f);
                    }
                    else if (extensionFilte == currnameext)
                    {
                        result.Add(f);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// 获取1M大小字符
        /// 1M = 1024k = 1048576字节 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetOneM(this string text, string defaultStr = "0")
        {
            if (text.IsNullOrWhiteSpace())
            {
                text = defaultStr;
            }
            StringBuilder sb = new StringBuilder();
            int getByte = GetBytesOfString(text);
            int getnCount = 1048576 / getByte;
            for (int i = 0; i < getnCount; i++)
            {
                sb.Append(text);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取输入数据字节大小 返回大小
        /// 1M = 1024k = 1048576字节 
        /// 1:m,2:k,3:byte
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static int GetBytesOfString(this string Text, int type = 3)
        {
            int nByte = 0;
            byte[] bytes = Encoding.Unicode.GetBytes(Text);
            for (int i = 0; i < bytes.GetLength(0); i++)
            {
                //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
                if (i % 2 == 0)
                {
                    nByte++;      //  在UCS2第一个字节时n加1
                }
                else
                {
                    //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
                    if (bytes[i] > 0)
                    {
                        nByte++;
                    }
                }
            }
            int result = nByte;
            switch (type)
            {
                case 1:
                    result = nByte / 1048576;
                    break;
                case 2:
                    result = nByte / 1024;
                    break;
                default:
                    break;
            }
            return result;
        }
    }
    /// <summary>
    /// K,List 字典
    /// Dictionary<K, ICollection<V>>
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class MultisDictionary<K, V> : Dictionary<K, ICollection<V>>
    {
        public void Add(K key, V value)
        {
            ICollection<V> list = null;
            if (!TryGetValue(key, out list))
            {
                list = new List<V>();
                Add(key, list);
            }
            list.Add(value);
        }

        public void Remove(K key, V value)
        {
            ICollection<V> list = this[key];
            list.Remove(value);
            if (list.Count == 0)
            {
                Remove(key);
            }
        }
    }
    public class MultisConcurrentDictionary<K, V> : ConcurrentDictionary<K, ICollection<V>>
    {
        public void TryAdd(K key, V value)
        {
            ICollection<V> list = null;
            if (!TryGetValue(key, out list))
            {
                list = new List<V>();
                TryAdd(key, list);
            }
            list.Add(value);
        }

        public void TryRemove(K key, V value)
        {
            ICollection<V> list = this[key];
            list.Remove(value);
            if (list.Count == 0)
            {
                TryRemove(key, out _);
            }
        }
    }
    public static class strExtsLen
    {
        public static int getLetterLen(this string StrContert)
        {
            char[] s = StrContert.ToCharArray();
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= 'a' && s[i] <= 'z' || s[i] >= 'A' && s[i] <= 'Z')
                {
                    count++;
                }
            }
            return count;

            //if (string.IsNullOrEmpty(StrContert))
            //    return 0;
            //StrContert = StrContert.Replace("\n", " ").Replace("\r", " ");
            //int k = Regex.Matches(StrContert, @"^[a-z]+$").Count;
            //return k;
        }
        public static int getDigiLen(this string StrContert)
        {
            char[] s = StrContert.ToCharArray();
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= 48 && s[i] <= 57)
                {
                    count++;
                }
            }
            return count;
            //if (string.IsNullOrEmpty(StrContert))
            //    return 0;
            //StrContert = StrContert.Replace("\n", " ").Replace("\r", " ");
            //return Regex.Matches(StrContert, @"^[0-9]*$").Count;
        }
        public static int getUpperLen(this string StrContert)
        {
            char[] s = StrContert.ToCharArray();
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] >= 'A' && s[i] <= 'Z')
                {
                    count++;
                }
            }
            return count;

            //if (string.IsNullOrEmpty(StrContert))
            //    return 0;
            //StrContert = StrContert.Replace("\n", " ").Replace("\r", " ");
            //return Regex.Matches(StrContert, @"^[A-Z]+$").Count;
        }
        public static int getSpecLen(this string StrContert)
        {
            if (string.IsNullOrEmpty(StrContert))
                return 0;

            StrContert = StrContert.Replace("\n", " ").Replace("\r", " ");
            Regex reg = new Regex("((?=[\x21-\x7e]+)[^A-Za-z0-9])");
            char[] s = StrContert.ToCharArray();
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (reg.IsMatch(s[i].ToString()))
                {
                    count++;
                }
            }
            return count;
        }
        /// <summary>
        /// 截出2位小数
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string formatDataSplit(string s)
        {
            try
            {
                if (s.IsNullOrWhiteSpace())
                {
                    return "0.00";
                }

                var spliList = s.Trim().Split('.');

                if (spliList.Length <= 1)
                {
                    return $"{s.Trim()}.00";
                }
                var next2 = "";
                switch (spliList[1].Length)
                {
                    case 0:
                        next2 = "00";
                        break;
                    case 1:
                        next2 = $"{spliList[1]}0";
                        break;
                    default:
                        next2 = spliList[1].Substring(0, 2);
                        break;
                }
                return $"{spliList[0]}.{next2}";
            }
            catch (Exception)
            {
                return s;
            }
        }
    }
}
