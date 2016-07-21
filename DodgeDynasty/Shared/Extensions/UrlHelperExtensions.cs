using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DodgeDynasty.Shared.Extensions
{
	//TODO:  Combine with DodgeDynasty.Helpers.UIHelper
	public static class UrlHelperExtensions
	{
		private readonly static string _majorVersion = ConfigurationManager.AppSettings[Constants.AppSettings.MajorVersion];
		private readonly static string _jsVersion = ConfigurationManager.AppSettings[Constants.AppSettings.JSVersion];

		public static string Script(this UrlHelper helper, string fileName)
		{
			return helper.Content(string.Format("{0}?v{1}.{2}", fileName, _majorVersion, _jsVersion));
		}
	}
}