using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class ActivateDraftModel
	{
		public string DraftStatus { get; set; }
		public List<Draft> AllDrafts { get; set; }
		public List<Draft> ActiveDrafts { get; set; }
		public List<Draft> ScheduledDrafts { get; set; }
		public List<Draft> CompleteDrafts { get; set; }
	}
}