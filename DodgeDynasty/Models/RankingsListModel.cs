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
		public List<DraftRankModel> GetCurrentAvailableDraftRanks()
		{
			var fullDraftRanks = from dr in DraftRanks
									  join r in Ranks on dr.RankId equals r.RankId
									  where ((dr.DraftId == null && r.Year == CurrentDraft.DraftYear) || dr.DraftId == DraftId)
										&& (dr.OwnerId == null || dr.OwnerId == CurrentLoggedInOwnerUser.OwnerId)
									  select GetDraftRankModel(dr, r);
			return fullDraftRanks.ToList();
		}

		private DraftRankModel GetDraftRankModel(DraftRank dr, Rank r)
		{
			return new DraftRankModel
			{
				DraftRankId = dr.DraftRankId,
				RankId = r.RankId,
				DraftId = dr.DraftId,
				PrimaryDraftRanking = dr.PrimaryDraftRanking,
				OwnerId = dr.OwnerId,
				RankName = r.RankName,
				Year = r.Year,
				RankDate = r.RankDate,
				Url = r.Url,
				AddTimestamp = r.AddTimestamp,
				LastUpdateTimestamp = r.LastUpdateTimestamp
			};
		}

		public List<DraftRankModel> GetPublicRankings()
		{
			List<DraftRankModel> publicRankings = new List<DraftRankModel>();
			var currentDraftRanks = GetCurrentAvailableDraftRanks();

			var allLeaguesRanks = currentDraftRanks.Where(dr => dr.DraftId == null && dr.OwnerId == null);
			var thisLeagueRanks = currentDraftRanks.Where(dr => dr.DraftId == DraftId && dr.OwnerId == null);
			var primaryCurrentLeagueRanks = currentDraftRanks.Where(dr => dr.OwnerId == null &&
				(dr.PrimaryDraftRanking.HasValue && dr.PrimaryDraftRanking.Value));
			
			publicRankings.AddRange(primaryCurrentLeagueRanks
				.OrderByDescending(r=>r.RankDate).ThenByDescending(r=>r.LastUpdateTimestamp));
			publicRankings.AddRange(thisLeagueRanks.Where(r => !primaryCurrentLeagueRanks.Contains(r))
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));
			publicRankings.AddRange(allLeaguesRanks.Where(r => !primaryCurrentLeagueRanks.Contains(r))
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));

			return publicRankings;
		}

		public List<DraftRankModel> GetPrivateRankings()
		{
			var currentDraftRanks = GetCurrentAvailableDraftRanks();
			var privateRankings = currentDraftRanks.Where(dr => dr.OwnerId == CurrentLoggedInOwnerUser.OwnerId)
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