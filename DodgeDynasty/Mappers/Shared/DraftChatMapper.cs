using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Shared
{
	public class DraftChatMapper : MapperBase<DraftChatModel>
	{
		public string UserName { get; set; }
		public string MessageText { get; set; }
		public ChatJson ChatJsonResult { get; set; }

		public DraftChatMapper() { }
		public DraftChatMapper(string userName, string text)
		{
			UserName = userName;
			MessageText = text;
		}

		protected override void PopulateModel()
		{
			var currentDraft = GetUserCurrentDraft(Utilities.GetLoggedInUserName());
			var currentLeagueId = currentDraft.LeagueId;

			Model.IsDraftActive = currentDraft.IsActive;
			if (Model.IsDraftActive)
			{
				var leagueOwners = HomeEntity.LeagueOwners.Where(lo => lo.LeagueId == currentLeagueId).ToList();
				var draftChats = HomeEntity.DraftChats.Where(o => o.DraftId == currentDraft.DraftId).ToList();
				Model.ChatMessages = MessagesHelper.GetChatMessages(leagueOwners, draftChats);
			}
			else
			{
				Model.ChatMessages = new List<UserChatMessage>();
			}
		}

		protected override DraftChatModel CreateModelForUpdate()
		{
			try
			{
				var currentDraft = GetUserCurrentDraft(UserName);
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
			var currentTime = Utilities.GetEasternTime();
			var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == UserName);
			var encodedText = Regex.Replace(model.MessageText, @"\p{Cs}{1,}", "<emoji>");
			DraftChat chatMessage = new DraftChat
			{
				DraftId = model.DraftId,
				LeagueId = model.LeagueId,
				AuthorId = user.UserId,
				MessageText = encodedText,
				AddTimestamp = currentTime,
				LastUpdateTimestamp = currentTime
			};
			HomeEntity.DraftChats.AddObject(chatMessage);
			HomeEntity.SaveChanges();

			chatMessage.MessageText = model.MessageText;
			ChatJsonResult = CreateChatJsonResult(chatMessage, user);
		}

		private ChatJson CreateChatJsonResult(DraftChat chatMessage, User user)
		{
			var leagueOwners = HomeEntity.LeagueOwners.Where(lo => lo.LeagueId == chatMessage.LeagueId);
			var leagueOwner = leagueOwners.First(o => o.UserId == user.UserId);
			return new ChatJson
			{
				author = user.NickName,
				css = leagueOwner.CssClass,
				msg = chatMessage.MessageText,
				time = chatMessage.AddTimestamp.ToString(Constants.Times.ChatTimeFormat)
			};
		}

		private Draft GetUserCurrentDraft(string userName)
		{
			var userId = HomeEntity.Users.FirstOrDefault(u => u.UserName == userName).UserId;
			var drafts = HomeEntity.Drafts.ToList();
			var currentUserDraftId = Utilities.GetLatestUserDraftId(userId, drafts, 
				HomeEntity.DraftOwners.ToList(), HomeEntity.UserRoles.Where(o => o.UserId == userId).ToList());
			var currentDraft = drafts.First(d => d.DraftId == currentUserDraftId);
			return currentDraft;
		}
	}
}