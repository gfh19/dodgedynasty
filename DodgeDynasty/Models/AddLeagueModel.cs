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
	}
}