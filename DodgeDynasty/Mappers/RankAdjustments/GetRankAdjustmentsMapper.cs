using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.RankAdjustments
{
	public class GetRankAdjustmentsMapper : MapperBase<RankAdjustmentsModel>
	{
		public int Year { get; set; }

		protected override void PopulateModel()
		{
			//TODO:  Someday maybe consider allowing Year to be input if wanted
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
									 AutoImportId = r.AutoImportId,
									 AddTimestamp = r.AddTimestamp,
									 LastUpdateTimestamp = r.LastUpdateTimestamp,
									 PlayerCount = HomeEntity.PlayerRanks.Where(o => o.RankId == r.RankId).Count()
								 }).OrderByDescending(o=>o.LastUpdateTimestamp).ToList();
			Model.AutoImports = HomeEntity.AutoImports.ToList();

			Model.InactiveRankedPlayers = GetInactiveRankedPlayers();
			Model.DuplicateRankedPlayers = GetDuplicateRankedPlayers();
        }

		private List<AdjustedPlayer> GetInactiveRankedPlayers()
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var auditPlayers = (from r in HomeEntity.Ranks
								join dr in HomeEntity.DraftRanks on r.RankId equals dr.RankId
								join pr in HomeEntity.PlayerRanks on r.RankId equals pr.RankId
								join p in HomeEntity.Players on pr.PlayerId equals p.PlayerId
								where r.Year == Year && !p.IsActive
								select p).Distinct().OrderBy(o => o.PlayerName).ThenByDescending(o => o.AddTimestamp);
			foreach (var auditPlayer in auditPlayers)
			{
				players.Add(AuditPlayerHelper.GetAuditedPlayer(auditPlayer,
					new List<Draft>(), GetMatchingRanks(auditPlayer), HomeEntity.DraftRanks));
			}
			return players;
		}

		private List<AdjustedPlayer> GetDuplicateRankedPlayers()
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var auditPlayers = from r in HomeEntity.Ranks
							  join dr in HomeEntity.DraftRanks on r.RankId equals dr.RankId
							  join pr in HomeEntity.PlayerRanks on r.RankId equals pr.RankId
							  join p in HomeEntity.Players on pr.PlayerId equals p.PlayerId
							  where r.Year == Year
							  group p by new
							  {
								  PlayerId = p.PlayerId,
								  RankId = r.RankId,
								  Player = p,
                                  Rank = r
							  } into rp
							  where rp.Count() > 1
							  select new { RankId = rp.Key.RankId, Player = rp.Key.Player, Rank = rp.Key.Rank };
			foreach (var auditPlayer in auditPlayers)
			{
				players.Add(AuditPlayerHelper.GetAuditedPlayer(auditPlayer.Player,
					new List<Draft>(), new List<Rank> { auditPlayer.Rank }, HomeEntity.DraftRanks));
			}
			return players;
		}

		private List<Rank> GetMatchingRanks(Player p)
		{
			return (from r in HomeEntity.Ranks
					join pr in HomeEntity.PlayerRanks on r.RankId equals pr.RankId
					where pr.PlayerId == p.PlayerId
					select r).ToList();
		}
	}
}
