using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Mappers.Admin
{
	public class UserStatusMapper : MapperBase<UserStatusModel>
	{
		public UserStatusMapper(string userId, string isActive)
		{
			CreateModel();
			Model.UserId = Int32.Parse(userId);
			if (!string.IsNullOrEmpty(isActive)) {
				Model.IsActive = Convert.ToBoolean(isActive);
			}
		}

		protected override void DoUpdate(UserStatusModel model)
		{
			var user = HomeEntity.Users.Where(u => u.UserId == model.UserId).FirstOrDefault();
			user.IsActive = model.IsActive;
			user.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();
		}
	}
}