using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.Account;
using DodgeDynasty.Models.Account;

namespace DodgeDynasty.Mappers.Admin
{
	public class UserInfoMapper : SharedUserInfoMapper<UserInfoModel>
	{
		protected override void PopulateModel()
		{
			Model.AdminMode = true;
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
		}
	}
}