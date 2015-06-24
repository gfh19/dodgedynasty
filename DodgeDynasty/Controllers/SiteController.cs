using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Mappers.Site;
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
    }
}
