using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers
{
	public class DraftHistoryMapper<T> : MapperBase<T> where T : DraftHistoryModel, new()
	{
		protected override void PopulateModel()
		{
			if (DBUtilities.IsUserAdmin())
			{
				Model.Leagues = HomeEntity.Leagues.ToList();
			}
			else
			{
				Model.Leagues = (from l in HomeEntity.Leagues.AsEnumerable()
								 join lo in HomeEntity.LeagueOwners.AsEnumerable() on l.LeagueId equals lo.LeagueId
								 where lo.UserId == HomeEntity.Users.GetLoggedInUserId() && lo.IsActive
								 select l).ToList();
			}
			Model.AllDrafts = HomeEntity.Drafts.ToList();
			//Remove Example League for all but Admin
			if (!DBUtilities.IsUserAdmin())
			{
				Model.Leagues.RemoveAll(o => o.LeagueId == Constants.Test.ExampleLeagueId);
				Model.AllDrafts.RemoveAll(o => o.LeagueId == Constants.Test.ExampleLeagueId);
			}
			Model.AllDraftPicks = HomeEntity.DraftPicks.ToList();
			Model.AllUsers = HomeEntity.Users.ToList();
		}
	}
}