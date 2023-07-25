using DodgeDynasty.Models.Notification;
using DodgeDynasty.Shared;
using System;
using System.Linq;

namespace DodgeDynasty.Mappers.Notifications
{
	public class SubscribeNotificationMapper : MapperBase<PushSub>
	{
		public bool Unsubscribe { get; set; }
		public int? LoggedInUserId { get; set; }
		public bool? IsUserAdmin { get; set; }

		public SubscribeNotificationMapper() { }
		public SubscribeNotificationMapper(bool unsubscribe, int? loggedInUserId = null, bool? isUserAdmin = null)
		{
			Unsubscribe = unsubscribe;
			LoggedInUserId = loggedInUserId;
			IsUserAdmin = isUserAdmin;
		}

		protected override void DoUpdate(PushSub model)
		{
			LoggedInUserId = LoggedInUserId ?? HomeEntity.Users.GetLoggedInUserId();
			IsUserAdmin = IsUserAdmin ?? DBUtilities.IsUserAdmin();
			var userId = (!string.IsNullOrEmpty(model.UserId) && IsUserAdmin.Value)
				? int.Parse(model.UserId) : LoggedInUserId.Value;
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