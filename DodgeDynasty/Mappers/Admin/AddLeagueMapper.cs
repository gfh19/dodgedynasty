using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Admin
{
	public class AddLeagueMapper<T> : MapperBase<T> where T : AddEditLeagueModel, new()
	{
		protected override void PopulateModel()
		{
			Model.OwnerUsers = OwnerUserMapper.GetOwnerUsers(HomeEntity.LeagueOwners.ToList(), HomeEntity.Users.ToList());
			Model.ActiveLeagueUsers = HomeEntity.Users.Where(o => o.IsActive).ToList();
			var numOwners = Int32.Parse(
				ConfigurationManager.AppSettings[Constants.AppSettings.DefaultNumOwners] ?? "4");
			Model.LeagueOwnerUsers = new List<OwnerUser>();
			for (int i = 0; i < numOwners; i++)
			{
				Model.LeagueOwnerUsers.Add(new OwnerUser { UserId = 0, IsActive = true });
			}
			Model.NumRounds = 15;
			Model.NumKeepers = 0;
			//TODO:  Add League setting & default here
			Model.Format = Constants.DraftFormats.Durant;
			Model.CombineWRTE = false;
			Model.PickTimeSeconds = Constants.Defaults.PickTimeSeconds;
			Model.CssColors = HomeEntity.CssColors.ToList();
			Model.CommishUserIds = new List<int>();
		}

		protected override void DoUpdate(T model)
		{
			League league = new Entities.League
			{
				LeagueName = model.LeagueName,
				NumRounds = Convert.ToInt16(model.NumRounds),
				NumKeepers = Convert.ToInt16(model.NumKeepers),
				Format = model.Format,
				CombineWRTE = model.CombineWRTE,
                PickTimeSeconds = Convert.ToInt16(model.PickTimeSeconds),
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

				LeagueOwner owner = new LeagueOwner
				{
					UserId = lo.UserId,
					LeagueId = league.LeagueId,
					TeamName = lo.TeamName,
					CssClass = lo.CssClass,
					IsActive = lo.IsActive,
					AnnounceAllPicks = lo.AnnounceAllPicks,
					AnnouncePrevPick = lo.AnnouncePrevPick,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				HomeEntity.LeagueOwners.AddObject(owner);
			}
			if (model.CommishUserIds != null)
			{
				foreach (var commishUserId in model.CommishUserIds)
				{
					HomeEntity.UserRoles.AddObject(new UserRole
					{
						UserId = commishUserId,
						RoleId = Constants.Roles.Commish,
						LeagueId = league.LeagueId,
						AddTimestamp = DateTime.Now,
						LastUpdateTimestamp = DateTime.Now
					});
				}
			}
			HomeEntity.SaveChanges();
		}
	}
}