using DodgeDynasty.WebPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Notification
{
	public class BroadcastNotificationModel
	{
		public List<BroadcastNotification> Notifications { get; set; } = new List<BroadcastNotification>();
		public string Payload { get; set; }
	}

	public class BroadcastNotification
	{
		public PushSubscription Subscription { get; set; }
		public VapidDetails VapidDetails { get; set; }
	}

	public class NotificationData
	{
		public string title { get; set; }
		public string body { get; set; }
		public string icon { get; set; }
	}
}