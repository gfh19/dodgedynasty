using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace DodgeDynasty.SignalR
{
	public class ChatHub : Hub
	{
		public void Send(string message)
		{
			// Call the broadcastMessage method to update clients.
			Clients.All.broadcastMessage(message);
		}
	}
}