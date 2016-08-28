using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.RankAdjustments
{
	public class ImportRankModel
	{
		public string FirstPlayerText { get; set; }
		public string LastPlayerText { get; set; }
		public int PlayerCount { get; set; }
		public int MaxPlayerCount { get; set; }
		public string ErrorMessage { get; set; }
		public string StackTrace { get; set; }
		public bool MyProperty { get; set; }
	}
}
