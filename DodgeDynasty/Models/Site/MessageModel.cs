using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Site
{
	public class MessageModel
	{
		public int MessageId { get; set; }
		public int AuthorId { get; set; }
		public string Message { get; set; }
		public bool AllUsers { get; set; }
		public int LeagueId { get; set; }
	}
}