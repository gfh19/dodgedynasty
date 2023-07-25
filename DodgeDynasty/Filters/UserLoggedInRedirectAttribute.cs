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
	public class UserLoggedInRedirectAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (Utilities.IsUserLoggedIn())
			{
				string returnUrl = "";
				if (filterContext.ActionParameters.ContainsKey("returnUrl") 
					&& !string.IsNullOrEmpty(filterContext.ActionParameters["returnUrl"]?.ToString()))
				{
					returnUrl = filterContext.ActionParameters["returnUrl"]?.ToString();
				}
				filterContext.Result = Utilities.GetHomeRedirect(returnUrl);
			}
		}
	}
}