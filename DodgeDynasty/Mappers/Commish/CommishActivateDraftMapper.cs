using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.AdminShared;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Admin
{
	public class CommishActivateDraftMapper<T> : ActivateDraftMapper<T> where T : ActivateDraftModel, new()
	{
		protected override void SetDrafts()
		{
			var commishLeagueIds = DBUtilities.GetCommishLeagueIds();
			Model.AllDrafts = HomeEntity.Drafts.Where(d => commishLeagueIds.Contains(d.LeagueId)).OrderBy(d => d.DraftDate).ToList();
		}
	}
}