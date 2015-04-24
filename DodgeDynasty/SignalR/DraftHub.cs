using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
	}
}