using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Mappers.Site
{
	public static class MessagesHelper
	{
		public static List<Message> GetUserMessages(Entities.HomeEntity homeEntity, User user)
		{
			var userLeagueIds = (from lo in homeEntity.LeagueOwners
								 where lo.UserId == user.UserId
								 select lo.LeagueId).ToList();
			//TODO:  Test all three conditions
			var userMessages = (from m in homeEntity.Messages
								where m.AuthorId == user.UserId
								  || m.AllUsers == true
								  || (m.LeagueId != null && userLeagueIds.Contains(m.LeagueId.Value))
								  || (m.LeagueId == null &&
									  (from lo in homeEntity.LeagueOwners
									   where userLeagueIds.Contains(lo.LeagueId)
									   select lo.UserId).Contains(m.AuthorId))
								select m).ToList();
			return userMessages;
		}
	}
}