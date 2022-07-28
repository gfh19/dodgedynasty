using System.Collections.Generic;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.Highlights
{
	public class DraftHighlightModel
	{
		public int? DraftHighlightId { get; set; }
		public int UserId { get; set; }
		public int DraftId { get; set; }
		public int DraftYear{ get; set; }
		public string QueueName { get; set; }
		public string LeagueName { get; set; }
		//New ID To Copy Last DH To
		public int NewDraftHighlightId { get; set; }
	}
}
