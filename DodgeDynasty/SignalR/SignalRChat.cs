using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DodgeDynasty.SignalR.SignalRChat))]

namespace DodgeDynasty.SignalR
{
	public class SignalRChat
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}
