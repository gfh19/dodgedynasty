using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models.Shared
{
	public class LeagueOwnerHelper
	{
		public static List<CssColor> GetAvailableLeagueColors(LeagueOwner ownerLeague, IEnumerable<CssColor> cssColors,
			IEnumerable<LeagueOwner> leagueOwners)
		{
			List<CssColor> availableLeagueColors = (from cc in cssColors
													where (!(from lo in leagueOwners
															 where lo.LeagueId == ownerLeague.LeagueId && lo.IsActive
															 select lo.CssClass).Contains(cc.ClassName) ||
															cc.ClassName == ownerLeague.CssClass ||
															cc.ClassName == Constants.CssClass.None)
													select cc).ToList();
			return availableLeagueColors;
		}
	}
}