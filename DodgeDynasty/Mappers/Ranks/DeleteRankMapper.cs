using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

namespace DodgeDynasty.Mappers.Highlights
{
	public class DeleteRankMapper : MapperBase<RankSetupModel>
	{
		protected override void DoUpdate(RankSetupModel model)
		{
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var isAdmin = HomeEntity.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == Constants.Roles.Admin);
			var draftRank = HomeEntity.DraftRanks.FirstOrDefault(o => o.RankId == model.RankId);
			var hasAccess = draftRank != null && (draftRank.UserId == userId || (draftRank.UserId == null && isAdmin));
			if (hasAccess)
			{
				var rank = HomeEntity.Ranks.FirstOrDefault(o => o.RankId == model.RankId);
				var playerRanks = HomeEntity.PlayerRanks.Where(o => o.RankId == model.RankId).AsEnumerable();

				if (playerRanks != null && playerRanks.Any())
				{
					playerRanks.ForEach(pr => HomeEntity.PlayerRanks.DeleteObject(pr));
				}
				HomeEntity.DraftRanks.DeleteObject(draftRank);
				if (rank != null)
				{
					HomeEntity.Ranks.DeleteObject(rank);
				}
				HomeEntity.SaveChanges();
			}
		}
	}
}