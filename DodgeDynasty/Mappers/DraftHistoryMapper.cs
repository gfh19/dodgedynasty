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
			if (Utilities.IsUserAdmin())
			{
				Model.Leagues = HomeEntity.Leagues.ToList();
			}
			else
			{
				Model.Leagues = (from l in HomeEntity.Leagues.AsEnumerable()
								 join lo in HomeEntity.LeagueOwners.AsEnumerable() on l.LeagueId equals lo.LeagueId
								 where lo.UserId == Utilities.GetLoggedInUserId(HomeEntity.Users.AsEnumerable())
								 select l).ToList();
			}
			Model.AllDrafts = HomeEntity.Drafts.ToList();
			Model.AllUsers = HomeEntity.Users.ToList();
		}
	}
}