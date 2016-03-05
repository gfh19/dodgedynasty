using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.ViewTypes
{
	public class RankCategoryModel
	{
		public string DataLink { get; set; }
		public string Header { get; set; }
		public bool ShowPos { get; set; }
		public bool ShowByeWeek { get; set; }
		public string ExpandId { get; set; }
		public string ExpandValue { get; set; }
		public List<RankedPlayer> PlayerList { get; set; }
		//TODO: Someday, configure all positions to be configureable
	}
}
