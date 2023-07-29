using System;
using System.Linq;
using DodgeDynasty.Models.Site;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Site
{
	public class MessagesCountMapper : MapperBase<MessagesCountModel>
	{
		protected override void PopulateModel()
		{
			var user = HomeEntity.Users.GetLoggedInUser();
			var userMessages = MessagesHelper.GetUserMessages(HomeEntity, user);
			var latestMessage = userMessages.OrderByDescending(m=>m.AddTimestamp).FirstOrDefault();

			Model.NewMessages = userMessages.Where(
				m => user.LastMessageView == null || m.AddTimestamp > user.LastMessageView).ToList();
			Model.LatestMessageTime = (latestMessage != null) ? latestMessage.AddTimestamp : DateTime.MinValue;
		}
	}
}