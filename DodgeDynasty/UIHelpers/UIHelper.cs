using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DodgeDynasty.UIHelpers
{
	public static class UIHelper
	{
		public static string FullPath(this UrlHelper urlHelper, string content)
		{
			var path = urlHelper.Content(content);
			var url = new Uri(HttpContext.Current.Request.Url, path);
			return url.AbsoluteUri;
		}
	}
}
