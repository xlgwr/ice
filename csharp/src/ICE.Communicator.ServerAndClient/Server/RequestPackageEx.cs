using Solomon.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlmCommunicator.Server
{
	public class RequestPackageEx
	{
		private RequestPackage _package;

		public RequestPackage PackageData
		{
			get { return _package; }
			set { _package = value; }
		}

		public int ServiceNo
		{
			get { return _package.ServiceNo; }
			set { _package.ServiceNo = value; }
		}

		public string ClientId
		{
			get { return _package.ClientId; }
		}

		public RequestPackageEx(RequestPackage package)
		{
			_package = package;
		}

		public RequestPackageEx()
		{

		}


		public string GetValueByName(string name)
		{
			if (_package == null)
			{
				return string.Empty;
			}
			for (int i = 0; i < _package.Head.Length; i++)
			{
				if (_package.Head[i].Equals(name.ToLower()))
				{
					return _package.DataBody[i];
				}
			}
			return string.Empty;
		}
		/// <summary>
		/// 获取表头分页信息
		/// </summary>
		/// <returns></returns>
		public Solomon.Logger.PagingInfo GetPaging()
		{
			var pageHeadstr = string.Empty;
			try
			{
				if (_package.Head.Length - _package.DataBody.Length != 1)
				{
					return null;
				}
				pageHeadstr = _package.Head.TryGetArrayByIndex(_package.Head.Length - 1);
				LogHelper.CoreJsonLog($"Send:{ServiceNo}:{pageHeadstr}");
				if (pageHeadstr.Contains("pSize") && pageHeadstr.Contains("pIndex"))
				{
					return pageHeadstr.JsonTo<Solomon.Logger.PagingInfo>();
				}
				return null;
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex, pageHeadstr);
				return null;
			}
		}
	}

}
