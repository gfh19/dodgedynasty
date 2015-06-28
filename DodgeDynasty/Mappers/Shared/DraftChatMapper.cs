﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Shared
{
	public class DraftChatMapper : MapperBase<DraftChatModel>
	{
		public string MessageText { get; set; }
		public ChatJson ChatJsonResult { get; set; }

		public DraftChatMapper() { }
		public DraftChatMapper(string text)
		{
			MessageText = text;
		}

		protected override void PopulateModel()
		{
			var currentDraft = GetUserCurrentDraft();
			var currentLeagueId = currentDraft.LeagueId;

			Model.IsDraftActive = currentDraft.IsActive;
			if (Model.IsDraftActive)
			{
				var leagueOwners = HomeEntity.LeagueOwners.Where(lo => lo.LeagueId == currentLeagueId);
				Model.ChatMessages = HomeEntity.DraftChats.Where(o => o.DraftId == currentDraft.DraftId)
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

		public override DraftChatModel CreateModelForUpdate()
		{
			try
			{
				var currentDraft = GetUserCurrentDraft();
				if (currentDraft.IsActive && !Utilities.IsTrimEmpty(MessageText))
				{
					Model = new DraftChatModel
					{
						DraftId = currentDraft.DraftId,
						LeagueId = currentDraft.LeagueId,
						MessageText = MessageText
					};
				}
			}
			catch {
				Model = null;
			}
			return Model;
		}

		protected override void DoUpdate(DraftChatModel model)
		{
			var user = Utilities.GetLoggedInUser(HomeEntity.Users);
			DraftChat chatMessage = new DraftChat
			{
				DraftId = model.DraftId,
				LeagueId = model.LeagueId,
				AuthorId = user.UserId,
				MessageText = model.MessageText,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.DraftChats.AddObject(chatMessage);
			HomeEntity.SaveChanges();

			ChatJsonResult = CreateChatJsonResult(chatMessage, user);
		}

		private ChatJson CreateChatJsonResult(DraftChat chatMessage, User user)
		{
			var leagueOwner = HomeEntity.LeagueOwners.First(o=>o.UserId == user.UserId);
			return new ChatJson
			{
				author = user.NickName,
				css = leagueOwner.CssClass,
				msg = chatMessage.MessageText,
				time = chatMessage.AddTimestamp.ToString(Constants.ChatTimeFormat)
			};
		}

		private Draft GetUserCurrentDraft()
		{
			var user = Utilities.GetLoggedInUser(HomeEntity.Users);
			var drafts = HomeEntity.Drafts.ToList();
			var currentUserDraftId = Utilities.GetLatestUserDraftId(user, drafts, HomeEntity.DraftOwners.ToList());
			var currentDraft = drafts.First(d => d.DraftId == currentUserDraftId);
			return currentDraft;
		}
	}
}