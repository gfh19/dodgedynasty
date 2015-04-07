using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers
{
	public class AddLeagueMapper<T> : MapperBase<T> where T : AddLeagueModel, new()
	{
		public override void UpdateModel()
		{
			Model.Users = HomeEntity.Users.ToList();
			Model.Owners = HomeEntity.Owners.ToList();
			Model.OwnerUsers =  GetOwnerUsers(Model.Users, Model.Owners);
			Model.ActiveOwnerUsers = Model.OwnerUsers.Where(o => o.IsActive).ToList();
			Model.NumOwners = Int32.Parse(
				ConfigurationManager.AppSettings[Constants.AppSettings.DefaultNumOwners] ?? "4");
			Model.NewOwnerUsers = new List<OwnerUser>();
			for (int i=0; i<Model.NumOwners; i++) {
				Model.NewOwnerUsers.Add(new OwnerUser());
			}
		}

		private List<OwnerUser> GetOwnerUsers(List<User> users, List<Owner> owners)
		{
			var ownerUsers = from o in owners
							 join u in users on o.UserId equals u.UserId
							 select OwnerUserMapper.GetOwnerUser(o, u, null);
			return ownerUsers.ToList();
		}
	}
}