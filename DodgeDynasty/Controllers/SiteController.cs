using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Mappers.Shared;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Site;

namespace DodgeDynasty.Controllers
{
    public class SiteController : Controller
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
		public JsonResult DraftChat(DraftChatModel model)
		{
			var updateSucceeded = false;
			var mapper = new DraftChatMapper();
			mapper.ModelState = ModelState;
			try
			{
				if (mapper.UpdateEntity(model))
				{
					updateSucceeded = true;
				}
			}
			catch { }
			return Json(new { updateSucceeded = updateSucceeded });
		}
    }
}
