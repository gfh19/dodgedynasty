using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.Drafts;

namespace DodgeDynasty.Mappers.Drafts
{
	public class DraftViewMapper : MapperBase<DraftViewModel>
	{
		public int DraftId { get; set; }

		public DraftViewMapper(int draftId)
		{
			DraftId = draftId;
		}

		protected override void PopulateModel()
		{
			Model.DraftId = DraftId;
			Model.DraftPicks = HomeEntity.DraftPicks.Where(o => o.DraftId == DraftId).ToList();
		}
	}
}
