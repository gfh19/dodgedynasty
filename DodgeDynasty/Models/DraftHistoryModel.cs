﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class DraftHistoryModel
	{
		public List<League> Leagues { get; set; }
		public List<Draft> AllDrafts { get; set; }
		public List<DraftPick> AllDraftPicks { get; set; }
		public List<User> AllUsers { get; set; }

		public List<Draft> GetLeagueDrafts(int leagueId)
		{
			var leagueDrafts = AllDrafts.Where(d => d.LeagueId == leagueId).OrderByDescending(d => d.DraftDate).ToList();
			return leagueDrafts;
		}

		public bool IsDraftEntered(Draft draft)
		{
			var draftPickCount = AllDraftPicks.Where(p => p.DraftId == draft.DraftId).Count();
			return draftPickCount > 0 || !draft.IsComplete;
		}

		public string GetDraftWinnerName(int? userId)
		{
			var userName = string.Empty;
			if (userId != null)
			{
				userName = AllUsers.Where(u => u.UserId == userId.Value).Select(u => u.FullName).FirstOrDefault();
			}
			return userName;
		}

		public bool LeagueContainsWinner(int leagueId)
		{
			var leagueDrafts = GetLeagueDrafts(leagueId);
			return leagueDrafts.Any(d => d.WinnerId != null);
		}
	}
}