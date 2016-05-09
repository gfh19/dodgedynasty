using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Types
{
	public class RankedPlayer
	{
		public int PlayerId { get; set; }
		public int TruePlayerId { get; set; }
		public int RankId { get; set; }
		public int PlayerRankId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PlayerName { get; set; }
		public string NFLTeam { get; set; }
		public string NFLTeamDisplay { get; set; }
		public string Position { get; set; }
		public int? RankNum { get; set; }
		public int? PosRankNum { get; set; }
		public double? AvgRankNum { get; set; }
		public decimal? AuctionValue { get; set; }
		public string PickNum { get; set; }
		public string UserId { get; set; }
		public string NickName { get; set; }
		public string CssClass { get; set; }
		public string HighlightClass { get; set; }
		public string HighlightRankNum { get; set; }
	}
}