﻿using System;
using System.Configuration;
using System.Threading.Tasks;
using DodgeDynasty.Shared;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DodgeDynasty.WebSockets.DD_SignalR))]

namespace DodgeDynasty.WebSockets
{
	public class DD_SignalR
	{
		public void Configuration(IAppBuilder app)
		{
			var webSocketsKillSwitch = 
				ConfigurationManager.AppSettings[DodgeDynasty.Shared.Constants.AppSettings.WebSocketsKillSwitch];
			if (!Utilities.ToBool(webSocketsKillSwitch))
			{
				app.MapSignalR();
			}
		}
	}
}
