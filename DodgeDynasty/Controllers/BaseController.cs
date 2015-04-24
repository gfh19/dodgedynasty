﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Models;
using DodgeDynasty.Shared.Exceptions;
using DodgeDynasty.Shared;
using DodgeDynasty.Models.Types;
using Newtonsoft.Json;
using DodgeDynasty.SignalR;

namespace DodgeDynasty.Controllers
{
    public class BaseController : Controller
    {
		protected ActionResult InputDraftPick(string viewName, DraftInputModel draftInputModel, bool isAdmin)
		{
			DraftInputModel nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
			var playerModel = draftInputModel.Player;
			nextDraftInputModel.Player = playerModel;
			if (!ModelState.IsValid)
			{
				return View(nextDraftInputModel);
			}
			else if (!isAdmin && !nextDraftInputModel.IsDraftingUserLoggedIn())
			{
				ModelState.Clear();
				ModelState.AddModelError("", "Error - It is not your turn to pick.");
				nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
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
						nextDraftInputModel.GetTeamName(pick.OwnerId)));
				nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
				return View(nextDraftInputModel);
			}

			ModelState.Clear();
			nextDraftInputModel = DraftFactory.GetCurrentDraftInputModel();
			nextDraftInputModel.Message = string.Format("{0} {1} has been drafted by {2}.",
				playerModel.FirstName, playerModel.LastName, playerModel.TeamName);
			nextDraftInputModel.PickMade = true;
			return View(viewName, nextDraftInputModel);
		}

		public PlayerRankOptions GetPlayerRankOptions()
		{
			PlayerRankOptions options = new PlayerRankOptions();
			var optionsCookie = Request.Cookies[Constants.Cookies.PlayerRankOptions];
			if (optionsCookie == null)
			{
				Response.SetCookie(new HttpCookie("playerRankOptions")
				{
					Expires = DateTime.Now.AddDays(100),
					Value = JsonConvert.SerializeObject(options)
				});
			}
			else
			{
				var decodedCookie = HttpUtility.UrlDecode(optionsCookie.Value);
				options = JsonConvert.DeserializeObject<PlayerRankOptions>(decodedCookie);
			}
			return options;
		}

    }
}
