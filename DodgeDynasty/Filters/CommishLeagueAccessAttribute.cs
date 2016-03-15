using System.Web.Mvc;
using DodgeDynasty.Models.Admin;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Filters
{
	public class CommishLeagueAccessAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			int? leagueId = Utilities.CheckActionParameterId(filterContext, "leagueId", "id");
			if (leagueId != null)
			{
				if (!DBUtilities.IsUserAdminOrCommish(leagueId))
				{
					filterContext.Result = Utilities.GetUnauthorizedRedirect();
				}
			}
			else
			{
				var model = Utilities.CheckActionParameterModel<IAdminLeagueModel>(filterContext, "model");
				if (model != null)
				{
					if (!DBUtilities.IsUserAdminOrCommish(model.LeagueId))
					{
						filterContext.Result = Utilities.GetUnauthorizedRedirect();
					}
				}
				else if (!DBUtilities.IsUserAdminOrCommish())
				{
					filterContext.Result = Utilities.GetUnauthorizedRedirect();
				}
			}
		}
	}
}