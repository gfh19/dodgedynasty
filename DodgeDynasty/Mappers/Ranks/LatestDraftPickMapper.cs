using System;
using System.Linq;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Ranks
{
	public class LatestDraftPickMapper : MapperBase<LatestPickInfoJson>
	{
		public LatestDraftPickMapper() {}

		protected override void PopulateModel()
		{
			DraftDisplayModel model = DraftFactory.GetDraftDisplayModel();
			Model = new LatestPickInfoJson { status = LatestPickStatusCodes.Empty };

			//Since current pick playerid is always null, to client the secondPrev = prev, and prev = latest
			var prevDraftPick = model.SecondPreviousDraftPick;
			if (prevDraftPick == null)
			{
				prevDraftPick = new Entities.DraftPick { PickEndDateTime = DateTime.MinValue };
            }
			if (model.PreviousDraftPick != null)
			{
				var latestDraftPick = model.PreviousDraftPick;
				var pickUser = model.Users.First(u => u.UserId == latestDraftPick.UserId);
				var currentLgOwner = model.CurrentLeagueOwners.First(lo => lo.UserId == latestDraftPick.UserId);

				Model.pid = latestDraftPick.PlayerId;
				Model.pnum = latestDraftPick.PickNum;
				Model.puid = latestDraftPick.UserId;
				Model.uturnid = model.CurrentClockOwnerUser != null ? (int?)model.CurrentClockOwnerUser.UserId : null;
                Model.oname = pickUser.NickName;
				Model.ocss = currentLgOwner.CssClass;
				Model.ptime = latestDraftPick.PickEndDateTime.ToDateTimeString();
				Model.prevtm = prevDraftPick.PickEndDateTime.ToDateTimeString();
                Model.status = LatestPickStatusCodes.Success;
			}
		}
	}

	public class LatestPickStatusCodes
	{
		public const string Empty = "empty";
		public const string Mismatch = "mismatch";
		public const string Success = "success";
	}
}