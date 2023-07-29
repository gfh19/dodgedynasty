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
using DodgeDynasty.WebSockets;

namespace DodgeDynasty.Controllers
{
	public class NotificationController : Controller
	{

		[HttpPost]
		public HttpStatusCode Subscribe(PushSub request)
		{
			var mapper = new SubscribeNotificationMapper();
			mapper.UpdateEntity(request);
			return HttpStatusCode.OK;
		}

		[HttpPost]
		public HttpStatusCode Unsubscribe(PushSub request)
		{
			var mapper = new SubscribeNotificationMapper(true);
			mapper.UpdateEntity(request);
			return HttpStatusCode.OK;
		}

		[HttpGet]
		public HttpStatusCode Simulate()
		{
			var model = new BroadcastNotificationMapper() { GetAllSubsForUser = true }.GetModel();
			model.Payload = JsonConvert.SerializeObject(new NotificationData
			{
				title = "Test Turn",
				body = "Notification stimulated & simulated",
				icon = Constants.Notifications.IconUrl
			});
			DraftHubHelper.SendNotifications(model);
			return HttpStatusCode.OK;
		}
	}
}