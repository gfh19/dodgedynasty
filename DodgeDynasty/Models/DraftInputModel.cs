using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using System.Text;
using System.Data.Objects;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Linq.Expressions;
using DodgeDynasty.Shared;
using DodgeDynasty.Shared.Exceptions;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class DraftInputModel : DraftModel
	{
		public OwnerUser CurrentDraftPickOwnerUser { get; set; }
		public PlayerModel Player { get; set; }
		public string Message { get; set; }

		public DraftInputModel() : this(null)
		{}

		public DraftInputModel(int? draftId)
			: base(draftId)
		{
			base.GetCurrentDraft();

			if (CurrentDraftPick != null)
			{
				ResetCurrentPickPlayerModel();
			}
		}

		private void SetCurrentDraftPickOwnerUser()
		{
			var owner = Owners.First(o => o.OwnerId == CurrentDraftPick.OwnerId);
			CurrentDraftPickOwnerUser = GetOwnerUser(owner,
										Users.First(u=>u.UserId == owner.UserId),
										CurrentLeagueOwners.First(lo=>lo.OwnerId == owner.OwnerId));
		}

		public void ResetCurrentPickPlayerModel()
		{
			SetCurrentDraftPickOwnerUser();
			Player = new PlayerModel();
			if (CurrentDraftPick.PlayerId != null)
			{
				var player = DraftedPlayers.First(p => p.PlayerId == CurrentDraftPick.PlayerId);
				Player.FirstName = player.FirstName;
				Player.LastName = player.LastName;
				Player.Position = player.Position;
				Player.NFLTeam = player.NFLTeam;
				Player.PlayerId = player.PlayerId;
			}
			Player.TeamName = CurrentDraftPickOwnerUser.TeamName ?? CurrentDraftPickOwnerUser.NickName;
			Player.DraftPickId = CurrentDraftPick.DraftPickId;
		}

		public void SelectPlayer(PlayerModel playerModel)
		{
			using (HomeEntity = new HomeEntity())
			{
				Player draftedPlayer = Players.FirstOrDefault(
					Utilities.FindPlayerMatch(playerModel.FirstName, playerModel.LastName, playerModel.Position, playerModel.NFLTeam));
				if (draftedPlayer == null)
				{
					draftedPlayer = new Entities.Player
					{
						FirstName = playerModel.FirstName,
						LastName = playerModel.LastName,
						Position = playerModel.Position.ToUpper(),
						NFLTeam = playerModel.NFLTeam.ToUpper(),
						AddTimestamp = DateTime.Now,
						LastUpdateTimestamp = DateTime.Now,
						IsActive = true
					};
					HomeEntity.Players.AddObject(draftedPlayer);
					HomeEntity.SaveChanges();

					string userName = Utilities.GetLoggedInUserName();
					var playerAdd = new Entities.PlayerAdjustment
					{
						NewPlayerId = draftedPlayer.PlayerId,
						NewFirstName = draftedPlayer.FirstName,
						NewLastName = draftedPlayer.LastName,
						NewPosition = draftedPlayer.Position,
						NewNFLTeam = draftedPlayer.NFLTeam,
						Action = "Draft Add Player",
						UserId = HomeEntity.Users.First(u => u.UserName == userName).UserId,
						AddTimestamp = DateTime.Now,
						LastUpdateTimestamp = DateTime.Now
					};
					HomeEntity.PlayerAdjustments.AddObject(playerAdd);
					HomeEntity.SaveChanges();
				}
				else
				{
					var draftedPlayers = DraftPicks.Select(p => p.PlayerId).ToList();
					if (draftedPlayers.Contains(draftedPlayer.PlayerId))
					{
						var duplicateDraftPick = DraftPicks.First(p => p.PlayerId == draftedPlayer.PlayerId);
						throw new DuplicatePickException(duplicateDraftPick);
					}
				}

				var draftPick = HomeEntity.DraftPicks.First(p => p.DraftPickId == playerModel.DraftPickId);
				draftPick.PlayerId = draftedPlayer.PlayerId;
				draftPick.LastUpdateTimestamp = DateTime.Now;
				draftPick.PickEndDateTime = GetCurrentTimeEastern(DateTime.UtcNow);
				HomeEntity.SaveChanges();
				var nextDraftPick = HomeEntity.DraftPicks.Where(d => d.DraftId == DraftId)
					.OrderBy(p => p.PickNum).FirstOrDefault(p => p.PlayerId == null);
				if (nextDraftPick != null)
				{
					var currentTime = DateTime.Now;
					nextDraftPick.PickStartDateTime = GetCurrentTimeEastern(DateTime.UtcNow.AddSeconds(3));
					HomeEntity.SaveChanges();
				}
			}
		}

		public string GetTeamName(int ownerId)
		{
			string teamName = "N/A";
			var ownerUser = DraftOwnerUsers.FirstOrDefault(ou => ou.OwnerId == ownerId);
			if (ownerUser != null)
			{
				teamName = ownerUser.TeamName;
			}
			return teamName;
		}

		public bool IsDraftingUserLoggedIn()
		{
			if (CurrentDraftPick != null)
			{
				var loggedInUser = DraftOwnerUsers.FirstOrDefault(ou => ou.UserName == Utilities.GetLoggedInUserName());
				var currentDraftPickUser = DraftOwnerUsers.FirstOrDefault(ou => ou.OwnerId == CurrentDraftPick.OwnerId);
				if (loggedInUser != null && currentDraftPickUser != null)
				{
					return loggedInUser.UserId == currentDraftPickUser.UserId;
				}
			}
			return false;
		}

		public void GetPreviousDraftPick(string draftPickId)
		{
			DraftPick currentDraftPick = GetSafeCurrentDraftPick(draftPickId);
			var previousDraftPick = DraftPicks.OrderByDescending(dp=>dp.PickNum)
				.FirstOrDefault(dp => dp.PickNum < currentDraftPick.PickNum);
			CurrentDraftPick = previousDraftPick ?? currentDraftPick;
			SetCurrentDraftPickOwnerUser();
			ResetCurrentPickPlayerModel();
		}

		public void GetNextDraftPick(string draftPickId)
		{
			DraftPick currentDraftPick = GetSafeCurrentDraftPick(draftPickId);
			var nextDraftPick = DraftPicks.OrderBy(dp=>dp.PickNum)
				.FirstOrDefault(dp => dp.PickNum > currentDraftPick.PickNum);
			CurrentDraftPick = nextDraftPick ?? currentDraftPick;
			SetCurrentDraftPickOwnerUser();
			ResetCurrentPickPlayerModel();
		}

		public void DeleteDraftPick(int draftPickId)
		{
			using (HomeEntity = new HomeEntity())
			{
				var draftPick = HomeEntity.DraftPicks.First(p => p.DraftPickId == draftPickId);
				draftPick.PlayerId = null;
				HomeEntity.SaveChanges();
			}
		}

		private DraftPick GetSafeCurrentDraftPick(string draftPickId)
		{
			DraftPick currentDraftPick;
			if (!string.IsNullOrEmpty(draftPickId))
			{
				currentDraftPick = DraftPicks.First(dp => dp.DraftPickId == Convert.ToInt32(draftPickId));
			}
			else
			{
				currentDraftPick = DraftPicks.OrderBy(p => p.PickNum).FirstOrDefault(p => p.PlayerId == null);
				if (currentDraftPick == null)
				{
					currentDraftPick = DraftPicks.OrderBy(p => p.PickNum).Last();
				}
			}
			return currentDraftPick;
		}

		public void PreloadPlayerModel(int playerId)
		{
			var player = Players.First(p => p.PlayerId == playerId);
			Player.FirstName = player.FirstName;
			Player.LastName = player.LastName;
			Player.Position = player.Position;
			Player.NFLTeam = player.NFLTeam;
			Player.PlayerId = player.PlayerId;
		}
	}
}