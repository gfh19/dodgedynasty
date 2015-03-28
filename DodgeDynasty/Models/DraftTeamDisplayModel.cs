using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class DraftTeamDisplayModel : DraftDisplayModel
	{
		public List<DraftPick> TeamDraftPicks { get; set; }
		public List<Player> TeamPlayers { get; set; }
		
		public DraftTeamDisplayModel()
		{}

		public List<DraftPick> GetTeamDraftPicks(int ownerId)
		{
			TeamDraftPicks = DraftPicks.Where(dp=>dp.OwnerId == ownerId).OrderBy(dp=>dp.PickNum).ToList();
			var playerIds = TeamDraftPicks.Select(dp=>dp.PlayerId).ToList();
			TeamPlayers = DraftedPlayers.Where(p => playerIds.Contains(p.PlayerId)).ToList();
			return TeamDraftPicks;
		}

		public List<Owner> GetTeamDraftOwners()
		{
			return DraftOwners.OrderBy(o => o.OwnerId == CurrentLoggedInOwnerUser.OwnerId ? 1 : 2)
				.ThenBy(o => o.NickName).ToList();
		}
	}
}