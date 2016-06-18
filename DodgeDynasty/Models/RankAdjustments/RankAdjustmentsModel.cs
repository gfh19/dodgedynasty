using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.RankAdjustments
{
	public class RankAdjustmentsModel
	{
		public List<AdminRankModel> PublicRanks { get; set; }
		public AdminRankModel Rank { get; set; }
	}
}
