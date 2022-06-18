using DodgeDynasty.Models.Types;
using Microsoft.AspNet.SignalR;

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
	}
}