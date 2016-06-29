using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.RankAdjustments
{
	public class AdminEditRankMapper : MapperBase<AdminRankModel>
	{
		protected override void DoUpdate(AdminRankModel rankModel)
		{
			var now = Utilities.GetEasternTime();
			var rank = HomeEntity.Ranks.First(o => o.RankId == rankModel.RankId);
			rank.RankName = rankModel.RankName;
			rank.Year = Convert.ToInt16(rankModel.Year);
			rank.Url = string.IsNullOrEmpty(rankModel.Url) ? null : rankModel.Url;
			rank.AutoImportId = rankModel.AutoImportId;
			HomeEntity.SaveChanges();

			var draftRank = HomeEntity.DraftRanks.First(o => o.RankId == rankModel.RankId);
			draftRank.RankId = rank.RankId;
			draftRank.DraftId = rankModel.DraftId;
			draftRank.PrimaryDraftRanking = rankModel.PrimaryDraftRanking;
			draftRank.LastUpdateTimestamp = now;
			HomeEntity.SaveChanges();
		}
	}
}
