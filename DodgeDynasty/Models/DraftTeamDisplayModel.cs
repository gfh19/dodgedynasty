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

		public DraftTeamDisplayModel(int? draftId = null) : base(draftId)
		{}

		public List<DraftPick> GetTeamDraftPicks(int userId)
		{
			TeamDraftPicks = DraftPicks.Where(dp => dp.UserId == userId).OrderBy(dp => dp.PickNum).ToList();
			var playerIds = TeamDraftPicks.Select(dp=>dp.PlayerId).ToList();
			TeamPlayers = DraftedPlayers.Where(p => playerIds.Contains(p.PlayerId)).ToList();
			return TeamDraftPicks;
		}

		public List<User> GetTeamDraftOwners()
		{
			return DraftUsers.OrderBy(o => o.UserId == CurrentLoggedInOwnerUser.UserId ? 1 : 2)
				.ThenBy(o => o.NickName).ToList();
		}
	}
}