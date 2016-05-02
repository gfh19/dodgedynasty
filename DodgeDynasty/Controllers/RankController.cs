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
using DodgeDynasty.Models.Types;
using System.Web.Helpers;
using DodgeDynasty.Mappers.Ranks;

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
			model.Options = PlayerRankUIHelper.Instance.GetPlayerRankOptions(Request, Response);
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
		public HttpStatusCode AddPlayerHighlight(PlayerHighlightModel model)
		{
			AddPlayerHighlightMapper mapper = Factory.Create<AddPlayerHighlightMapper>();
			mapper.UpdateEntity(model);
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public HttpStatusCode DeletePlayerHighlight(int playerId)
		{
			DeletePlayerHighlightMapper mapper = Factory.Create<DeletePlayerHighlightMapper>();
			mapper.UpdateEntity(new PlayerHighlightModel { PlayerId = playerId });
			return HttpStatusCode.OK;
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

		//May not be necessary...
		[HttpGet]
		public JsonResult GetPlayerRankOptions()
		{
			var options = PlayerRankUIHelper.Instance.GetPlayerRankOptions(Request, Response);
			return Json(options);
		}

		[HttpPost]
		public HttpStatusCode PostPlayerRankOptions(PlayerRankOptions options)
		{
			var mapper = MapperFactory.CreatePlayerRankOptionsMapper(options.Id);
			return (mapper.UpdateEntity(options)) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
		}
	}
}
