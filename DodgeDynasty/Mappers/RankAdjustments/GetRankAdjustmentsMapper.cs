using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.PlayerAdjustments;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using Microsoft.Ajax.Utilities;

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
								 where r.Year >= Year-1 && dr.UserId == null
								 select new AdminRankModel
								 {
									 RankId = r.RankId,
									 RankName = r.RankName,
									 Year = r.Year,
									 RankDate = r.RankDate,
									 Url = r.Url,
									 DraftId = dr.DraftId,
									 DraftIdList = (dr.DraftId == null ? "" : SqlFunctions.StringConvert((double)dr.DraftId).Trim()),
									 PrimaryDraftRanking = dr.PrimaryDraftRanking.Value,
									 AutoImportId = r.AutoImportId,
									 AddTimestamp = r.AddTimestamp,
									 LastUpdateTimestamp = r.LastUpdateTimestamp,
									 PlayerCount = HomeEntity.PlayerRanks.Where(o => o.RankId == r.RankId).Count(),
									 DraftIdCount = HomeEntity.DraftRanks.Where(o => o.RankId == r.RankId).Count(),
								 })
								 .DistinctBy(o=>o.RankId)
								 .OrderByDescending(o=>o.LastUpdateTimestamp).ToList();
			foreach (var multiDraftRank in Model.PublicRanks.Where(o => o.DraftIdCount > 1))
			{
				multiDraftRank.DraftIdList = string.Join(", ", HomeEntity.DraftRanks
					.Where(o => o.RankId == multiDraftRank.RankId)
					.Select(o => (o.DraftId == null ? "" : SqlFunctions.StringConvert((double)o.DraftId)).Trim()));
			}
			Model.AutoImports = HomeEntity.AutoImports.ToList();

			Model.InactiveRankedPlayers = GetInactiveRankedPlayers();
			Model.DuplicateRankedPlayers = GetDuplicateRankedPlayers();
        }

		private List<AdjustedPlayer> GetInactiveRankedPlayers()
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var auditPlayers = (from r in HomeEntity.Ranks
								join pr in HomeEntity.PlayerRanks on r.RankId equals pr.RankId
								join p in HomeEntity.Players on pr.PlayerId equals p.PlayerId
								where r.Year == Year && !p.IsActive
								select p).Distinct().OrderBy(o => o.PlayerName).ThenByDescending(o => o.AddTimestamp);
			foreach (var auditPlayer in auditPlayers)
			{
				players.Add(AuditPlayerHelper.GetAuditedPlayer(auditPlayer,
					new List<Draft>(), GetMatchingRanks(auditPlayer), HomeEntity.DraftRanks));
			}
			var auditHighlightPlayers = (from ph in HomeEntity.PlayerHighlights
								join d in HomeEntity.Drafts on ph.DraftId equals d.DraftId
								join p in HomeEntity.Players on ph.PlayerId equals p.PlayerId
								where d.DraftYear == Year && !p.IsActive
								select p).Distinct().OrderBy(o => o.PlayerName).ThenByDescending(o => o.AddTimestamp);
			foreach (var auditHighlightPlayer in auditHighlightPlayers)
			{
				var newAuditPlayer = AuditPlayerHelper.GetAuditedPlayer(auditHighlightPlayer,
					new List<Draft>(), GetMatchingRanks(auditHighlightPlayer), HomeEntity.DraftRanks);
				newAuditPlayer.DraftsRanks.Insert(0, new DraftsRanksTextModel { Text = "Player Highlights!" });
                players.Add(newAuditPlayer);
			}
			return players;
		}

		private List<AdjustedPlayer> GetDuplicateRankedPlayers()
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var auditTruePlayers = from r in HomeEntity.Ranks
							  join pr in HomeEntity.PlayerRanks on r.RankId equals pr.RankId
							  join p in HomeEntity.Players on pr.PlayerId equals p.PlayerId
							  where r.Year == Year
							  group p by new
							  {
								  TruePlayerId = p.TruePlayerId,
								  RankId = r.RankId,
                                  Rank = r
							  } into rp
							  where rp.Count() > 1
							  select new { RankId = rp.Key.RankId, TruePlayerId = rp.Key.TruePlayerId, Rank = rp.Key.Rank };
			foreach (var auditTruePlayer in auditTruePlayers)
			{
				var auditPlayers = HomeEntity.Players.Where(o => o.TruePlayerId == auditTruePlayer.TruePlayerId).ToList();
				foreach (var auditPlayer in auditPlayers.Where(o=> HomeEntity.PlayerRanks.Any(
					p => p.RankId == auditTruePlayer.RankId && p.PlayerId == o.PlayerId)))
				{
					players.Add(AuditPlayerHelper.GetAuditedPlayer(auditPlayer,
						new List<Draft>(), new List<Rank> { auditTruePlayer.Rank }, HomeEntity.DraftRanks));
				}
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
