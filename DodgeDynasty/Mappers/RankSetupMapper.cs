using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Mappers.Shared;

namespace DodgeDynasty.Mappers
{
	public class RankSetupMapper : ModelBase
	{
		public int CopyRankFrom(int copyFromRankId, string copyCount, RankingsListModel model)
		{
			RankSetupModel rankSetupModel = new RankSetupModel();
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(copyFromRankId);
			playerRankModel.GetRankedPlayersAll();
			int copyRowCount;
			if (!string.IsNullOrEmpty(copyCount) && Int32.TryParse(copyCount, out copyRowCount) && copyRowCount > 0)
			{
				copyRowCount = Convert.ToInt32(copyCount);
			}
			else
			{
				copyRowCount = playerRankModel.GetRankedPlayersAll().Count();
			}
			rankSetupModel.RankedPlayers = playerRankModel.RankedPlayers.OrderBy(rp => rp.RankNum).Take(copyRowCount).ToList();
			SetRankInfo(model, rankSetupModel);
			return AddNewRank(rankSetupModel);
		}

		public int AddRank(RankingsListModel model)
		{
			RankSetupModel rankSetupModel = new RankSetupModel();
			rankSetupModel.RankedPlayers = new List<RankedPlayer>();
			SetRankInfo(model, rankSetupModel);
			return AddNewRank(rankSetupModel);
		}

		private int AddNewRank(RankSetupModel model)
		{
			int rankId;
			using (HomeEntity = new HomeEntity())
			{
				var now = Utilities.GetEasternTime();
				var rank = new Rank
				{
					RankName = model.RankName,
					Year = model.DraftYear,
					RankDate = now,
					Url = null,
					AddTimestamp = now,
					LastUpdateTimestamp = now
				};
				HomeEntity.Ranks.AddObject(rank);
				HomeEntity.SaveChanges();

				rankId = rank.RankId;
				var draftRank = new DraftRank
				{
					RankId = rankId,
					PrimaryDraftRanking = true,
					UserId = model.UserId,
					AddTimestamp = now,
					LastUpdateTimestamp = now
				};
				HomeEntity.DraftRanks.AddObject(draftRank);
				HomeEntity.SaveChanges();

				foreach (var player in model.RankedPlayers)
				{
					HomeEntity.PlayerRanks.AddObject(new PlayerRank
					{
						RankId = rankId,
						PlayerId = player.PlayerId,
						RankNum = player.RankNum,
						PosRankNum = player.PosRankNum,
						AuctionValue = player.AuctionValue,
						AddTimestamp = now,
						LastUpdateTimestamp = now
					});
				}
				HomeEntity.SaveChanges();
			}
			return rankId;
		}

		private static void SetRankInfo(RankingsListModel model, RankSetupModel rankSetupModel)
		{
			var now = Utilities.GetEasternTime();
			rankSetupModel.DraftId = model.DraftId;
			rankSetupModel.DraftYear = model.CurrentDraft.DraftYear ?? Convert.ToInt16(now.Year);
			rankSetupModel.UserId = model.CurrentLoggedInOwnerUser.UserId;
			SetAddRankName(model, rankSetupModel);
		}

		private static void SetAddRankName(RankingsListModel model, RankSetupModel rankSetupModel)
		{
			var currentAvailRanks = model.GetCurrentAvailableDraftRanks();
			var addRankName = string.Format("{0}'s {1} Ranks", model.CurrentLoggedInOwnerUser.NickName,
				model.GetCurrentLeague().LeagueName);
			var rankNames = currentAvailRanks.Select(r => r.RankName).ToList();
			var version = 2;
			string newRankName = addRankName;
			while (rankNames.Contains(newRankName))
			{
				newRankName = string.Format("{0} {1}", addRankName, version++);
			}
			rankSetupModel.RankName = newRankName;
		}

		public void UpdatePlayerRanks(RankSetupModel rankSetupModel)
		{
			RankSetupModel currentModel = DraftFactory.GetRankSetupModel(rankSetupModel.RankId);
			currentModel.GetCurrentDraft();
			using (HomeEntity = new HomeEntity())
			{
				int newGroupId = 1;
				newGroupId = GetNextPlayerRankGroupId(newGroupId);

				UpdateRankInfo(rankSetupModel);
				ArchiveAndDeletePlayerRanks(rankSetupModel, newGroupId);
				SaveNewPlayerRanks(rankSetupModel);
			}
		}

		public bool AddNewPlayer(RankSetupModel rankSetupModel)
		{
			bool playerAdded = false;
			RankSetupModel currentModel = DraftFactory.GetRankSetupModel(rankSetupModel.RankId);
			currentModel.GetCurrentDraft();

			using (HomeEntity = new HomeEntity())
			{
				var now = Utilities.GetEasternTime();
				int? inactiveTruePlayerId = null;
				bool justActivated = false;
				Player player = DBUtilities.FindMatchingPlayer("Rank",
						HomeEntity, currentModel.ActivePlayers,
						rankSetupModel.Player.FirstName, rankSetupModel.Player.LastName, rankSetupModel.Player.Position,
						rankSetupModel.Player.NFLTeam, out inactiveTruePlayerId, out justActivated);
				if (player == null)
				{
					player = new Entities.Player
					{
						FirstName = rankSetupModel.Player.FirstName,
						LastName = rankSetupModel.Player.LastName,
						Position = rankSetupModel.Player.Position.ToUpper(),
						NFLTeam = rankSetupModel.Player.NFLTeam.ToUpper(),
						IsActive = true,
						AddTimestamp = now,
						LastUpdateTimestamp = now
					};
					HomeEntity.Players.AddObject(player);
					HomeEntity.SaveChanges();
					player.TruePlayerId = inactiveTruePlayerId ?? player.PlayerId;
					HomeEntity.SaveChanges();
					playerAdded = true;

					var loggedInUserName = Utilities.GetLoggedInUserName();
					var playerAdd = new Entities.PlayerAdjustment
					{
						NewPlayerId = player.PlayerId,
						TruePlayerId = player.TruePlayerId,
						NewFirstName = player.FirstName,
						NewLastName = player.LastName,
						NewPosition = player.Position.ToUpper(),
						NewNFLTeam = player.NFLTeam.ToUpper(),
						Action = "Rank Add Player",
						UserId = HomeEntity.Users.First(u => u.UserName == loggedInUserName).UserId,
						AddTimestamp = now,
						LastUpdateTimestamp = now
					};
					if (inactiveTruePlayerId != null)
					{
						playerAdd.Action = playerAdd.Action + ", Merge TruePlayerId";
						playerAdd.TruePlayerId = inactiveTruePlayerId;
					}
					HomeEntity.PlayerAdjustments.AddObject(playerAdd);
					HomeEntity.SaveChanges();
				}
				else if (justActivated)
				{
					playerAdded = true;
				}
			}
			return playerAdded;
		}

		private void UpdateRankInfo(RankSetupModel rankSetupModel)
		{
			var now = Utilities.GetEasternTime();
			var rank = HomeEntity.Ranks.First(r => r.RankId == rankSetupModel.RankId);
			rank.RankName = rankSetupModel.RankName;
			rank.RankDate = now;
			rank.LastUpdateTimestamp = now;
			HomeEntity.SaveChanges();
		}

		private void SaveNewPlayerRanks(RankSetupModel rankSetupModel)
		{
			var now = Utilities.GetEasternTime();
			var newRankedPlayers = rankSetupModel.RankedPlayers.ToList();
			foreach (var player in newRankedPlayers)
			{
				HomeEntity.PlayerRanks.AddObject(new PlayerRank
				{
					RankId = player.RankId,
					PlayerId = player.PlayerId,
					RankNum = player.RankNum,
					PosRankNum = player.PosRankNum,
					AuctionValue = player.AuctionValue,
					AddTimestamp = now,
					LastUpdateTimestamp = now
				});
			}
			HomeEntity.SaveChanges();
		}

		private void ArchiveAndDeletePlayerRanks(RankSetupModel rankSetupModel, int newGroupId)
		{
			var now = Utilities.GetEasternTime();
			var currentRankedPlayers = HomeEntity.PlayerRanks.Where(pr => pr.RankId == rankSetupModel.RankId).ToList();
			foreach (var player in currentRankedPlayers)
			{
				HomeEntity.PlayerRankHistories.AddObject(new PlayerRankHistory
				{
					PlayerRankGroupId = newGroupId,
					RankId = player.RankId,
					PlayerId = player.PlayerId,
					RankNum = player.RankNum,
					PosRankNum = player.PosRankNum,
					AuctionValue = player.AuctionValue,
					AddTimestamp = player.AddTimestamp,
					LastUpdateTimestamp = now
				});
				HomeEntity.PlayerRanks.DeleteObject(player);
			}
		}

		private int GetNextPlayerRankGroupId(int newGroupId)
		{
			var maxPlayerRankGroupId = HomeEntity.PlayerRankHistories.Max(h => h.PlayerRankGroupId);
			if (maxPlayerRankGroupId.HasValue)
			{
				newGroupId = maxPlayerRankGroupId.Value + 1;
			}
			return newGroupId;
		}
	}
}