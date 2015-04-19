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
		public static OwnerUser GetOwnerUser(Owner o, User u, LeagueOwner lo)
		{
			o = o ?? new Owner();
			u = u ?? new User();
			lo = lo ?? new LeagueOwner();
			return new OwnerUser
			{
				OwnerId = o.OwnerId,
				UserId = u.UserId,
				UserName = u.UserName,
				Password = u.Password,
				Salt = u.Salt,
				FirstName = u.FirstName,
				LastName = u.LastName,
				FullName = u.FullName,
				NickName = o.NickName,
				CssClass = lo.CssClass,
				TeamName = lo.TeamName ?? u.FullName,
				IsActive = lo.IsActive,
				AddDateTime = u.AddDateTime,
				LastLogin = u.LastLogin,
				LastUpdateTimestamp = o.LastUpdateTimestamp
			};
		}

		public static List<OwnerUser> GetOwnerUsers(List<LeagueOwner> leagueOwners, List<Owner> owners,	List<User> users,
			int? leagueId = null)
		{
			var leagueOwnerUsers = (leagueId == null)
				? leagueOwners.Select(lo => new { lo.UserId }).Distinct()
				: leagueOwners.Where(lo => lo.LeagueId == leagueId).Select(lo => new { lo.UserId });

			var ownerUsers = from o in owners
							 join u in users on o.UserId equals u.UserId
							 join lo in leagueOwnerUsers on o.UserId equals lo.UserId
							 select OwnerUserMapper.GetOwnerUser(o, u,
								leagueOwners.Where(l => l.UserId == u.UserId)
								.OrderByDescending(l => l.IsActive).FirstOrDefault());
			return ownerUsers.ToList();
		}
	}
}