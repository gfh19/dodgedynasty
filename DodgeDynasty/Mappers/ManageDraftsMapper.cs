using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class ManageDraftsMapper<T> : MapperBase<T> where T : ManageDraftsModel, new()
	{
		public string LeagueId { get; set; }

		protected override void PopulateModel()
		{
			if (!string.IsNullOrEmpty(LeagueId))
			{
				Model.LeagueId = Int32.Parse(LeagueId);
				Model.LeagueDrafts = HomeEntity.Drafts.Where(d => d.LeagueId == Model.LeagueId).ToList();
			}
			else
			{
				Model.LeagueDrafts = HomeEntity.Drafts.ToList();
			}
		}
	}
}