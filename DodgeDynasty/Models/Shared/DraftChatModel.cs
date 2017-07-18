using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.Shared
{
	public class DraftChatModel
	{
		//Display
		public bool IsDraftActive { get; set; }
		public List<UserChatMessage> ChatMessages { get; set; }
		public bool ChatExpanded { get; set; }

		//Add
		public int DraftId { get; set; }
		public int LeagueId { get; set; }
		public string MessageText { get; set; }
	}
}