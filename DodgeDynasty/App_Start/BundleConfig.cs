﻿using System.Web;
using System.Web.Optimization;

namespace DodgeDynasty
{
	public class BundleConfig
	{
		// For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/lib/jquery-{version}.js",
						"~/Scripts/lib/bootstrap/js/bootstrap.js")); //jquery MUST come before bootstrap, & before jquery-ui

			bundles.Add(new ScriptBundle("~/bundles/lib").Include(
						"~/Scripts/lib/jquery-{version}.js",
						"~/Scripts/lib/jquery.cookie.js",
						"~/Scripts/lib/jquery.validate*",
						"~/Scripts/lib/jquery.signalR-{version}.js",
						"~/Scripts/lib/jquery-ui-{version}.js", //jquery-ui MUST come after bootstrap
						"~/Scripts/lib/moment.js",
						"~/Scripts/lib/moment.min.js",
						"~/Scripts/lib/jQuery-addTouch.js"
						));

			bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

			bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
						"~/Content/themes/base/jquery-ui-1.10.4.custom.css"));
		}
	}
}