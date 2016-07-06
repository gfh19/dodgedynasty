using System;
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
		public List<SelectListItem> AudioOptions { get; set; }

		public AddEditLeagueModel()
		{
			AudioOptions = new List<SelectListItem>();
			AudioOptions.Add(new SelectListItem { Value = Constants.Audio.None, Text = Constants.Audio.NoneText });
			AudioOptions.Add(new SelectListItem { Value = Constants.Audio.All, Text = Constants.Audio.AllText });
			AudioOptions.Add(new SelectListItem { Value = Constants.Audio.Prev, Text = Constants.Audio.PrevText });
		}

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

		public SelectListItem GetSelectedAudioOption(OwnerUser leagueOwner)
		{
			SelectListItem item = null;
			if (leagueOwner.AnnounceAllPicks)
			{
				item = AudioOptions.FirstOrDefault(o => o.Value == Constants.Audio.All);
			}
			else if (leagueOwner.AnnouncePrevPick)
			{
				item = AudioOptions.FirstOrDefault(o => o.Value == Constants.Audio.Prev);
			}
			else
			{
				item = AudioOptions.FirstOrDefault(o => o.Value == Constants.Audio.None);
			}
			return item;
		}

		public List<SelectListItem> GetAudioOptions(OwnerUser leagueOwner)
		{
			return Utilities.GetListItems<SelectListItem>(AudioOptions, o=>o.Text, o=>o.Value,
				false, GetSelectedAudioOption(leagueOwner).Value);
		}

	}
}