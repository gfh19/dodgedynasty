using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Display", "Draft");
			}

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = Utilities.IsStartDraftingDomain(Request) ? 
				"StartDrafting.com (under construction)" : 
				"Dodge Dynasty League (under construction)";

			return View();
		}
	}
}
