using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public abstract class DraftModelBase : IDraftModel
	{
		private IDraftModel _draftModel;
		public IDraftModel DraftModel
		{
			get
			{
				if (_draftModel == null)
				{
					_draftModel = DraftFactory.GetDraftModel();
				}
				return _draftModel;
			}
			set { _draftModel = value; }
		}
		public DraftModelBase()
		{ }
		public DraftModelBase(int? draftId)
		{
			DraftModel = DraftFactory.GetDraftModel(draftId);
		}
		public DraftModelBase(IDraftModel draftModel)
		{
			DraftModel = draftModel;
		}


		public List<Player> ActivePlayers
		{
			get { return DraftModel.ActivePlayers; }
			set { DraftModel.ActivePlayers = value; }
		}

		public List<DraftOwner> AllDraftOwners
		{
			get { return DraftModel.AllDraftOwners; }
			set { DraftModel.AllDraftOwners = value; }
		}

		public List<Player> AllPlayers
		{
			get { return DraftModel.AllPlayers; }
			set { DraftModel.AllPlayers = value; }
		}

		public List<ByeWeek> ByeWeeks
		{
			get { return DraftModel.ByeWeeks; }
			set { DraftModel.ByeWeeks = value; }
		}

		public OwnerUser CurrentClockOwnerUser
		{
			get { return DraftModel.CurrentClockOwnerUser; }
			set { DraftModel.CurrentClockOwnerUser = value; }
		}

		public Draft CurrentDraft
		{
			get { return DraftModel.CurrentDraft; }
			set { DraftModel.CurrentDraft = value; }
		}

		public DraftPick CurrentDraftPick
		{
			get { return DraftModel.CurrentDraftPick; }
			set { DraftModel.CurrentDraftPick = value; }
		}

		public OwnerUser CurrentGridOwnerUser
		{
			get { return DraftModel.CurrentGridOwnerUser; }
			set { DraftModel.CurrentGridOwnerUser = value; }
		}

		public PlayerContext CurrentGridPlayer
		{
			get { return DraftModel.CurrentGridPlayer; }
			set { DraftModel.CurrentGridPlayer = value; }
		}

		public List<LeagueOwner> CurrentLeagueOwners
		{
			get { return DraftModel.CurrentLeagueOwners; }
			set { DraftModel.CurrentLeagueOwners = value; }
		}

		public OwnerUser CurrentLoggedInOwnerUser
		{
			get { return DraftModel.CurrentLoggedInOwnerUser; }
			set { DraftModel.CurrentLoggedInOwnerUser = value; }
		}

		public int CurrentRoundNum
		{
			get { return DraftModel.CurrentRoundNum; }
			set { DraftModel.CurrentRoundNum = value; }
		}

		public int CurrentUserId
		{
			get { return DraftModel.CurrentUserId; }
			set { DraftModel.CurrentUserId = value; }
		}

		public List<Player> DraftedPlayers
		{
			get { return DraftModel.DraftedPlayers; }
			set { DraftModel.DraftedPlayers = value; }
		}

		public int? DraftId
		{
			get { return DraftModel.DraftId; }
			set { DraftModel.DraftId = value; }
		}

		public List<OwnerUser> DraftOwnerUsers
		{
			get { return DraftModel.DraftOwnerUsers; }
			set { DraftModel.DraftOwnerUsers = value; }
		}

		public List<DraftPick> DraftPicks
		{
			get { return DraftModel.DraftPicks; }
			set { DraftModel.DraftPicks = value; }
		}

		public List<DraftRank> DraftRanks
		{
			get { return DraftModel.DraftRanks; }
			set { DraftModel.DraftRanks = value; }
		}

		public List<Draft> Drafts
		{
			get { return DraftModel.Drafts; }
			set { DraftModel.Drafts = value; }
		}

		public List<User> DraftUsers
		{
			get { return DraftModel.DraftUsers; }
			set { DraftModel.DraftUsers = value; }
		}

		public List<LeagueOwner> LeagueOwners
		{
			get { return DraftModel.LeagueOwners; }
			set { DraftModel.LeagueOwners = value; }
		}

		public List<League> Leagues
		{
			get { return DraftModel.Leagues; }
			set { DraftModel.Leagues = value; }
		}

		public DraftPick NextDraftPick
		{
			get { return DraftModel.NextDraftPick; }
			set { DraftModel.NextDraftPick = value; }
		}

		public List<NFLTeam> NFLTeams
		{
			get { return DraftModel.NFLTeams; }
			set { DraftModel.NFLTeams = value; }
		}

		public int PickTimeSeconds
		{
			get { return DraftModel.PickTimeSeconds; }
			set { DraftModel.PickTimeSeconds = value; }
		}

		public List<Position> Positions
		{
			get { return DraftModel.Positions; }
			set { DraftModel.Positions = value; }
		}

		public DraftPick PreviousDraftPick
		{
			get { return DraftModel.PreviousDraftPick; }
			set { DraftModel.PreviousDraftPick = value; }
		}

		public List<Rank> Ranks
		{
			get { return DraftModel.Ranks; }
			set { DraftModel.Ranks = value; }
		}

		public DraftPick SecondPreviousDraftPick
		{
			get { return DraftModel.SecondPreviousDraftPick; }
			set { DraftModel.SecondPreviousDraftPick = value; }
		}

		public List<User> Users
		{
			get { return DraftModel.Users; }
			set { DraftModel.Users = value; }
		}


		public bool AreYearByeWeeksFound()
		{
			return DraftModel.AreYearByeWeeksFound();
		}

		public List<DraftRankModel> GetCurrentAvailableDraftRanks()
		{
			return DraftModel.GetCurrentAvailableDraftRanks();
		}

		public Draft GetCurrentDraft(int? draftId = null)
		{
			return DraftModel.GetCurrentDraft(draftId);
		}

		public int GetCurrentDraftId(int? draftId = null)
		{
			return DraftModel.GetCurrentDraftId(draftId);
		}

		public int GetCurrentDraftId(User user, int? draftId = null)
		{
			return DraftModel.GetCurrentDraftId(user, draftId);
		}

		public string GetCurrentDraftName()
		{
			return DraftModel.GetCurrentDraftName();
		}

		public League GetCurrentLeague()
		{
			return DraftModel.GetCurrentLeague();
		}

		public DateTime GetCurrentTimeEastern(DateTime utcTime)
		{
			return DraftModel.GetCurrentTimeEastern(utcTime);
		}

		public void GetDraftInfo(int? draftId = default(int?))
		{
			DraftModel.GetDraftInfo(draftId);
		}

		public List<DraftPick> GetDraftPicks()
		{
			return DraftModel.GetDraftPicks();
		}

		public List<SelectListItem> GetNFLListItems(bool showInactive = false)
		{
			return DraftModel.GetNFLListItems(showInactive);
		}

		public int? GetNFLTeamByeWeek(string nflTeam)
		{
			return DraftModel.GetNFLTeamByeWeek(nflTeam);
		}

		public string GetNFLTeamDisplay(string nflTeamAbbr)
		{
			return DraftModel.GetNFLTeamDisplay(nflTeamAbbr);
		}

		public int GetPickCountUntilNextTurn()
		{
			return DraftModel.GetPickCountUntilNextTurn();
		}

		public int GetPickCountUntilNextTurn(int userId)
		{
			return DraftModel.GetPickCountUntilNextTurn(userId);
		}

		public Player GetPlayer(int? playerId)
		{
			return DraftModel.GetPlayer(playerId);
		}

		public PlayerContext GetPlayerContext(Player player)
		{
			return DraftModel.GetPlayerContext(player);
		}

		public string GetPlayerHints(bool excludeDrafted)
		{
			return DraftModel.GetPlayerHints(excludeDrafted);
		}

		public List<SelectListItem> GetPositionListItems()
		{
			return DraftModel.GetPositionListItems();
		}

		public bool IsDraftActive()
		{
			return DraftModel.IsDraftActive();
		}

		public bool IsLoggedInUser(string userId)
		{
			return DraftModel.IsLoggedInUser(userId);
		}

		public bool IsUserTurn()
		{
			return DraftModel.IsUserTurn();
		}

		public void ResetCurrentDraft()
		{
			DraftModel.ResetCurrentDraft();
		}

		public void SetCurrentGridOwnerUser(int userId)
		{
			DraftModel.SetCurrentGridOwnerUser(userId);
		}

		public void SetCurrentGridPlayer(int? playerId)
		{
			DraftModel.SetCurrentGridPlayer(playerId);
		}

		public string ShowCurrentGridPlayerInfo()
		{
			return DraftModel.ShowCurrentGridPlayerInfo();
		}

		public string UseTimeZone()
		{
			return DraftModel.UseTimeZone();
		}
	}
}
