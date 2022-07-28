using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.Highlights
{
	public class PlayerQueueOrderModel
	{
		public int DraftHighlightId { get; set; }
		public int UpdatedPlayerId { get; set; }
		public int? PreviousPlayerId { get; set; }
	}
}
