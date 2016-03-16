using System.Linq;
using DodgeDynasty.Mappers.AdminShared;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Commish
{
	public class CommishManageDraftsMapper<T> : ManageDraftsMapper<T> where T : ManageDraftsModel, new()
	{
		protected override void GetAllAccessibleDrafts()
		{
			var commishLeagueIds = DBUtilities.GetCommishLeagueIds();
			Model.LeagueDrafts = HomeEntity.Drafts.Where(o => commishLeagueIds.Contains(o.LeagueId)).ToList();
		}
	}
}