using System;

namespace DodgeDynasty.Models.Types
{
	public class AdjustedPlayer : RankedPlayer
	{
		public string Action { get; set; }
		public string UserFullName { get; set; }
		public string DraftsRanksText { get; set; }
		public bool IsActive { get; set; }
		public bool IsDrafted { get; set; }
		public DateTime AddTimestamp { get; set; }
	}
}
