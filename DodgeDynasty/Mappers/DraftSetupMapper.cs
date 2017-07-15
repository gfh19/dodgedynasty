using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using DodgeDynasty.Models.Shared;

namespace DodgeDynasty.Mappers
{
	public class DraftSetupMapper : ModelBase
	{
		public void UpdateDraftPicks(DraftPicksModel picksModel)
		{
			DraftSetupModel currentModel = DraftFactory.GetDraftSetupModel(picksModel.DraftId);
			currentModel.GetCurrentDraft(picksModel.DraftId);
			using (HomeEntity = new HomeEntity())
			{
				var newDraftPicks = picksModel.DraftPicks.Where(p => p.DraftPickId == 0).ToList();
				var updatedDraftPicks = picksModel.DraftPicks.Where(p => p.DraftPickId != 0).ToList(); ;
				var updatedDraftPickIds = updatedDraftPicks.Select(p => p.DraftPickId).ToList();
				var deletedDraftPickIds = currentModel.DraftPicks
					.Where(p => !updatedDraftPickIds.Contains(p.DraftPickId)).Select(p => p.DraftPickId).ToList();
				var now = Utilities.GetEasternTime();
				foreach (var pick in newDraftPicks)
				{
					var newDraftPick = new DraftPick
					{
						DraftId = picksModel.DraftId,
						PickNum = pick.PickNum,
						RoundNum = pick.RoundNum,
						UserId = pick.UserId,
						PlayerId = pick.PlayerId,
						PickStartDateTime = (pick.PlayerId != null) ? (DateTime?)now : null,
						PickEndDateTime = (pick.PlayerId != null) ? (DateTime?)now : null,
						AddTimestamp = now,
						LastUpdateTimestamp = now
					};
					HomeEntity.DraftPicks.AddObject(newDraftPick);
				}
				HomeEntity.SaveChanges();
				foreach (var pick in updatedDraftPicks)
				{
					var draftPick = HomeEntity.DraftPicks.First(p => p.DraftPickId == pick.DraftPickId);
					draftPick.PickNum = pick.PickNum;
					draftPick.RoundNum = pick.RoundNum;
					draftPick.UserId = pick.UserId;
					draftPick.PlayerId = pick.PlayerId;
					draftPick.PickStartDateTime = (draftPick.PickStartDateTime != null) ? draftPick.PickStartDateTime : ((pick.PlayerId != null) ? (DateTime?)now : null);
					draftPick.PickEndDateTime = (draftPick.PickEndDateTime != null) ? draftPick.PickEndDateTime : ((pick.PlayerId != null) ? (DateTime?)now : null);
					draftPick.LastUpdateTimestamp = now;
				}
				HomeEntity.SaveChanges();
				foreach (var pickId in deletedDraftPickIds)
				{
					var draftPick = HomeEntity.DraftPicks.First(p => p.DraftPickId == pickId);
					HomeEntity.DraftPicks.DeleteObject(draftPick);
				}
				HomeEntity.SaveChanges();

				DraftHelper.SetFirstPickStartTime(HomeEntity, currentModel.CurrentDraft);
			}
		}
	}
}