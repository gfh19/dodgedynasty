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
using DodgeDynasty.Mappers.Drafts;

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
			DraftInputModel draftInputModel = DraftFactory.GetCurrentDraftInputModel(id.ToNullInt());
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
			DraftDisplayModel draftDisplayModel = DraftFactory.GetDraftDisplayModel(id.ToNullInt());
			return View(draftDisplayModel);
		}

		public ActionResult DisplayPartial()
		{
			DraftDisplayModel draftDisplayModel = DraftFactory.GetDraftDisplayModel();
			return PartialView(Constants.Views.Display, draftDisplayModel);
		}

		public ActionResult TeamDisplay(string id)
		{
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel(id.ToNullInt());
			return View(draftTeamDisplayModel);
		}

		public ActionResult TeamDisplayPartial(string id)
		{
			var byPositions = Request.QueryString[Constants.QS.ByPositions];
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel(id.ToNullInt(), Utilities.ToBool(byPositions));
			return PartialView(Constants.Views.TeamDisplay, draftTeamDisplayModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailable(string rankId, string id)
		{
//TODO:  Check CompareRankIds owner rank access/valid ints; clear cookie values if not

			var helper = PlayerRankUIHelper.Instance;
			var playerRankModel = helper.GetPlayerRankPartial(rankId, true, Request, Response);
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailablePartial(string rankId)
		{
//TODO:  Check CompareRankIds owner rank access/valid ints; clear cookie values if not

			var helper = PlayerRankUIHelper.Instance;
			var playerRankModel = helper.GetPlayerRankPartial(rankId, true, Request, Response);
			return PartialView(Constants.Views.BestAvailable, playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanks(string rankId, string id)
		{
//TODO:  Check CompareRankIds owner rank access/valid ints; clear cookie values if not

			var helper = PlayerRankUIHelper.Instance;
			var playerRankModel = helper.GetPlayerRankPartial(rankId, false, Request, Response);
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanksPartial(string rankId)
		{
//TODO:  Check CompareRankIds owner rank access/valid ints; clear cookie values if not

			var helper = PlayerRankUIHelper.Instance;
			var playerRankModel = helper.GetPlayerRankPartial(rankId, false, Request, Response);
			return PartialView(Constants.Views.PlayerRanks, playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult HighlightQueuePartial(bool isBestAvailable)
		{
			var helper = PlayerRankUIHelper.Instance;
			var options = helper.GetPlayerRankOptions(Request, Response);
			PlayerRankModel playerRankModel = helper.DetermineRankModel(null, null, options, Response);
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

		[HttpPost]
		[OwnerRankAccess]
		public ActionResult UpdateCompareRankSelects(string compRankIds, bool isBestAvailable)
		{
//TODO:  Check CompareRankIds owner rank access/valid ints; clear cookie values if not
			var helper = PlayerRankUIHelper.Instance;
			var options = helper.GetPlayerRankOptions(Request, Response);
			options.CompareRankIds = compRankIds;
			helper.UpdatePlayerRankOptions(options, Response);
			var playerRankModel = helper.GetPlayerRankPartial(null, isBestAvailable, Request, Response);
			return PartialView(isBestAvailable ? Constants.Views.BestAvailable : Constants.Views.PlayerRanks, playerRankModel);
		}

		[HttpGet]
		public ActionResult RankingsList(string id)
		{
			var helper = PlayerRankUIHelper.Instance;
			RankingsListModel model = DraftFactory.GetRankingsListModel(id.ToNullInt());
			model.Options = helper.GetPlayerRankOptions(Request, Response);
			return View(model);
		}

		[HttpGet]
		public ActionResult RankingsListPartial()
		{
			var helper = PlayerRankUIHelper.Instance;
			RankingsListModel model = DraftFactory.GetRankingsListModel();
			model.Options = helper.GetPlayerRankOptions(Request, Response);
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

		[HttpGet]
		public JsonResult GetLastDraftPickAudio()
		{
			var mapper = new DraftPickAudioMapper();
			return Json(mapper.GetModel(), JsonRequestBehavior.AllowGet);
		}
	}
}
