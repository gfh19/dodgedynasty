﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Models;
using DodgeDynasty.Mappers;
using DodgeDynasty.Shared;
using DodgeDynasty.Filters;
using DodgeDynasty.Shared.Exceptions;
using DodgeDynasty.Models.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DodgeDynasty.UIHelpers;

namespace DodgeDynasty.Controllers
{
	public class DraftController : BaseController
	{
		[HttpGet]
		public ActionResult Pick(string playerId, string id)
		{
			if (TempData.ContainsKey(Constants.TempData.NextDraftInputModel))
			{
				return View((DraftInputModel)TempData[Constants.TempData.NextDraftInputModel]);
			}
			DraftInputModel draftInputModel = DraftFactory.GetCurrentDraftInputModel(Utilities.ToNullInt(id));
			if (!string.IsNullOrEmpty(playerId))
			{
				int pickPlayerId = Convert.ToInt32(playerId);
				draftInputModel.PreloadPlayerModel(pickPlayerId);
			}
			return View(draftInputModel);
		}

		[HttpGet]
		public ActionResult PickPartial()
		{
			DraftInputModel draftInputModel = DraftFactory.GetCurrentDraftInputModel();
			return PartialView(Constants.Views.Pick, draftInputModel);
		}

		[HttpPost]
		public ActionResult Pick(DraftInputModel draftInputModel)
		{
			return InputDraftPick(Constants.Views.Pick, draftInputModel, false);
		}

		public ActionResult Display(string id)
		{
			DraftDisplayModel draftDisplayModel = DraftFactory.GetDraftDisplayModel(Utilities.ToNullInt(id));
			return View(draftDisplayModel);
		}

		public ActionResult DisplayPartial()
		{
			DraftDisplayModel draftDisplayModel = DraftFactory.GetDraftDisplayModel();
			return PartialView(Constants.Views.Display, draftDisplayModel);
		}

		public ActionResult TeamDisplay(string id)
		{
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel(Utilities.ToNullInt(id));
			return View(draftTeamDisplayModel);
		}

		public ActionResult TeamDisplayPartial(string id)
		{
			var byPositions = Request.QueryString[Constants.QS.ByPositions];
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel(Utilities.ToNullInt(id), Utilities.ToBool(byPositions));
			return PartialView(Constants.Views.TeamDisplay, draftTeamDisplayModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailable(string rankId, string id)
		{
			var options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			PlayerRankModel playerRankModel = PlayerRankHelper.Instance.DetermineRankModel(rankId, id, options, Response);
			playerRankModel.Options = options;
			playerRankModel.GetBestAvailPlayerRanks();
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailablePartial(string rankId)
		{
			var options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			PlayerRankModel playerRankModel = PlayerRankHelper.Instance.DetermineRankModel(rankId, null, options, Response, false);
			playerRankModel.Options = options;
			playerRankModel.GetBestAvailPlayerRanks();
			return PartialView(Constants.Views.BestAvailable, playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanks(string rankId, string id)
		{
			var options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			PlayerRankModel playerRankModel = PlayerRankHelper.Instance.DetermineRankModel(rankId, id, options, Response);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanksPartial(string rankId)
		{
			var options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			PlayerRankModel playerRankModel = PlayerRankHelper.Instance.DetermineRankModel(rankId, null, options, Response, false);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return PartialView(Constants.Views.PlayerRanks, playerRankModel);
		}

		[HttpGet]
		public ActionResult HighlightQueuePartial(bool isBestAvailable)
		{
			var options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			PlayerRankModel playerRankModel = PlayerRankHelper.Instance.DetermineRankModel(null, null, options, Response);
			playerRankModel.Options = options;
			if (isBestAvailable)
			{
				playerRankModel.GetBestAvailPlayerRanks();
			}
			else
			{
				playerRankModel.GetAllPlayerRanksByPosition();
			}
			return PartialView(Constants.Views.HighlightQueuePartial, playerRankModel);
		}

		[HttpGet]
		public ActionResult RankingsList(string id)
		{
			RankingsListModel model = DraftFactory.GetRankingsListModel(Utilities.ToNullInt(id));
			model.Options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			return View(model);
		}

		[HttpGet]
		public ActionResult RankingsListPartial()
		{
			RankingsListModel model = DraftFactory.GetRankingsListModel();
			model.Options = PlayerRankHelper.Instance.GetPlayerRankOptions(Request, Response);
			return PartialView(Constants.Views.RankingsList, model);
		}

		[HttpGet]
		public ActionResult CurrentDraftPickPartial()
		{
			DraftModel model = new DraftModel();
			model.GetCurrentDraft();
			return PartialView(Constants.Views.CurrentDraftPickPartial, model);
		}

		[HttpGet]
		public ActionResult History()
		{
			var mapper = new DraftHistoryMapper<DraftHistoryModel>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		public JsonResult GetUserTurnPickInfo()
		{
			DraftDisplayModel model = DraftFactory.GetDraftDisplayModel();
			PickInfoJson pickInfo = new PickInfoJson
			{
				turn = model.IsDraftActive() && model.IsUserTurn(),
				num = (model.CurrentDraftPick != null) ? model.CurrentDraftPick.PickNum : 0,
				hasPrev = (model.PreviousDraftPick != null),
				prevName = (model.PreviousDraftPick != null) ? model.PreviousDraftPick.Player.PlayerName : null
			};
			return Json(pickInfo, JsonRequestBehavior.AllowGet);
		}
	}
}
