using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers.Admin
{
	public class AdminManageLeaguesMapper<T> : MapperBase<T>, IManageLeaguesMapper<T> where T : ManageLeaguesModel, new()
	{
		protected override void PopulateModel()
		{
			Model.Leagues = HomeEntity.Leagues.ToList();
		}
	}
}