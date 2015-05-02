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
		public ActionResult ManageDrafts(string id)
		{
			var mapper = new ManageDraftsMapper<ManageDraftsModel> { LeagueId = id };
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult AddLeague()
		{
			var mapper = new AddLeagueMapper<AddEditLeagueModel>();
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult AddLeague(AddEditLeagueModel model)
		{
			var mapper = new AddLeagueMapper<AddEditLeagueModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { leagueId = model.LeagueId });
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult EditLeague(string id)
		{
			var mapper = new EditLeagueMapper<AddEditLeagueModel> { LeagueId = id };
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
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

		[HttpGet]
		[AdminAccess]
		public ActionResult AddDraft(string id)
		{
			var mapper = new AddDraftMapper<AddEditDraftModel>();
			mapper.LeagueId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult AddDraft(AddEditDraftModel model)
		{
			var mapper = new AddDraftMapper<AddEditDraftModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult EditDraft(string id)
		{
			var mapper = new EditDraftMapper<AddEditDraftModel>();
			mapper.DraftId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult EditDraft(AddEditDraftModel model)
		{
			var mapper = new EditDraftMapper<AddEditDraftModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult ActivateDraft()
		{
			var mapper = new ActivateDraftMapper<ActivateDraftModel>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult SetDraftStatus(string id)
		{
			var mapper = new DraftStatusMapper<DraftStatusModel>(id, 
				Request.QueryString[Constants.QS.IsActive], Request.QueryString[Constants.QS.IsComplete]);
			mapper.UpdateEntity(mapper.Model);
			return RedirectToAction(Constants.Views.ActivateDraft);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult ChangePassword(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "The password has been changed."
				: "";
			ViewBag.ReturnUrl = Url.Action("ChangePassword");
			var mapper = new AdminPasswordMapper();
			return View(mapper.GetModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePassword(AdminPasswordModel model)
		{
			ViewBag.ReturnUrl = Url.Action("ChangePassword");
			var mapper = new AdminPasswordMapper();
			if (ModelState.IsValid)
			{
				bool changePasswordSucceeded;
				try
				{
					mapper.UpdateEntity(model);
					changePasswordSucceeded = mapper.UpdateSucceeded;
				}
				catch (Exception)
				{
					changePasswordSucceeded = false;
				}

				if (changePasswordSucceeded)
				{
					return RedirectToAction("ChangePassword", new { Message = ManageMessageId.ChangePasswordSuccess });
				}
				else
				{
					ModelState.AddModelError("", "The password was not changed - user was not found.");
					return View(mapper.GetModel());
				}
			}

			// If we got this far, something failed, redisplay form
			return View(mapper.GetModel());
		}
	}
}