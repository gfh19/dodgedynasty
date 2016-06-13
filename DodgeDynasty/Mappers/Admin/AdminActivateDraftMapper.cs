using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.AdminShared;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers.Admin
{
	public class AdminActivateDraftMapper<T> : ActivateDraftMapper<T> where T : ActivateDraftModel, new()
	{
		protected override void SetDrafts()
		{
			Model.AllDrafts = HomeEntity.Drafts.OrderBy(d => d.DraftDate).ToList();
		}
	}
}