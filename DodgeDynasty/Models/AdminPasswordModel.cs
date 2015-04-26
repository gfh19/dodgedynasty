using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models
{
	public class AdminPasswordModel : LocalPasswordModel
	{
		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "User Name")]
		public new string UserName { get; set; }

		public List<User> Users { get; set; }

		public List<SelectListItem> GetUserNames(string userId = null)
		{
			return Utilities.GetListItems<User>(Users.OrderBy(u => u.UserName).ToList(),
				u => string.Format("{0} ({1})", u.UserName, u.FullName), u => u.UserName, true, userId);
		}
	}
}