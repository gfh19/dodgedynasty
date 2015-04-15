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
	public class EditLeagueMapper<T> : MapperBase<T> where T : LeagueModel, new()
	{
		public string LeagueId { get; set; }

		protected override void PopulateModel()
		{
			Model.LeagueId = Int32.Parse(LeagueId);
			var owners = HomeEntity.Owners.ToList();
			var users = HomeEntity.Users.ToList();
			var allLeagueOwners = HomeEntity.LeagueOwners.ToList();
			Model.OwnerUsers = OwnerUserMapper.GetOwnerUsers(allLeagueOwners, owners, users);
			Model.ActiveOwnerUsers = Model.OwnerUsers.Where(o => o.IsActive).ToList();
			var leagueOwners = allLeagueOwners.Where(o=>o.LeagueId == Model.LeagueId).ToList();
			Model.LeagueOwnerUsers = OwnerUserMapper.GetOwnerUsers(leagueOwners, owners, users, Model.LeagueId);
			Model.LeagueName = HomeEntity.Leagues.Where(o => o.LeagueId == Model.LeagueId).FirstOrDefault().LeagueName;
		}

		protected override void DoUpdate(T model)
		{
			League league = HomeEntity.Leagues.Where(l=>l.LeagueId == model.LeagueId).FirstOrDefault();
			league.LeagueName = model.LeagueName;
			league.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();

			var leagueOwners = HomeEntity.LeagueOwners.Where(o => o.LeagueId == model.LeagueId).ToList();

			foreach (var oldOwner in leagueOwners)
			{
				HomeEntity.LeagueOwners.DeleteObject(oldOwner);
			}

			foreach (var lo in model.LeagueOwnerUsers)
			{
				var ou = (from o in HomeEntity.Owners.AsEnumerable()
						  join u in HomeEntity.Users.AsEnumerable() on o.UserId equals u.UserId
						  where u.UserId == lo.UserId
						  select new { OwnerId = o.OwnerId, UserName = u.UserName }).FirstOrDefault();
				lo.OwnerId = ou.OwnerId;
				lo.CssClass = ou.UserName;
				//Someday add more in depth CssClass assignment & lookup i.e. CssClass SQL table)

				//TODO:  Consolidate into one table!
				LeagueOwner owner = new LeagueOwner
				{
					OwnerId = lo.OwnerId,
					UserId = lo.UserId,
					LeagueId = model.LeagueId,
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