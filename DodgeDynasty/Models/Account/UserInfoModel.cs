using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DodgeDynasty.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Entities;
using System.Web.Mvc;

namespace DodgeDynasty.Models.Account
{
	public class UserInfoModel
	{
		[Display(Name = "User Name")]
		public string UserName { get; set; }
		[Display(Name = "First Name")]
		[Required]
		[StringLength(15, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 2)]
		public string FirstName { get; set; }
		[Display(Name = "Last Name")]
		[Required]
		[StringLength(25, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 2)]
		public string LastName { get; set; }
		[Display(Name = "Nick Name")]
		[Required]
		[StringLength(10, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 2)]
		public string NickName { get; set; }

		[Display(Name = "Draft page - Show Position Colors:")]
		public string DraftShowPositionColors { get; set; }
		[Display(Name = "Teams page - Show Position Colors:")]
		public string TeamsShowPositionColors { get; set; }

		public List<LeagueOwner> OwnerLeagues { get; set; }

		public bool AdminMode { get; set; }
		public List<User> AllUsers { get; set; }
		public Dictionary<int,List<CssColor>> AvailableLeaguesColors { get; set; }
		public List<SelectListItem> ShowPositionColorOptions { get; set; }

		public List<SelectListItem> GetLeagueColorOptions(int leagueId, string selectedClassName)
		{
			return Utilities.GetListItems<CssColor>(AvailableLeaguesColors[leagueId], cc => cc.ColorText, cc => cc.ClassName,
				false, selectedClassName);
		}

		public List<SelectListItem> GetUserListItems(string userName)
		{
			return Utilities.GetListItems<User>(AllUsers, u => u.UserName, u => u.UserName, false, 
				userName ?? Utilities.GetLoggedInUserName());
		}

		public List<SelectListItem> GetShowPositionColorsOptions(string selectedOption)
		{
			return Utilities.GetListItems<SelectListItem>(getShowPositionColorOptions(), o => o.Text, u => u.Value, false, selectedOption);
		}

		private List<SelectListItem> getShowPositionColorOptions()
		{
			var options = new List<SelectListItem>();
			options.Add(new SelectListItem { Value = Constants.ShowPositionColors.Off, Text = Constants.ShowPositionColors.OffText });
			options.Add(new SelectListItem { Value = Constants.ShowPositionColors.Header, Text = Constants.ShowPositionColors.HeaderText});
			options.Add(new SelectListItem { Value = Constants.ShowPositionColors.Full, Text = Constants.ShowPositionColors.FullText});
			return options;
		}
	}
}