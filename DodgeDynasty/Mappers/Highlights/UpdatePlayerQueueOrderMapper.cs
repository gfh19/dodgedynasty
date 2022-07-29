using System;
using System.Linq;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Highlights
{
	public class UpdatePlayerQueueOrderMapper : MapperBase<PlayerQueueOrderModel>
	{
		protected override void DoUpdate(PlayerQueueOrderModel model)
		{
			var currentDraft = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var playerHighlights = HomeEntity.PlayerHighlights.AsEnumerable()
				.Where(o => o.DraftId == currentDraft.DraftId && o.UserId == userId && o.DraftHighlightId == model.DraftHighlightId);

			if (playerHighlights != null && playerHighlights.Any())
			{
				if (model.NextPlayerId != null)
				{
					var nextRankNum = playerHighlights.FirstOrDefault(o => o.PlayerId == model.NextPlayerId).RankNum;
					var updatedPlayer = playerHighlights.Where(o => o.PlayerId == model.UpdatedPlayerId).FirstOrDefault();
					updatedPlayer.RankNum = nextRankNum;
					var laterPlayers = playerHighlights
						.Where(o => o.RankNum >= nextRankNum && o.PlayerId != model.UpdatedPlayerId)
						.OrderBy(o => o.RankNum).ToList();
					var laterPlayerRankNum = nextRankNum + 1;
					foreach (var player in laterPlayers)
					{
						player.RankNum = laterPlayerRankNum++;
						player.LastUpdateTimestamp = DateTime.Now;
					}
				}
				else if (model.PreviousPlayerId != null)
				{
					var prevRankNum = playerHighlights.FirstOrDefault(o => o.PlayerId == model.PreviousPlayerId).RankNum;
					var updatedPlayer = playerHighlights.Where(o => o.PlayerId == model.UpdatedPlayerId).FirstOrDefault();
					updatedPlayer.RankNum = prevRankNum + 1;
					var laterPlayers = playerHighlights
						.Where(o => o.RankNum > prevRankNum && o.PlayerId != model.UpdatedPlayerId)
						.OrderBy(o => o.RankNum).ToList();
					var laterPlayerRankNum = prevRankNum + 2;
					foreach (var player in laterPlayers)
					{
						player.RankNum = laterPlayerRankNum++;
						player.LastUpdateTimestamp = DateTime.Now;
					}
				}
				else
				{
					var updatedPlayer = playerHighlights.Where(o => o.PlayerId == model.UpdatedPlayerId).FirstOrDefault();
					updatedPlayer.RankNum = 1;

					var laterPlayers = playerHighlights.Where(o => o.PlayerId != model.UpdatedPlayerId).OrderBy(o => o.RankNum).ToList();
					var laterPlayerRankNum = 2;
					foreach (var player in laterPlayers)
					{
						player.RankNum = laterPlayerRankNum++;
						player.LastUpdateTimestamp = DateTime.Now;
					}
				}
				HomeEntity.DraftHighlights.FirstOrDefault(dh => dh.DraftHighlightId == model.DraftHighlightId).LastUpdateTimestamp = DateTime.Now;
				HomeEntity.SaveChanges();
			}
		}
	}
}
