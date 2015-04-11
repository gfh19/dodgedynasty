using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class AddDraftMapper<T> : MapperBase<T> where T : AddDraftModel, new()
	{
		public int LeagueId { get; set; }

		public override void PopulateModel()
		{
			Model.LeagueId = LeagueId;
		}
	}
}