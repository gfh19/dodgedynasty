using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Mappers.Site;

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

    }
}
