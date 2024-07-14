using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Account
{
	public class RoleAccessMapper : MapperBase<RoleAccessModel>
	{
		protected override void PopulateModel()
		{
			int userId = (Model.UserId != null) ? Model.UserId.Value : HomeEntity.Users.GetLoggedInUserId();
			Model.UserRoles = HomeEntity.UserRoles.Where(o => o.UserId == userId).ToList();
            Model.IsUserAdmin = Model.UserRoles.Any(o => o.RoleId == Constants.Roles.Admin);
			if (Model.LeagueId != null)
			{
				Model.IsUserCommish = Model.UserRoles.Any(o => o.RoleId == Constants.Roles.Commish && o.LeagueId == Model.LeagueId);
			}
			else
			{
				Model.IsUserCommish = Model.UserRoles.Any(o => o.RoleId == Constants.Roles.Commish);
			}
			if (Model.IsUserAdmin || Model.IsUserCommish)
			{
				var privilegedDrafts = HomeEntity.Drafts.AsEnumerable();
                if (!Model.IsUserAdmin)
                {
					var commishLeagueIds = DBUtilities.GetCommishLeagueIds(Model.UserRoles);
					privilegedDrafts = privilegedDrafts.Where(d => commishLeagueIds.Contains(d.LeagueId)).AsEnumerable();
				}
				var activePrivilegedDraft = privilegedDrafts.Where(d => d.IsActive).OrderBy(d => d.DraftDate).FirstOrDefault();
				if (activePrivilegedDraft != null)
				{
					Model.PrivilegedDraftId = activePrivilegedDraft.DraftId;
					Model.IsPrivilegedDraftActive = activePrivilegedDraft.IsActive;
					Model.IsActivePrivilegedDraftPaused = activePrivilegedDraft.IsPaused;
				}
				if (Model.IsUserAdmin)
				{
					var adminStatus = HomeEntity.AdminStatus.FirstOrDefault(o => o.UserId == userId);
					if (adminStatus != null && adminStatus.LastPlayerAdjView.HasValue)
					{
						Model.LastPlayerAdjView = adminStatus.LastPlayerAdjView.Value;
					}
					else
					{
						Model.LastPlayerAdjView = DateTime.MinValue;
					}
					var latestPlayerAdjDT = HomeEntity.PlayerAdjustments.Where(o => !o.HideReporting).OrderByDescending(o => o.AddTimestamp)
						.Select(o => o.AddTimestamp).FirstOrDefault();
					if (latestPlayerAdjDT != null)
					{
						Model.NewPlayerAdjExists = latestPlayerAdjDT.CompareTo(Model.LastPlayerAdjView) > 0;
					}
				}
			}
		}
	}
}
