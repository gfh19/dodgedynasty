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

			Model.OwnerLeagues = HomeEntity.LeagueOwners.Where(lo => lo.UserId == user.UserId).ToList();
		}

		protected override void DoUpdate(MessagesModel model)
		{
			var user = HomeEntity.Users.GetLoggedInUser();
			Message message = new Message{
				AuthorId = user.UserId,
				Title = model.Title,
				MessageText = model.MessageText,
				AllUsers = false,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			if (model.LeagueId < 0)
			{
				message.AllUsers = true;
			}
			if (model.LeagueId <= 0)
			{
				message.LeagueId = null;
			}
			else
			{
				message.LeagueId = model.LeagueId;
			}
			HomeEntity.Messages.AddObject(message);
			HomeEntity.SaveChanges();
		}
	}
}
