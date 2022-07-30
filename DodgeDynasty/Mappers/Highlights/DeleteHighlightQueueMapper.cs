using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Mappers.Highlights
{
	public class DeleteHighlightQueueMapper : MapperBase<DraftHighlightModel>
	{
		protected override void DoUpdate(DraftHighlightModel model)
		{
			var draftModel = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();

			var draftHighlight = HomeEntity.DraftHighlights.FirstOrDefault(dh => dh.DraftHighlightId == model.DraftHighlightId);
			var currentPlayerHighlights = HomeEntity.PlayerHighlights.AsEnumerable()
				.Where(o => o.UserId == userId && o.DraftId == draftModel.DraftId && o.DraftHighlightId == model.DraftHighlightId).ToList();
			//Only delete queue if no player highlights for it exist
			if (draftHighlight != null && !currentPlayerHighlights.Any())
			{
				HomeEntity.DraftHighlights.DeleteObject(draftHighlight);
				HomeEntity.SaveChanges();
			}
		}
	}
}