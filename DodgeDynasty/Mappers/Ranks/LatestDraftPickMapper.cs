using System.Linq;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Ranks
{
	public class LatestDraftPickMapper : MapperBase<LatestPickInfoJson>
	{
		public string LastPickEndTime { get; set; }

		public LatestDraftPickMapper(string lastPickEndTime)
		{
			LastPickEndTime = lastPickEndTime;
        }

		protected override void PopulateModel()
		{
			DraftDisplayModel model = DraftFactory.GetDraftDisplayModel();
			Model = new LatestPickInfoJson { status = LatestPickStatusCodes.Empty };

			if (model.SecondPreviousDraftPick == null || model.SecondPreviousDraftPick.PickEndDateTime.ToDateTimeString() != LastPickEndTime)
			{
				Model.status = LatestPickStatusCodes.Mismatch;
			}
			else if (model.PreviousDraftPick != null)
			{
				var latestDraftPick = model.PreviousDraftPick;
				var pickUser = model.Users.First(u => u.UserId == latestDraftPick.UserId);
				var currentLgOwner = model.CurrentLeagueOwners.First(lo => lo.UserId == latestDraftPick.UserId);

				Model.pid = latestDraftPick.PlayerId;
				Model.pnum = latestDraftPick.PickNum;
				Model.yours = (model.CurrentUserId == latestDraftPick.UserId);
				Model.uturn = model.IsUserTurn();
				Model.oname = pickUser.NickName;
				Model.ocss = currentLgOwner.CssClass;
				Model.ptime = latestDraftPick.PickEndDateTime.ToDateTimeString();
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