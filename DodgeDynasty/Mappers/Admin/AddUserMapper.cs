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
		protected override void PopulateModel()
		{
			Model.IsActive = true;
		}

		protected override bool ValidateModel(AddUserModel model)
		{
			ModelState.Clear();
			var isValid = !HomeEntity.Users.Any(u=>u.UserName == model.UserName);
			if (!isValid)
			{
				ModelState.AddModelError("", string.Format("Error - User Name '{0}' is already used.", model.UserName));
			}
			return isValid && base.ValidateModel(model);
		}

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