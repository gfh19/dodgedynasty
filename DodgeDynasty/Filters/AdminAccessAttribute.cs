using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Models;
using System.Web.Routing;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Filters
{
	public class AdminAccessAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (!Utilities.IsUserAdmin())
			{
				filterContext.Result = new RedirectToRouteResult(
					new RouteValueDictionary(new { action = "Unauthorized", controller = "Shared" }));
			}
		}
	}
}