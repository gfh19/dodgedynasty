using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.Shared;
using Microsoft.AspNet.SignalR;

namespace DodgeDynasty.SignalR
{
	public class DraftHub : Hub
	{
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