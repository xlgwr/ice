using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools.OpenExe.Exts
{
    public static class FileSearchExts
    {
        /// <summary>
        /// 用于查文件内容功能,符合条件，直接返回
        /// </summary>
        /// <param name="m"></param>
        /// <param name="searchStr">包含内容</param>
        /// <param name="nosearchStr">不包含内容</param>
        /// <param name="compFalg">或，并关系</param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static Dictionary<int, string> FilePathGetAllLineSearchAll(this string m, string searchStr, string nosearchStr, string compFalg, Encoding encoder, int topCount = 1)
        {
            var listAll = new Dictionary<int, string>();
            try
            {
                if (!File.Exists(m))
                {
                    return listAll;
                }

                var searchStrArr = searchStr.SplitTrim(',', '，', ';', '；');
                //150=0&39=0 | 150=8&39 =8 | 150=1 & 39=1 | 150=2 & 39=2 | 150=5 & 39 = 0 
                var isTTsearch = nosearchStr.Contains("|");
                var nosearchStrArr = isTTsearch ? nosearchStr.SplitTrim('|') : nosearchStr.SplitTrim(',', '，', ';', '；');
                var noAndSearchDetails = new List<string>();//分组
                var noAndSearchFlag = true;
                Dictionary<int, bool> noSearFlagDic = new Dictionary<int, bool>();
                int i = 0;
                using (FileStream fs = new FileStream(m, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoder))
                {
                    while (sr.Peek() > -1)
                    {
                        i++;
                        var currLine = sr.ReadLine();

                        if (currLine.IsNullOrWhiteSpace())
                        {
                            continue;
                        }

                        if (searchStr.IsNotNullOrWhiteSpace())
                        {
                            if (currLine.Contains(searchStrArr))
                            {
                                if (currLine.Contains(nosearchStrArr))
                                {
                                    continue;
                                }
                                listAll[i] = currLine;
                            }
                        }
                        else if (searchStr.IsNullOrWhiteSpace() && nosearchStr.IsNotNullOrWhiteSpace())
                        {
                            if (isTTsearch)
                            {
                                noAndSearchDetails.Clear();
                                noAndSearchFlag = false;
                                foreach (var item in nosearchStrArr)
                                {
                                    noAndSearchDetails = item.SplitTrim('&');
                                    if (currLine.ContainsAll(noAndSearchDetails))
                                    {
                                        noAndSearchFlag = true;
                                        continue;
                                    }
                                }
                                if (!noAndSearchFlag)
                                {
                                    listAll[i] = currLine;
                                }

                            }
                            else
                            {
                                if (currLine.Contains(nosearchStrArr))
                                {
                                    noSearFlagDic[i] = false;
                                    continue;
                                }
                            }

                            noSearFlagDic[i] = true;
                        }
                        if (listAll.Count > topCount)
                        {
                            break;
                        }
                    }

                    if (searchStr.IsNullOrWhiteSpace() && nosearchStr.IsNotNullOrWhiteSpace())
                    {
                        if (!isTTsearch && !noSearFlagDic.Values.Contains(false))
                        {
                            listAll.Clear();
                            listAll[0] = $"当前文件符合条件,不包含：{nosearchStr}";
                        }
                    }

                }
                return listAll;
            }
            catch //(Exception ex)
            {
                return listAll;
            }
        }

        /// <summary>
        /// 用于查文件内容功能,符合条件，直接返回,正则统计
        /// </summary>
        /// <param name="m"></param>
        /// <param name="searchStrReg">包含内容</param>
        /// <param name="nosearchStr">不包含内容</param>
        /// <param name="compFalg">或，并关系</param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static int FilePathGetAllLineSearchAll(this string m, Dictionary<string, int> listAll, Encoding encoder, string searchStrReg = @"\d+=")
        {
            int tmpCount = 0;
            try
            {
                if (!File.Exists(m) || searchStrReg.IsNullOrWhiteSpace())
                {
                    return tmpCount;
                }
                Regex regex = new Regex(searchStrReg);

                return m.FilePathGetAllLineSearchAll(listAll, encoder, regex);
            }
            catch //(Exception ex)
            {
                return tmpCount;
            }
        }

        public static int FilePathGetAllLineSearchAll(this string m, Dictionary<string, int> listAll, Encoding encoder, Regex regex)
        {
            int tmpCount = 0;
            try
            {
                if (!File.Exists(m) || regex == null)
                {
                    return tmpCount;
                }
                using (FileStream fs = new FileStream(m, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, encoder))
                {
                    while (sr.Peek() > -1)
                    {
                        var currLine = sr.ReadLine();

                        if (currLine.IsNullOrWhiteSpace())
                        {
                            continue;
                        }
                        //fix tag 
                        var allmatch = regex.Matches(currLine);
                        if (allmatch.Count > 0)
                        {
                            foreach (Match item in allmatch)
                            {
                                if (listAll.ContainsKey(item.Value))
                                {
                                    listAll[item.Value]++;
                                }
                                else
                                {
                                    listAll[item.Value] = 1;
                                }
                                tmpCount++;
                            }
                        }
                    }

                }
                return tmpCount;
            }
            catch //(Exception ex)
            {
                return tmpCount;
            }
        }

    }
}
