using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models.Site
{
	public class MessagesModel
	{
		public List<Message> Messages { get; set; }
		public List<Draft> UserChatDrafts { get; set; }
		public List<UserChatMessage> DraftChatMessages { get; set; }

		public string MessageId { get; set; }
		public int LeagueId { get; set; }
		[StringLength(50)]
		public string Title { get; set; }
		[Required]
		[StringLength(1000)]
		public string MessageText { get; set; }

		public List<LeagueOwner> OwnerLeagues { get; set; }

		public List<SelectListItem> GetAudienceLeagues()
		{
			OwnerLeagues.Insert(0, new LeagueOwner{LeagueId=0, LeagueName="Everyone (All My Leagues)"});
			if (Utilities.IsUserAdmin()) {
				OwnerLeagues.Insert(0, new LeagueOwner{LeagueId=-1, LeagueName="Entire Site"});
			}
			return Utilities.GetListItems<LeagueOwner>(OwnerLeagues, o => o.LeagueName, o => o.LeagueId.ToString(),
				false);
		}
	}
}