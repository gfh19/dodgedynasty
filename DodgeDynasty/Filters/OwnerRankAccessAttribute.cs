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
	public class OwnerRankAccessAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			bool validate = false;
			int rankId = 0;
			if (filterContext.ActionParameters.ContainsKey("id"))
			{
				if (filterContext.ActionParameters["id"] != null)
				{
					validate = true;
					if (filterContext.ActionParameters["id"] is string)
					{
						rankId = Convert.ToInt32((string)filterContext.ActionParameters["id"]);
					}
					else
					{
						rankId = (int)filterContext.ActionParameters["id"];
					}
				}
			}
			else if (filterContext.ActionParameters.ContainsKey("model"))
			{
				var rankIdModel = filterContext.ActionParameters["model"] as RankIdModel;
				if (rankIdModel != null)
				{
					validate = true;
					rankId = rankIdModel.RankId;
				}
			}
			if (validate)
			{
				AccessModel accessModel = new AccessModel();
				if (!accessModel.CanUserAccessRank(rankId))
				{
					filterContext.Result = new RedirectToRouteResult(
						new RouteValueDictionary(new { action = "Unauthorized", controller = "Shared" }));
				}
			}
		}
	}
}