using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Mappers.Site
{
	public static class MessagesHelper
	{
		public static List<Message> GetUserMessages(Entities.HomeEntity homeEntity, User user)
		{
			//Get all messages for user's leagues/"all users"/by users in any of user's leagues
			//TODO:  Optimize someday
			var userLeagueIds = (from lo in homeEntity.LeagueOwners
								 where lo.UserId == user.UserId && lo.IsActive
								 select lo.LeagueId).ToList();
			var userMessages = (from m in homeEntity.Messages
								where m.AuthorId == user.UserId
								  || m.AllUsers == true
								  || (m.LeagueId != null && userLeagueIds.Contains(m.LeagueId.Value))
								  || (m.LeagueId == null &&
									  (from lo in homeEntity.LeagueOwners
									   where lo.IsActive && userLeagueIds.Contains(lo.LeagueId)
									   select lo.UserId).Contains(m.AuthorId))
								select m).ToList();
			return userMessages;
		}

		public static List<UserChatMessage> GetChatMessages(List<LeagueOwner> leagueOwners, List<DraftChat> draftChats)
		{
			var chatMessages = draftChats.Join(leagueOwners, dc => dc.AuthorId, lo => lo.UserId,
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
			return chatMessages;
		}

	}
}