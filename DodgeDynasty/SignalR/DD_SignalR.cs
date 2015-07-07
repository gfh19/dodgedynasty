using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DodgeDynasty.SignalR.DD_SignalR))]

namespace DodgeDynasty.SignalR
{
	public class DD_SignalR
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}
