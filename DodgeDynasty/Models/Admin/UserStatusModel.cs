using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Admin
{
	public class UserStatusModel
	{
		public int UserId { get; set; }
		public bool IsActive { get; set; }
	}
}