﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Site;
using DodgeDynasty.Models.Types;
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

			//Get User Messages Display
			Model.OwnerLeagues = HomeEntity.LeagueOwners.Where(lo => lo.UserId == user.UserId).ToList();
			var userMessages = MessagesHelper.GetUserMessages(HomeEntity, user);
			Model.Messages = userMessages;

			//Get User DraftChats Display
			var userDraftIds = HomeEntity.DraftOwners.Where(o => o.UserId == user.UserId).Select(o => o.DraftId).ToList();
			var userChatMessages = HomeEntity.DraftChats.Where(dc => userDraftIds.Contains(dc.DraftId))
									.OrderBy(dc=>dc.AddTimestamp).ToList();
			var userChatDraftIds = userChatMessages.Select(dc => dc.DraftId).Distinct().ToList();
			Model.UserChatDrafts = HomeEntity.Drafts.Where(d => userChatDraftIds.Contains(d.DraftId))
									.OrderByDescending(o => o.AddTimestamp).ToList();

			Model.DraftChatMessages = new List<UserChatMessage>();
			foreach (var userDraft in Model.UserChatDrafts)
			{
				var leagueOwners = HomeEntity.LeagueOwners.Where(lo => lo.LeagueId == userDraft.LeagueId).ToList();
				var draftMessages = userChatMessages.Where(dc => dc.DraftId == userDraft.DraftId).ToList();
				Model.DraftChatMessages.AddRange(MessagesHelper.GetChatMessages(leagueOwners, draftMessages));
			}

			//Model.DraftChatMessages = HomeEntity.DraftChats.Where(dc => userChatDraftIds.Contains(dc.DraftId)).ToList();
		}

		protected override void DoUpdate(MessagesModel model)
		{
			var currentTime = Utilities.GetEasternTime();
			var user = HomeEntity.Users.GetLoggedInUser();
			Message message = new Message{
				AuthorId = user.UserId,
				Title = model.Title,
				MessageText = model.MessageText,
				AllUsers = false,
				AddTimestamp = currentTime,
				LastUpdateTimestamp = currentTime
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
