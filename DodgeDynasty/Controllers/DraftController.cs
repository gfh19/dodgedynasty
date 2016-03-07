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
			var options = GetPlayerRankOptions();
			PlayerRankModel playerRankModel = DetermineRankModel(rankId, id, options);
			playerRankModel.Options = options;
			playerRankModel.GetBestAvailPlayerRanks();
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailablePartial(string rankId)
		{
			//TBD
			var options = GetPlayerRankOptions();
			PlayerRankModel playerRankModel = DetermineRankModel(rankId, null, options, false);
			playerRankModel.Options = options;
			playerRankModel.GetBestAvailPlayerRanks();
			return PartialView(Constants.Views.BestAvailable, playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanks(string rankId, string id)
		{
			var options = GetPlayerRankOptions();
			PlayerRankModel playerRankModel = DetermineRankModel(rankId, id, options);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanksPartial(string rankId)
		{
			//TBD
			var options = GetPlayerRankOptions();
			PlayerRankModel playerRankModel = DetermineRankModel(rankId, null, options, false);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return PartialView(Constants.Views.PlayerRanks, playerRankModel);
		}

		[HttpGet]
		public ActionResult HighlightQueuePartial()
		{
			var options = GetPlayerRankOptions();
			PlayerRankModel playerRankModel = DetermineRankModel(null, null, options);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return PartialView(Constants.Views.HighlightQueuePartial, playerRankModel);
		}

		[HttpGet]
		public ActionResult RankingsList(string id)
		{
			GetPlayerRankOptions();
			RankingsListModel model = DraftFactory.GetRankingsListModel(Utilities.ToNullInt(id));
			return View(model);
		}

		[HttpGet]
		public ActionResult RankingsListPartial()
		{
			RankingsListModel model = DraftFactory.GetRankingsListModel();
			return PartialView(Constants.Views.RankingsList, model);
		}

		//TBD
		private PlayerRankModel DetermineRankModel(string id, string draftId, PlayerRankOptions options, bool setCookie = true)
		{
			int? draftIdInt = null;
			if (!string.IsNullOrEmpty(draftId))
			{
				draftIdInt = Convert.ToInt32(draftId);
			}
			PlayerRankModel playerRankModel = DraftFactory.GetEmptyPlayerRankModel(draftIdInt);
			int rankId = 0;
			if (!string.IsNullOrEmpty(id))
			{
				rankId = Convert.ToInt32(id);
				//Access not checked due to "OwnerRankAccess" attribute check
			}
			else if (!string.IsNullOrEmpty(options.RankId))
			{
				rankId = Convert.ToInt32(options.RankId);
				if (!new AccessModel().CanUserAccessRank(rankId))
				{
					rankId = 0;
				}
				else if (setCookie && !string.IsNullOrEmpty(options.DraftId))
				{	//If viewing ranks flipping between drafts (i.e. history), clear cached rankId
					if (string.IsNullOrEmpty(draftId))
					{
						draftId = playerRankModel.GetCurrentDraftId().ToString();
					}
					if (draftId != options.DraftId)
					{
						rankId = 0;
					}
				}
			}
			if (rankId == 0)
			{
				RankingsListModel rankingsListModel = DraftFactory.GetRankingsListModel(draftIdInt);
				rankId = rankingsListModel.GetPrimaryRankId();
			}
			if (setCookie && (options.RankId != rankId.ToString() || options.DraftId != draftId))
			{
				options.RankId = rankId.ToString();
				if (string.IsNullOrEmpty(draftId))
				{
					draftId = playerRankModel.GetCurrentDraftId().ToString();
				}
				options.DraftId = draftId;
				Response.Cookies["playerRankOptions"].Value = JsonConvert.SerializeObject(options);
			}
			playerRankModel.SetPlayerRanks(rankId);
			return playerRankModel;
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
