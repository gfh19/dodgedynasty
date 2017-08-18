using System;
using System.Collections.Generic;
using System.Linq;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Ranks
{
	public class BroadcastLatestDraftPickMapper : MapperBase<LatestPickInfoJson>
	{
		public BroadcastLatestDraftPickMapper() {}

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
				var currentDraftPick = model.CurrentDraftPick;
				var pickUser = model.Users.First(u => u.UserId == latestDraftPick.UserId);
				var currentLgOwner = model.CurrentLeagueOwners.First(lo => lo.UserId == latestDraftPick.UserId);
				var player = model.AllPlayers.FirstOrDefault(p => p.PlayerId == latestDraftPick.PlayerId);
				var nflTeam = (player != null) ? model.NFLTeams.First(o => o.TeamAbbr == player.NFLTeam.ToUpper()) : null;

				Model.pid = latestDraftPick.PlayerId;
				Model.pnum = latestDraftPick.PickNum;
				Model.puid = latestDraftPick.UserId;
				Model.uturnid = model.CurrentClockOwnerUser != null ? (int?)model.CurrentClockOwnerUser.UserId : null;
                Model.oname = pickUser.NickName;
				Model.ocss = currentLgOwner.CssClass;
				Model.petime = latestDraftPick.PickEndDateTime.ToDateTimeString();
				Model.prevtm = prevDraftPick.PickEndDateTime.ToDateTimeString();
				Model.auduids = new List<int>();
				foreach (var draftOwner in model.DraftOwnerUsers)
				{
					if (draftOwner.AnnounceAllPicks || (draftOwner.AnnouncePrevPick && (draftOwner.UserId == Model.uturnid || !Model.uturnid.HasValue)))
					{
						Model.auduids.Add(draftOwner.UserId);
					}
				}
				foreach (var guestId in model.UserRoles.Where(o=>o.RoleId == Constants.Roles.Guest).Select(o=>o.UserId))
				{
					Model.auduids.Add(guestId);
				}
				if (player != null)	//No reason should ever be null...
				{
					Model.pname = player.PlayerName;
					Model.team = nflTeam.AbbrDisplay;
					Model.pos = player.Position;
				}
				if (currentDraftPick != null)
				{
					Model.curpnum = currentDraftPick.PickNum;
					var currPickUser = model.Users.First(u => u.UserId == currentDraftPick.UserId);
					Model.curoname = currPickUser.NickName;
                    Model.curpstime = currentDraftPick.PickStartDateTime.ToDateTimeString();
					Model.pctr = new Dictionary<int, int>();
					foreach (var user in model.DraftUsers)
					{
						Model.pctr.Add(user.UserId, model.GetPickCountUntilNextTurn(user.UserId));
					}
					Model.status = LatestPickStatusCodes.Success;
				}
				else
				{
					Model.status = LatestPickStatusCodes.Completed;
				}
				Model.curtm = model.GetCurrentTimeEastern(DateTime.UtcNow).ToDateTimeString();
			}
		}
	}

	public class LatestPickStatusCodes
	{
		public const string Empty = "empty";
		public const string Mismatch = "mismatch";
		public const string Success = "success";
		public const string Completed = "completed";
	}
}