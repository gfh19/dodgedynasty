using System.Collections.Generic;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.Highlights
{
	public class PlayerHighlightModel
	{
		public int? DraftHighlightId { get; set; }
		public string QueueName { get; set; }
		public int PlayerId { get; set; }
		public int? RankNum { get; set; }
		public string HighlightClass { get; set; }
		public HighlightedPlayer Player { get; set; }
		public List<HighlightedPlayer> HighlightedPlayers { get; set; }
	}
}
