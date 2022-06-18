using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Shared;
using DodgeDynasty.WebSockets;

namespace DodgeDynasty.Mappers.Account
{
	public class MyInfoMapper : SharedUserInfoMapper<UserInfoModel>
	{
		protected override bool ValidateModel(UserInfoModel model)
		{
			UserName = Utilities.GetLoggedInUserName();
			return base.ValidateModel(model);
		}

		protected override void DoUpdate(UserInfoModel model)
		{
			UserName = Utilities.GetLoggedInUserName();
			base.DoUpdate(model);
			DraftHubHelper.BroadcastDraftToUser(UserName);
		}
	}
}