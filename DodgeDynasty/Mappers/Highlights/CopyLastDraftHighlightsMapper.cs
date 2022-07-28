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
	public class CopyLastDraftHighlightsMapper : MapperBase<DraftHighlightModel>
	{
		public List<DraftHighlightModel> LastHighlightDrafts { get; set; }

		protected override void DoUpdate(DraftHighlightModel model)
		{
			var currentDraft = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var dhToCopy = HomeEntity.DraftHighlights
				.FirstOrDefault(dh => dh.UserId == userId && dh.DraftHighlightId == model.DraftHighlightId);
			if (dhToCopy != null)
			{
				var lastPlayerHighlights = HomeEntity.PlayerHighlights
					.Where(ph => ph.DraftHighlightId == dhToCopy.DraftHighlightId && ph.UserId == userId).ToList();
				lastPlayerHighlights.ForEach(ph => HomeEntity.PlayerHighlights.AddObject(new Entities.PlayerHighlight
				{
					DraftId = currentDraft.DraftId,
					DraftHighlightId = model.NewDraftHighlightId,
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

		public List<DraftHighlightModel> GetLastHighlightDrafts()
		{
			using (HomeEntity = new Entities.HomeEntity())
			{
				var currentDraft = Factory.Create<SingleDraftMapper>().GetModel();
				var userId = HomeEntity.Users.GetLoggedInUserId();
				var differentDraftHighlights = HomeEntity.DraftHighlights
					.Where(dh => dh.UserId == userId && dh.DraftYear == currentDraft.DraftYear && dh.DraftId != currentDraft.DraftId
							&& HomeEntity.PlayerHighlights.Any(ph => ph.DraftHighlightId == dh.DraftHighlightId));

				LastHighlightDrafts = new List<DraftHighlightModel>();
				differentDraftHighlights.ToList().ForEach(dh =>
					LastHighlightDrafts.Add(new DraftHighlightModel
					{
						DraftHighlightId = dh.DraftHighlightId,
						UserId = dh.UserId,
						DraftId = dh.DraftId.Value, //TODO: Can make it required now
						DraftYear = dh.DraftYear,
						QueueName = dh.QueueName,
						LeagueName = currentDraft.LeagueName
					}));
				return LastHighlightDrafts;
			}
		}
	}
}
