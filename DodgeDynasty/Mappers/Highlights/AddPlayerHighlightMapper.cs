using System;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Highlights
{
	public class AddPlayerHighlightMapper : MapperBase<PlayerHighlightModel>
	{
		protected override void DoUpdate(PlayerHighlightModel model)
		{
			var draftModel = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();

			var highlightId = HomeEntity.Highlights.First(o => o.HighlightClass == model.HighlightClass).HighlightId;
			var currentPlayerHighlights = HomeEntity.PlayerHighlights.AsEnumerable()
				.Where(o => o.UserId == userId && o.DraftId == draftModel.DraftId).ToList();
			var maxPlayerRankNum = currentPlayerHighlights.Select(o=>o.RankNum).DefaultIfEmpty().Max();
            var playerHighlight = currentPlayerHighlights.FirstOrDefault(o => o.PlayerId == model.PlayerId);
			if (playerHighlight != null)
			{
				playerHighlight.HighlightId = highlightId;
				playerHighlight.RankNum = model.RankNum ?? playerHighlight.RankNum;
				playerHighlight.LastUpdateTimestamp = DateTime.Now;
			}
			else
			{
				playerHighlight = new PlayerHighlight
				{
					DraftId = draftModel.DraftId,
					UserId = userId,
					PlayerId = model.PlayerId,
					HighlightId = highlightId,
					RankNum = maxPlayerRankNum + 1,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				HomeEntity.PlayerHighlights.AddObject(playerHighlight);
			}
			HomeEntity.SaveChanges();
		}
	}
}