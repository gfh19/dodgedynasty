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
			Model.DraftId = Utilities.GetLatestUserDraftId(
				HomeEntity.Users.GetLoggedInUser(), commishDrafts, HomeEntity.DraftOwners.ToList());
		}
	}
}
