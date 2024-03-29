﻿using System;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Highlights
{
	public class DeleteAllHighlightsMapper : MapperBase<PlayerHighlightModel>
	{
		protected override void DoUpdate(PlayerHighlightModel model)
		{
			var draftModel = Factory.Create<SingleDraftMapper>().GetModel();
			var userId = HomeEntity.Users.GetLoggedInUserId();

			var currentPlayerHighlights = HomeEntity.PlayerHighlights.AsEnumerable()
				.Where(o => o.UserId == userId && o.DraftId == draftModel.DraftId && o.DraftHighlightId == model.DraftHighlightId).ToList();
			foreach (var playerHighlight in currentPlayerHighlights)
			{
				HomeEntity.PlayerHighlights.DeleteObject(playerHighlight);
			}
			HomeEntity.DraftHighlights.FirstOrDefault(dh => dh.DraftHighlightId == model.DraftHighlightId).LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();
		}
	}
}