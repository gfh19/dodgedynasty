using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.Account
{
	public class RoleAccessModel
	{
		public int? UserId { get; set; }
		public int? LeagueId { get; set; }
		public List<UserRole> UserRoles { get; set; }
		public bool IsUserAdmin { get; set; }
		public bool IsUserCommish { get; set; }
	}
}
