using DodgeDynasty.Mappers.Notifications;
using DodgeDynasty.Models.Notification;
using DodgeDynasty.Shared;
using DodgeDynasty.Shared.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.WebPush;
using Newtonsoft.Json;

namespace DodgeDynasty.Controllers
{
	public class NotificationController : Controller
	{
		public static List<PushSub> Subscriptions { get; set; } = new List<PushSub>();

		[HttpPost]
		public HttpStatusCode Subscribe(PushSub request)
		{
			Subscriptions.Add(request);
			var mapper = new SubscribeNotificationMapper();
			mapper.UpdateEntity(request);
			return HttpStatusCode.OK;
		}

//TODO:  Unsubscribe

		[HttpGet]
		public HttpStatusCode Simulate()
		{
//TODO:  Replace with proper user broadcast logic
			var s = Subscriptions.LastOrDefault();
			if (s != null)
			{
				var webPushClient = new WebPushClient();
				var pushSubscription = new PushSubscription(s.EndPoint, s.Keys["p256dh"], s.Keys["auth"]);
				var vapidDetails = new VapidDetails(Constants.Notifications.Email, Constants.Notifications.PublicKey, Constants.Notifications.PrivateKey);
				webPushClient.SendNotification(pushSubscription, JsonConvert.SerializeObject(new NotificationData
				{
					title = "Test Turn",
					body = "Notification stimulated & simulated",
					icon = Constants.Notifications.IconUrl
				}), vapidDetails);				
			}
			return HttpStatusCode.OK;
		}
	}
}