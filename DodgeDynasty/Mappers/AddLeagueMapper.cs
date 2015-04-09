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
	public class AddLeagueMapper<T> : MapperBase<T> where T : AddLeagueModel, new()
	{
		public override void PopulateModel()
		{
			Model.OwnerUsers = GetOwnerUsers();
			Model.ActiveOwnerUsers = Model.OwnerUsers.Where(o => o.IsActive).ToList();
			var numOwners = Int32.Parse(
				ConfigurationManager.AppSettings[Constants.AppSettings.DefaultNumOwners] ?? "4");
			Model.LeagueOwnerUsers = new List<OwnerUser>();
			for (int i = 0; i < numOwners; i++)
			{
				Model.LeagueOwnerUsers.Add(new OwnerUser { IsActive=true });
			}
		}

		public override void DoUpdate(T model)
		{
			League league = new Entities.League
			{
				LeagueName = model.LeagueName,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.Leagues.AddObject(league);
			HomeEntity.SaveChanges();

			foreach (var lo in model.LeagueOwnerUsers)
			{
				var ou = (from o in HomeEntity.Owners.AsEnumerable()
						  join u in HomeEntity.Users.AsEnumerable() on o.UserId equals u.UserId
						  where u.UserId == lo.UserId
						  select OwnerUserMapper.GetOwnerUser(o, u, null)).FirstOrDefault();
				lo.OwnerId = ou.OwnerId;
				lo.CssClass = ou.UserName;
				//Someday add more in depth CssClass assignment & lookup i.e. CssClass SQL table)

				//TODO:  Consolidate into one table!
				LeagueOwner owner = new LeagueOwner
				{
					OwnerId = lo.OwnerId,
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

		private List<OwnerUser> GetOwnerUsers()
		{
			var leagueOwners = HomeEntity.LeagueOwners.ToList();
			var ownerUsers = from o in HomeEntity.Owners.AsEnumerable()
							 join u in HomeEntity.Users.AsEnumerable() on o.UserId equals u.UserId
							 select OwnerUserMapper.GetOwnerUser(o, u,
								leagueOwners.Where(l => l.UserId == l.UserId)
								.OrderByDescending(l => l.IsActive).FirstOrDefault());
			return ownerUsers.ToList();
		}
	}
}