using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Shared;
using DodgeDynasty.Models;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Admin
{
	public class AddDraftMapper<T> : MapperBase<T> where T : AddEditDraftModel, new()
	{
		public int LeagueId { get; set; }

		protected override void PopulateModel()
		{
			Model.LeagueId = LeagueId;
			var league = HomeEntity.Leagues.AsEnumerable().Where(o => o.LeagueId == LeagueId).FirstOrDefault();
			Model.LeagueName = league.LeagueName;
			Model.LeagueOwnerUsers = OwnerUserMapper.GetOwnerUsers(
				HomeEntity.LeagueOwners.Where(o=>o.LeagueId == LeagueId).ToList(),
				HomeEntity.Users.ToList(), LeagueId).Where(o => o.IsActive).ToList();

			var defaultDraftDate = DateTime.Now.AddDays(1).Date + new TimeSpan(20, 0, 0);
			Model.DraftDate = defaultDraftDate.ToString("yyyy-MM-dd");
			Model.DraftTime = defaultDraftDate.ToString("HH:mm");
			Model.DraftYear = (short)defaultDraftDate.Year;
			Model.DraftLocation = "Online";
			Model.NumRounds = league.NumRounds;
			Model.NumKeepers = league.NumKeepers;
			Model.CombineWRTE = league.CombineWRTE;
			Model.Format = league.Format;
			Model.PickTimeSeconds = league.PickTimeSeconds;
			Model.DraftOwnerUsers = Model.LeagueOwnerUsers;
		}

		protected override void DoUpdate(T model)
		{
			Draft draft = new Draft {
				LeagueId = model.LeagueId,
				DraftDate = DateTime.ParseExact(
					string.Format("{0} {1}", model.DraftDate, model.DraftTime), 
					"yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture),
				DraftLocation = model.DraftLocation,
				DraftYear = Convert.ToInt16(model.DraftYear),
				NumRounds = Convert.ToInt16(model.NumRounds),
				NumKeepers = Convert.ToInt16(model.NumKeepers),
				Format = model.Format,
				CombineWRTE = model.CombineWRTE,
				PickTimeSeconds = Convert.ToInt16(model.PickTimeSeconds),
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.Drafts.AddObject(draft);
			HomeEntity.SaveChanges();

			model.DraftId = draft.DraftId;

			foreach (var ownerUser in model.DraftOwnerUsers)
			{
				DraftOwner owner = new DraftOwner
				{
					UserId = ownerUser.UserId,
					DraftId = draft.DraftId,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				HomeEntity.DraftOwners.AddObject(owner);
			}
			HomeEntity.SaveChanges();
		}
	}
}