using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models
{
	public class AccessModel : ModelBase
	{
		public bool CanUserAccessRank(int rankId, bool isUpdate = false)
		{
			bool hasAccess = false;
			if (HttpContext.Current.User == null)
			{
				return hasAccess;
			}
			using (HomeEntity = new HomeEntity())
			{
				var identity = HttpContext.Current.User.Identity;
				var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == identity.Name);
				if (user != null)
				{
					var draftRank = HomeEntity.DraftRanks.FirstOrDefault(dr => dr.RankId == rankId);
					hasAccess = draftRank != null && (draftRank.UserId == user.UserId || (draftRank.UserId == null && !isUpdate));
					if (!hasAccess && isUpdate && draftRank.UserId == null)
					{
						var isAdmin = HomeEntity.UserRoles
							.Where(ur => ur.UserId == user.UserId && ur.RoleId == Constants.Roles.Admin).Any();
						hasAccess = isAdmin;
					}
				}
			}
			return hasAccess;
		}

		public bool CanUserAccessUpdateRank(int rankId)
		{
			return CanUserAccessRank(rankId, true);
		}
	}
}