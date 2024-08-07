﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers;
using DodgeDynasty.Mappers.Admin;
using DodgeDynasty.Mappers.PlayerAdjustments;
using DodgeDynasty.Mappers.RankAdjustments;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Models.Admin;
using DodgeDynasty.Models.PlayerAdjustments;
using DodgeDynasty.Models.RankAdjustments;
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
			DraftSetupModel model = new DraftSetupModel();
			model.GetDraftInfo(draftPicksModel.DraftId);
			return PartialView(Constants.Views.SetupDraftPartial, model);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult Input()
		{
			if (ViewData.ContainsKey(Constants.ViewData.NextDraftInputModel))
			{
				return View((DraftInputModel)ViewData[Constants.ViewData.NextDraftInputModel]);
			}
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
			var mapper = Factory.Create<AdminManageLeaguesMapper<ManageLeaguesModel>>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult ManageDrafts(string id)
		{
			var mapper = new AdminManageDraftsMapper<ManageDraftsModel> { LeagueId = id };
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
				mapper.DraftId = model.DraftId;
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult ActivateDraft()
		{
			var mapper = new AdminActivateDraftMapper<ActivateDraftModel>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult SetDraftStatus(string id)
		{
			var mapper = new DraftStatusMapper(id, 
				Request.QueryString[Constants.QS.IsActive], 
				Request.QueryString[Constants.QS.IsComplete],
				Request.QueryString[Constants.QS.IsPaused]);
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
					if (mapper.PasswordStatus == ManageMessageId.ConfirmPasswordMismatch)
					{
						ModelState.AddModelError("", "New Password and Confirm Password do not match.");
					}
					else if (mapper.PasswordStatus == ManageMessageId.CurrentPasswordInvalid)
					{
						ModelState.AddModelError("CurrentPassword", "Current Password is incorrect.");
					}
					else
					{
						ModelState.AddModelError("", "The password was not changed - user was not found.");
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult UserInfo(string userName)
		{
			var mapper = new UserInfoMapper { UserName = userName };
			var model = mapper.GetModel();
			return View(model);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult UserInfoPartial(string userName)
		{
			var mapper = new UserInfoMapper { UserName = userName };
			var model = mapper.GetModel();
			return PartialView(Constants.Views.UserInfoPartial, model);
		}

		[HttpPost]
		public ActionResult UserInfo(UserInfoModel model)
		{
			var mapper = new UserInfoMapper();
			mapper.ModelState = ModelState;
			if (!mapper.UpdateEntity(model))
			{
				return View(mapper.GetUpdatedModel(model));
			}
			return View(mapper.GetModel());
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult ManageUsers()
		{
			var mapper = new ManageUsersMapper();
			var model = mapper.GetModel();
			return View(model);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult AddUser()
		{
			var mapper = new AddUserMapper();
			var model = mapper.GetModel();
			return View(model);
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult AddUser(AddUserModel model)
		{
			var mapper = new AddUserMapper { ModelState = ModelState };
			if (!mapper.UpdateEntity(model))
			{
				return View(mapper.GetUpdatedModel(model));
			}
			return RedirectToAction(Constants.Views.ManageUsers);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult SetUserStatus(string id)
		{
			var mapper = new UserStatusMapper(id, Request.QueryString[Constants.QS.IsActive]);
			mapper.UpdateEntity(mapper.Model);
			return RedirectToAction(Constants.Views.ManageUsers);
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult PlayerAdjustments()
		{
			var mapper = new GetPlayerAdjustmentsMapper();
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public JsonResult AddNewPlayer(AdminPlayerModel player)
		{
			var mapper = new AdminAddPlayerMapper();
			mapper.UpdateEntity(player);
            return Json(new { status="" });
		}

		[HttpPost]
		[AdminAccess]
		public JsonResult EditPlayer(AdminPlayerModel player)
		{
			var mapper = new AdminEditPlayerMapper();
			mapper.UpdateEntity(player);
			return Json(new { status = "" });
		}

		[HttpPost]
		[AdminAccess]
		public JsonResult SetPlayerStatus(AdminPlayerModel player)
		{
			var mapper = new AdminEditPlayerMapper();
			mapper.UpdateEntity(player);
			return Json(new { status = "" });
		}

		[HttpPost]
		[AdminAccess]
		public JsonResult InactivatePlayers(string playerGroup)
		{
			var mapper = new InactivatePlayersMapper();
			mapper.UpdateEntity(new InactivatePlayersModel { PlayerGroup = playerGroup });
			return Json(new { status = "" });
		}

		[HttpGet]
		[AdminAccess]
		public ActionResult RankAdjustments()
		{
			var mapper = new GetRankAdjustmentsMapper();
			return View(mapper.GetModel());
		}

		[HttpPost]
		[AdminAccess]
		public JsonResult AddNewRank(AdminRankModel rank)
		{
			var mapper = new AdminAddRankMapper();
			mapper.UpdateEntity(rank);
			return Json(new { status = "" });
		}

		[HttpPost]
		[AdminAccess]
		public JsonResult EditRank(AdminRankModel rank)
		{
			var mapper = new AdminEditRankMapper();
			mapper.UpdateEntity(rank);
			return Json(new { status = "" });
		}

		[HttpPost]
		[AdminAccess]
		public ActionResult AutoImportRank(string rankId, bool confirmed, int? maxCount)
		{
			var mapper = new ImportRankMapper(rankId, confirmed, maxCount);
			var model = mapper.GetModel();
			return Json(new
			{
				error = model.ErrorMessage,
				stack = model.StackTrace,
				first = model.FirstPlayerText,
				last = model.LastPlayerText,
				count = model.PlayerCount,
				max = model.MaxPlayerCount,
				blklstpos = string.Join(",", model.BlacklistPosFound.Distinct()),
				unkpos = string.Join(",", model.UnknownPosFound.Distinct()),
				blklstposcnt = string.Join(",", model.BlacklistPosFound.Count()),
				unkposcnt = string.Join(",", model.UnknownPosFound.Count()),
			});
		}
	}
}