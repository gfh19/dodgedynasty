using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Shared.Security;

namespace DodgeDynasty.Mappers
{
	public class PasswordMapper<T> : MapperBase<T> where T : LocalPasswordModel, new()
	{
		public string UserName { get; set; }

		protected override void DoUpdate(T model)
		{
			var passwordInfo = EncryptUtil.EncryptPasswordForStorage(model.NewPassword);

			var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == UserName);
			if (user != null)
			{
				user.Password = passwordInfo.PasswordHash;
				user.Salt = passwordInfo.Salt;
				HomeEntity.SaveChanges();
				UpdateSucceeded = true;
			}
			else
			{
				UpdateSucceeded = false;
			}
		}
	}
}