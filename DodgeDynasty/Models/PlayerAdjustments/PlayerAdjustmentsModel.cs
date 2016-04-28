using System.Collections.Generic;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models.PlayerAdjustments
{
	public class PlayerAdjustmentsModel
	{
		public List<AdjustedPlayer> AddedPlayers { get; set; }
		public List<AdjustedPlayer> OtherAdjPlayers { get; set; }
		public List<AdjustedPlayer> NonUniquePlayers { get; set; }
		public List<AdjustedPlayer> DuplicateActivePlayers { get; set; }
		public List<int> NewAdjustmentIds { get; set; }
		public List<NFLTeam> NFLTeams { get; set; }
		public List<Position> Positions { get; set; }
		public List<Player> AllPlayers { get; set; }
		public int ActivePlayerCount { get; set; }
		public int InactivePlayerCount { get; set; }
		public AdminPlayerModel Player { get; set; }

		public List<SelectListItem> GetPositionListItems()
		{
			return Utilities.GetListItems<Position>(Positions,
				p => (string.Format("{0} ({1})", p.PosCode, p.PosDesc)), p => p.PosCode);
		}

		public List<SelectListItem> GetNFLListItems()
		{
			return Utilities.GetListItems<NFLTeam>(NFLTeams,
				t => (string.Format("{0} ({1} {2})", t.AbbrDisplay, t.LocationName, t.TeamName)), t => t.AbbrDisplay);
		}

		public string GetPlayerHints()
		{
			return Utilities.GetAutoCompleteTruePlayerHints(AllPlayers, NFLTeams);
		}
	}
}
