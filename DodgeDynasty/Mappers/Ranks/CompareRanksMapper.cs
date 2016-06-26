using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Mappers.Ranks
{
	public class CompareRanksMapper : MapperBase<PlayerRankingsModel>
	{
		//Offset to adjust unranked player ranking, relative to rank length
		public const int UnrankedPlayerOffset = 1;

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
				var compRankIds = PlayerRankModel.Options.CompareRankIds.Split(',');
                foreach (var compareRankId in compRankIds)
				{
					var rankId = Convert.ToInt32(compareRankId);
                    Model = helper.CreatePlayerRankingsModel(PlayerRankModel);
					helper.SetPlayerRanks(PlayerRankModel, HomeEntity, rankId);
					helper.SetPlayerRanks(Model, HomeEntity, rankId);
					Model.RankedPlayers = PlayerRankModel.GetRankedPlayersAllWithDraftPickInfo();
					if (ShowBestAvailable)
					{
						Model.OverallRankedPlayers = helper.GetBestAvailOverallCompRanks(Model.RankedPlayers, Model.DraftedPlayers);
					}
					else
					{
						Model.OverallRankedPlayers = helper.GetAllPlayersOverallCompRanks(Model.RankedPlayers);
					}
					PlayerRankModel.CompareRankModels.Add(Model);
				}
				if (PlayerRankModel.Options.ShowAvgCompRanks && PlayerRankModel.CompareRankModels.Count > 0)
				{
					var averagePlayerRank = helper.CreatePlayerRankingsModel(PlayerRankModel);
					averagePlayerRank.RankedPlayers = CalculateAvgCompareRanks();
					if (ShowBestAvailable)
					{
						averagePlayerRank.OverallRankedPlayers = helper.GetBestAvailOverallCompRanks(averagePlayerRank.RankedPlayers, Model.DraftedPlayers);
					}
					else
					{
						averagePlayerRank.OverallRankedPlayers = helper.GetAllPlayersOverallCompRanks(averagePlayerRank.RankedPlayers);
					}
					PlayerRankModel.AveragePlayerRank = averagePlayerRank;
				}
			}
		}

		public List<RankedPlayer> CalculateAvgCompareRanks()
		{
			List<RankedPlayerAverage> rankedPlayerAverages = new List<RankedPlayerAverage>();
			List<RankedPlayer> allRankedPlayers = new List<RankedPlayer>();
			PlayerRankModel.CompareRankModels.ForEach(o => allRankedPlayers.AddRange(o.RankedPlayers));
			var distinctTruePlayerIds = (from player in allRankedPlayers
										 select player.TruePlayerId).Distinct().OrderBy(o=>o).ToList();
            for (int i=0; i<PlayerRankModel.CompareRankModels.Count; i++)
			{
				var rank = PlayerRankModel.CompareRankModels[i];
				foreach (var tpid in distinctTruePlayerIds)
				{
					SetAverageRankedPlayer(rankedPlayerAverages, tpid, rank.RankedPlayers, i);
                }
			}
			rankedPlayerAverages.ForEach(o => o.RankedPlayer.AvgRankNum = o.AllRankNums.Take(o.AllRankNums.Count()).Average());
			allRankedPlayers = new List<RankedPlayer>();
			int playerCount = 1;
			int prevRankNum = playerCount;
			double prevAvgRankNum = 0.0;
			foreach (var rankedPlayerAvg in rankedPlayerAverages.OrderBy(o => o.RankedPlayer.AvgRankNum).ThenBy(o => o.AllRankNums[0]).ToList())
			{
				if (rankedPlayerAvg.RankedPlayer.AvgRankNum > prevAvgRankNum)
				{
					rankedPlayerAvg.RankedPlayer.RankNum = playerCount;
					prevRankNum = playerCount;
				}
				else
				{
					rankedPlayerAvg.RankedPlayer.RankNum = prevRankNum;
				}
				playerCount++;
				prevAvgRankNum = rankedPlayerAvg.RankedPlayer.AvgRankNum.Value;
				allRankedPlayers.Add(rankedPlayerAvg.RankedPlayer);
            }
			return allRankedPlayers;
		}

		public void SetAverageRankedPlayer(List<RankedPlayerAverage> rankedPlayerAverages, int truePlayerId, List<RankedPlayer> rankedPlayers, int rankIx)
		{
			var rankedPlayer = rankedPlayers.FirstOrDefault(o => o.TruePlayerId == truePlayerId);
			var playerAvg = rankedPlayerAverages.FirstOrDefault(o => o.TruePlayerId == truePlayerId);
			if (playerAvg == null)
			{
				playerAvg = new RankedPlayerAverage { TruePlayerId = truePlayerId, AllRankNums = new int[PlayerRankModel.CompareRankModels.Count] };
				rankedPlayerAverages.Add(playerAvg);
            }
			if (rankedPlayer != null)
			{
				playerAvg.RankedPlayer = playerAvg.RankedPlayer ?? PlayerRankModelHelper.Instance.CopyRankedPlayer(rankedPlayer);
				playerAvg.AllRankNums[rankIx] = rankedPlayer.RankNum.Value;
			}
			else
			{
				playerAvg.AllRankNums[rankIx] = rankedPlayers.Count + UnrankedPlayerOffset;
            }
		}
    }
}