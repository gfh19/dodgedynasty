using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Drafts;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Highlights
{
	public class CopyLastDraftHighlightsMapper : MapperBase<SingleDraftModel>
	{
		protected override void PopulateModel()
		{
			var currentDraft = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();
			GetLastHighlightDraft(currentDraft, userId);
		}

		protected override void DoUpdate(SingleDraftModel model)
		{
			var currentDraft = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();
			GetLastHighlightDraft(currentDraft, userId);
			if (Model != null)
			{
				var lastPlayerHighlights = HomeEntity.PlayerHighlights
					.Where(ph => ph.DraftId == Model.DraftId && ph.UserId == userId).ToList();
				lastPlayerHighlights.ForEach(ph=>HomeEntity.PlayerHighlights.AddObject(new Entities.PlayerHighlight
				{
					DraftId = currentDraft.DraftId,
					UserId = userId,
					PlayerId = ph.PlayerId,
					HighlightId = ph.HighlightId,
					RankNum = ph.RankNum,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				}));
				HomeEntity.SaveChanges();
            }
		}

		private void GetLastHighlightDraft(SingleDraftModel currentDraft, int userId)
		{
			Model = (from ph in HomeEntity.PlayerHighlights.AsEnumerable()
					 join d in HomeEntity.Drafts.AsEnumerable() on ph.DraftId equals d.DraftId
					 where ph.UserId == userId && d.DraftYear == currentDraft.DraftYear
					 orderby ph.LastUpdateTimestamp descending
					 select new SingleDraftModel
					 {
						 DraftId = ph.DraftId,
						 DraftYear = d.DraftYear.Value,
						 LeagueName = d.LeagueName
					 }).FirstOrDefault();
		}
	}
}
