﻿using System;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Highlights
{
	public class DeletePlayerHighlightMapper : MapperBase<PlayerHighlightModel>
	{
		protected override void DoUpdate(PlayerHighlightModel model)
		{
			var draftModel = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();

			var currentPlayerHighlights = HomeEntity.PlayerHighlights.AsEnumerable()
				.Where(o => o.UserId == userId && o.DraftId == draftModel.DraftId && o.DraftHighlightId == model.DraftHighlightId).ToList();
			var playerHighlight = currentPlayerHighlights.FirstOrDefault(o => o.PlayerId == model.PlayerId);
            if (playerHighlight != null)
			{
				HomeEntity.PlayerHighlights.DeleteObject(playerHighlight);
			}
			HomeEntity.DraftHighlights.FirstOrDefault(dh => dh.DraftHighlightId == model.DraftHighlightId).LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();
		}
	}
}