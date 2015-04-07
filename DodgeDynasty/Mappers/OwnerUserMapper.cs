using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class OwnerUserMapper : ModelBase
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
				TeamName = lo.TeamName ?? string.Format("Team {0}", o.NickName),
				IsActive = o.IsActive,
				AddDateTime = u.AddDateTime,
				LastLogin = u.LastLogin,
				LastUpdateTimestamp = o.LastUpdateTimestamp
			};
		}
	}
}