using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using System.Web.Mvc;

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

		public List<SelectListItem> GetNumOwnersListItems()
		{
			var numOwnersList = new List<int>();
			for (int i=1; i<21; i++) {
				numOwnersList.Add(i);
			}
			return Utilities.GetListItems<int>(numOwnersList, i => i.ToString(), i => i.ToString());
		}
	}
}