using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers;
using DodgeDynasty.Mappers.Admin;
using DodgeDynasty.Mappers.Commish;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Controllers
{
	public class CommishController : BaseController
	{
		#region Manage Leagues

		[HttpGet]
		[CommishAccess]
		public ActionResult ManageLeagues()
		{
			var mapper = Factory.Create<CommishManageLeaguesMapper<ManageLeaguesModel>>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[CommishLeagueAccess]
		public ActionResult EditLeague(string id)
		{
			var mapper = new EditLeagueMapper<AddEditLeagueModel> { LeagueId = id };
			return View(mapper.GetModel());
		}

		[HttpPost]
		[CommishLeagueAccess]
		public ActionResult EditLeague(AddEditLeagueModel model)
		{
			var mapper = new EditLeagueMapper<AddEditLeagueModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { leagueId = model.LeagueId });
		}

		#endregion

		#region Manage Drafts

		[HttpGet]
		[CommishLeagueAccess]
		public ActionResult ManageDrafts(string id)
		{
			var mapper = MapperFactory.CreateCommishManageDraftsMapper(id);
			return View(mapper.GetModel());
		}

		[HttpGet]
		[CommishLeagueAccess]
		public ActionResult AddDraft(string id)
		{
			var mapper = Factory.Create<AddDraftMapper<AddEditDraftModel>>();
			mapper.LeagueId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[CommishLeagueAccess]
		public ActionResult AddDraft(AddEditDraftModel model)
		{
			var mapper = Factory.Create<AddDraftMapper<AddEditDraftModel>>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}

		[HttpGet]
		[CommishDraftAccess]
		public ActionResult EditDraft(string id)
		{
			var mapper = new EditDraftMapper<AddEditDraftModel>();
			mapper.DraftId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[CommishDraftAccess]
		public ActionResult EditDraft(AddEditDraftModel model)
		{
			var mapper = new EditDraftMapper<AddEditDraftModel>();
			if (!ModelState.IsValid)
			{
				mapper.DraftId = model.DraftId;
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}

		#endregion

		#region Setup Draft

		[HttpGet]
		[CommishDraftAccess]
		public ActionResult SetupDraft(string id)
		{
			DraftSetupModel model = new DraftSetupModel();
			int? draftId = null;
			if (!string.IsNullOrEmpty(id))
			{
				draftId = Int32.Parse(id);
			}
			else
			{
				draftId = DBUtilities.GetCommishCurrentDraft().DraftId;
            }
			model.GetDraftInfo(draftId);
			return View(model);
		}

		[HttpPost]
		[CommishDraftAccess]
		public HttpStatusCode SetupDraft(DraftPicksModel model)
		{
			DraftSetupMapper mapper = DraftFactory.GetDraftSetupMapper();
			mapper.UpdateDraftPicks(model);
			return HttpStatusCode.OK;
		}

		#endregion

		#region Activate Draft

		[HttpGet]
		[CommishAccess]
		public ActionResult ActivateDraft()
		{
			var mapper = new CommishActivateDraftMapper<ActivateDraftModel>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[CommishDraftAccess]
		public ActionResult SetDraftStatus(string id)
		{
			var mapper = new DraftStatusMapper(id,
				Request.QueryString[Constants.QS.IsActive], Request.QueryString[Constants.QS.IsComplete]);
			mapper.UpdateEntity(mapper.Model);
			return RedirectToAction(Constants.Views.ActivateDraft);
		}

		#endregion

		#region Input Draft Pick

		[HttpGet]
		[CommishAccess]
		public ActionResult Input()
		{
			if (ViewData.ContainsKey(Constants.ViewData.NextDraftInputModel))
			{
				return View((DraftInputModel)ViewData[Constants.ViewData.NextDraftInputModel]);
			}
			DraftInputModel draftInputModel = DraftFactory.GetCurrentDraftInputModel(DBUtilities.GetCommishCurrentDraft().DraftId);
			return View(draftInputModel);
		}

		[HttpPost]
		[CommishDraftPickAccess]
		public ActionResult InputPrevious(string draftPickId)
		{
			DraftInputModel previousDraftInputModel = DraftFactory.GetCurrentDraftInputModel(DBUtilities.GetCommishCurrentDraft().DraftId);
			previousDraftInputModel.GetPreviousDraftPick(draftPickId);
			return PartialView(Constants.Views.Input, previousDraftInputModel);
		}

		[HttpPost]
		[CommishDraftPickAccess]
		public ActionResult InputNext(string draftPickId)
		{
			DraftInputModel nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(DBUtilities.GetCommishCurrentDraft().DraftId);
			nextDraftInputModel.GetNextDraftPick(draftPickId);
			return PartialView(Constants.Views.Input, nextDraftInputModel);
		}

		[HttpPost]
		[CommishDraftPickAccess]
		public ActionResult Input(DraftInputModel model)
		{
			return InputDraftPick(Constants.Views.Input, model, true);
		}

		[HttpPost]
		[CommishDraftPickAccess]
		public ActionResult InputDelete(string draftPickId)
		{
			DraftInputModel nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(DBUtilities.GetCommishCurrentDraft().DraftId);
			nextDraftInputModel.DeleteDraftPick(Convert.ToInt32(draftPickId));
			nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(DBUtilities.GetCommishCurrentDraft().DraftId);
			return PartialView(Constants.Views.Input, nextDraftInputModel);
		}

		#endregion
	}
}