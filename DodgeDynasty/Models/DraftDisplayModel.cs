﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models
{
	public class DraftDisplayModel : DraftModel
	{
		public DraftDisplayModel(int? draftId=null) : base(draftId)
		{
			base.GetCurrentDraft(draftId);
		}

		public bool IsSnakeRound(int roundNum)
		{
			return CurrentDraft.Format == Constants.DraftFormats.Snake && roundNum % 2 == 0;
		}

		public bool IsLastPickBeforeSnakeRound(DraftPick pick)
		{
			var nextPick = DraftPicks.FirstOrDefault(p => p.PickNum > pick.PickNum);
			if (nextPick != null)
			{
				if (!IsSnakeRound(pick.RoundNum) && IsSnakeRound(nextPick.RoundNum))
				{
					return true;
				}
			}
			return false;
		}

		public string GetCurrentGridPlayerClasses(string page, string displayMode)
		{
			string cssClasses = "";
			if (!string.IsNullOrEmpty(CurrentGridPlayer?.PlayerName))
			{
				cssClasses += "filled ";
				var showPosColorsPref = page == "teams" ? UserPreference.TeamsShowPositionColors : UserPreference.DraftShowPositionColors;
				if (displayMode == showPosColorsPref)
				{
					cssClasses += "show-pos-colors ";
				}
				cssClasses += (CurrentDraft.CombineWRTE && CurrentGridPlayer.Position == "TE")
					? $"dp-wrte "
					: $"dp-{CurrentGridPlayer.Position.ToLower()} ";
			}
			return cssClasses;
		}
	}
}