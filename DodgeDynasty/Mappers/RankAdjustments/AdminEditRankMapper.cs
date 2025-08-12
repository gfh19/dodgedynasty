using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Shared;
using Microsoft.Ajax.Utilities;

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

			HomeEntity.DraftRanks.Where(o => o.RankId == rankModel.RankId).ForEach(o => HomeEntity.DraftRanks.DeleteObject(o));
			HomeEntity.SaveChanges();

			if (!string.IsNullOrWhiteSpace(rankModel.DraftIdList))
			{
				var draftIdList = rankModel.DraftIdList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(s =>
					{
						int id;
						return int.TryParse(s.Trim(), out id) ? (int?)id : null;
					})
					.ToList();
				foreach (var draftId in draftIdList)
				{
					HomeEntity.DraftRanks.AddObject(addDraftRank(rankModel, rank, draftId));
				}
			}
			else
			{
				HomeEntity.DraftRanks.AddObject(addDraftRank(rankModel, rank, null));
			}
			HomeEntity.SaveChanges();
		}

		private static Entities.DraftRank addDraftRank(AdminRankModel rankModel, Entities.Rank rank, int? draftId)
		{
			var now = Utilities.GetEasternTime();
			return new Entities.DraftRank
			{
				RankId = rank.RankId,
				DraftId = draftId,
				PrimaryDraftRanking = rankModel.PrimaryDraftRanking,
				AddTimestamp = now,
				LastUpdateTimestamp = now
			};
		}
	}
}
