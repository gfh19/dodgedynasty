﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Mappers
{
	public class RankSetupMapper : ModelBase
	{
		public int CopyRankFrom(int copyFromRankId, string copyCount, RankingsListModel model)
		{
			RankSetupModel rankSetupModel = new RankSetupModel();
			PlayerRankModel playerRankModel = DraftFactory.GetPlayerRankModel(copyFromRankId);
			playerRankModel.GetAllPlayerRanks();
			int copyRowCount;
			if (!string.IsNullOrEmpty(copyCount) && Int32.TryParse(copyCount, out copyRowCount) && copyRowCount > 0)
			{
				copyRowCount = Convert.ToInt32(copyCount);
			}
			else
			{
				copyRowCount = playerRankModel.GetAllPlayerRanks().Count();
			}
			rankSetupModel.RankedPlayers = playerRankModel.RankedPlayers.OrderBy(rp=>rp.RankNum).Take(copyRowCount).ToList();
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
				var rank = new Rank
				{
					RankName = model.RankName,
					Year = model.DraftYear,
					RankDate = DateTime.Now,
					Url = null,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				HomeEntity.Ranks.AddObject(rank);
				HomeEntity.SaveChanges();

				rankId = rank.RankId;
				var draftRank = new DraftRank
				{
					RankId = rankId,
					PrimaryDraftRanking = true,
					OwnerId = model.OwnerId,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
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
						AddTimestamp = DateTime.Now,
						LastUpdateTimestamp = DateTime.Now
					});
				}
				HomeEntity.SaveChanges();
			}
			return rankId;
		}

		private static void SetRankInfo(RankingsListModel model, RankSetupModel rankSetupModel)
		{
			rankSetupModel.DraftId = model.DraftId;
			rankSetupModel.DraftYear = model.CurrentDraft.DraftYear ?? Convert.ToInt16(DateTime.Now.Year);
			rankSetupModel.OwnerId = model.CurrentLoggedInOwnerUser.OwnerId;
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
			Player player = currentModel.Players.FirstOrDefault(
				Utilities.FindPlayerMatch(rankSetupModel.Player.FirstName, rankSetupModel.Player.LastName, 
					rankSetupModel.Player.Position, rankSetupModel.Player.NFLTeam));
			if (player == null)
			{
				using (HomeEntity = new HomeEntity())
				{
					player = new Entities.Player
					{
						FirstName = rankSetupModel.Player.FirstName,
						LastName = rankSetupModel.Player.LastName,
						Position = rankSetupModel.Player.Position.ToUpper(),
						NFLTeam = rankSetupModel.Player.NFLTeam.ToUpper(),
						IsActive = true,
						AddTimestamp = DateTime.Now,
						LastUpdateTimestamp = DateTime.Now
					};
					HomeEntity.Players.AddObject(player);
					HomeEntity.SaveChanges();
					playerAdded = true;

					var loggedInUserName = Utilities.GetLoggedInUserName();
					var playerAdd = new Entities.PlayerAdjustment
					{
						NewPlayerId = player.PlayerId,
						NewFirstName = player.FirstName,
						NewLastName = player.LastName,
						NewPosition = player.Position,
						NewNFLTeam = player.NFLTeam,
						Action = "Rank Add Player",
						UserId = HomeEntity.Users.First(u => u.UserName == loggedInUserName).UserId,
						AddTimestamp = DateTime.Now,
						LastUpdateTimestamp = DateTime.Now
					};
					HomeEntity.PlayerAdjustments.AddObject(playerAdd);
					HomeEntity.SaveChanges();
				}
			}
			return playerAdded;
		}

		private void UpdateRankInfo(RankSetupModel rankSetupModel)
		{
			var rank = HomeEntity.Ranks.First(r => r.RankId == rankSetupModel.RankId);
			rank.RankName = rankSetupModel.RankName;
			rank.RankDate = DateTime.Now;
			rank.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();
		}

		private void SaveNewPlayerRanks(RankSetupModel rankSetupModel)
		{
			var newRankedPlayers = rankSetupModel.RankedPlayers.ToList();
			foreach (var player in newRankedPlayers)
			{
				if (player.PlayerId < 1)
				{
					Player listedPlayer = HomeEntity.Players.FirstOrDefault(
						Utilities.FindPlayerMatch(player.FirstName, player.LastName, player.Position, player.NFLTeam));
					if (listedPlayer == null)
					{
						listedPlayer = new Entities.Player
						{
							FirstName = player.FirstName,
							LastName = player.LastName,
							Position = player.Position.ToUpper(),
							NFLTeam = player.NFLTeam.ToUpper(),
							IsActive = true,
							AddTimestamp = DateTime.Now,
							LastUpdateTimestamp = DateTime.Now
						};
						HomeEntity.Players.AddObject(listedPlayer);
						HomeEntity.SaveChanges();
					}
					player.PlayerId = listedPlayer.PlayerId;
				}
				HomeEntity.PlayerRanks.AddObject(new PlayerRank
				{
					RankId = player.RankId,
					PlayerId = player.PlayerId,
					RankNum = player.RankNum,
					PosRankNum = player.PosRankNum,
					AuctionValue = player.AuctionValue,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				});
			}
			HomeEntity.SaveChanges();
		}

		private void ArchiveAndDeletePlayerRanks(RankSetupModel rankSetupModel, int newGroupId)
		{
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
					LastUpdateTimestamp = DateTime.Now
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