using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class PlayerRankingsModel : IPlayerRankModel
	{
		public List<NFLTeam> NFLTeams { get; set; }
		public List<ByeWeek> ByeWeeks { get; set; }
		public List<Player> AllPlayers { get; set; }
		public List<Player> ActivePlayers { get; set; }
		public List<Player> DraftedPlayers { get; set; }
		public List<Position> Positions { get; set; }
		public List<League> Leagues { get; set; }
		public List<PlayerHighlight> CurrentPlayerHighlights { get; set; }

		public int? DraftId { get; set; }
		public int RankId { get; set; }
		public Rank CurrentRank { get; set; }
		public List<PlayerRank> PlayerRanks { get; set; }
		public List<RankedPlayer> RankedPlayers { get; set; }
		public List<RankedPlayer> OverallRankedPlayers { get; set; }
		public List<RankedPlayer> QBRankedPlayers { get; set; }
		public List<RankedPlayer> RBRankedPlayers { get; set; }
		public List<RankedPlayer> WRTERankedPlayers { get; set; }
		public List<RankedPlayer> WRRankedPlayers { get; set; }
		public List<RankedPlayer> TERankedPlayers { get; set; }
		public List<RankedPlayer> DEFRankedPlayers { get; set; }
		public List<RankedPlayer> KRankedPlayers { get; set; }
		public PlayerRankOptions Options { get; set; }
		public List<RankedPlayer> HighlightedPlayers { get; set; }
		public string CategoryRankHeader { get; set; }
		public string CompRankPosition { get; set; }

		#region Misc IDraftDataModel properties
		public int PickTimeSeconds { get; set; }
		public List<Draft> Drafts { get; set; }
		public List<DraftPick> DraftPicks { get; set; }
		public List<User> DraftUsers { get; set; }
		public List<OwnerUser> DraftOwnerUsers { get; set; }
		public List<User> Users { get; set; }
		public List<LeagueOwner> LeagueOwners { get; set; }
		public List<DraftOwner> AllDraftOwners { get; set; }
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

		#endregion Misc IDraftDataModel properties
	}
}
