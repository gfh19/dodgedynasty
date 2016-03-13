using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Models
{
	public class ManageLeaguesModel : AdminModeModel
	{
		public List<League> Leagues { get; set; }
	}
}