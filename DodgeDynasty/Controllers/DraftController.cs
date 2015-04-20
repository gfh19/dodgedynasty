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
		//TODO:  Hard-coded draft for testing; make accurate
		private int? _draftId = Constants.DraftId;

		[HttpGet]
		public ActionResult Pick(string id)
		{
			DraftInputModel draftInputModel = DraftFactory.GetCurrentDraftInputModel();
			if (!string.IsNullOrEmpty(id))
			{
				int playerId = Convert.ToInt32(id);
				draftInputModel.PreloadPlayerModel(playerId);
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

		public ActionResult Display()
		{
			DraftDisplayModel draftDisplayModel = DraftFactory.GetDraftDisplayModel();
			return View(draftDisplayModel);
		}

		public ActionResult DisplayPartial()
		{
			DraftDisplayModel draftDisplayModel = DraftFactory.GetDraftDisplayModel();
			return PartialView(Constants.Views.Display, draftDisplayModel);
		}

		public ActionResult TeamDisplay()
		{
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel();
			return View(draftTeamDisplayModel);
		}

		public ActionResult TeamDisplayPartial()
		{
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel();
			return PartialView(Constants.Views.TeamDisplay, draftTeamDisplayModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailable(string rankId)
		{
			var options = GetPlayerRankOptions();
			int currentRankId = DetermineRankId(rankId, options);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(currentRankId);
			playerRankModel.Options = options;
			playerRankModel.GetBestAvailPlayerRanks();
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailablePartial(string rankId)
		{
			var options = GetPlayerRankOptions();
			int currentRankId = DetermineRankId(rankId, options, false);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(currentRankId);
			playerRankModel.Options = options;
			playerRankModel.GetBestAvailPlayerRanks();
			return PartialView(Constants.Views.BestAvailable, playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanks(string rankId)
		{
			var options = GetPlayerRankOptions();
			int currentRankId = DetermineRankId(rankId, options);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(currentRankId);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return View(playerRankModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult PlayerRanksPartial(string rankId)
		{
			var options = GetPlayerRankOptions();
			int currentRankId = DetermineRankId(rankId, options, false);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(currentRankId);
			playerRankModel.Options = options;
			playerRankModel.GetAllPlayerRanksByPosition();
			return PartialView(Constants.Views.PlayerRanks, playerRankModel);
		}

		[HttpGet]
		public ActionResult RankingsList()
		{
			GetPlayerRankOptions();
			RankingsListModel model = DraftFactory.GetRankingsListModel();
			model.GetCurrentDraft();
			return View(model);
		}

		[HttpGet]
		public ActionResult RankingsListPartial()
		{
			RankingsListModel model = DraftFactory.GetRankingsListModel();
			model.GetCurrentDraft();
			return PartialView(Constants.Views.RankingsList, model);
		}

		private int DetermineRankId(string id, PlayerRankOptions options, bool setCookie = true)
		{
			int rankId = 0;
			if (!string.IsNullOrEmpty(id))
			{
				rankId = Convert.ToInt32(id);
			}
			else if (!string.IsNullOrEmpty(options.RankId))
			{
				rankId = Convert.ToInt32(options.RankId);
				if (!new AccessModel().CanUserAccessRank(rankId))
				{
					rankId = 0;
				}
			}
			
			if (rankId == 0)
			{
				RankingsListModel rankingsListModel = DraftFactory.GetRankingsListModel();
				rankingsListModel.GetCurrentDraft();
				rankId = rankingsListModel.GetPrimaryRankId(rankingsListModel);
			}
			if (setCookie && options.RankId != rankId.ToString())
			{
				options.RankId = rankId.ToString();
				Response.Cookies["playerRankOptions"].Value = JsonConvert.SerializeObject(options);
			}
			return rankId;
		}

		[HttpGet]
		public ActionResult CurrentDraftPickPartial()
		{
			DraftModel model = new DraftModel(_draftId);
			model.GetCurrentDraft();
			return PartialView(Constants.Views.CurrentDraftPickPartial, model);
		}

		[HttpGet]
		public ActionResult History()
		{
			var mapper = new DraftHistoryMapper<DraftHistoryModel>();
			return View(mapper.GetModel());
		}
	}
}
