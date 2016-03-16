using System.Web.Mvc;
using DodgeDynasty.Mappers.Commish;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Admin;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Filters
{
	public class CommishDraftPickAccessAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (DBUtilities.IsUserAdminOrCommish())
			{
				int? draftPickId = Utilities.CheckActionParameterId(filterContext, "draftPickId", "id");
				if (draftPickId != null)
				{
					var mapper = Factory.Create<CommishCurrentDraftMapper>();
					var currentDraftId = mapper.GetModel().DraftId;
					if (!DBUtilities.IsDraftPickInDraft(draftPickId.Value, currentDraftId))
					{
						filterContext.Result = Utilities.GetUnauthorizedRedirect();
					}
				}
				else
				{
					var model = Utilities.CheckActionParameterModel<DraftInputModel>(filterContext, "model", "draftInputModel");
                    if (model != null && model.Player != null)
					{
						var currentDraftId = DBUtilities.GetCommishCurrentDraft().DraftId;
						if (model.DraftId != currentDraftId ||
							!DBUtilities.IsDraftPickInDraft(model.Player.DraftPickId, currentDraftId))
						{
							filterContext.Result = Utilities.GetUnauthorizedRedirect();
						}
					}
				}
			}
			else
			{
				filterContext.Result = Utilities.GetUnauthorizedRedirect();
			}
		}
	}
}