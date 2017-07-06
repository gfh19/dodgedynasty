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
	public interface IDraftDataModel
	{
		#region Properties
		int? DraftId { get; set; }
		int PickTimeSeconds { get; set; }

		List<Draft> Drafts { get; set; }
		List<DraftPick> DraftPicks { get; set; }
		List<User> DraftUsers { get; set; }
		List<OwnerUser> DraftOwnerUsers { get; set; }
		List<User> Users { get; set; }
		List<LeagueOwner> LeagueOwners { get; set; }
		List<NFLTeam> NFLTeams { get; set; }
		List<ByeWeek> ByeWeeks { get; set; }
		List<Player> AllPlayers { get; set; }
		List<Player> ActivePlayers { get; set; }
		List<Player> DraftedPlayers { get; set; }
		List<Position> Positions { get; set; }
		List<League> Leagues { get; set; }
		List<DraftOwner> AllDraftOwners { get; set; }

		List<PlayerHighlight> CurrentPlayerHighlights { get; set; }
		Draft CurrentDraft { get; set; }
		List<LeagueOwner> CurrentLeagueOwners { get; set; }
		DraftPick CurrentDraftPick { get; set; }
		DraftPick PreviousDraftPick { get; set; }
		DraftPick SecondPreviousDraftPick { get; set; }
		DraftPick NextDraftPick { get; set; }
		OwnerUser CurrentClockOwnerUser { get; set; }
		OwnerUser CurrentLoggedInOwnerUser { get; set; }
		int CurrentUserId { get; set; }

		PlayerContext CurrentGridPlayer { get; set; }
		OwnerUser CurrentGridOwnerUser { get; set; }
		int CurrentRoundNum { get; set; }

		List<DraftRank> DraftRanks { get; set; }
		List<Rank> Ranks { get; set; }
		#endregion Properties
	}
}
