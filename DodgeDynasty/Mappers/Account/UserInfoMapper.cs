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

		protected override bool ValidateModel(UserInfoModel model)
		{
			var isValid = true;
			var userId = Utilities.GetLoggedInUserId(HomeEntity.Users.AsEnumerable());
			foreach (var ownerLeague in model.OwnerLeagues)
			{
				var leagueColors = HomeEntity.LeagueOwners
					.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId != userId)
					.Select(lo => lo.CssClass).ToList();
				var leagueTeamNames = HomeEntity.LeagueOwners
					.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId != userId)
					.Select(lo => lo.TeamName).ToList();
				if (leagueColors.Contains(ownerLeague.CssClass))
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Error - Color already being used that league.");
					isValid = false;
				}
				if (ownerLeague.CssClass == string.Empty)
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Error - Color cannot be left blank.");
					isValid = false;
				}
				if (leagueTeamNames.Contains(ownerLeague.TeamName))
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Error - Team Name already being used that league.");
					isValid = false;
				}
				if (ownerLeague.TeamName == string.Empty)
				{
					ModelState.Clear();
					ModelState.AddModelError("", "Error - Team Name cannot be left blank.");
					isValid = false;
				}
			}
			return isValid && base.ValidateModel(model);
		}

		protected override void DoUpdate(UserInfoModel model)
		{
			var user = HomeEntity.Users.Where(u => u.UserName == Utilities.GetLoggedInUserName()).FirstOrDefault();
			user.UserName = model.UserName;
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.NickName = model.NickName;
			HomeEntity.SaveChanges();

			foreach (var ownerLeague in Model.OwnerLeagues)
			{
				var leagueOwner = HomeEntity.LeagueOwners.Where(lo => lo.LeagueId == ownerLeague.LeagueId)
					.FirstOrDefault();
				leagueOwner.TeamName = ownerLeague.TeamName;
				leagueOwner.CssClass = ownerLeague.CssClass;
				leagueOwner.IsActive = ownerLeague.IsActive;
			}
		}
	}
}