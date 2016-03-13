using System.Linq;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Commish
{
	public class CommishManageLeaguesMapper<T> : MapperBase<T>, IManageLeaguesMapper<T> where T : ManageLeaguesModel, new()
	{
		protected override void PopulateModel()
		{
			var commishLeagueIds = DBUtilities.GetCommishLeagueIds();
            Model.Leagues = HomeEntity.Leagues.Where(o=> commishLeagueIds.Contains(o.LeagueId)).ToList();
		}
	}
}