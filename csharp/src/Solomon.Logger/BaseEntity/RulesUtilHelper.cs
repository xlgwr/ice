using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class RulesUtilHelper
    {
        /// <summary>
        /// 返回是否合规输入
        /// </summary>
        /// <param name="rulesClasses"> 检验数据</param>
        /// <param name="languageType">语言（0中文1英文2繁体） </param>
        /// <returns></returns>
        public static string ResultRulesMsg(List<RulesClass> rulesClasses, int languageType = 0)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < rulesClasses.Count; i++)
            {
                var str = rulesClasses[i].Name;
                rulesClasses[i].ResultMsg = rulesClasses[i].ResultMsg.Trim();   //去掉前后空格

                if (rulesClasses[i].ResultMsg.IsNullOrWhiteSpace())
                {
                    if (rulesClasses[i].ResultType == ResultTypeEnum.Empty)
                    {
                        str += languageType == 0 ? "不能为空" : languageType == 1 ? "Can not be empty" : "不能为空";
                        stringBuilder.AppendLine(str);
                    }
                    continue;
                }

                if (rulesClasses[i].ResultType == ResultTypeEnum.Digital)
                {
                    str += languageType == 0 ? "只能输入数字" : languageType == 1 ? "You can only enter Numbers" : "只能输入数字";
                }
                else if (rulesClasses[i].ResultType == ResultTypeEnum.English || rulesClasses[i].ResultType == ResultTypeEnum.SingleByte)
                {
                    str += languageType == 0 ? "只能输入英文,符号和字母" : languageType == 1 ? "English, symbols and letters only" : "只能输入英文,符号和字母,单字节字符";
                }
                else if (rulesClasses[i].ResultType == ResultTypeEnum.Phone)
                {
                    var name = rulesClasses[i].Name.Trim(':').Trim('：');
                    str += languageType == 0 ? "请输入正确的" + name : languageType == 1 ? "Please enter the correct one " + name : "请输入正确的" + name;
                }
                else if (rulesClasses[i].ResultType == ResultTypeEnum.EMail)
                {
                    str += languageType == 0 ? "输入正确的邮箱" : languageType == 1 ? "Enter the correct email address" : "输入正确的邮箱";
                }
                else if (rulesClasses[i].ResultType == ResultTypeEnum.Integer)
                {
                    str += languageType == 0 ? "只能输入整数" : languageType == 1 ? "Integer only" : "只能输入整数";
                }
                else if (rulesClasses[i].ResultType == ResultTypeEnum.PositiveDigital)
                {
                    str += languageType == 0 ? "只能输入正数" : languageType == 1 ? "Positive number only" : "只能输入正数";
                }
                else if (rulesClasses[i].ResultType == ResultTypeEnum.Decimal)
                {
                    str += languageType == 0 ? "只能输入数字" : languageType == 1 ? "You can only enter Numbers" : "只能输入数字";
                }
                else
                {
                    continue;
                }
                var resultstr = resultDigital(rulesClasses[i].ResultMsg, rulesClasses[i].ResultType) == false ? str : string.Empty;
                if (resultstr.IsNotNullOrWhiteSpace() == true) stringBuilder.AppendLine(resultstr);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 只能输入数字
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool resultDigital(string content, ResultTypeEnum typeEnum)
        {
            string strValidRealPattern;
            if (typeEnum == ResultTypeEnum.Digital)
            {
                strValidRealPattern = @"^-[0-9]+(.[0-9]+)?|[0-9]+(.[0-9]+)?$";// "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
                Regex reg = new Regex(strValidRealPattern);
                Match ma = reg.Match(content);
                if (ma.Success) return true;
            }
            else if (typeEnum == ResultTypeEnum.English)
            {
                strValidRealPattern = "[^\x00-\xff]";
                if (Regex.IsMatch(content, strValidRealPattern)) return false;

                return !Regex.IsMatch(content, @"[\u4e00-\u9fa5]");
            }
            else if (typeEnum == ResultTypeEnum.Phone)
            {
                strValidRealPattern = "[^\x00-\xff]";
                if (Regex.IsMatch(content, strValidRealPattern)) return false;

                strValidRealPattern = @"^[\d-+]+$";
                Regex reg = new Regex(strValidRealPattern);
                Match ma = reg.Match(content);
                if (ma.Success) return true;
            }
            else if (typeEnum == ResultTypeEnum.EMail)
            {
                strValidRealPattern = "[^\x00-\xff]";
                if (Regex.IsMatch(content, strValidRealPattern)) return false;

                // strValidRealPattern = @"\w[-\w.+]*@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,14}";
                return !Regex.IsMatch(content, @"[\u4e00-\u9fa5]");
            }
            else if (typeEnum == ResultTypeEnum.Integer)
            {
                strValidRealPattern = "^[0-9]*$";
                return Regex.IsMatch(content, strValidRealPattern);
            }
            else if (typeEnum == ResultTypeEnum.PositiveDigital)
            {
                strValidRealPattern = "^[0-9]+(.[0-9]+)?$";
                return Regex.IsMatch(content, strValidRealPattern);
            }
            else if (typeEnum == ResultTypeEnum.SingleByte)
            {
                strValidRealPattern = "[^\x00-\xff]";
                return !Regex.IsMatch(content, strValidRealPattern);
            }
            else if (typeEnum == ResultTypeEnum.Decimal)
            {
                strValidRealPattern = @"^-?\d*[.]?\d*$";
                return Regex.IsMatch(content, strValidRealPattern);
            }
            return false;
        }

        /// <summary>
        /// 校验保证金的系数是否合规
        /// </summary>
        /// <param name="margin">导入保证金</param>
        /// <param name="margindb">原始保证金</param>
        /// <param name="coefficient">系数值</param>
        /// <returns></returns>
        public static bool MarginCheck(decimal margin, decimal margindb, decimal coefficient)
        {
            if (coefficient <= 0) return true;
            if ((margindb - margindb * coefficient) <= margin && margin <= (margindb + margindb * coefficient))
                return true;
            return false;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dirPath"></param>
        public static void CreateDirectory(string dirPath)
        {
            if (string.IsNullOrWhiteSpace(dirPath) == false &&
                Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        public static bool HasDoubleByte(string strTemp)
        {
            if (strTemp.IsNullOrWhiteSpace())
            {
                return false;
            }
            for (int i = 0; i < strTemp.Length; i++)
            {
                //获取每个字符
                string sTemp = strTemp.Substring(i, 1);
                //对字符进行格式化
                int iLen = System.Text.Encoding.Default.GetByteCount(sTemp);
                if (iLen == 2)
                {
                    //双字节字符(汉字)
                    return true;
                }
                else
                {
                    //单字节字符
                    //return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 规则声明类
    /// </summary>
    public class RulesClass
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string ResultMsg { get; set; }
        /// <summary>
        /// 判断类型
        /// </summary>
        public ResultTypeEnum ResultType { get; set; }
    }

    /// <summary>
    /// 规则枚举
    /// </summary>
    public enum ResultTypeEnum
    {
        /// <summary>
        /// 数字
        /// </summary>
        Digital = 1,
        /// <summary>
        /// 英文
        /// </summary>
        English = 2,
        /// <summary>
        /// 电话
        /// </summary>
        Phone = 3,
        /// <summary>
        /// 邮箱
        /// </summary>
        EMail = 4,
        /// <summary>
        /// 整数
        /// </summary>
        Integer = 5,
        /// <summary>
        /// 验证非空
        /// </summary>
        Empty = 6,
        /// <summary>
        /// 正数
        /// </summary>
        PositiveDigital = 7,
        /// <summary>
        /// 单字节字符
        /// </summary>
        SingleByte = 8,
        /// <summary>
        /// 小数(不包含逗号)
        /// </summary>
        Decimal = 9
    }

}
