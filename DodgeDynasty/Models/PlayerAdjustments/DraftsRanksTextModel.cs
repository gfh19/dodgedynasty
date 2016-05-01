using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.PlayerAdjustments
{
	public class DraftsRanksTextModel
	{
		public string Text { get; set; }
		public int? UserId { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
