﻿using System;
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
		[Required(ErrorMessage = "First Name is required")]
		[StringLength(25, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 1)]
		public string FirstName { get; set; }
		[Display(Name = "Player Last Name")]
		[Required(ErrorMessage = "Last Name is required")]
		[StringLength(25, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 1)]
		public string LastName { get; set; }
		[Display(Name = "Position")]
		[Required(ErrorMessage = "Position is required")]
		public string Position { get; set; }
		[Display(Name = "NFL Team")]
		[Required(ErrorMessage = "Team is required")]
		public string NFLTeam { get; set; }
		[Display(Name = "User")]
		public int UserId { get; set; }

		public string TeamName { get; set; }
		public int DraftId { get; set; }
		public int DraftPickId { get; set; }
		[Display(Name = "PId")]
		public int PlayerId { get; set; }
		[Display(Name = "True PId")]
		public int? TruePlayerId { get; set; }
		[Display(Name = "Is Active?")]
		public bool IsActive { get; set; }
		[Display(Name = "Is Drafted?")]
		public bool IsDrafted { get; set; }
	}
}