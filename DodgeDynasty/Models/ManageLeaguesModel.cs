using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class ManageLeaguesModel
	{
		public List<League> AllLeagues { get; set; }
		//Future concept:  Only expose User's leagues if Commish of them ("Commish" access)
//		public List<League> UserLeagues { get; set; }
	}
}