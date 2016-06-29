using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.RankAdjustments
{
	public class AdminAddRankMapper : MapperBase<AdminRankModel>
	{
		protected override void DoUpdate(AdminRankModel rankModel)
		{
			var now = Utilities.GetEasternTime();
			var rank = new Entities.Rank
			{
				RankName = rankModel.RankName,
				Year = Convert.ToInt16(rankModel.Year),
				Url = string.IsNullOrEmpty(rankModel.Url) ? null : rankModel.Url,
				AutoImportId = rankModel.AutoImportId,
				RankDate = now.Date,
				AddTimestamp = now,
				LastUpdateTimestamp = now
			};
			HomeEntity.Ranks.AddObject(rank);
			HomeEntity.SaveChanges();

			var draftRank = new Entities.DraftRank
			{
				RankId = rank.RankId,
				DraftId = rankModel.DraftId,
				PrimaryDraftRanking = rankModel.PrimaryDraftRanking,
                AddTimestamp = now,
				LastUpdateTimestamp = now
			};
			HomeEntity.DraftRanks.AddObject(draftRank);
			HomeEntity.SaveChanges();
		}
	}
}
