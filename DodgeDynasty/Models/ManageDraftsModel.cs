using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class ManageDraftsModel
	{
		public int? LeagueId { get; set; }
		public List<Draft> LeagueDrafts { get; set; }
	}
}