using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models
{
	public class DraftOrderUserModel
	{
		public int DraftId { get; set; }
		public int RoundId { get; set; }
		public int OrderId { get; set; }
		public int OwnerId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string TeamName { get; set; }
	}
}