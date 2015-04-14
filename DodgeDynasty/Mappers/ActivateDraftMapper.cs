using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class ActivateDraftMapper<T> : MapperBase<T> where T : ActivateDraftModel, new()
	{
		protected override void PopulateModel()
		{
			using (HomeEntity = new Entities.HomeEntity())
			{
				Model.AllDrafts = HomeEntity.Drafts.OrderBy(d => d.DraftDate).ToList();
				Model.ActiveDrafts = Model.AllDrafts.Where(d => d.IsActive).ToList();
				Model.ScheduledDrafts = Model.AllDrafts.Where(d => !d.IsActive && !d.IsComplete).ToList();
				Model.CompleteDrafts = Model.AllDrafts.Where(d => !d.IsActive && d.IsComplete)
					.OrderByDescending(d => d.DraftDate).ToList();
			}
		}
	}
}