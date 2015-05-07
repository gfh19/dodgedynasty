﻿using System;
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
			Model.UserName = user.UserName;
			Model.FirstName = user.FirstName;
			Model.LastName = user.LastName;
			Model.NickName = user.NickName;
			Model.OwnerLeagues = HomeEntity.LeagueOwners.Where(lo => lo.UserId == user.UserId).ToList();
			var cssColors = HomeEntity.CssColors.ToList();
			Model.AvailableLeaguesColors = new Dictionary<int, List<CssColor>>();
			foreach (var ownerLeague in Model.OwnerLeagues)
			{
				List<CssColor> availableLeagueColors = LeagueOwnerHelper.GetAvailableLeagueColors(ownerLeague, 
					HomeEntity.CssColors.AsEnumerable(), HomeEntity.LeagueOwners.AsEnumerable());
				Model.AvailableLeaguesColors.Add(ownerLeague.LeagueId, availableLeagueColors);
			}
		}

		protected override bool ValidateModel(T model)
		{
			var userId = HomeEntity.Users.Where(u => u.UserName == UserName).Select(u=>u.UserId).FirstOrDefault();

			var isValid = true;
			ModelState.Clear();
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
					ModelState.AddModelError("", "Error - Color already being used that league.");
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
						string.Format("Error - Team Name '{0}' already being used that league.", ownerLeague.TeamName));
					isValid = false;
				}
				if (string.IsNullOrEmpty(ownerLeague.TeamName))
				{
					ModelState.AddModelError("", "Error - Team Name cannot be left blank.");
					isValid = false;
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

			foreach (var ownerLeague in model.OwnerLeagues)
			{
				var leagueOwner = HomeEntity.LeagueOwners
					.Where(lo => lo.LeagueId == ownerLeague.LeagueId && lo.UserId == user.UserId)
					.FirstOrDefault();
				leagueOwner.TeamName = ownerLeague.TeamName;
				leagueOwner.CssClass = ownerLeague.CssClass;
				leagueOwner.IsActive = ownerLeague.IsActive;
				leagueOwner.LastUpdateTimestamp = DateTime.Now;
			}
			HomeEntity.SaveChanges();
		}
	}
}