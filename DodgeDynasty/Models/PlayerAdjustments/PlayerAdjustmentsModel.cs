using System.Collections.Generic;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.PlayerAdjustments
{
	public class PlayerAdjustmentsModel
	{
		public List<AdjustedPlayer> AddedPlayers { get; set; }
		public List<AdjustedPlayer> OtherAdjPlayers { get; set; }
	}
}
