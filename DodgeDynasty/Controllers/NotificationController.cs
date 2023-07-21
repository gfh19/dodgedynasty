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

namespace DodgeDynasty.Controllers
{
	public class NotificationController : Controller
	{
		public static List<PushSub> Subscriptions { get; set; } = new List<PushSub>();

		[HttpPost]
		public HttpStatusCode Subscribe(PushSub request)
		{
			var req = Request.InputStream;
			req.Seek(0, System.IO.SeekOrigin.Begin);
			string json = new StreamReader(req).ReadToEnd();
			Subscriptions.Add(request);
			var mapper = new NotificationMapper();
			request.JsonBody = json;
			mapper.UpdateEntity(request);
			return HttpStatusCode.OK;
		}

		[HttpGet]
		public HttpStatusCode Broadcast()
		{
			var webPushClient = new WebPushClient();
//TODO:  Replace with proper user broadcast logic
			var s = Subscriptions.LastOrDefault();
			if (s != null)
			{
				var pushSubscription = new PushSubscription(s.EndPoint, s.Keys["p256dh"], s.Keys["auth"]);
				var vapidDetails = new VapidDetails(Constants.Notifications.Email, Constants.Notifications.PublicKey, Constants.Notifications.PrivateKey);
				webPushClient.SendNotification(pushSubscription, "", vapidDetails);
			}
			return HttpStatusCode.OK;
		}
	}
}