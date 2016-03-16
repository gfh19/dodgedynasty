using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Models
{
	public class ActivateDraftModel : AdminModeModel
	{
		public string DraftStatus { get; set; }
		public List<Draft> AllDrafts { get; set; }
		public List<Draft> ActiveDrafts { get; set; }
		public List<Draft> ScheduledDrafts { get; set; }
		public List<Draft> CompleteDrafts { get; set; }
	}
}