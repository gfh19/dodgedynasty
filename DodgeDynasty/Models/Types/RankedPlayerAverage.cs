using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.Types
{
	public class RankedPlayerAverage
	{
		public int TruePlayerId { get; set; }
		public RankedPlayer RankedPlayer { get; set; }
		public int[] AllRankNums { get; set; }
		public double AvgRankNum { get; set; }
	}
}
