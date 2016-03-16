using System.Collections.Generic;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.Drafts
{
	public class DraftViewModel
	{
		public int DraftId { get; set; }
		public List<DraftPick> DraftPicks { get; set; }
	}
}
