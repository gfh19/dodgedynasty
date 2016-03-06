using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.Types
{
	public class DraftPlayerHighlight
	{
		public int DraftId { get; set; }
		public short DraftYear { get; set; }
		public string LeagueName { get; set; }
		public int PlayerId { get; set; }
	}
}
