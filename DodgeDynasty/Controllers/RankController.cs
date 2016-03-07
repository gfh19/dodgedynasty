using System;
using System.Web.Mvc;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;
using DodgeDynasty.Mappers;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers.Highlights;
using DodgeDynasty.Models.Highlights;
using System.Net;
using DodgeDynasty.UIHelpers;

namespace DodgeDynasty.Controllers
{
	public class RankController : BaseController
	{
		[HttpGet]
		[OwnerRankAccess]
		public ActionResult AddRank(string rankId, string copyCount)
		{
			int newRankId;
			RankingsListModel model = DraftFactory.GetRankingsListModel();
			model.GetCurrentDraft();
			RankSetupMapper mapper = DraftFactory.GetRankSetupMapper();
			if (!string.IsNullOrEmpty(rankId))
			{
				newRankId = mapper.CopyRankFrom(Convert.ToInt32(rankId), copyCount, model);
			}
			else
			{
				newRankId = mapper.AddRank(model);
			}
			return RedirectToAction(Constants.Views.SetupRank, new { rankId = newRankId });
		}

		[HttpGet]
		[OwnerUpdateRankAccess]
		public ActionResult SetupRank(int rankId)
		{
			GetPlayerRankOptions();
			PlayerRankModel model = DraftFactory.GetPlayerRankModel(rankId);
			model.GetAllPlayerRanks();
			if (TempData.ContainsKey(Constants.TempData.RankStatus))
			{
				model.RankStatus = (string)TempData[Constants.TempData.RankStatus];
			}
			return View(model);
		}

		[HttpPost]
		[OwnerUpdateRankAccess]
		public string UpdateRankSetup(RankSetupModel model)
		{
			RankSetupMapper mapper = DraftFactory.GetRankSetupMapper();
			mapper.UpdatePlayerRanks(model);
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(model.RankId);
			playerRankModel.RankStatus = "saved";
			if (model.Player != null)
			{
				bool playerAdded = mapper.AddNewPlayer(model);
				playerRankModel.Player = model.Player;
				playerRankModel.RankStatus = (playerAdded) ? "player-added" : "player-existed";
			}
			TempData[Constants.TempData.RankStatus] = playerRankModel.RankStatus;
			return playerRankModel.RankStatus;
		}

		[HttpPost]
		public ActionResult AddPlayerHighlight(PlayerHighlightModel model, bool showBestAvailable)
		{
			AddPlayerHighlightMapper mapper = Factory.Create<AddPlayerHighlightMapper>();
			mapper.UpdateEntity(model);
			PlayerRankHelper helper = Factory.Create<PlayerRankHelper>();
			var playerRankModel = helper.GetPlayerRankPartial(null, showBestAvailable, Request, Response);
			var result = PartialView(helper.GetPlayerRankPartialName(showBestAvailable), playerRankModel);
            return result;
		}

		[HttpPost]
		public ActionResult DeletePlayerHighlight(int playerId, bool showBestAvailable)
		{
			DeletePlayerHighlightMapper mapper = Factory.Create<DeletePlayerHighlightMapper>();
			mapper.UpdateEntity(new PlayerHighlightModel { PlayerId = playerId });
			PlayerRankHelper helper = Factory.Create<PlayerRankHelper>();
			var playerRankModel = helper.GetPlayerRankPartial(null, showBestAvailable, Request, Response);
			return PartialView(helper.GetPlayerRankPartialName(showBestAvailable), playerRankModel);
		}

		[HttpPost]
		public HttpStatusCode DeleteAllHighlights()
		{
			DeleteAllHighlightsMapper mapper = Factory.Create<DeleteAllHighlightsMapper>();
			mapper.UpdateEntity();
			return HttpStatusCode.OK;
		}
		
		[HttpPost]
		public HttpStatusCode CopyLastDraftHighlights()
		{
			CopyLastDraftHighlightsMapper mapper = Factory.Create<CopyLastDraftHighlightsMapper>();
			mapper.UpdateEntity();
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public HttpStatusCode UpdatePlayerQueueOrder(PlayerQueueOrderModel model)
		{
			UpdatePlayerQueueOrderMapper mapper = Factory.Create<UpdatePlayerQueueOrderMapper>();
			mapper.UpdateEntity(model);
			return HttpStatusCode.OK;
		}
	}
}
