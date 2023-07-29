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
		public bool GetAllSubsForUser { get; set; } = false;
		public int? GetAllSubsUserId { get; set; } = null;
		public LatestPickInfoJson LatestPickInfo { get; set; }

		protected override void PopulateModel()
		{
			var killSwitch = DBUtilities.GetBoolSiteConfigValue(Constants.AppSettings.PushNotificationsKillSwitch);
			if (!killSwitch)
			{
				if (GetAllSubsForUser)
				{
					var userId = (GetAllSubsUserId != null && DBUtilities.IsUserAdmin()) ? GetAllSubsUserId.Value : HomeEntity.Users.GetLoggedInUserId();
					PopulateUserSubscriptions(userId);
				}
				else if (LatestPickInfo?.uturnid != null && LatestPickInfo?.puid != null && LatestPickInfo?.uturnid != LatestPickInfo?.puid)
				{
					PopulateUserSubscriptions(LatestPickInfo.uturnid.Value);
					Model.Payload = JsonConvert.SerializeObject(new NotificationData
					{
						title = "Your Turn!",
						body = $"Last pick: {LatestPickInfo.pname}",
						icon = Constants.Notifications.IconUrl
					});
				}
			}
		}

		private void PopulateUserSubscriptions(int userId)
		{
			var cutoffDate = DateTime.Now.AddDays(-90);
			var userNotifications = HomeEntity.Notifications.Where(
				n => n.UserId == userId
				&& n.LastUpdateTimestamp >= cutoffDate);

			Model.UserId = userId;
			foreach (var notification in userNotifications)
			{
				Model.Notifications.Add(new BroadcastNotification
				{
					Subscription = new PushSubscription(notification.EndPoint, notification.P256dh, notification.Auth),
					VapidDetails = new VapidDetails(Constants.Notifications.Email, Constants.Notifications.PublicKey, Constants.Notifications.PrivateKey)
				});
			}
		}
	}
}