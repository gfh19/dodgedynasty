using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Mappers.Admin
{
	public class ManageUsersMapper : MapperBase<ManageUsersModel>
	{
		protected override void PopulateModel()
		{
			Model.ActiveUsers = HomeEntity.Users.Where(u => u.IsActive).OrderBy(u => u.FirstName).ToList();
			Model.InactiveUsers = HomeEntity.Users.Where(u => !u.IsActive).OrderBy(u => u.FirstName).ToList();
		}
	}
}