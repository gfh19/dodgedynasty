using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DodgeDynasty.Controllers
{
    public class SharedController : Controller
    {
        //
        // GET: /Shared/

		public ActionResult Error()
		{
			return View();
		}

		public ActionResult Unauthorized()
		{
			return View();
		}
    }
}
