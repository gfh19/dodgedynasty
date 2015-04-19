using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class EditDraftMapper<T> : MapperBase<T> where T : AddEditDraftModel, new()
	{
		public int LeagueId { get; set; }
		public int DraftId { get; set; }

		protected override void PopulateModel()
		{
			Model.DraftId = DraftId;

			var draft = HomeEntity.Drafts.Where(o => o.DraftId == DraftId).FirstOrDefault();
			var league = HomeEntity.Leagues.AsEnumerable().Where(o => o.LeagueId == draft.LeagueId).FirstOrDefault();
			Model.LeagueId = league.LeagueId;
			var owners = HomeEntity.Owners.ToList();
			var users = HomeEntity.Users.ToList();
			var allLeagueOwners = HomeEntity.LeagueOwners.ToList();
			var leagueOwners = allLeagueOwners.Where(o => o.LeagueId == Model.LeagueId).ToList();

			Model.LeagueName = league.LeagueName;
			Model.LeagueOwnerUsers = OwnerUserMapper.GetOwnerUsers(leagueOwners, owners, users);

			Model.DraftDate = draft.DraftDate.ToString("yyyy-MM-dd");
			Model.DraftTime = draft.DraftDate.ToString("HH:mm");
			Model.DraftYear = draft.DraftYear.Value;
			Model.DraftLocation = draft.DraftLocation;
			Model.NumOwners = draft.NumOwners.Value;
			Model.NumRounds = draft.NumRounds.Value;
			Model.NumKeepers = draft.NumKeepers.Value;
			Model.Format = draft.Format;
			Model.DraftOwnerUsers = (from dro in HomeEntity.DraftOwners.AsEnumerable()
									 join o in owners on dro.OwnerId equals o.OwnerId
									 join u in users on o.UserId equals u.UserId
									 join lo in leagueOwners on o.UserId equals lo.UserId
									 where dro.DraftId == Model.DraftId
									 select OwnerUserMapper.GetOwnerUser(o, u, lo)).ToList();
		}

		protected override void DoUpdate(T model)
		{
			Draft draft = HomeEntity.Drafts.Where(o => o.DraftId == model.DraftId).FirstOrDefault();
			draft.LeagueId = model.LeagueId;
			draft.DraftDate = DateTime.ParseExact(
				string.Format("{0} {1}", model.DraftDate, model.DraftTime),
				"yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
			draft.DraftLocation = model.DraftLocation;
			draft.DraftYear = Convert.ToInt16(model.DraftYear);
			draft.NumOwners = Convert.ToInt16(model.NumOwners);
			draft.NumRounds = Convert.ToInt16(model.NumRounds);
			draft.NumKeepers = Convert.ToInt16(model.NumKeepers);
			draft.Format = model.Format;
			draft.IsActive = model.IsActive;
			draft.IsComplete = model.IsComplete;
			draft.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();

			var draftOwners = HomeEntity.DraftOwners.Where(o => o.DraftId == model.DraftId).ToList();

			foreach (var oldOwner in draftOwners)
			{
				HomeEntity.DraftOwners.DeleteObject(oldOwner);
			}

			foreach (var ownerUser in model.DraftOwnerUsers)
			{
				DraftOwner owner = new DraftOwner
				{
					OwnerId = ownerUser.UserId,
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