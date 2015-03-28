using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class DraftPicksModel
	{
		public int DraftId { get; set; }
		public List<DraftPick> DraftPicks { get; set; }
	}
}