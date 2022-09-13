using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools.OpenExe.Exts
{
    public static class StringExt
    {
        /// <summary>
        /// 运行cmd 如
        /// fc命令
        /// FC path1 path2
        /// 
        /// XCOPY source [destination] [/Y][/-Y] [/A | /M] [/D[:date]] [/P] [/S [/E]] [/W][/C] [/I] [/Q] [/F] [/L] [/H] [/R] [/T] [/U][/K] [/N]
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string RunCmd(this string m, params string[] pars)
        {
            string tmpparam = string.Empty;

            foreach (var item in pars)
            {
                tmpparam += item + " ";
            }
            string cmd = $"{m} {tmpparam}";
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine(cmd);
            proc.StandardInput.WriteLine("exit");
            string outStr = proc.StandardOutput.ReadToEnd();
            proc.Close();
            return outStr;
        }
        public static async Task<string> RunCmdAsync(this string m, params string[] pars)
        {
            var result = await Task.Factory.StartNew(() =>
            {
                return RunCmd(m, pars);
            });
            return result;
        }
        public static async Task<string> RunCmdListAsync(this string m, List<string> otherCmds, params string[] pars)
        {
            var result = await Task.Factory.StartNew(() =>
              {
                  return RunCmdList(m, otherCmds, pars);
              });
            return result;
        }
        public static string RunCmdList(this string m, List<string> otherCmds, params string[] pars)
        {
            string tmpparam = string.Empty;

            foreach (var item in pars)
            {
                tmpparam += item + " ";
            }
            string cmd = $"{m} {tmpparam}";
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine(cmd);
            foreach (var item in otherCmds)
            {
                proc.StandardInput.WriteLine(item);
            }
            proc.StandardInput.WriteLine("exit");
            string outStr = proc.StandardOutput.ReadToEnd();
            proc.Close();
            return outStr;
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <returns>文件名称</returns>
        public static string DownloadFile(this string url, string fileName)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile(url, fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建文件名称
        /// </summary>
        public static string CreateFileName(this string url)
        {
            return url.Substring(url.LastIndexOf("/") + 1).Trim();
        }

        /// <summary>
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(this string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                var versionmsgMd5 = sb.ToString();
                return versionmsgMd5;
            }
            catch (Exception ex)
            {
                return "GetMD5HashFromFile() fail,error:" + ex.Message;
            }
        }
        public static string GetFileNameWithoutExtension(this string m)
        {
            return System.IO.Path.GetFileNameWithoutExtension(m);
        }
        public static string GetFileNameWithExtension(this string m)
        {
            return System.IO.Path.GetFileName(m);
        }
        public static string GetDirectoryName(this string m)
        {
            return System.IO.Path.GetDirectoryName(m);
        }
        public static FileVersionInfo GetFileVerion(this string filename)
        {
            System.Diagnostics.FileVersionInfo myFileVersionInfo = null;
            try
            {

                myFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return myFileVersionInfo;
        }
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }
        //public static int TryToInt(this string str, int defaultvalue = 0)
        //{
        //    int result = defaultvalue;
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return result;
        //    }
        //    int.TryParse(str, out result);
        //    return result;
        //}
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
        public static bool FileDirectoryExists(this string m)
        {
            try
            {
                if (m.isNull())
                {
                    return false;
                }
                return Directory.Exists(m);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        ///  直接删除指定目录下的所有文件及文件夹(保留目录)
        /// </summary>
        /// <param name="filepath"></param>
        public static void FileDirectoryDelete(this string filepath)
        {
            try
            {
                //判断文件夹是否还存在
                if (!Directory.Exists(filepath))
                {
                    return;
                }
                //去除文件夹和子文件的只读属性
                //去除文件夹的只读属性
                System.IO.DirectoryInfo fileInfo = new DirectoryInfo(filepath);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;

                //去除文件的只读属性
                System.IO.File.SetAttributes(filepath, System.IO.FileAttributes.Normal);

                foreach (string f in Directory.GetFileSystemEntries(filepath))
                {
                    if (File.Exists(f))
                    {
                        try
                        {
                            //如果有子文件删除文件
                            File.Delete(f);
                            //Console.WriteLine(f);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message.ToString());// 异常信息
                        }
                    }
                    else
                    {
                        //循环递归删除子文件夹
                        FileDirectoryDelete(f);
                    }
                }

                //删除空文件夹
                Directory.Delete(filepath);
                //Console.WriteLine(filepath); 

            }
            catch (Exception ex) // 异常处理
            {
                Console.WriteLine(ex.Message.ToString());// 异常信息
            }
        }

        /// <summary>
        /// 文件路径获取内容，（文件）
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string[] FilePathGetContentAllLine(this string m, Encoding encoder)
        {
            var currcontent = new string[0];
            try
            {
                if (!File.Exists(m))
                {
                    return currcontent;
                }
                return File.ReadAllLines(m);
            }
            catch (Exception ex)
            {
                return currcontent;
            }
            return currcontent;
        }


        public static bool ContainsAll(this string m, List<string> checkV)
        {
            if (m.isNull() || checkV == null)
            {
                return false;
            }
            foreach (var item in checkV)
            {
                if (!m.ContainsWithLower(item))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取当前程序集资源
        /// </summary>
        /// <param name="filenameOnly"></param>
        /// <param name="tmpnamespace"></param>
        /// <returns></returns>

        public static Object GetPropertiesResources(this Assembly assembly, string filenameOnly, string filetype = "json")
        {
            var assemblyName = new AssemblyName(assembly.FullName);
            string resourceName = assemblyName.Name + "" + ".Properties.Resources";
            ResourceManager resourceManager = new ResourceManager(resourceName, assembly);//根据资源名获得资源对象
            return resourceManager.GetObject(filenameOnly);
        }
        /// <summary>
        /// 获取流String
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string StreamReadString(this Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            using (StreamReader streamReader = new StreamReader(stream, encoding))
            {
                return streamReader.ReadToEnd();
            };
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool SaveByPath(this Stream stream, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                stream.Position = 0;
                sw.Write(stream);
                sw.Flush();
            }
            return true;
            ;
        }

    }
}
