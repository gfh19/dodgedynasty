using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.RankAdjustments;

namespace DodgeDynasty.Mappers.RankAdjustments
{
	public class AdminAddRankMapper : MapperBase<AdminRankModel>
	{
		protected override void DoUpdate(AdminRankModel rankModel)
		{
			var rank = new Entities.Rank
			{
				RankName = rankModel.RankName,
				Year = Convert.ToInt16(rankModel.Year),
				Url = string.IsNullOrEmpty(rankModel.Url) ? null : rankModel.Url,
				AutoImport = rankModel.AutoImport,
				RankDate = DateTime.Now.Date,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.Ranks.AddObject(rank);
			HomeEntity.SaveChanges();

			var draftRank = new Entities.DraftRank
			{
				RankId = rank.RankId,
				DraftId = rankModel.DraftId,
				PrimaryDraftRanking = rankModel.PrimaryDraftRanking,
                AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.DraftRanks.AddObject(draftRank);
			HomeEntity.SaveChanges();
		}
	}
}
