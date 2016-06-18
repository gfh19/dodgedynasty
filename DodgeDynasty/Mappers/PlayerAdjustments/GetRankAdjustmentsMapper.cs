using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.PlayerAdjustments
{
	public class GetRankAdjustmentsMapper : MapperBase<RankAdjustmentsModel>
	{
		public int Year { get; set; }

		protected override void PopulateModel()
		{
			//TODO:  Someday consider allowing Year to be input
			Year = Utilities.GetEasternTime().Year;
			Model.Rank = new AdminRankModel { Year = Year };
			Model.PublicRanks = (from r in HomeEntity.Ranks
								 join dr in HomeEntity.DraftRanks on r.RankId equals dr.RankId
								 where r.Year == Year && dr.UserId == null
								 select new AdminRankModel
								 {
									 RankId = r.RankId,
									 RankName = r.RankName,
									 Year = r.Year,
									 RankDate = r.RankDate,
									 Url = r.Url,
									 DraftId = dr.DraftId,
									 PrimaryDraftRanking = dr.PrimaryDraftRanking.Value,
									 AutoImport = r.AutoImport,
									 AddTimestamp = r.AddTimestamp,
									 LastUpdateTimestamp = r.LastUpdateTimestamp,
									 PlayerCount = HomeEntity.PlayerRanks.Where(o => o.RankId == r.RankId).Count()
								 }).ToList();
		}
	}
}
