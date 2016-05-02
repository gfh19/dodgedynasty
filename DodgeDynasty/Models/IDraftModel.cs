using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public interface IDraftModel
	{
		int? DraftId { get; set; }
		List<NFLTeam> NFLTeams { get; set; }
		List<ByeWeek> ByeWeeks { get; set; }
		List<Player> AllPlayers { get; set; }
		List<Player> ActivePlayers { get; set; }
		List<Player> DraftedPlayers { get; set; }
		List<Position> Positions { get; set; }
		List<League> Leagues { get; set; }
		List<PlayerHighlight> CurrentPlayerHighlights { get; set; }
	}
}
