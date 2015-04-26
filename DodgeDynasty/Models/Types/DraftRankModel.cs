using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Types
{
	public class DraftRankModel
	{
		public int DraftRankId { get; set; }
		public int RankId { get; set; }
		public int? DraftId { get; set; }
		public bool? PrimaryDraftRanking { get; set; }
		public int? UserId { get; set; }
		public string RankName { get; set; }
		public short Year { get; set; }
		public DateTime RankDate { get; set; }
		public string Url { get; set; }
		public DateTime AddTimestamp { get; set; }
		public DateTime LastUpdateTimestamp { get; set; }
	}
}