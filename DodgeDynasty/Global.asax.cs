using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;
using DodgeDynasty.Shared.Log;

namespace DodgeDynasty
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
		}

		protected void Application_Error()
		{
			var ex = Server.GetLastError() ?? new Exception("Application_Error handler reached.");
			HttpContext httpContext = HttpContext.Current;
			if (httpContext != null)
			{
				var requestUrl = httpContext.Request.Url.AbsoluteUri;
				var userName = Utilities.GetLoggedInUserName();
				var currentDraft = Factory.Create<SingleDraftMapper>().GetModel();
				Logger.LogError(ex, requestUrl, userName, currentDraft.DraftId);
			}
			else
			{
				Logger.LogError(ex);
			}
		}
	}
}