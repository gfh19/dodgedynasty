using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Mappers;
using DodgeDynasty.Mappers.Ranks;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using Newtonsoft.Json;

namespace DodgeDynasty.UIHelpers
{
	public class PlayerRankHelper
	{
		public static PlayerRankHelper Instance {
			get { return new PlayerRankHelper(); }
		}

		public PlayerRankModel GetPlayerRankPartial(string rankId, bool showBestAvailable, 
			HttpRequestBase request, HttpResponseBase response)
		{
			var options = GetPlayerRankOptions(request, response);
			PlayerRankModel playerRankModel = DetermineRankModel(rankId, null, options, response, false);
			playerRankModel.Options = options;
			if (showBestAvailable)
			{
				playerRankModel.GetBestAvailPlayerRanks();
			}
			else
			{
				playerRankModel.GetAllPlayerRanksByPosition();
			}
			return playerRankModel;
		}
		
		public PlayerRankOptions GetPlayerRankOptions(HttpRequestBase request, HttpResponseBase response)
		{
			string cookieId = null;

			var optionsCookie = request.Cookies[Constants.Cookies.PlayerRankOptions];
			if (optionsCookie != null)
			{
				var decodedCookie = HttpUtility.UrlDecode(optionsCookie.Value);
				cookieId = JsonConvert.DeserializeObject<PlayerRankOptionsCookie>(decodedCookie).Id;
			}
			if (optionsCookie == null || cookieId == null)
			{
				cookieId = SetNewPlayerRankOptionsCookie(response).Id;
			}
			var mapper = MapperFactory.CreatePlayerRankOptionsMapper(cookieId);
			var playerRankOptions = mapper.GetModel();
			CheckSetUpdatedOptionsCookie(mapper, response);
			return playerRankOptions;
		}

		private void CheckSetUpdatedOptionsCookie(PlayerRankOptionsMapper mapper, HttpResponseBase response)
		{
			if (mapper.UpdatedPlayerRankOptionId != null)
			{
				SetPlayerRankOptionsCookie(response, new PlayerRankOptionsCookie(mapper.UpdatedPlayerRankOptionId));
			}
		}

		public PlayerRankOptionsCookie SetNewPlayerRankOptionsCookie(HttpResponseBase response)
		{
			var cookie = new PlayerRankOptionsCookie(Guid.NewGuid().ToString());
			return SetPlayerRankOptionsCookie(response, cookie);
		}

		public PlayerRankOptionsCookie SetPlayerRankOptionsCookie(HttpResponseBase response, PlayerRankOptionsCookie cookie)
		{
			response.SetCookie(new HttpCookie(Constants.Cookies.PlayerRankOptions)
			{
				Expires = DateTime.Now.AddDays(100),
				Value = JsonConvert.SerializeObject(cookie)
			});
			return cookie;
		}

		public PlayerRankModel DetermineRankModel(string id, string draftId, PlayerRankOptions options, 
			HttpResponseBase response, bool setCookie = true)
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
				{   //If viewing ranks flipping between drafts (i.e. history), clear cached rankId
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
				PlayerRankOptionsMapper mapper = MapperFactory.CreatePlayerRankOptionsMapper(options.Id);
				mapper.UpdateEntity(options);
				CheckSetUpdatedOptionsCookie(mapper, response);
			}
			playerRankModel.SetPlayerRanks(rankId);
			return playerRankModel;
		}

		public string GetPlayerRankPartialName(bool showBestAvailable)
		{
			return string.Format("../Draft/{0}", showBestAvailable ? Constants.Views.BestAvailable : Constants.Views.PlayerRanks); ;
		}
	}
}
