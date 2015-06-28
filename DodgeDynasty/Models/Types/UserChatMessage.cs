using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Types
{
	public class UserChatMessage
	{
		public int DraftId { get; set; }
		public int LeagueId { get; set; }
		public int AuthorId { get; set; }
		public string NickName { get; set; }
		public string CssClass { get; set; }
		public string MessageText { get; set; }
		public DateTime AddTimestamp { get; set; }
		public DateTime LastUpdateTimestamp { get; set; }
	}
}