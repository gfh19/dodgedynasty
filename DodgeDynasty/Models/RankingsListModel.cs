using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Ranks;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class RankingsListModel : DraftModelBase
	{
		public RankingsListModel(int? draftId = null) : base(draftId)
		{
			GetCurrentDraft(draftId);
		}

		public RankingsListModel(IDraftModel draftModel) : base(draftModel)
		{}

		public PlayerRankOptions Options { get; set; }

		public List<DraftRankModel> GetPublicRankings()
		{
			List<DraftRankModel> publicRankings = new List<DraftRankModel>();
			var currentDraftRanks = GetCurrentAvailableDraftRanks();

			return PlayerRankModelHelper.GetDraftPublicRankings(DraftId.Value, currentDraftRanks);
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
									select PlayerRankModelHelper.GetDraftRankModel(dr, r);
			return openPublicRanks.FirstOrDefault();
		}

		public List<DraftRankModel> GetAllUserDraftRankings()
		{
			List<DraftRankModel> rankings = new List<DraftRankModel>();
			rankings.AddRange(GetPublicRankings());
			rankings.AddRange(GetPrivateRankings());
			return rankings;
		}
	}
}