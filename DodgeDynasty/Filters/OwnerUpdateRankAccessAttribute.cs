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
	public class OwnerUpdateRankAccessAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			bool validate = false;
			int rankId = 0;
			if (filterContext.ActionParameters.ContainsKey("rankId"))
			{
				if (filterContext.ActionParameters["rankId"] != null)
				{
					validate = true;
					if (filterContext.ActionParameters["rankId"] is string)
					{
						rankId = Convert.ToInt32((string)filterContext.ActionParameters["rankId"]);
					}
					else
					{
						rankId = (int)filterContext.ActionParameters["rankId"];
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
				if (!accessModel.CanUserAccessUpdateRank(rankId))
				{
					filterContext.Result = Utilities.GetUnauthorizedRedirect();
				}
			}
		}
	}
}