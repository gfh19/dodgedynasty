using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.RankAdjustments
{
	public class AdminRankModel
	{
		public int RankId { get; set; }
		[Display(Name = "Rank Name")]
		[Required]
		public string RankName { get; set; }
		[Required]
		public int Year { get; set; }
		public DateTime RankDate { get; set; }
		[Display(Name = "Url (Optional)")]
		public string Url { get; set; }
		[Display(Name = "Draft(s) (Optional)")]
		public string DraftIdList { get; set; }
		public int? DraftId { get; set; }
		[Display(Name = "Primary Draft Rank?")]
		public bool PrimaryDraftRanking { get; set; }
		[Display(Name = "Auto Import?")]
		public int? AutoImportId { get; set; }
		public DateTime AddTimestamp { get; set; }
		public DateTime LastUpdateTimestamp { get; set; }
		public int PlayerCount { get; set; }
		public int DraftIdCount { get; set; }
	}
}
