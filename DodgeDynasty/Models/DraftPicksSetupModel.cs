using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Models
{
	public class DraftPicksModel : IDraftIdModel
	{
		public int DraftId { get; set; }
		public List<DraftPick> DraftPicks { get; set; }
	}
}