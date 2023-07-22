using DodgeDynasty.Models.Notification;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using DodgeDynasty.WebPush;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DodgeDynasty.Mappers.Notifications
{
	public class BroadcastNotificationMapper : MapperBase<BroadcastNotificationModel>
	{
		public LatestPickInfoJson LatestPickInfo { get; set; }

		protected override void PopulateModel()
		{
			if (LatestPickInfo.uturnid != null && LatestPickInfo.puid != null && LatestPickInfo.uturnid != LatestPickInfo.puid)
			{
				var cutoffDate = DateTime.Now.AddDays(-90);
				var userNotifications = HomeEntity.Notifications.Where(
					n => n.UserId == LatestPickInfo.uturnid
					&& n.LastUpdateTimestamp >= cutoffDate);

				foreach (var notification in userNotifications)
				{
					Model.Notifications.Add(new BroadcastNotification
					{
						Subscription = new PushSubscription(notification.EndPoint, notification.P256dh, notification.Auth),
						VapidDetails = new VapidDetails(Constants.Notifications.Email, Constants.Notifications.PublicKey, Constants.Notifications.PrivateKey)
					});
					Model.Payload = JsonConvert.SerializeObject(new NotificationData
					{
						title = "Your Turn!",
						body = $"Last pick: {LatestPickInfo.pname}",
						icon = Constants.Notifications.IconUrl
					});
				}
			}
		}
	}
}