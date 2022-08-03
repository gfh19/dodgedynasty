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
			using (HomeEntity = new HomeEntity())
			{
				return canUserAccessRank(HomeEntity, rankId, isUpdate);
			}
		}

		public bool CanUserAccessUpdateRank(int rankId)
		{
			using (HomeEntity = new HomeEntity())
			{
				return doesRankExist(HomeEntity, rankId) && canUserAccessRank(HomeEntity, rankId, true);
			}
		}

		public bool DoesRankExistAndCanUserAccess(int rankId)
		{
			using (HomeEntity = new HomeEntity())
			{
				return doesRankExist(HomeEntity, rankId) && canUserAccessRank(HomeEntity, rankId);
			}
		}

		private bool canUserAccessRank(HomeEntity entity, int rankId, bool isUpdate = false)
		{
			bool hasAccess = false;
			if (HttpContext.Current.User == null)
			{
				return hasAccess;
			}
			var identity = HttpContext.Current.User.Identity;
			var user = entity.Users.FirstOrDefault(u => u.UserName == identity.Name);
			if (user != null)
			{
				if (doesRankExist(entity, rankId))
				{
					var draftRank = entity.DraftRanks.FirstOrDefault(dr => dr.RankId == rankId);
					hasAccess = (draftRank.UserId == user.UserId || (draftRank.UserId == null && !isUpdate));
					if (!hasAccess && isUpdate && draftRank.UserId == null)
					{
						var isAdmin = entity.UserRoles
							.Any(ur => ur.UserId == user.UserId && ur.RoleId == Constants.Roles.Admin);
						hasAccess = isAdmin;
					}
				}
				else
				{
					//If DraftRankId no longer exists (i.e. private rank just deleted), allow access so latest can be returned
					hasAccess = true;
				}
			}
			return hasAccess;
		}

		private bool doesRankExist(HomeEntity entity, int rankId, bool isUpdate = false)
		{
			return HomeEntity.Ranks.Any(r => r.RankId == rankId) && HomeEntity.DraftRanks.Any(dr => dr.RankId == rankId);
		}
	}
}