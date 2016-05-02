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

		public CompareRanksMapper(PlayerRankModel playerRankModel)
		{
			PlayerRankModel = playerRankModel;
		}

		protected override void PopulateModel()
		{
			var helper = PlayerRankModelHelper.Instance;
			PlayerRankModel.CompareRankModels = new List<PlayerRankingsModel>();
            foreach (var compareRankId in PlayerRankModel.Options.CompareRankIds.Split(','))
			{
				Model = helper.CreatePlayerRankingsModel(PlayerRankModel);
				helper.SetPlayerRanks(Model, HomeEntity, Convert.ToInt32(compareRankId));
				helper.GetBestAvailOverallCompPlayerRanks(Model);
				PlayerRankModel.CompareRankModels.Add(Model);
            }
		}
	}
}