using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class AddDraftMapper<T> : MapperBase<T> where T : AddEditDraftModel, new()
	{
		public int LeagueId { get; set; }

		protected override void PopulateModel()
		{
			Model.LeagueId = LeagueId;
			var league = HomeEntity.Leagues.AsEnumerable().Where(o => o.LeagueId == LeagueId).FirstOrDefault();
			Model.LeagueName = league.LeagueName;
			Model.LeagueOwnerUsers = OwnerUserMapper.GetOwnerUsers(HomeEntity.LeagueOwners.ToList(), HomeEntity.Users.ToList(), LeagueId);

			var defaultDraftDate = DateTime.Now.AddDays(1).Date + new TimeSpan(20, 0, 0);
			Model.DraftDate = defaultDraftDate.ToString("yyyy-MM-dd");
			Model.DraftTime = defaultDraftDate.ToString("HH:mm");
			Model.DraftYear = defaultDraftDate.Year;
			Model.DraftLocation = "Online";
			Model.NumOwners = Model.LeagueOwnerUsers.Count;
			Model.NumRounds = 15;
			Model.NumKeepers = 0;
			Model.Format = DodgeDynasty.Shared.Constants.DraftFormats.Snake;
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
				NumOwners = Convert.ToInt16(model.NumOwners),
				NumRounds = Convert.ToInt16(model.NumRounds),
				NumKeepers = Convert.ToInt16(model.NumKeepers),
				Format = model.Format,
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