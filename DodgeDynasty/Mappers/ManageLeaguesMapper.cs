using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class ManageLeaguesMapper<T> : MapperBase<T> where T : ManageLeaguesModel, new()
	{
		protected override void PopulateModel()
		{
			Model.AllLeagues = HomeEntity.Leagues.ToList();
		}
	}
}