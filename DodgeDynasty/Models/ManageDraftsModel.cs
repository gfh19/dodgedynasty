using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Models
{
	public class ManageDraftsModel : AdminModeModel
	{
		public int? LeagueId { get; set; }
		public List<Draft> LeagueDrafts { get; set; }
	}
}