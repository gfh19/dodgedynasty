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
		[Required]
		[StringLength(20, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 1)]
		public string UserName { get; set; }
		[Display(Name = "First Name")]
		[Required]
		[StringLength(15, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 1)]
		public string FirstName { get; set; }
		[Display(Name = "Last Name")]
		[Required]
		[StringLength(25, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 1)]
		public string LastName { get; set; }
		[Display(Name = "Nick Name")]
		[Required]
		[StringLength(10, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 1)]
		public string NickName { get; set; }
		public List<LeagueOwner> OwnerLeagues { get; set; }

		public Dictionary<int,List<CssColor>> AvailableLeagueColors { get; set; }

		public List<SelectListItem> GetLeagueColorOptions(int leagueId, string selectedClassName)
		{
			return Utilities.GetListItems<CssColor>(AvailableLeagueColors[leagueId], cc => cc.ColorText, cc => cc.ClassName,
				false, selectedClassName);
		}
	}
}