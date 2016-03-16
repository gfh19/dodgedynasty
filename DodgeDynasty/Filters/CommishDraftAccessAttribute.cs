using System.Web.Mvc;
using DodgeDynasty.Models.Admin;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Filters
{
	public class CommishDraftAccessAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			int? draftId = Utilities.CheckActionParameterId(filterContext, "draftId", "id");
			if (draftId != null)
			{
				if (!DBUtilities.IsUserAdminOrCommishForDraft(draftId))
				{
					filterContext.Result = Utilities.GetUnauthorizedRedirect();
				}
			}
			else
			{
				var model = Utilities.CheckActionParameterModel<IDraftIdModel>(filterContext, "model");
				if (model != null)
				{
					if (!DBUtilities.IsUserAdminOrCommishForDraft(model.DraftId))
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