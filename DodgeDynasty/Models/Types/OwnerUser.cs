using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Types
{
	public class OwnerUser
	{
		public int OwnerId { get; set; }
		public int UserId { get; set; }
		public string UserName { get; set; }
		public byte[] Password { get; set; }
		public byte[] Salt { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public string NickName { get; set; }
		public string CssClass { get; set; }
		public string TeamName { get; set; }
		public bool IsActive { get; set; }
		public DateTime? AddDateTime { get; set; }
		public DateTime? LastLogin { get; set; }
		public DateTime? LastUpdateTimestamp { get; set; }
	}
}