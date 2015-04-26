using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers
{
	public class AddLeagueMapper<T> : MapperBase<T> where T : AddEditLeagueModel, new()
	{
		protected override void PopulateModel()
		{
			Model.OwnerUsers = OwnerUserMapper.GetOwnerUsers(HomeEntity.LeagueOwners.ToList(), HomeEntity.Users.ToList());
			Model.ActiveOwnerUsers = Model.OwnerUsers.Where(o => o.IsActive).ToList();
			var numOwners = Int32.Parse(
				ConfigurationManager.AppSettings[Constants.AppSettings.DefaultNumOwners] ?? "4");
			Model.LeagueOwnerUsers = new List<OwnerUser>();
			for (int i = 0; i < numOwners; i++)
			{
				Model.LeagueOwnerUsers.Add(new OwnerUser { IsActive=true });
			}
		}

		protected override void DoUpdate(T model)
		{
			League league = new Entities.League
			{
				LeagueName = model.LeagueName,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.Leagues.AddObject(league);
			HomeEntity.SaveChanges();

			model.LeagueId = league.LeagueId;

			foreach (var lo in model.LeagueOwnerUsers)
			{
				var user = (from u in HomeEntity.Users.AsEnumerable()
							where u.UserId == lo.UserId
							select u).FirstOrDefault();
				lo.UserId = user.UserId;
				lo.CssClass = user.UserName;
				//TODO:  Someday add more in depth CssClass assignment & lookup i.e. CssClass SQL table)

				//TODO:  Consolidate into one table!
				LeagueOwner owner = new LeagueOwner
				{
					UserId = lo.UserId,
					LeagueId = league.LeagueId,
					TeamName = lo.TeamName,
					CssClass = lo.CssClass,
					IsActive = lo.IsActive,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				HomeEntity.LeagueOwners.AddObject(owner);
			}
			HomeEntity.SaveChanges();
		}
	}
}