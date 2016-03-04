using System.Collections.Generic;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.Highlights
{
	public class PlayerHighlightModel
	{
		public int PlayerId { get; set; }
		public int? RankNum { get; set; }
		public int HighlightId { get; set; }
		public HighlightedPlayer Player { get; set; }
		public List<HighlightedPlayer> HighlightedPlayers { get; set; }
	}
}
