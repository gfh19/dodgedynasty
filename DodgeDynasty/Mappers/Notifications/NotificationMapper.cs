using DodgeDynasty.Models.Notification;
using DodgeDynasty.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Mappers.Notifications
{
	public class NotificationMapper : MapperBase<PushSub>
	{
		protected override void DoUpdate(PushSub model)
		{
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var now = Utilities.GetEasternTime();
			var newNotification = new Entities.Notification
			{
				UserId = userId,
				EndPoint = model.EndPoint,
				P256dh = model.Keys[Constants.Notifications.P256dh],
				Auth = model.Keys[Constants.Notifications.Auth],
				AddTimestamp = now,
				LastUpdateTimestamp = now,
				JsonBody = model.JsonBody
			};
			HomeEntity.Notifications.AddObject(newNotification);
			HomeEntity.SaveChanges();
		}
	}
}