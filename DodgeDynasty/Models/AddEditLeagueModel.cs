﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using System.Web.Mvc;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Models
{
	public class AddEditLeagueModel : AdminModeModel, ILeagueIdModel
	{
		[Display(Name = "League Name")]
		[Required]
		public string LeagueName { get; set; }
		public List<OwnerUser> LeagueOwnerUsers { get; set; }
		public int LeagueId { get; set; }

		public List<OwnerUser> OwnerUsers { get; set; }
		public List<User> ActiveLeagueUsers { get; set; }
		public List<CssColor> CssColors { get; set; }
		public List<int> CommishUserIds { get; set; }

		public List<SelectListItem> GetActiveLeagueUsers(string userId = null)
		{
			return Utilities.GetListItems<User>(ActiveLeagueUsers.OrderBy(u => u.FirstName).ToList(),
				u => u.FullName, u => u.UserId.ToString(), true, userId);
		}

		public List<SelectListItem> GetLeagueColorOptions(string selectedClassName)
		{
			return Utilities.GetListItems<CssColor>(CssColors, cc => cc.ColorText, cc => cc.ClassName,
				false, selectedClassName);
		}

		public string GetColorText(string cssClass)
		{
			return CssColors.FirstOrDefault(o=>o.ClassName == (cssClass ?? Constants.CssClass.None)).ColorText;
		}

		public bool IsLoggedInUserId(int userId)
		{
			return Utilities.GetLoggedInUserId(ActiveLeagueUsers) == userId;
		}
	}
}