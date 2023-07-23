using DodgeDynasty.Mappers.Notifications;
using DodgeDynasty.Models.Notification;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using DodgeDynasty.Shared.Log;
using DodgeDynasty.WebPush;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace DodgeDynasty.WebSockets
{
	//Methods in this class called from the server
	public static class DraftHubHelper
	{
		public static void BroadcastDraftToClients(LatestPickInfoJson pickInfo)
		{
			var context = GlobalHost.ConnectionManager.GetHubContext<DraftHub>();
			context.Clients.All.broadcastDraft(pickInfo);
		}

		public static void BroadcastDraftToUser(string userName)
		{
			var context = GlobalHost.ConnectionManager.GetHubContext<DraftHub>();
			context.Clients.All.broadcastDraftToUser(userName);
		}

		public static void BroadcastDisconnectToClients()
		{
			var context = GlobalHost.ConnectionManager.GetHubContext<DraftHub>();
			context.Clients.All.broadcastDisconnect();
		}

		/* ServiceWorker - Push Notifications */
		public static void BroadcastNotification(LatestPickInfoJson pickInfo)
		{
			try
			{
				var model = new BroadcastNotificationMapper { LatestPickInfo = pickInfo }.GetModel();
				var webPushClient = new WebPushClient();
				foreach (var notification in model.Notifications)
				{
					try
					{
						webPushClient.SendNotification(notification.Subscription, model.Payload, notification.VapidDetails);
					}
					catch (WebPushException ex)
					{
						if (new[] { HttpStatusCode.NotFound, HttpStatusCode.Gone }.Contains(ex.StatusCode))
						{
							//User subscription expired; unsubscribe/delete from DB, and continue loop
							var mapper = new SubscribeNotificationMapper(true);
							mapper.UpdateEntity(new PushSub
							{
								UserId = pickInfo.uturnid?.ToString(),
								EndPoint = notification.Subscription.Endpoint,
								Keys = new Dictionary<string, string> {
									{ Constants.Notifications.P256dh, notification.Subscription.P256DH },
									{ Constants.Notifications.Auth, notification.Subscription.Auth },
								}
							});
							Logger.LogErrorPrefix($"Subscription gone - {ex.StatusCode}; ", ex);
						}
						else
						{
							throw;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.LogErrorPrefix("Notification failed - ", ex);
			}
		}
	}
}