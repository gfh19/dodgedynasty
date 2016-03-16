using System.Linq;
using DodgeDynasty.Mappers.AdminShared;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers.Admin
{
	public class AdminManageDraftsMapper<T> : ManageDraftsMapper<T> where T : ManageDraftsModel, new()
	{
		protected override void GetAllAccessibleDrafts()
		{
			Model.LeagueDrafts = HomeEntity.Drafts.ToList();
		}
	}
}
