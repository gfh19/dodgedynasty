using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.Shared
{
	public class LeagueOwnerHelper
	{
		public static List<CssColor> GetAvailableLeagueColors(LeagueOwner ownerLeague, IEnumerable<CssColor> cssColors,
			IEnumerable<LeagueOwner> leagueOwners)
		{
			List<CssColor> availableLeagueColors = (from cc in cssColors
													where (!(from lo in leagueOwners
															 where lo.LeagueId == ownerLeague.LeagueId
															 select lo.CssClass).Contains(cc.ClassName) ||
															cc.ClassName == ownerLeague.CssClass)
													select cc).ToList();
			return availableLeagueColors;
		}
	}
}