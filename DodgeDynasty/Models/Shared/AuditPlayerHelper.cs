using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.PlayerAdjustments;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.Shared
{
	public class AuditPlayerHelper
	{
		public static AdjustedPlayer GetAuditedPlayer(Player p, List<Draft> drafts, List<Rank> ranks, IEnumerable<DraftRank> draftRanks)
		{
			return new AdjustedPlayer
			{
				PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId.Value,
				PlayerName = p.PlayerName,
				NFLTeam = p.NFLTeam,
				Position = p.Position,
				DraftsRanks = GetDraftsRanks(p, drafts, ranks, draftRanks),
				IsActive = p.IsActive,
				IsDrafted = p.IsDrafted,
				AddTimestamp = p.AddTimestamp.Value
			};
		}

		public static List<DraftsRanksTextModel> GetDraftsRanks(Player p, List<Draft> drafts, List<Rank> ranks, IEnumerable<DraftRank> draftRanks)
		{
			List<DraftsRanksTextModel> results = new List<DraftsRanksTextModel>();

			results.AddRange(drafts.OrderByDescending(o => o.DraftDate).Select(o => new DraftsRanksTextModel
			{
				Text = string.Format("{0} {1} Draft", o.DraftYear, o.LeagueName),
				Timestamp = o.DraftDate
			}));
			results.AddRange(ranks.Join(draftRanks, r => r.RankId, dr => dr.RankId,
				(r, dr) => new
				{
					Rank = r,
					DraftRank = dr
				})
				.OrderByDescending(o => o.Rank.AddTimestamp).Select(o => new DraftsRanksTextModel
				{
					Text = string.Format("{0} {1}", o.Rank.Year, FormatRankTextName(o.Rank)),
					UserId = o.DraftRank.UserId,
					Timestamp = o.Rank.AddTimestamp
				}));
			results = results.OrderByDescending(o => o.Timestamp).ToList();
			if (results.Count == 0)
			{
				results.Add(new DraftsRanksTextModel { Text = "---", Timestamp = DateTime.Now });
			}

			return results;
		}

		public static string FormatRankTextName(Rank rank)
		{
			var rankName = rank.RankName;
			if (!rank.RankName.EndsWith(" Ranks"))
			{
				rankName = rankName + " Ranks";
			}
			return rankName;
		}
	}
}
