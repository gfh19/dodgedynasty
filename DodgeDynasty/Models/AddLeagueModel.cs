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
	public class AddLeagueModel
	{
		[Display(Name = "League Name")]
		[Required]
		public string LeagueName { get; set; }
		[Display(Name = "Number of Owners")]
		[Required]
		public int NumOwners { get; set; }

		public List<User> Users { get; set; }
		public List<Owner> Owners { get; set; }
		public List<OwnerUser> OwnerUsers { get; set; }
		public List<OwnerUser> ActiveOwnerUsers { get; set; }
		public List<OwnerUser> NewOwnerUsers { get; set; }

		public List<SelectListItem> GetActiveOwnerUsers()
		{
			return Utilities.GetListItems<OwnerUser>(ActiveOwnerUsers.OrderBy(u=>u.FirstName).ToList(),
				u => u.FullName, u => u.OwnerId.ToString());
		}

	}
}