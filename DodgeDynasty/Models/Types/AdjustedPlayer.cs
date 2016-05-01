using System;
using System.Collections.Generic;
using DodgeDynasty.Models.PlayerAdjustments;

namespace DodgeDynasty.Models.Types
{
	public class AdjustedPlayer : RankedPlayer
	{
		public int AdjustmentId { get; set; }
		public string Action { get; set; }
		public string UserFullName { get; set; }
		public List<DraftsRanksTextModel> DraftsRanks { get; set; }
		public bool IsActive { get; set; }
		public bool IsDrafted { get; set; }
		public DateTime AddTimestamp { get; set; }
	}
}
