using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Tools.OpenExe
{
    public class VersionCheckMd5
    {
        public string name { get; set; }
        public string version { get; set; }
        public string md5 { get; set; }
        public string md5All { get; set; } = "";
        public string remark { get; set; }

        public VersionCheckMd5 loadLine(string content)
        {
            var arrList = content.Split(',');
            if (arrList.Length > 4)
            {
                name = arrList[0];
                version = arrList[1];
                md5 = arrList[2];
                md5All = arrList[3];
                remark = arrList[4];
            }
            return this;
        }
    }
    public class VersionCheckMd5Check : VersionCheckMd5
    {
        public StackPanel StackPanel { get; set; }
    }
    public class VersionCheckMd5LstCheck : List<VersionCheckMd5Check>
    {
        public Dictionary<string, VersionCheckMd5Check> cacheDic = new Dictionary<string, VersionCheckMd5Check>();
        public void loadLine(string content, StackPanel stackPanel)
        {
            var curr = new VersionCheckMd5Check();
            curr.loadLine(content);
            curr.StackPanel = stackPanel;
            cacheDic.TryAdd(curr.md5, curr);
            this.Add(curr);
        }
    }
    public class VersionCheckMd5Lst : List<VersionCheckMd5>
    {
        public Dictionary<string, VersionCheckMd5> cacheDic = new Dictionary<string, VersionCheckMd5>();
        public void loadLine(string content)
        {
            var curr = new VersionCheckMd5();
            curr.loadLine(content);
            cacheDic.TryAdd(curr.md5, curr);
            this.Add(curr);
        }
        public void loadLine(string[] content, int skip = 1)
        {
            var countskip = skip;
            foreach (var item in content)
            {
                if (countskip > 0)
                {
                    countskip--;
                    continue;
                }
                var curr = new VersionCheckMd5();
                curr.loadLine(item);
                cacheDic.TryAdd(curr.md5, curr);
                this.Add(curr);
            }
        }
    }
}
