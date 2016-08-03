using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public interface IPlayerRankModel : IDraftModel
	{
		int RankId { get; set; }
		Rank CurrentRank { get; set; }
		List<PlayerRank> PlayerRanks { get; set; }
		List<RankedPlayer> RankedPlayers { get; set; }
		List<RankedPlayer> OverallRankedPlayers { get; set; }
		List<RankedPlayer> QBRankedPlayers { get; set; }
		List<RankedPlayer> RBRankedPlayers { get; set; }
		List<RankedPlayer> WRTERankedPlayers { get; set; }
		List<RankedPlayer> DEFRankedPlayers { get; set; }
		List<RankedPlayer> KRankedPlayers { get; set; }
		PlayerRankOptions Options { get; set; }
		List<RankedPlayer> HighlightedPlayers { get; set; }
		string CategoryRankHeader { get; set; }
		string CompRankPosition { get; set; }
	}
}
