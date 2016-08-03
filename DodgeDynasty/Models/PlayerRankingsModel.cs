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
		public List<RankedPlayer> DEFRankedPlayers { get; set; }
		public List<RankedPlayer> KRankedPlayers { get; set; }
		public PlayerRankOptions Options { get; set; }
		public List<RankedPlayer> HighlightedPlayers { get; set; }
		public string CategoryRankHeader { get; set; }
		public string CompRankPosition { get; set; }
	}
}
