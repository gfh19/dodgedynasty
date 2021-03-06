﻿using DodgeDynasty.Models.PlayerAdjustments;
using System.Linq;
using WebGrease.Css.Extensions;

namespace DodgeDynasty.Mappers.PlayerAdjustments
{
	public class InactivatePlayersMapper : MapperBase<InactivatePlayersModel>
	{
		protected override void DoUpdate(InactivatePlayersModel model)
		{
			HomeEntity.Players.ForEach(o => o.IsActive = false);
			if (model.PlayerGroup == "def")
			{
				HomeEntity.Players.Join(HomeEntity.NFLTeams.Where(o=>o.IsActive), p=>p.NFLTeam, t=>t.TeamAbbr, (p, t) => p)
					.Where(o => o.Position == "DEF" && o.NFLTeam != "FA")
					.ForEach(o=>o.IsActive = true);
			}
			HomeEntity.SaveChanges();
		}
	}
}
