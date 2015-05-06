using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.Admin
{
	public class ManageUsersModel
	{
		public List<User> ActiveUsers { get; set; }
		public List<User> InactiveUsers { get; set; }
	}
}