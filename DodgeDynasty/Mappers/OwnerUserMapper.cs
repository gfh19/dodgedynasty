using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class OwnerUserMapper
	{
		public static OwnerUser GetOwnerUser(User u, LeagueOwner lo)
		{
			u = u ?? new User();
			lo = lo ?? new LeagueOwner();
			return new OwnerUser
			{
				UserId = u.UserId,
				UserName = u.UserName,
				Password = u.Password,
				Salt = u.Salt,
				FirstName = u.FirstName,
				LastName = u.LastName,
				FullName = u.FullName,
				NickName = u.NickName,
				CssClass = lo.CssClass,
				TeamName = lo.TeamName ?? u.FullName,
				IsActive = lo.IsActive,
				AddDateTime = u.AddTimestamp,
				LastLogin = u.LastLogin,
				LastUpdateTimestamp = u.LastUpdateTimestamp
			};
		}

		public static List<OwnerUser> GetOwnerUsers(List<LeagueOwner> leagueOwners, List<User> users, int? leagueId = null)
		{
			var leagueOwnerUsers = (leagueId == null)
				? leagueOwners.Select(lo => new { lo.UserId }).Distinct()
				: leagueOwners.Where(lo => lo.LeagueId == leagueId).Select(lo => new { lo.UserId });

			var ownerUsers = from u in users
							 join lo in leagueOwnerUsers on u.UserId equals lo.UserId
							 select OwnerUserMapper.GetOwnerUser(u,
								leagueOwners.Where(l => l.UserId == u.UserId)
								.OrderByDescending(l => l.IsActive).FirstOrDefault());
			return ownerUsers.ToList();
		}
	}
}