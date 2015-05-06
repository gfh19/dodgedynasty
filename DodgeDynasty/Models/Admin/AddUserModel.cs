using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models.Admin
{
	public class AddUserModel : UserInfoModel
	{
		[Display(Name = "User Name")]
		[Required]
		[StringLength(20, ErrorMessage = Constants.Messages.StringLength, MinimumLength = 2)]
		public new string UserName { get; set; }
		
		[Display(Name = "Active?")]
		[Required]
		public bool IsActive { get; set; }
	}
}