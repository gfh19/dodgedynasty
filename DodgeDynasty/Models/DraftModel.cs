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
	public class DraftModel : ModelBase, IDraftModel
	{
		public int? DraftId { get; set; }
		public int PickTimeSeconds { get; set; }

		public List<Draft> Drafts { get; set; }
		public List<DraftPick> DraftPicks { get; set; }
		public List<User> DraftUsers { get; set; }
		public List<OwnerUser> DraftOwnerUsers { get; set; }
		public List<User> Users { get; set; }
		public List<LeagueOwner> LeagueOwners { get; set; }
		public List<NFLTeam> NFLTeams { get; set; }
		public List<ByeWeek> ByeWeeks { get; set; }
		public List<Player> AllPlayers { get; set; }
		public List<Player> ActivePlayers { get; set; }
		public List<Player> DraftedPlayers { get; set; }
		public List<Position> Positions { get; set; }
		public List<League> Leagues { get; set; }
		public List<DraftOwner> AllDraftOwners { get; set; }

		public List<PlayerHighlight> CurrentPlayerHighlights { get; set; }
		public Draft CurrentDraft { get; set; }
		public List<LeagueOwner> CurrentLeagueOwners { get; set; }
		public DraftPick CurrentDraftPick { get; set; }
		public DraftPick PreviousDraftPick { get; set; }
		public DraftPick SecondPreviousDraftPick { get; set; }
		public DraftPick NextDraftPick { get; set; }
		public OwnerUser CurrentClockOwnerUser { get; set; }
		public OwnerUser CurrentLoggedInOwnerUser { get; set; }
		public int CurrentUserId { get; set; }

		public PlayerContext CurrentGridPlayer { get; set; }
		public OwnerUser CurrentGridOwnerUser { get; set; }
		public int CurrentRoundNum { get; set; }

		public List<DraftRank> DraftRanks { get; set; }
		public List<Rank> Ranks { get; set; }

		public DraftModel() : this(null)
		{ }

		public DraftModel(int? draftId)
		{
			DraftId = draftId;
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
					ByeWeeks = HomeEntity.ByeWeeks.ToList();
					AllPlayers = HomeEntity.Players.ToList();
					ActivePlayers = AllPlayers.Where(p => p.IsActive).ToList();
					Positions = HomeEntity.Positions.ToList();
					Leagues = HomeEntity.Leagues.ToList();
					Users = HomeEntity.Users.ToList();
					LeagueOwners = HomeEntity.LeagueOwners.ToList();
					AllDraftOwners = HomeEntity.DraftOwners.ToList();
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
			PickTimeSeconds = CurrentDraft.PickTimeSeconds;
            return DraftId;
		}

		public int GetCurrentDraftId(int? draftId = null)
		{
			if (draftId != null)
			{
				return draftId.Value;
			}
			return GetCurrentDraftId(Users.GetLoggedInUser(), draftId);
		}

		public int GetCurrentDraftId(User user, int? draftId=null)
		{
			if (draftId != null)
			{
				return draftId.Value;
			}
			return Utilities.GetLatestUserDraftId(user, Drafts, AllDraftOwners);
		}

		private void SetCurrentDraftInfo(int? draftId = null)
		{
			CurrentUserId = Users.GetLoggedInUserId();
            DraftId = SetCurrentDraft(draftId);

			DraftPicks = HomeEntity.DraftPicks.Where(d => d.DraftId == DraftId).OrderBy(d => d.PickNum).ToList();

			var draftUserIds = AllDraftOwners.Where(d => d.DraftId == DraftId).Select(d => d.UserId).ToList();
			DraftUsers = HomeEntity.Users.Where(u => draftUserIds.Contains(u.UserId)).OrderBy(o => o.NickName).ToList();
			DraftOwnerUsers = GetOwnerUsers(DraftUsers);

			var playerIds = DraftPicks.Select(dp => dp.PlayerId).ToList();
			DraftedPlayers = HomeEntity.Players.Where(p =>
				playerIds.Contains(p.PlayerId)).ToList();
			CurrentDraftPick = DraftPicks.OrderBy(p => p.PickNum)
				.FirstOrDefault(p => p.PlayerId == null);
			CurrentPlayerHighlights = HomeEntity.PlayerHighlights
				.Where(o => o.DraftId == DraftId && o.UserId == CurrentUserId).OrderBy(o=>o.RankNum).ToList();
			if (CurrentDraftPick != null)
			{
				var currentPickNum = CurrentDraftPick.PickNum;
				SecondPreviousDraftPick = DraftPicks.FirstOrDefault(p => p.PickNum == (currentPickNum - 2));
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
			var player = GetPlayer(playerId);
			CurrentGridPlayer = GetPlayerContext(player);
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

		public PlayerContext GetPlayerContext(Player player)
		{
			//Get Player with current year/draft context (just ByeWeek)
			var playerContext = new PlayerContext(player);
			playerContext.ByeWeek = GetNFLTeamByeWeek(player.NFLTeam);
			return playerContext;
		}

		public int? GetNFLTeamByeWeek(string nflTeam)
		{
			var byeWeek = ByeWeeks.FirstOrDefault(o => o.Year == CurrentDraft.DraftYear
				&& o.NFLTeam == nflTeam);
			if (byeWeek != null)
			{
				return byeWeek.WeekNum;
			}
			return null;
		}

		public bool AreYearByeWeeksFound()
		{
			return ByeWeeks.Any(o => o.Year == CurrentDraft.DraftYear);
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

		public int GetPickCountUntilNextTurn()
		{
			return GetPickCountUntilNextTurn(Utilities.GetLoggedInUserName());
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
				if (ownerUser != null)
				{
					var nextUserPick =
						DraftPicks.FirstOrDefault(p => p.PickNum > CurrentDraftPick.PickNum && p.UserId == ownerUser.UserId);
					if (nextUserPick != null)
					{
						return nextUserPick.PickNum - CurrentDraftPick.PickNum;
					}
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
			nflTeamAbbr = (!string.IsNullOrEmpty(nflTeamAbbr) ? nflTeamAbbr.ToUpper() : "");
			var nflTeam = NFLTeams.FirstOrDefault(t => t.TeamAbbr == nflTeamAbbr);
			return (nflTeam != null) ? nflTeam.AbbrDisplay : "";
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
				var draftedPlayers = DraftPicks.Select(p => p.PlayerId).ToList();

				var currentRankings = GetCurrentAvailableDraftRanks();
				List<PlayerRank> playerRankings = null;
				if (currentRankings.Count > 0)
				{
					var sortedRankings = currentRankings.OrderBy(r => r.UserId).ThenByDescending(r => r.PrimaryDraftRanking)
						.ThenBy(r => r.DraftId);
					var ranking = sortedRankings.FirstOrDefault();
					playerRankings = HomeEntity.PlayerRanks.Where(r => r.RankId == ranking.RankId).ToList();
				}
				var currentPlayersSorted = (playerRankings != null)
					? (from p in ActivePlayers
					   join r in playerRankings on p.PlayerId equals r.PlayerId into rkPlyrsLeft        //Left outer join
					   from r in rkPlyrsLeft.DefaultIfEmpty()
					   orderby (r != null ? r.RankNum : Int32.MaxValue), p.FirstName, p.LastName
					   select p).ToList()
					: ActivePlayers.OrderBy(p => p.FirstName).ThenBy(p => p.LastName).ToList();

				return Utilities.GetAutoCompletePlayerHints(currentPlayersSorted, NFLTeams, excludeDrafted, draftedPlayers);
			}
		}

		public List<DraftRankModel> GetCurrentAvailableDraftRanks()
		{
			var fullDraftRanks = from dr in DraftRanks
								 join r in Ranks on dr.RankId equals r.RankId
								 where ((dr.DraftId == null && r.Year == CurrentDraft.DraftYear) || dr.DraftId == DraftId)
								   && (dr.UserId == null || dr.UserId == CurrentLoggedInOwnerUser.UserId)
								 orderby dr.PrimaryDraftRanking descending, dr.UserId descending, dr.DraftId descending, r.RankName
								 select GetDraftRankModel(dr, r);
			return fullDraftRanks.ToList();
		}

		public static DraftRankModel GetDraftRankModel(DraftRank dr, Rank r)
		{
			return new DraftRankModel
			{
				DraftRankId = dr.DraftRankId,
				RankId = r.RankId,
				DraftId = dr.DraftId,
				PrimaryDraftRanking = dr.PrimaryDraftRanking,
				UserId = dr.UserId,
				RankName = r.RankName,
				Year = r.Year,
				RankDate = r.RankDate,
				Url = r.Url,
				AddTimestamp = r.AddTimestamp,
				LastUpdateTimestamp = r.LastUpdateTimestamp
			};
		}
	}
}