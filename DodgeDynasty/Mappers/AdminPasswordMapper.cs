using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Shared.Security;

namespace DodgeDynasty.Mappers
{
	public class AdminPasswordMapper : PasswordMapper<AdminPasswordModel>
	{
		protected override void PopulateModel()
		{
			base.PopulateModel();
			Model.Users = HomeEntity.Users.ToList();
		}

		protected override void DoUpdate(AdminPasswordModel model)
		{
			UserName = model.UserName;
			base.DoUpdate(model);
		}
	}
}