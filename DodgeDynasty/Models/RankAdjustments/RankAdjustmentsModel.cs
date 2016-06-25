using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models.RankAdjustments
{
	public class RankAdjustmentsModel
	{
		public List<AdminRankModel> PublicRanks { get; set; }
		public AdminRankModel Rank { get; set; }
		public List<AutoImport> AutoImports { get; set; }

		public List<SelectListItem> GetAutoImportChoices(int? autoImportId)
		{
			return Utilities.GetListItems<AutoImport>(AutoImports.OrderBy(o => o.AutoImportId).ToList(),
				o => string.Format("{0}-{1}", o.AutoImportId, o.RankName), o => o.AutoImportId.ToString(), true, 
				autoImportId.HasValue ? autoImportId.ToString() : null);
		}
	}
}
