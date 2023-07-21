using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Account
{
	public class SharedUserInfoMapper<T> : MapperBase<T> where T : UserInfoModel, new()
	{
		public string UserName { get; set; }

		protected override void PopulateModel()
		{
			Model.AllUsers = HomeEntity.Users.ToList();
			var userName = UserName ?? Utilities.GetLoggedInUserName();
			var user = Model.AllUsers.FirstOrDefault(u => u.UserName == userName);
			Model.UserId = user.UserId;
			Model.UserName = user.UserName;
			Model.FirstName = user.FirstName;
			Model.LastName = user.LastName;
			Model.NickName = user.NickName;
			var ownerLeagueSelect = HomeEntity.LeagueOwners.ToList()
					.Join(HomeEntity.Leagues.ToList(), lo => lo.LeagueId, l => l.LeagueId,
						(lo, l) => new
						{
							LeagueOwner = lo,
							AddTimestamp = l.AddTimestamp
						})
				.Where(lo => lo.LeagueOwner.UserId == user.UserId)
				.OrderBy(l=>l.AddTimestamp).ToList();
			if (Model.AdminMode)
			{
				Model.OwnerLeagues = ownerLeagueSelect.Select(ol => ol.LeagueOwner).ToList();
			}
			else
			{
				Model.OwnerLeagues = ownerLeagueSelect.Where(ol=>ol.LeagueOwner.IsActive)
					.Select(ol => ol.LeagueOwner).ToList();
			}
			var cssColors = HomeEntity.CssColors.ToList();
			Model.AvailableLeaguesColors = new Dictionary<int, List<CssColor>>();
			foreach (var ownerLeague in Model.OwnerLeagues)
			{
				List<CssColor> availableLeagueColors = LeagueOwnerHelper.GetAvailableLeagueColors(ownerLeague,
					HomeEntity.CssColors.AsEnumerable(), HomeEntity.LeagueOwners.AsEnumerable());
				Model.AvailableLeaguesColors.Add(ownerLeague.LeagueId, availableLeagueColors);
			}
			var userPreference = HomeEntity.UserPreferences.FirstOrDefault(up => up.UserId == user.UserId)
				?? new UserPreference();
			Model.DraftShowPositionColors = userPreference.DraftShowPositionColors;
			Model.TeamsShowPositionColors = userPreference.TeamsShowPositionColors;
		}

		protected override bool ValidateModel(T model)
		{
			var users = HomeEntity.Users.ToList();
			var userId = users.Where(u => u.UserName == UserName).Select(u => u.UserId).FirstOrDefault();

			var isValid = true;
			ModelState.Clear();
			if (model.OwnerLeagues != null)
			{
				foreach (var ownerLeague in model.OwnerLeagues)
				{
					var leagueColors = HomeEntity.LeagueOwners
						.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId != userId && lo.IsActive)
						.Select(lo => lo.CssClass).ToList();
					var leagueTeamNames = HomeEntity.LeagueOwners
						.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId != userId)
						.Select(lo => lo.TeamName).ToList();
					var leagueNickNames = HomeEntity.LeagueOwners.ToList()
						.Join(users, lo => lo.UserId, u => u.UserId,
							(lo, u) => new
							{
								LeagueId = lo.LeagueId,
								UserId = lo.UserId,
								NickName = u.NickName
							})
						.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId != userId)
						.Select(lo => lo.NickName).ToList();
					if (leagueColors.Contains(ownerLeague.CssClass))
					{
						ModelState.AddModelError("", string.Format("Error - Color already used in {0}.", ownerLeague.LeagueName));
						isValid = false;
					}
					if (string.IsNullOrEmpty(ownerLeague.CssClass))
					{
						ModelState.AddModelError("", "Error - Color cannot be left blank.");
						isValid = false;
					}
					if (leagueTeamNames.Contains(ownerLeague.TeamName))
					{
						ModelState.AddModelError("",
							string.Format("Error - Team Name '{0}' already being used in that league.", ownerLeague.TeamName));
						isValid = false;
					}
					if (string.IsNullOrEmpty(ownerLeague.TeamName))
					{
						ModelState.AddModelError("", "Error - Team Name cannot be left blank.");
						isValid = false;
					}
					if (leagueNickNames.Contains(model.NickName))
					{
						ModelState.AddModelError("",
							string.Format("Error - Nick Name '{0}' already used in league '{1}'.",
								model.NickName, ownerLeague.LeagueName));
						isValid = false;
					}
				}
			}
			return isValid && base.ValidateModel(model);
		}

		protected override void DoUpdate(T model)
		{
			var user = HomeEntity.Users.Where(u => u.UserName == UserName).FirstOrDefault();
			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			user.NickName = model.NickName;
			user.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();

			if (model.OwnerLeagues != null)
			{
				foreach (var ownerLeague in model.OwnerLeagues)
				{
					var leagueOwner = HomeEntity.LeagueOwners
						.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId == user.UserId)
						.FirstOrDefault();
					leagueOwner.TeamName = ownerLeague.TeamName;
					leagueOwner.CssClass = ownerLeague.CssClass;
					leagueOwner.IsActive = ownerLeague.IsActive;
					if (!Utilities.GetBoolConfigVal(Constants.AppSettings.UserEditAudioKillSwitch))
					{
						leagueOwner.AnnouncePrevPick = ownerLeague.AnnouncePrevPick;
						if (!ownerLeague.AnnouncePrevPick)
						{
							leagueOwner.AnnounceAllPicks = false;
						}
					}
					leagueOwner.LastUpdateTimestamp = DateTime.Now;
				}
				HomeEntity.SaveChanges();
			}

			var userPreference = HomeEntity.UserPreferences.FirstOrDefault(up => up.UserId == user.UserId);
			if (userPreference == null)
			{
				userPreference = new UserPreference { UserId = user.UserId, AddTimestamp = DateTime.Now };
				HomeEntity.UserPreferences.AddObject(userPreference);
			}
			userPreference.DraftShowPositionColors = model.DraftShowPositionColors;
			userPreference.TeamsShowPositionColors = model.TeamsShowPositionColors;
			userPreference.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();
		}
	}
}