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
	public interface IDraftModel : IDraftDataModel
	{
		#region Methods
		Draft GetCurrentDraft(int? draftId = null);
		void GetDraftInfo(int? draftId = null);
		int GetCurrentDraftId(int? draftId = null);
		int GetCurrentDraftId(User user, int? draftId = null);
		void ResetCurrentDraft();
		List<DraftPick> GetDraftPicks();
		void SetCurrentGridPlayer(int? playerId);
		Player GetPlayer(int? playerId);
		PlayerContext GetPlayerContext(Player player);
		int? GetNFLTeamByeWeek(string nflTeam);
		bool AreYearByeWeeksFound();
		void SetCurrentGridOwnerUser(int userId);
		string ShowCurrentGridPlayerInfo();
		int GetPickCountUntilNextTurn();
		int GetPickCountUntilNextTurn(int userId);
		string GetCurrentDraftName();
		League GetCurrentLeague();
		string GetNFLTeamDisplay(string nflTeamAbbr);
		bool IsDraftActive();
		bool IsUserTurn();
		bool IsLoggedInUser(string userId);
		string UseTimeZone();
		DateTime GetCurrentTimeEastern(DateTime utcTime);
		List<SelectListItem> GetPositionListItems();
		List<SelectListItem> GetNFLListItems();
		string GetPlayerHints(bool excludeDrafted);
		List<DraftRankModel> GetCurrentAvailableDraftRanks();

		#endregion Methods
	}
}
