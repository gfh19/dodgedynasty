using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models
{
	public class LocalPasswordModel
	{
		[Required]
		[StringLength(20, ErrorMessage = "The Current password must be between {2} and {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Please verify your Current Password")]
		public string CurrentPassword { get; set; }

		[Required]
		[StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[Required]
		[StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public enum ManageMessageId
	{
		ChangePasswordSuccess,
		SetPasswordSuccess,
		RemoveLoginSuccess,
		CurrentPasswordInvalid,
		ConfirmPasswordMismatch
	}
}