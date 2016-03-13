using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers;
using DodgeDynasty.Mappers.Commish;
using DodgeDynasty.Models;

namespace DodgeDynasty.Controllers
{
	public class CommishController : BaseController
	{
		[HttpGet]
		[CommishAccess]
		public ActionResult ManageLeagues()
		{
			var mapper = Factory.Create<CommishManageLeaguesMapper<ManageLeaguesModel>>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[CommishAccess]
		public ActionResult ManageDrafts(string id)
		{
			var mapper = MapperFactory.CreateCommishManageDraftsMapper(id);
            return View(mapper.GetModel());
		}
	}
}