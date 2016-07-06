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
	public class EditLeagueMapper<T> : MapperBase<T> where T : AddEditLeagueModel, new()
	{
		public string LeagueId { get; set; }

		protected override void PopulateModel()
		{
			Model.LeagueId = Int32.Parse(LeagueId);
			var currentUserId = HomeEntity.Users.GetLoggedInUserId();
			var users = HomeEntity.Users.ToList();
			var allLeagueOwners = HomeEntity.LeagueOwners.ToList();
			var leagueOwners = allLeagueOwners.Where(o => o.LeagueId == Model.LeagueId).ToList();
			var leagueUserIds = leagueOwners.Select(o => o.UserId);
			Model.ActiveLeagueUsers = HomeEntity.Users.Where(o => o.IsActive || leagueUserIds.Contains(o.UserId)).ToList();
			Model.LeagueOwnerUsers = OwnerUserMapper.GetOwnerUsers(leagueOwners, users, Model.LeagueId);
			Model.LeagueName = HomeEntity.Leagues.Where(o => o.LeagueId == Model.LeagueId).FirstOrDefault().LeagueName;
			Model.CssColors = HomeEntity.CssColors.ToList();
			Model.CommishUserIds = HomeEntity.UserRoles
				.Where(o => o.RoleId == Constants.Roles.Commish && o.LeagueId == Model.LeagueId)
				.OrderBy(o=>o.UserId != currentUserId).ThenBy(o=>o.UserRoleId)
				.Select(o => o.UserId).ToList();
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
				var user = (from u in HomeEntity.Users.AsEnumerable()
						  where u.UserId == lo.UserId
						  select u).FirstOrDefault();
				lo.UserId = user.UserId;

				LeagueOwner owner = new LeagueOwner
				{
					UserId = lo.UserId,
					LeagueId = model.LeagueId,
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

			var leagueCommishes = HomeEntity.UserRoles.Where(
				o => o.RoleId == Constants.Roles.Commish && o.LeagueId == model.LeagueId).ToList();
			foreach (var oldCommish in leagueCommishes)
			{
				HomeEntity.UserRoles.DeleteObject(oldCommish);
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