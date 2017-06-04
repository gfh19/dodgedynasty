using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Models;
using DodgeDynasty.Shared.Exceptions;
using DodgeDynasty.Shared;
using DodgeDynasty.Mappers.Shared;
using DodgeDynasty.Models.Types;
using Newtonsoft.Json;
using DodgeDynasty.WebSockets;
using DodgeDynasty.Models.Shared;

namespace DodgeDynasty.Controllers
{
    public class BaseController : Controller
    {
		protected ActionResult InputDraftPick(string viewName, DraftInputModel draftInputModel, bool isAdmin)
		{
			var draftId = isAdmin ? draftInputModel.DraftId : null;
			DraftInputModel nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(draftId);
			var playerModel = draftInputModel.Player;
			playerModel.DraftPickId = isAdmin ? playerModel.DraftPickId : nextDraftInputModel.CurrentDraftPick.DraftPickId;
			nextDraftInputModel.Player = playerModel;
			if (!ModelState.IsValid)
			{
				return View(nextDraftInputModel);
			}
			else if (!isAdmin && !nextDraftInputModel.IsDraftingUserLoggedIn())
			{
				ModelState.Clear();
				ModelState.AddModelError("", "Error - It is not your turn to pick.");
				nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(draftInputModel.DraftId);
				return View(nextDraftInputModel);
			}
			try
			{
				nextDraftInputModel.SelectPlayer(playerModel);
			}
			catch (DuplicatePickException ex)
			{
				ModelState.Clear();
				var pick = ex.DuplicatePick;
				ModelState.AddModelError("", string.Format("Error - {0} {1} has already been drafted (Pick #{2}, by {3}).",
						playerModel.FirstName, playerModel.LastName, pick.PickNum, 
						nextDraftInputModel.GetTeamName(pick.UserId)));
				nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(draftInputModel.DraftId);
				return View(nextDraftInputModel);
			}

			ModelState.Clear();
			nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel(draftInputModel.DraftId);
			nextDraftInputModel.Message = string.Format("{0} {1} has been drafted by {2}.",
				playerModel.FirstName, playerModel.LastName, playerModel.TeamName);
			nextDraftInputModel.PickMade = true;
			TempData[Constants.TempData.NextDraftInputModel] = nextDraftInputModel;
			if (!string.IsNullOrEmpty(draftInputModel.Referrer))
			{
				return Redirect(draftInputModel.Referrer);
			}
			return RedirectToAction(viewName);			
		}
		
		public DodgeDynastyContent GetDodgeDynastyCookie()
		{
			var dodgeDynastyContent = new DodgeDynastyContent();
			var dodgeDynastyCookie = Request.Cookies[Constants.Cookies.DodgeDynasty];
			if (dodgeDynastyCookie == null)
			{
				dodgeDynastyContent = SetNewDodgeDynastyCookie();
			}
			else
			{
				var decodedCookie = HttpUtility.UrlDecode(dodgeDynastyCookie.Value);
				dodgeDynastyContent = JsonConvert.DeserializeObject<DodgeDynastyContent>(decodedCookie);
			}
			return dodgeDynastyContent;
		}

		public DodgeDynastyContent SetNewDodgeDynastyCookie()
		{
			var dodgeDynastyContent = new DodgeDynastyContent();
			dodgeDynastyContent.SessionId = Guid.NewGuid().ToString();
			Response.SetCookie(new HttpCookie(Constants.Cookies.DodgeDynasty)
			{
				Expires = DateTime.Now.AddDays(100),
				Value = JsonConvert.SerializeObject(dodgeDynastyContent)
			});
			return dodgeDynastyContent;
		}
    }
}
