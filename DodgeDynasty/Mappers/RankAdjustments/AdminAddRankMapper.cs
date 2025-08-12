using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Shared;
using Microsoft.Ajax.Utilities;

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
