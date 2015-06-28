using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Shared
{
	public class DraftChatMapper : MapperBase<DraftChatModel>
	{
		protected override void PopulateModel()
		{
			var user = Utilities.GetLoggedInUser(HomeEntity.Users);
			var drafts = HomeEntity.Drafts.ToList();
			var currentUserDraftId = Utilities.GetLatestUserDraftId(user, drafts, HomeEntity.DraftOwners.ToList());
			var currentDraft = drafts.First(d => d.DraftId == currentUserDraftId);
			var currentLeagueId = currentDraft.LeagueId;

			Model.IsDraftActive = currentDraft.IsActive;
			if (Model.IsDraftActive)
			{
				var leagueOwners = HomeEntity.LeagueOwners.Where(lo => lo.LeagueId == currentLeagueId);
				Model.ChatMessages = HomeEntity.DraftChats.Where(o => o.DraftId == currentUserDraftId)
					.Join(leagueOwners, dc => dc.AuthorId, lo => lo.UserId,
					(dc, lo) => new UserChatMessage
						{
							DraftId = dc.DraftId,
							LeagueId = dc.LeagueId,
							AuthorId = dc.AuthorId,
							NickName = dc.NickName,
							CssClass = lo.CssClass,
							MessageText = dc.MessageText,
							AddTimestamp = dc.AddTimestamp,
							LastUpdateTimestamp = dc.LastUpdateTimestamp
						})
					.Distinct().OrderBy(o => o.AddTimestamp).ToList();
			}
			else
			{
				Model.ChatMessages = new List<UserChatMessage>();
			}
		}
	}
}