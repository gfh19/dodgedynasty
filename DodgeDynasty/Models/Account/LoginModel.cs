using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DodgeDynasty.Shared.Security;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models
{
	public class LoginModel : ModelBase
	{
		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		public bool Login()
		{
			bool loginSuccess = false;
			using (HomeEntity = new Entities.HomeEntity())
			{
				var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == UserName);
				if (user != null && user.IsActive)
				{
					loginSuccess = EncryptUtil.VerifyPassword(Password, user.Password, user.Salt);
					if (loginSuccess)
					{
						user.LastLogin = DateTime.Now;
						HomeEntity.SaveChanges();
					}
				}
			}
			return loginSuccess;
		}

		public bool IsUserAdmin()
		{
			bool isUserAdmin = false;
			if (HttpContext.Current.User == null)
			{
				return isUserAdmin;
			}
			using (HomeEntity = new Entities.HomeEntity())
			{
				var identity = HttpContext.Current.User.Identity;
				var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == identity.Name);
				if (user != null)
				{
					var matches = HomeEntity.UserRoles.Where(ur => ur.UserId == user.UserId && ur.RoleId == Constants.Roles.Admin);
					isUserAdmin = matches.Count() > 0;
				}
			}
			return isUserAdmin;
		}
	}
}