using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;
using DodgeDynasty.Shared.Security;

namespace DodgeDynasty.Mappers
{
	public class PasswordMapper<T> : MapperBase<T> where T : LocalPasswordModel, new()
	{
		public string UserName { get; set; }
		public ManageMessageId PasswordStatus { get; set; }

		protected override void DoUpdate(T model)
		{
			DoUpdateFailed = true;
			User user;
			var currentUser = HomeEntity.Users.GetLoggedInUser();
			if (UserName == Utilities.GetLoggedInUserName())
			{
				user = currentUser;
			}
			else
			{
				user = HomeEntity.Users.FirstOrDefault(u => u.UserName == UserName);
			}

			if (EncryptUtil.VerifyPassword(model.CurrentPassword, currentUser.Password, currentUser.Salt))
			{
				if (model.NewPassword == model.ConfirmPassword)
				{
					var passwordInfo = EncryptUtil.EncryptPasswordForStorage(model.NewPassword);

					if (user != null)
					{
						user.Password = passwordInfo.PasswordHash;
						user.Salt = passwordInfo.Salt;
						HomeEntity.SaveChanges();
						DoUpdateFailed = false;
					}
				}
				else
				{
					PasswordStatus = ManageMessageId.ConfirmPasswordMismatch;
				}
			}
			else
			{
				PasswordStatus = ManageMessageId.CurrentPasswordInvalid;
			}
		}
	}
}