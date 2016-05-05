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
		public RankingsListModel(int? draftId = null)
			: base(draftId)
		{
			GetCurrentDraft(DraftId);
		}

		public PlayerRankOptions Options { get; set; }

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

		public int GetPrimaryRankId()
		{
			int rankId;
			var privateRankings = GetPrivateRankings();
			var publicRankings = GetPublicRankings();
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
				var lastPublicRanking = GetLastOpenPublicRanking();
				rankId = (lastPublicRanking != null) ? lastPublicRanking.RankId : 1;
			}
			return rankId;
		}

		public DraftRankModel GetLastOpenPublicRanking()
		{
			var openPublicRanks =	from dr in DraftRanks
									join r in Ranks on dr.RankId equals r.RankId
									where (dr.DraftId == null && dr.UserId == null)
									orderby r.Year descending
									select GetDraftRankModel(dr, r);
			return openPublicRanks.FirstOrDefault();
		}

		public List<DraftRankModel> GetAllUserDraftRankings()
		{
			List<DraftRankModel> rankings = new List<DraftRankModel>();
			rankings.AddRange(GetPrivateRankings());
			rankings.AddRange(GetPublicRankings());
			return rankings;
		}
	}
}