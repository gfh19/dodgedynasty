using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Controllers
{
	public class AdminController : BaseController
	{
		[HttpGet]
		[AdminAccess]
		public ActionResult SetupDraft(string id)
		{
			DraftSetupModel model = new DraftSetupModel();
			int? draftId = null;
			if (!string.IsNullOrEmpty(id))
			{
				draftId = Int32.Parse(id);
			}
			model.GetDraftInfo(draftId);
			return View(model);
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult SetupDraft(DraftPicksModel draftPicksModel)
		{
			DraftSetupMapper mapper = DraftFactory.GetDraftSetupMapper();
			mapper.UpdateDraftPicks(draftPicksModel);
			DraftSetupModel draftSetupModel = new DraftSetupModel();
			draftSetupModel.GetDraftInfo();
			return PartialView(draftSetupModel);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult Input()
		{
			DraftInputModel draftInputModel = DraftFactory.GetCurrentDraftInputModel();
			return View(draftInputModel);
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult InputPrevious(string draftPickId)
		{
			DraftInputModel previousDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
			previousDraftInputModel.GetPreviousDraftPick(draftPickId);
			return PartialView(Constants.Views.Input, previousDraftInputModel);
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult InputNext(string draftPickId)
		{
			DraftInputModel nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
			nextDraftInputModel.GetNextDraftPick(draftPickId);
			return PartialView(Constants.Views.Input, nextDraftInputModel);
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult Input(DraftInputModel draftInputModel)
		{
			return InputDraftPick(Constants.Views.Input, draftInputModel, true);
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult InputDelete(string draftPickId)
		{
			DraftInputModel nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
			nextDraftInputModel.DeleteDraftPick(Convert.ToInt32(draftPickId));
			nextDraftInputModel.ResetCurrentDraft();
			nextDraftInputModel.ResetCurrentPickPlayerModel();
			return PartialView(Constants.Views.Input, nextDraftInputModel);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult ManageLeagues()
		{
			var mapper = new ManageLeaguesMapper<ManageLeaguesModel>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult AddLeague()
		{
			var mapper = new AddLeagueMapper<AddLeagueModel>();
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult AddLeague(AddLeagueModel addLeagueModel)
		{
			var mapper = new AddLeagueMapper<AddLeagueModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(addLeagueModel));
			}
			mapper.UpdateEntity(addLeagueModel);
			//return RedirectToAction(Constants.Views.AddDraft, new { id = mapper.LeagueId });
			return Json(new { leagueId = addLeagueModel.LeagueId });
		}



		[HttpPost]
		[AdminAccess]
		public ActionResult ValidateLeague(AddLeagueModel addLeagueModel)
		{
			var mapper = new AddLeagueMapper<AddLeagueModel>();
			return View(mapper.GetModel(addLeagueModel));
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult AddDraft(string id)
		{
			var mapper = new AddDraftMapper<AddDraftModel>();
			mapper.LeagueId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult AddDraft(AddDraftModel model)
		{
			var mapper = new AddDraftMapper<AddDraftModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			//return RedirectToAction(Constants.Views.SetupDraft, new { id = mapper.DraftId });
			return Json(new { draftId = model.DraftId });
		}
	}
}