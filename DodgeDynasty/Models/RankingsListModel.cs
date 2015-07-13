using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class RankingsListModel : DraftModel
	{
		public RankingsListModel(int? draftId = null) : base(draftId)
		{
			GetCurrentDraft(DraftId);
		}

		public List<DraftRankModel> GetPublicRankings()
		{
			List<DraftRankModel> publicRankings = new List<DraftRankModel>();
			var currentDraftRanks = GetCurrentAvailableDraftRanks();

			return GetDraftPublicRankings(DraftId.Value, currentDraftRanks);
		}

		public static List<DraftRankModel> GetDraftPublicRankings(int draftId, 
			List<DraftRankModel> currentDraftRanks)
		{
			List<DraftRankModel> publicRankings = new List<DraftRankModel>();
			var allLeaguesRanks = currentDraftRanks.Where(dr => dr.DraftId == null && dr.UserId == null);
			var thisLeagueRanks = currentDraftRanks.Where(dr => dr.DraftId == draftId && dr.UserId == null);
			var primaryCurrentLeagueRanks = currentDraftRanks.Where(dr => dr.UserId == null &&
				(dr.PrimaryDraftRanking.HasValue && dr.PrimaryDraftRanking.Value));

			publicRankings.AddRange(primaryCurrentLeagueRanks
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));
			publicRankings.AddRange(thisLeagueRanks.Where(r => !primaryCurrentLeagueRanks.Contains(r))
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));
			publicRankings.AddRange(allLeaguesRanks.Where(r => !primaryCurrentLeagueRanks.Contains(r))
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));

			return publicRankings;
		}

		public List<DraftRankModel> GetPrivateRankings()
		{
			var currentDraftRanks = GetCurrentAvailableDraftRanks();
			var privateRankings = currentDraftRanks.Where(dr => dr.UserId == CurrentLoggedInOwnerUser.UserId)
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp).ToList();
			return privateRankings.ToList();
		}

		public int GetPrimaryRankId(RankingsListModel rankingsListModel)
		{
			int rankId;
			var privateRankings = rankingsListModel.GetPrivateRankings();
			var publicRankings = rankingsListModel.GetPublicRankings();
			if (privateRankings.Count() > 0)
			{
				rankId = privateRankings[0].RankId;
			}
			else if (publicRankings.Count() > 0)
			{
				rankId = publicRankings[0].RankId;
			}
			else
			{
				rankId = 1;
			}
			return rankId;
		}
	}
}