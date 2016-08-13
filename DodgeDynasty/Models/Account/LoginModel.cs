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

		public bool Login(HttpRequestBase request)
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
						user.LoginDomain = request.Url.Host.Truncate(30);
						user.LoginUserAgent = request.UserAgent.Truncate(512);
						HomeEntity.SaveChanges();
					}
				}
			}
			return loginSuccess;
		}
	}
}