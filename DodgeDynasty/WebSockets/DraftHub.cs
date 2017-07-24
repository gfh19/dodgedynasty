using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.Shared;
using Microsoft.AspNet.SignalR;

namespace DodgeDynasty.WebSockets
{
	//Methods in this class called from the client
	public class DraftHub : Hub
	{
		public static ConcurrentDictionary<string, string> OpenHubConnections = new ConcurrentDictionary<string, string>();

		public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
		{
			string val;
			OpenHubConnections.TryRemove(Context.ConnectionId, out val);
			return base.OnDisconnected(stopCalled);
		}

		public void Pick()
		{
			// Call the broadcastMessage method to update clients.
			Clients.All.broadcastDraft();
		}

		public void Chat(string text)
		{
			string userName = Context.User.Identity.Name;
			var mapper = new DraftChatMapper(userName, text);
			mapper.UpdateEntity();
			Clients.All.broadcastChat(mapper.ChatJsonResult);
		}
	}
}