using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Filters;
using DodgeDynasty.Mappers;
using DodgeDynasty.Mappers.Admin;
using DodgeDynasty.Mappers.Commish;
using DodgeDynasty.Models;

namespace DodgeDynasty.Controllers
{
	public class CommishController : BaseController
	{
		/* Manage Leagues */

		[HttpGet]
		[CommishAccess]
		public ActionResult ManageLeagues()
		{
			var mapper = Factory.Create<CommishManageLeaguesMapper<ManageLeaguesModel>>();
			return View(mapper.GetModel());
		}

		[HttpGet]
		[CommishLeagueAccess]
		public ActionResult EditLeague(string id)
		{
			var mapper = new EditLeagueMapper<AddEditLeagueModel> { LeagueId = id };
			return View(mapper.GetModel());
		}

		[HttpPost]
		[CommishLeagueAccess]
		public ActionResult EditLeague(AddEditLeagueModel model)
		{
			var mapper = new EditLeagueMapper<AddEditLeagueModel>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { leagueId = model.LeagueId });
		}


		/* Manage Drafts */

		[HttpGet]
		[CommishLeagueAccess]
		public ActionResult ManageDrafts(string id)
		{
			var mapper = MapperFactory.CreateCommishManageDraftsMapper(id);
			return View(mapper.GetModel());
		}

		[HttpGet]
		[CommishLeagueAccess]
		public ActionResult AddDraft(string id)
		{
			var mapper = Factory.Create<AddDraftMapper<AddEditDraftModel>>();
			mapper.LeagueId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[CommishLeagueAccess]
		public ActionResult AddDraft(AddEditDraftModel model)
		{
			var mapper = Factory.Create<AddDraftMapper<AddEditDraftModel>>();
			if (!ModelState.IsValid)
			{
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}

		[HttpGet]
		[CommishDraftAccess]
		public ActionResult EditDraft(string id)
		{
			var mapper = new EditDraftMapper<AddEditDraftModel>();
			mapper.DraftId = Int32.Parse(id);
			return View(mapper.GetModel());
		}

		[HttpPost]
		[CommishDraftAccess]
		public ActionResult EditDraft(AddEditDraftModel model)
		{
			var mapper = new EditDraftMapper<AddEditDraftModel>();
			if (!ModelState.IsValid)
			{
				mapper.DraftId = model.DraftId;
				return View(mapper.GetModel(model));
			}
			mapper.UpdateEntity(model);
			return Json(new { draftId = model.DraftId });
		}
	}
}