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
	public class AddEditHighlightQueueMapper : MapperBase<PlayerHighlightModel>
	{
		public Entities.DraftHighlight DraftHighlight { get; set; }
		protected override void DoUpdate(PlayerHighlightModel model)
		{
			if (string.IsNullOrEmpty(model.QueueName)) {
				model.QueueName = Constants.Defaults.DraftHighlightQueueName;
			} 
			if (model.DraftHighlightId.HasValue)
			{
				DraftHighlight = HomeEntity.DraftHighlights.FirstOrDefault(dh => dh.DraftHighlightId == model.DraftHighlightId);
				if (DraftHighlight != null)
				{
					DraftHighlight.QueueName = model.QueueName;
					DraftHighlight.LastUpdateTimestamp = DateTime.Now;
				}
			}
			else
			{
				var draftModel = Factory.Create<SingleDraftMapper>().GetModel();
				var userId = HomeEntity.Users.GetLoggedInUserId();
				DraftHighlight = new Entities.DraftHighlight
				{
					UserId = userId,
					DraftYear = draftModel.DraftYear,
					DraftId = draftModel.DraftId,
					QueueName = model.QueueName,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				HomeEntity.DraftHighlights.AddObject(DraftHighlight);
			}
			HomeEntity.SaveChanges();
		}
	}
}