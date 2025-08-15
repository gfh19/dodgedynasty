using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Account;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Shared;
using DodgeDynasty.WebSockets;

namespace DodgeDynasty.Mappers.Admin
{
	public class UserInfoMapper : SharedUserInfoMapper<UserInfoModel>
	{
		protected override void PopulateModel()
		{
			Model.AdminMode = true;
			var userName = UserName ?? Utilities.GetLoggedInUserName();
			var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == userName);
			Model.IsUserAdmin = HomeEntity.UserRoles.Any(o => o.UserId == user.UserId && o.RoleId == Constants.Roles.Admin);
			if (Model.IsUserAdmin)
			{
				var adminStatus = HomeEntity.AdminStatus.FirstOrDefault(o => o.UserId == user.UserId);
				if (adminStatus != null)
				{
					Model.OnlyShowMyDrafts = adminStatus.OnlyShowMyDrafts;
				}
			}
			base.PopulateModel();
		}
		
		protected override bool ValidateModel(UserInfoModel model)
		{
			UserName = model.UserName;
			return base.ValidateModel(model);
		}

		protected override void DoUpdate(UserInfoModel model)
		{
			UserName = model.UserName;
			base.DoUpdate(model);
			var user = HomeEntity.Users.FirstOrDefault(u => u.UserName == UserName);
			var isUserAdmin = HomeEntity.UserRoles.Any(o => o.UserId == user.UserId && o.RoleId == Constants.Roles.Admin);
			if (isUserAdmin)
			{
				var adminStatus = HomeEntity.AdminStatus.FirstOrDefault(o => o.UserId == user.UserId);
				if (adminStatus != null)
				{
					adminStatus.OnlyShowMyDrafts = model.OnlyShowMyDrafts;
				}
				HomeEntity.SaveChanges();
			}
			DraftHubHelper.BroadcastDraftToUser(UserName);
		}
	}
}