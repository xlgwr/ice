using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tools.OpenExe.Exts;

namespace Tools.OpenExe
{
    public partial class FileGet
    {
        public static void OpenFolderAndSelectFile(String fileFullName)
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + fileFullName;
            System.Diagnostics.Process.Start(psi);
        }
        public static void OpenFolder(string v_OpenFolderPath)
        {
            System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
        }
        /// <summary>
        /// 最多50个exe,防爆
        /// </summary>
        private const int maxFile = 500;

        public static List<FileInfo> allGetFiles = new List<FileInfo>();

        /// <summary>
        /// 获得目录下所有文件或指定文件类型文件(包含所有子文件夹)
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extName">扩展名可以多个 例如 .exe</param>
        /// <returns>List<FileInfo></returns>
        public static void getFile(string path, string extName, List<string> notName, bool allFiles = false)
        {
            try
            {
                if (!path.FileDirectoryExists())
                {
                    return;
                }

                if (extName.IsNullOrWhiteSpace())
                {
                    allFiles = true;
                }

                string[] dir = Directory.GetDirectories(path); //文件夹列表   
                DirectoryInfo fdir = new DirectoryInfo(path);
                FileInfo[] file = fdir.GetFiles();
                //FileInfo[] file = Directory.GetFiles(path); //文件列表   
                if (file.Length != 0 || dir.Length != 0) //当前目录文件或文件夹不为空                   
                {
                    foreach (FileInfo f in file) //显示当前目录所有文件   
                    {
                        if (notName.ContainsReAll(f.Name))
                        {
                            continue;
                        }
                        if (allFiles)
                        {
                            allGetFiles.Add(f);
                            if (allGetFiles.Count >= maxFile)
                            {
                                return;
                            }
                            continue;
                        }
                        if (extName.StartsWith("."))
                        {
                            if (string.IsNullOrEmpty(f.Extension))
                            {
                                continue;
                            }
                            if (extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0)
                            {
                                allGetFiles.Add(f);
                                if (allGetFiles.Count >= maxFile)
                                {
                                    return;
                                }
                            }

                        }
                        else
                        {
                            if (extName.ToLower() == f.Name.ToLower())
                            {
                                allGetFiles.Add(f);
                                if (allGetFiles.Count >= maxFile)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    foreach (string d in dir)
                    {
                        getFile(d, extName, notName, allFiles);//递归   
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task getFileAsync(string path, string extName, List<string> notName, bool allFiles = false)
        {
            await Task.Factory.StartNew(() =>
            {
                getFile(path, extName, notName, allFiles);
            });
        }
    }

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
        /// 写dll文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public static void StreamToFile(this Stream stream, string fileName)
        {
            if (fileName.FileExists())
            {
                return;
            }
            // 把 Stream 转换成 byte[]
            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件
            FileStream fs = new FileStream(fileName, FileMode.Create);

            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);

            bw.Close();
            fs.Close();
        }
    }
    public class CheckBoxFlag : CommEntity
    {
        public bool IsDelLog { get; set; } = false;
        public Visibility IsShowDelLog { get; set; } = Visibility.Collapsed;
        public bool IsChecked { get; set; } = true;
        public string Content { get; set; }
        public string Name { get; set; }

        public string Fileversion { get; set; }
        public string VersionmsgMd5 { get; set; }
        public string VersionmsgMd5Short
        {
            get
            {
                try
                {
                    return $"{VersionmsgMd5.Substring(VersionmsgMd5.Length - 5)}";
                }
                catch (Exception)
                {

                    return VersionmsgMd5;
                }
            }
        }
    }
    public class OpenExeConfig
    {
        public string configPath { get; set; }
        public List<string> configPaths { get; set; }
        public string toolconfigFilter { get; set; }
        public string toolconfigGetName { get; set; }
        public string dellogpath { get; set; }
        public string txtfixurl { get; set; }
        public bool CloseToHide { get; set; } = false;
        public bool ReOpenNotice { get; set; } = false;
        public string IceServerIp { get; set; } = "0.0.0.0";
        public string IceServerPort { get; set; } = "10089";
        public string IceClientIp { get; set; } = "127.0.0.1";
        public string IceClientPort { get; set; } = "10089";
        public string IceClientId { get; set; } = "7777";
        public string IceClientType { get; set; } = "3";
        public string IceClientFilterNo { get; set; } = "920003,921001";
        public List<string> copyPathList { get; set; }
    }
}
