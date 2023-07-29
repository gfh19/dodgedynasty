using DodgeDynasty.Models.Site;
using System.Linq;

namespace DodgeDynasty.Mappers.Site
{
	public class SiteConfigVarMapper : MapperBase<SiteConfigVarListModel>
	{
		protected override void PopulateModel()
		{
			Model.List = HomeEntity.SiteConfigVars.Select(v => new SiteConfigVarModel { VarName = v.VarName, VarValue = v.VarValue }).ToList();
		}
	}
}