using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public abstract class ManageDraftsMapper<T> : MapperBase<T> where T : ManageDraftsModel, new()
	{
		public string LeagueId { get; set; }

		protected override void PopulateModel()
		{
			if (!string.IsNullOrEmpty(LeagueId))
			{
				GetDraftsForLeagueId();
            }
			else
			{
				GetAllAccessibleDrafts();
			}
		}

		protected virtual void GetDraftsForLeagueId()
		{
			Model.LeagueId = Int32.Parse(LeagueId);
			Model.LeagueDrafts = HomeEntity.Drafts.Where(d => d.LeagueId == Model.LeagueId).ToList();
		}

		protected abstract void GetAllAccessibleDrafts();
	}
}