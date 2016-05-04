using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers.Ranks
{
	public class CompareRanksMapper : MapperBase<PlayerRankingsModel>
	{
		public PlayerRankModel PlayerRankModel { get; set; }
		public bool ShowBestAvailable { get; set; }

		public CompareRanksMapper(PlayerRankModel playerRankModel, bool showBestAvailable)
		{
			PlayerRankModel = playerRankModel;
			ShowBestAvailable = showBestAvailable;
        }

		protected override void PopulateModel()
		{
			var helper = PlayerRankModelHelper.Instance;
			PlayerRankModel.CompareRankModels = new List<PlayerRankingsModel>();
			if (!string.IsNullOrEmpty(PlayerRankModel.Options.CompareRankIds))
			{
				foreach (var compareRankId in PlayerRankModel.Options.CompareRankIds.Split(','))
				{
					var rankId = Convert.ToInt32(compareRankId);
                    Model = helper.CreatePlayerRankingsModel(PlayerRankModel);
					helper.SetPlayerRanks(PlayerRankModel, HomeEntity, rankId);
					helper.SetPlayerRanks(Model, HomeEntity, rankId);
					if (ShowBestAvailable)
					{
						Model.RankedPlayers = PlayerRankModel.GetRankedPlayersAll();
						helper.GetBestAvailOverallCompRanks(Model);
					}
					else
					{
						Model.RankedPlayers = PlayerRankModel.GetRankedPlayersAllWithDraftPickInfo();
						helper.GetAllPlayersOverallCompRanks(Model);
					}
					PlayerRankModel.CompareRankModels.Add(Model);
				}
			}
		}
	}
}