using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
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
		public bool WrongDraftCompRankFound { get; set; }

		public CompareRanksMapper(PlayerRankModel playerRankModel, bool showBestAvailable)
		{
			PlayerRankModel = playerRankModel;
			ShowBestAvailable = showBestAvailable;
        }

		protected override void PopulateModel()
		{
			PlayerRankModel.CompareRankModels = new List<PlayerRankingsModel>();
			if (!string.IsNullOrEmpty(PlayerRankModel.Options.CompareRankIds))
			{
				var compRankIds = PlayerRankModel.Options.CompareRankIds.Split(',');
				Rank firstRank = null;
				List<PlayerRank> firstPlayerRanks = null;
				var inactiveDraftedPlayers = PlayerRankModel.DraftedPlayers.Where(o => !o.IsActive).ToList();
				var draftedTruePlayers = PlayerRankModelHelper.GetDraftedTruePlayersFor(inactiveDraftedPlayers, PlayerRankModel);
				foreach (var compareRankId in compRankIds)
				{
					var rankId = Convert.ToInt32(compareRankId);
					var draftRank = HomeEntity.DraftRanks.FirstOrDefault(o => o.RankId == rankId);
					//If Comp Rank Id is draft-specific and no longer the current draft, abort
					if (draftRank == null || (draftRank.DraftId != null && draftRank.DraftId != PlayerRankModel.GetCurrentDraftId()))
					{
						WrongDraftCompRankFound = true;
                        PlayerRankModel.CompareRankModels.Clear();
						break;
                    }
                    Model = PlayerRankModelHelper.CreatePlayerRankingsModel(PlayerRankModel);
					PlayerRankModelHelper.SetPlayerRanks(PlayerRankModel, HomeEntity, rankId);
					if (firstRank == null)
					{
						firstRank = PlayerRankModel.CurrentRank;
						firstPlayerRanks = PlayerRankModel.PlayerRanks;
                    }
					PlayerRankModelHelper.SetPlayerRanks(Model, HomeEntity, rankId);
					if (ShowBestAvailable)
					{
						Model.RankedPlayers = PlayerRankModel.GetRankedPlayersAll();
						Model.OverallRankedPlayers = PlayerRankModelHelper.GetBestAvailOverallCompRanks(Model.RankedPlayers, Model.DraftedPlayers);
					}
					else
					{
                        Model.RankedPlayers = PlayerRankModel.GetRankedPlayersAllWithDraftPickInfo(draftedTruePlayers);
						Model.OverallRankedPlayers = PlayerRankModelHelper.GetAllPlayersOverallCompRanks(Model.RankedPlayers);
					}
					PlayerRankModel.CompareRankModels.Add(Model);
				}
				PlayerRankModel.CurrentRank = firstRank;
				PlayerRankModel.PlayerRanks = firstPlayerRanks;
				if (PlayerRankModel.Options.ShowAvgCompRanks && PlayerRankModel.CompareRankModels.Count > 0)
				{
					var averagePlayerRank = PlayerRankModelHelper.CreatePlayerRankingsModel(PlayerRankModel);
					averagePlayerRank.RankedPlayers = CalculateAvgCompareRanks();
					if (ShowBestAvailable)
					{
						averagePlayerRank.OverallRankedPlayers = PlayerRankModelHelper.GetBestAvailOverallCompRanks(averagePlayerRank.RankedPlayers, Model.DraftedPlayers);
					}
					else
					{
						averagePlayerRank.OverallRankedPlayers = PlayerRankModelHelper.GetAllPlayersOverallCompRanks(averagePlayerRank.RankedPlayers);
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
				playerAvg.RankedPlayer = playerAvg.RankedPlayer ?? PlayerRankModelHelper.CopyRankedPlayer(rankedPlayer);
				playerAvg.AllRankNums[rankIx] = rankedPlayer.RankNum.Value;
			}
			else
			{
				playerAvg.AllRankNums[rankIx] = rankedPlayers.Count + UnrankedPlayerOffset;
            }
		}
    }
}