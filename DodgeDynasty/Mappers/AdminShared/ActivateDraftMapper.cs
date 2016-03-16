using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers.AdminShared
{
	public abstract class ActivateDraftMapper<T> : MapperBase<T> where T : ActivateDraftModel, new()
	{
		protected override void PopulateModel()
		{
			SetDrafts();
			Model.ActiveDrafts = Model.AllDrafts.Where(d => d.IsActive).ToList();
			Model.ScheduledDrafts = Model.AllDrafts.Where(d => !d.IsActive && !d.IsComplete).ToList();
			Model.CompleteDrafts = Model.AllDrafts.Where(d => !d.IsActive && d.IsComplete)
				.OrderByDescending(d => d.DraftDate).ToList();
		}

		protected abstract void SetDrafts();
	}
}