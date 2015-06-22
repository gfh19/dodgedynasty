using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Site;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Site
{
	public class MessagesMapper : MapperBase<MessagesModel>
	{
		protected override void PopulateModel()
		{
			//Update last message view 
			var user = HomeEntity.Users.GetLoggedInUser();
			user.LastMessageView = DateTime.Now;
			HomeEntity.SaveChanges();
			//Get all messages for user's leagues/"all users"/by users in any of user's leagues
			//TODO:  Optimize someday
			var userLeagueIds = (from lo in HomeEntity.LeagueOwners
								 where lo.UserId == user.UserId
								 select lo.LeagueId).ToList();
			//TODO:  Test all three conditions
			var userMessages = (from m in HomeEntity.Messages
								where m.AuthorId == user.UserId 
								  || m.AllUsers == true
								  || (m.LeagueId != null && userLeagueIds.Contains(m.LeagueId.Value))
								  || (m.LeagueId == null && 
									  (from lo in HomeEntity.LeagueOwners
									   where userLeagueIds.Contains(lo.LeagueId)
									   select lo.UserId).Contains(m.AuthorId))
								select m).ToList();
			Model.Messages = userMessages;
		}
	}
}
