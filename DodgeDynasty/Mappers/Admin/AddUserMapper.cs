using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Mappers.Admin
{
	public class AddUserMapper : MapperBase<AddUserModel>
	{
		protected override void DoUpdate(AddUserModel model)
		{
			User newUser = new User
			{
				UserName = model.UserName,
				FirstName = model.FirstName,
				LastName = model.LastName,
				NickName = model.NickName,
				IsActive = model.IsActive,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.Users.AddObject(newUser);
			HomeEntity.SaveChanges();
		}
	}
}