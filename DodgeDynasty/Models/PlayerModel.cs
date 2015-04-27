using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DodgeDynasty.Shared;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class PlayerModel : ModelBase
	{
		[Display(Name = "Player First Name")]
		[Required]
		[StringLength(25, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
		public string FirstName { get; set; }
		[Display(Name = "Player Last Name")]
		[Required]
		[StringLength(25, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
		public string LastName { get; set; }
		[Display(Name = "Position")]
		[Required]
		public string Position { get; set; }
		[Display(Name = "NFL Team")]
		[Required]
		public string NFLTeam { get; set; }
		[Display(Name = "User")]
		public int UserId { get; set; }

		public string TeamName { get; set; }
		public int DraftId { get; set; }
		public int DraftPickId { get; set; }
		public int PlayerId { get; set; }
	}
}