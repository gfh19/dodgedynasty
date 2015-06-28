using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace DodgeDynasty.Models
{
	public class DraftModel : ModelBase
	{
		private int? _latestOwnerDraftId;

		public int? DraftId { get; set; }
		public int PickTimeSeconds { get; set; }

		public List<Draft> Drafts { get; set; }
		public List<DraftPick> DraftPicks { get; set; }
		public List<User> DraftUsers { get; set; }
		public List<OwnerUser> DraftOwnerUsers { get; set; }
		public List<User> Users { get; set; }
		public List<LeagueOwner> LeagueOwners { get; set; }
		public List<NFLTeam> NFLTeams { get; set; }
		public List<Player> Players { get; set; }
		public List<Player> DraftedPlayers { get; set; }
		public List<Position> Positions { get; set; }
		public List<League> Leagues { get; set; }

		//public int? LatestOwnerDraftId
		//{
		//	get
		//	{
		//		if (_latestOwnerDraftId == null)
		//		{
		//			using (HomeEntity = new HomeEntity())
		//			{
		//				var user = HomeEntity.Users.GetLoggedInUser();
		//				_latestOwnerDraftId = GetCurrentDraftId(user);
		//			}
		//		}
		//		return _latestOwnerDraftId;
		//	}
		//}
		public Draft CurrentDraft { get; set; }
		public List<LeagueOwner> CurrentLeagueOwners { get; set; }
		public DraftPick CurrentDraftPick { get; set; }
		public DraftPick PreviousDraftPick { get; set; }
		public DraftPick NextDraftPick { get; set; }
		public OwnerUser CurrentClockOwnerUser { get; set; }
		public OwnerUser CurrentLoggedInOwnerUser { get; set; }

		public Player CurrentGridPlayer { get; set; }
		public OwnerUser CurrentGridOwnerUser { get; set; }
		public int CurrentRoundNum { get; set; }

		public List<DraftRank> DraftRanks { get; set; }
		public List<Rank> Ranks { get; set; }

		public DraftModel() : this(null)
		{ }

		public DraftModel(int? draftId)
		{
			DraftId = draftId;
			int seconds;
			PickTimeSeconds = int.TryParse(ConfigurationManager.AppSettings["PickTimeSeconds"], out seconds) ? seconds : 0;
		}

		public Draft GetCurrentDraft(int? draftId = null)
		{
			GetDraftInfo(draftId);
			return Drafts.First(d => d.DraftId == DraftId);
		}

		public void GetDraftInfo(int? draftId = null)
		{
			if (Drafts == null)
			{
				using (HomeEntity = new HomeEntity())
				{
					Drafts = HomeEntity.Drafts.ToList();
					NFLTeams = HomeEntity.NFLTeams.ToList();
					Players = HomeEntity.Players.Where(p => p.IsActive).ToList();
					Positions = HomeEntity.Positions.ToList();
					Leagues = HomeEntity.Leagues.ToList();
					Users = HomeEntity.Users.ToList();
					LeagueOwners = HomeEntity.LeagueOwners.ToList();
					DraftRanks = HomeEntity.DraftRanks.ToList();
					Ranks = HomeEntity.Ranks.ToList();

					SetCurrentDraftInfo(draftId);
				}
			}
		}

		private int? SetCurrentDraft(int? draftId)
		{
			var userName = Utilities.GetLoggedInUserName();
			var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == userName);
			DraftId = GetCurrentDraftId(user, draftId);
			CurrentDraft = Drafts.First(d => d.DraftId == DraftId);
			CurrentLeagueOwners = LeagueOwners.Where(lo => lo.LeagueId == CurrentDraft.LeagueId).ToList();
			var leagueOwner = CurrentLeagueOwners.FirstOrDefault(lo => lo.UserId == user.UserId);
			CurrentLoggedInOwnerUser = OwnerUserMapper.GetOwnerUser(user, leagueOwner);
			return DraftId;
		}

		public int GetCurrentDraftId(User user, int? draftId)
		{
			if (draftId != null)
			{
				return draftId.Value;
			}
			var draftOwners = HomeEntity.DraftOwners.ToList();
			return Utilities.GetLatestUserDraftId(user, Drafts, draftOwners);
		}

		private void SetCurrentDraftInfo(int? draftId = null)
		{
			DraftId = SetCurrentDraft(draftId);

			DraftPicks = HomeEntity.DraftPicks.Where(d => d.DraftId == DraftId).OrderBy(d => d.PickNum).ToList();

			var draftUserIds = HomeEntity.DraftOwners.Where(d => d.DraftId == DraftId).Select(d => d.UserId).ToList();
			DraftUsers = HomeEntity.Users.Where(u => draftUserIds.Contains(u.UserId)).OrderBy(o => o.NickName).ToList();
			DraftOwnerUsers = GetOwnerUsers(DraftUsers);

			DraftPicks = GetDraftPicks();
			var playerIds = DraftPicks.Select(dp => dp.PlayerId).ToList();
			DraftedPlayers = HomeEntity.Players.Where(p =>
				playerIds.Contains(p.PlayerId)).ToList();
			Users = HomeEntity.Users.ToList();
			CurrentDraftPick = DraftPicks.OrderBy(p => p.PickNum)
				.FirstOrDefault(p => p.PlayerId == null);
			if (CurrentDraftPick != null)
			{
				var currentPickNum = CurrentDraftPick.PickNum;
				PreviousDraftPick = DraftPicks.FirstOrDefault(p => p.PickNum == (currentPickNum - 1));
				NextDraftPick = DraftPicks.FirstOrDefault(p => p.PickNum == (currentPickNum + 1));
				var currentClockUser = Users.FirstOrDefault(u => u.UserId == CurrentDraftPick.UserId);
				var currentClockLeagueOwner = CurrentLeagueOwners.FirstOrDefault(lo => lo.UserId == CurrentDraftPick.UserId);
				CurrentClockOwnerUser = OwnerUserMapper.GetOwnerUser(currentClockUser, currentClockLeagueOwner);
			}
		}

		public void ResetCurrentDraft()
		{
			Drafts = null;
			GetDraftInfo();
		}

		private List<OwnerUser> GetOwnerUsers(List<User> draftUsers)
		{
			var ownerUsers = from o in draftUsers
							 join u in Users on o.UserId equals u.UserId
							 join lo in CurrentLeagueOwners on u.UserId equals lo.UserId
							 select OwnerUserMapper.GetOwnerUser(u, lo);
			return ownerUsers.ToList();
		}

		public List<DraftPick> GetDraftPicks()
		{
			GetDraftInfo();
			return DraftPicks.ToList();
		}

		public void SetCurrentGridPlayer(int? playerId)
		{
			CurrentGridPlayer = GetPlayer(playerId);
		}

		public Player GetPlayer(int? playerId)
		{
			Player player;
			if (playerId != null)
			{
				player = DraftedPlayers.First(p => p.PlayerId == playerId);
			}
			else
			{
				player = new Player();
			}
			return player;
		}

		public void SetCurrentGridOwnerUser(int userId)
		{
			var currentGridUser = Users.First(u => u.UserId == userId);
			var currentGridLeagueOwner = CurrentLeagueOwners.First(lo => lo.UserId == currentGridUser.UserId);
			CurrentGridOwnerUser = OwnerUserMapper.GetOwnerUser(currentGridUser, currentGridLeagueOwner);
		}

		public string ShowCurrentGridPlayerInfo()
		{
			string playerInfo = string.Empty;
			if (!string.IsNullOrEmpty(CurrentGridPlayer.Position))
			{
				playerInfo = string.Format("<span class=\"player-info\">{0}-<span class=\"pos\">{1}</span></span>",
					CurrentGridPlayer.NFLTeam, CurrentGridPlayer.Position);
			}
			return playerInfo;
		}

		public int GetPickCountUntilNextTurn(string userName)
		{
			if (CurrentDraftPick != null)
			{
				if (CurrentClockOwnerUser.UserName == userName)
				{
					return 0;
				}
				var ownerUser = DraftOwnerUsers.FirstOrDefault(ou => ou.UserName == userName);
				var nextUserPick = 
					DraftPicks.FirstOrDefault(p => p.PickNum > CurrentDraftPick.PickNum && p.UserId == ownerUser.UserId);
				if (nextUserPick != null)
				{
					return nextUserPick.PickNum - CurrentDraftPick.PickNum;
				}
			}
			return -1;
		}

		public string GetCurrentDraftName()
		{
			var league = GetCurrentLeague();
			return string.Format("{0} {1}", CurrentDraft.DraftYear, league.LeagueName);
		}

		public League GetCurrentLeague()
		{
			var league = Leagues.First(l => l.LeagueId == CurrentDraft.LeagueId);
			return league;
		}

		public string GetNFLTeamDisplay(string nflTeamAbbr)
		{
			var nflTeam = NFLTeams.First(t => t.TeamAbbr == nflTeamAbbr);
			return nflTeam.AbbrDisplay;
		}

		public bool IsDraftActive()
		{
			if (CurrentDraft != null)
			{
				return CurrentDraft.IsActive;
			}
			return false;
		}

		public bool IsUserTurn()
		{
			var currentUserName = DodgeDynasty.Shared.Utilities.GetLoggedInUserName();
			return (CurrentClockOwnerUser != null && CurrentClockOwnerUser.UserName == currentUserName);
		}

		public bool IsLoggedInUser(string userId)
		{
			if (!string.IsNullOrEmpty(userId)) {
				return CurrentLoggedInOwnerUser.UserId.ToString() == userId;
			}
			return false;
		}

		public string UseTimeZone() {
			if (DateTime.Now.AddMinutes(55) > CurrentDraft.DraftDate) {
				return "true";
			}
			return "false";
		}

		public DateTime GetCurrentTimeEastern(DateTime utcTime)
		{
			TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, easternZone);
			return easternTime;
		}

		public List<SelectListItem> GetPositionListItems()
		{
			return Utilities.GetListItems<Position>(Positions,
				p => (string.Format("{0} ({1})", p.PosCode, p.PosDesc)), p => p.PosCode);
		}

		public List<SelectListItem> GetNFLListItems()
		{
			return Utilities.GetListItems<NFLTeam>(NFLTeams,
				t => (string.Format("{0} ({1} {2})", t.AbbrDisplay, t.LocationName, t.TeamName)), t => t.AbbrDisplay);
		}

		public string GetPlayerHints(bool excludeDrafted)
		{
			using (HomeEntity = new HomeEntity())
			{
				StringBuilder playerHints = new StringBuilder("[");
				var draftedPlayers = DraftPicks.Select(p => p.PlayerId).ToList();
				foreach (var player in Players)
				{
					if (!excludeDrafted || !draftedPlayers.Contains(player.PlayerId))
					{
						var nflTeam = NFLTeams.First(t => t.TeamAbbr == player.NFLTeam);
						playerHints.Append(string.Format(
							"{{value:\"{0} {1} {3}-{4}\",firstName:\"{0}\",lastName:\"{1}\",nflTeam:\"{2}\",nflTeamDisplay:\"{3}\",pos:\"{4}\"}},",
							Utilities.JsonEncode(player.FirstName), Utilities.JsonEncode(player.LastName), Utilities.JsonEncode(nflTeam.TeamAbbr),
								Utilities.JsonEncode(nflTeam.AbbrDisplay), Utilities.JsonEncode(player.Position)));
					}
				}
				playerHints.Append("]");
				return playerHints.ToString();
			}
		}
	}
}