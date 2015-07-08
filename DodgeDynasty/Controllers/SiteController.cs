using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models.Site;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using DodgeDynasty.SignalR;

namespace DodgeDynasty.Controllers
{
    public class SiteController : BaseController
    {
		[HttpGet]
        public ActionResult Messages()
        {
			var mapper = new MessagesMapper();
			return View(mapper.GetModel());
        }

		[HttpPost]
		public ActionResult Messages(MessagesModel model)
		{
			var mapper = new MessagesMapper();
			mapper.ModelState = ModelState;
			if (!mapper.UpdateEntity(model))
			{
				return View(mapper.GetUpdatedModel(model));
			}
			return View(mapper.GetModel());
		}

		[HttpPost]
		public JsonResult OpenDraftHubConnection(string connectionId)
		{
			HubConnJson response = new HubConnJson { good = true };

			int maxTabConns;
			maxTabConns =
				Int32.TryParse(ConfigurationManager.AppSettings[Constants.AppSettings.MaxTabConnections], out maxTabConns)
				? maxTabConns : 4;

			var cookieContent = GetDodgeDynastyCookie();
			DraftHub.OpenHubConnections[connectionId] = cookieContent.SessionId;
			var openTabConns = DraftHub.OpenHubConnections.Count(o => o.Value == cookieContent.SessionId);
			response.conns = openTabConns;
			if (openTabConns > maxTabConns)
			{
				response.good = false;
				string val;
				DraftHub.OpenHubConnections.TryRemove(connectionId, out val);
			}
			return Json(response);
		}
    }
}
