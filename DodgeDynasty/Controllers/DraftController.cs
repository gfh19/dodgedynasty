using System;
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
		//TODO:  Test playerId passed during draft

		[HttpGet]
		public ActionResult Pick(string playerId, string id)
		{
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

		public ActionResult TeamDisplayPartial()
		{
			DraftTeamDisplayModel draftTeamDisplayModel = DraftFactory.GetDraftTeamDisplayModel();
			return PartialView(Constants.Views.TeamDisplay, draftTeamDisplayModel);
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BestAvailable(string rankId, string id)
		{
			var options = GetPlayerRankOptions();
			int currentRankId = DetermineRankId(rankId, options);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(currentRankId, Utilities.ToNullInt(id));
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
		public ActionResult PlayerRanks(string rankId, string id)
		{
			var options = GetPlayerRankOptions();
			int currentRankId = DetermineRankId(rankId, options);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(currentRankId, Utilities.ToNullInt(id));
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
		public ActionResult RankingsList(string id)
		{
			GetPlayerRankOptions();
			RankingsListModel model = DraftFactory.GetRankingsListModel(Utilities.ToNullInt(id));
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
				num = model.CurrentDraftPick.PickNum,
				hasPrev = (model.PreviousDraftPick != null),
				prevName = (model.PreviousDraftPick != null) ? model.PreviousDraftPick.Player.PlayerName : null
			};
			return Json(pickInfo, JsonRequestBehavior.AllowGet);
		}
	}
}
