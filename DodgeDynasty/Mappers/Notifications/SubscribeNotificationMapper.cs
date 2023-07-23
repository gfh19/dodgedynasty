using DodgeDynasty.Models.Notification;
using DodgeDynasty.Shared;
using System;
using System.Linq;

namespace DodgeDynasty.Mappers.Notifications
{
	public class SubscribeNotificationMapper : MapperBase<PushSub>
	{
		public bool Unsubscribe { get; set; }

		public SubscribeNotificationMapper() { }
		public SubscribeNotificationMapper(bool unsubscribe)
		{
			Unsubscribe = unsubscribe;
		}

		protected override void DoUpdate(PushSub model)
		{
			var userId = (!string.IsNullOrEmpty(model.UserId) && DBUtilities.IsUserAdmin())
				? int.Parse(model.UserId) : HomeEntity.Users.GetLoggedInUserId();
			var now = Utilities.GetEasternTime();
			var request = new Entities.Notification
			{
				UserId = userId,
				EndPoint = model.EndPoint,
				P256dh = model.Keys[Constants.Notifications.P256dh],
				Auth = model.Keys[Constants.Notifications.Auth],
				AddTimestamp = now,
				LastUpdateTimestamp = now
			};
			var existingNotif = HomeEntity.Notifications.FirstOrDefault(n =>
				n.UserId == userId &&
				n.EndPoint == request.EndPoint &&
				n.P256dh == request.P256dh &&
				n.Auth == request.Auth);
			if (Unsubscribe)
			{
				HandleUnsubscribe(existingNotif);
			}
			else
			{
				HandleSubscribe(request, existingNotif);
			}
		}

		private void HandleSubscribe(Entities.Notification request, Entities.Notification existingNotif)
		{
			if (existingNotif != null)
			{
				existingNotif.LastUpdateTimestamp = Utilities.GetEasternTime();
			}
			else
			{
				HomeEntity.Notifications.AddObject(request);
			}
			HomeEntity.SaveChanges();
		}

		private void HandleUnsubscribe(Entities.Notification existingNotif)
		{
			if (existingNotif != null)
			{
				HomeEntity.Notifications.DeleteObject(existingNotif);
				HomeEntity.SaveChanges();
			}
		}
	}
}