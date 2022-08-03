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
using System.Linq;

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
			PlayerRankModel model = DraftFactory.GetPlayerRankModel(rankId, null, PlayerRankUIHelper.Instance.GetPlayerRankOptions(Request, Response));
			model.GetRankedPlayersAll();
			model.SetUnrankedCompareList();
			model.SetAllHighlightedPlayers();
			if (ViewData.ContainsKey(Constants.ViewData.RankStatus))
			{
				model.RankStatus = (string)ViewData[Constants.ViewData.RankStatus];
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
			ViewData[Constants.ViewData.RankStatus] = playerRankModel.RankStatus;
			return playerRankModel.RankStatus;
		}

		[HttpGet]
		[OwnerUpdateRankAccess]
		public ActionResult DeleteRank(RankSetupModel model)
		{
			var mapper = Factory.Create<DeleteRankMapper>();
			mapper.UpdateEntity(model);
			return RedirectToAction(Constants.Views.RankingsList, "Draft");
		}

		#region Highlighting

		[HttpPost]
		public HttpStatusCode AddPlayerHighlight(PlayerHighlightModel model)
		{
			AddPlayerHighlightMapper mapper = Factory.Create<AddPlayerHighlightMapper>();
			mapper.UpdateEntity(model);
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public HttpStatusCode DeletePlayerHighlight(PlayerHighlightModel model)
		{
			DeletePlayerHighlightMapper mapper = Factory.Create<DeletePlayerHighlightMapper>();
			mapper.UpdateEntity(model);
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public HttpStatusCode DeleteAllHighlights(int draftHighlightId)
		{
			DeleteAllHighlightsMapper mapper = Factory.Create<DeleteAllHighlightsMapper>();
			mapper.UpdateEntity(new PlayerHighlightModel { DraftHighlightId = draftHighlightId });
			return HttpStatusCode.OK;
		}
		
		[HttpPost]
		public HttpStatusCode CopyLastDraftHighlights(int lastDraftHighlightId, int newDraftHighlightId)
		{
			CopyLastDraftHighlightsMapper mapper = Factory.Create<CopyLastDraftHighlightsMapper>();
			mapper.UpdateEntity(new DraftHighlightModel { DraftHighlightId = lastDraftHighlightId, NewDraftHighlightId = newDraftHighlightId });
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public HttpStatusCode UpdatePlayerQueueOrder(PlayerQueueOrderModel model)
		{
			UpdatePlayerQueueOrderMapper mapper = Factory.Create<UpdatePlayerQueueOrderMapper>();
			mapper.UpdateEntity(model);
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public ActionResult AddEditHighlightQueue(PlayerHighlightModel oldModel, PlayerHighlightModel newModel)
		{
			var oldMapper = Factory.Create<AddEditHighlightQueueMapper>();
			oldMapper.UpdateEntity(oldModel);
			var responseId = oldMapper.DraftHighlight?.DraftHighlightId;

			if (newModel != null) {
				var newMapper = Factory.Create<AddEditHighlightQueueMapper>();
				newMapper.UpdateEntity(newModel);
				responseId = newMapper.DraftHighlight?.DraftHighlightId;
			}
			return Json(new { queueId = responseId });
		}

		[HttpPost]
		public HttpStatusCode DeleteHighlightQueue(int draftHighlightId)
		{
			DeleteHighlightQueueMapper mapper = Factory.Create<DeleteHighlightQueueMapper>();
			mapper.UpdateEntity(new DraftHighlightModel { DraftHighlightId = draftHighlightId });
			return HttpStatusCode.OK;
		}

		#endregion Highlighting

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

		[HttpPost]
		public HttpStatusCode UpdateBestUnrankedOptions(PlayerRankOptions options)
		{
			var mapper = MapperFactory.CreatePlayerRankOptionsMapper(options.Id);
			var currentOptions = mapper.GetModel();
			currentOptions.ExpandBUP = options.ExpandBUP;
			currentOptions.HideBUP = options.HideBUP;
			currentOptions.BUPId = options.BUPId;
			return (mapper.UpdateEntity(currentOptions)) ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
		}

		[HttpGet]
		[OwnerRankAccess]
		public ActionResult BupSectionPartial(int rankId)
		{
			GetPlayerRankOptions();
			PlayerRankModel model = DraftFactory.GetPlayerRankModel(rankId, null, PlayerRankUIHelper.Instance.GetPlayerRankOptions(Request, Response));
			model.SetUnrankedCompareList();
			return PartialView(Constants.Views.BupSectionPartial, model);
		}

		[HttpGet]
		[OwnerRankAccess]
		public JsonResult GetHighlightedPlayers()
		{
			var playerRankModel = PlayerRankUIHelper.Instance.GetPlayerRankPartial(null, null, false, Request, Response);
			var players = playerRankModel.GetAllHighlightedPlayers();
			return Json(players.Select(p => new { id = p.PlayerId, name = p.PlayerName }).ToArray(), JsonRequestBehavior.AllowGet);
		}
	}
}
