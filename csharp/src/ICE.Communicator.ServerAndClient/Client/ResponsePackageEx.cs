using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmCommunicator.Client
{
    public class ResponsePackageEx
    {
        private ResponsePackage _package;

        public ResponsePackage PackageData
        {
            get { return _package; }
        }

        public int ErrorNo
        {
            get { return _package.ErrorNo; }
        }

        public string ErrorInfo
        {
            get { return _package.ErrorInfo; }
        }

        public string ClientId
        {
            get { return _package.ClientId; }
        }

        public int ServiceNo
        {
            get { return _package.ServiceNo; }
        }

        public int Rows
        {
            get { return _package.DataBody.Length; }
        }

        public Dictionary<string, int> HeadDic { private set; get; }

        public ResponsePackageEx(ResponsePackage package)
        {
            _package = package;
        }

        public string GetValueByName(int row, string name)
        {
            if (HeadDic == null)
            {
                SetHeadDic();
            }

            var key = name.ToLower();
            if (HeadDic.ContainsKey(key))
            {
                var index = HeadDic[key];
                return _package.DataBody[row][index];
            }
            else
            {
                for (int i = 0; i < _package.Head.Length; i++)
                {
                    if (_package.Head[i].Equals(key))
                    {
                        return _package.DataBody[row][i];
                    }
                }
                return string.Empty;
            }
        }

        private void SetHeadDic()
        {
            HeadDic = new Dictionary<string, int>();
            for (int i = 0; i < _package.Head.Length; i++)
            {
                HeadDic[_package.Head[i]] = i;
            }
        }


    }
}
