using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Account
{
	public class UserInfoMapper : MapperBase<UserInfoModel>
	{
		protected override void PopulateModel()
		{
			var allUsers = HomeEntity.Users.ToList();
			var userId = Utilities.GetLoggedInUserId(HomeEntity.Users.ToList());
			var user = allUsers.FirstOrDefault(u => u.UserId == userId);
			Model.UserName = user.UserName;
			Model.FirstName = user.FirstName;
			Model.LastName = user.LastName;
			Model.NickName = user.NickName;
			Model.OwnerLeagues = HomeEntity.LeagueOwners.Where(lo => lo.UserId == userId).ToList();
			var cssColors = HomeEntity.CssColors.ToList();
			Model.AvailableLeagueColors = new Dictionary<int, List<CssColor>>();
			foreach (var ownerLeague in Model.OwnerLeagues)
			{
				List<CssColor> availableLeagueColors = (from cc in HomeEntity.CssColors.AsEnumerable()
														where (!(from lo in HomeEntity.LeagueOwners.AsEnumerable()
																where lo.LeagueId == ownerLeague.LeagueId
																select lo.CssClass).Contains(cc.ClassName) || 
																cc.ClassName == ownerLeague.CssClass)
														select cc).ToList();
				Model.AvailableLeagueColors.Add(ownerLeague.LeagueId, availableLeagueColors);
			}
		}
	}
}