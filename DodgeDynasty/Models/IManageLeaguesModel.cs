using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public interface IManageLeaguesModel
	{
		List<League> AllLeagues { get; set; }
	}
}
