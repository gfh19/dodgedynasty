using System.Collections.Generic;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Drafts;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Commish
{
	public class CommishCurrentDraftMapper : MapperBase<SingleDraftModel>
	{
		protected override void PopulateModel()
		{
			var commishLeagueIds = DBUtilities.GetCommishLeagueIds();
			List<Draft> commishDrafts = new List<Draft>();
			foreach (var leagueId in commishLeagueIds)
			{
				commishDrafts.AddRange(HomeEntity.Drafts.Where(o => o.LeagueId == leagueId));
			}
			var userId = HomeEntity.Users.GetLoggedInUserId();
			Model.DraftId = Utilities.GetLatestUserDraftId(
				userId, commishDrafts, HomeEntity.DraftOwners.ToList(),
				HomeEntity.UserRoles.Where(o => o.UserId == userId).ToList());
		}
	}
}
