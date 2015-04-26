using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using System.Web.Mvc;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class AddEditLeagueModel
	{
		[Display(Name = "League Name")]
		[Required]
		public string LeagueName { get; set; }
		public List<OwnerUser> LeagueOwnerUsers { get; set; }
		public int LeagueId { get; set; }

		public List<OwnerUser> OwnerUsers { get; set; }
		public List<OwnerUser> ActiveOwnerUsers { get; set; }

		public List<SelectListItem> GetActiveOwnerUsers(string userId=null)
		{
			return Utilities.GetListItems<OwnerUser>(ActiveOwnerUsers.OrderBy(u=>u.FirstName).ToList(),
				u => u.FullName, u => u.UserId.ToString(), true, userId);
		}

	}
}