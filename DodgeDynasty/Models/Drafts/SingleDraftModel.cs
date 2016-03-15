using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.Drafts
{
	public class SingleDraftModel
	{
		public int DraftId { get; set; }
		public int LeagueId { get; set; }
		public string LeagueName { get; set; }
		public short DraftYear { get; set; }
	}
}
