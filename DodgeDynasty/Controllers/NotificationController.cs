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
			try
			{
				var webPushClient = new WebPushClient();
				//Subscriptions.ForEach(s =>
				//{
				var s = Subscriptions.LastOrDefault();
				if (s != null)
				{
					Dictionary<string, object> options = new Dictionary<string, object>();
					//Dictionary<string, object> headers = new Dictionary<string, object>();
					//headers.Add("Content-Encoding", "aesgcm");
					//options.Add("headers", headers);
					var pushSubscription = new PushSubscription(s.EndPoint, s.Keys["p256dh"], s.Keys["auth"]);
					var vapidDetails = new VapidDetails(Constants.Notifications.Email, Constants.Notifications.PublicKey, Constants.Notifications.PrivateKey);
					webPushClient.SetVapidDetails(vapidDetails);
					//var reqDetails = webPushClient.GenerateRequestDetails(pushSubscription, "{\"body\":\"Draft notification! \"}", options);
					var reqDetails = webPushClient.GenerateRequestDetails(pushSubscription, "", options);
					Logger.Log(Constants.LogTypes.Info, $"Headers: {reqDetails.Headers.ToString()}; ContentHeaders: {reqDetails.Content.Headers.ToString()}", "");
					//webPushClient.SendNotification(pushSubscription, "", vapidDetails);
					//webPushClient.SendNotification(pushSubscription, "", options);
					webPushClient.SendNotification(pushSubscription, "", vapidDetails);
				}
				//});
			}
			catch (WebPushException ex)
			{
				Logger.Log(Constants.LogTypes.Error, $"StatusCode: {ex.StatusCode}; Error: {ex.Message}", ex.StackTrace);
				throw;
			}
			catch (Exception ex)
			{
				Logger.Log(Constants.LogTypes.Error, $"Error: {ex.Message}; Inside Error: {ex.InnerException?.Message}", ex.StackTrace);
				throw;
			}
			return HttpStatusCode.OK;
		}
	}
}