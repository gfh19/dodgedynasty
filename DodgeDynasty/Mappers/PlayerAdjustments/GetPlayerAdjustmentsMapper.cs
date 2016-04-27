using System;
using System.Collections.Generic;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.PlayerAdjustments;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Mappers.PlayerAdjustments
{
	public class GetPlayerAdjustmentsMapper : MapperBase<PlayerAdjustmentsModel>
	{
		private int _playerAdjWindow = 20;
		private string _addPlayerActionText = "Add Player";

		protected override void PopulateModel()
		{
			var adjustments = HomeEntity.PlayerAdjustments.OrderByDescending(o => o.AddTimestamp).ToList();
			var mostRecentYear = adjustments.OrderByDescending(o=>o.AddTimestamp).Select(o=>o.AddTimestamp.Year).Distinct().FirstOrDefault();

			Model.AddedPlayers = GetAddedPlayers(adjustments, mostRecentYear);
			Model.OtherAdjPlayers = GetOtherAdjPlayers(adjustments, mostRecentYear);
			Model.NFLTeams = HomeEntity.NFLTeams.ToList();
			Model.Positions = HomeEntity.Positions.ToList();
			Model.AllPlayers = HomeEntity.Players.OrderBy(o=>o.PlayerName).ToList();
		}

		private List<AdjustedPlayer> GetAddedPlayers(List<PlayerAdjustment> adjustments, int mostRecentYear)
		{
			var addedPlayerAdjs = adjustments.Where(o => o.Action.Contains(_addPlayerActionText) && o.AddTimestamp.Year == mostRecentYear)
				.OrderByDescending(o => o.AddTimestamp).ToList();
			if (addedPlayerAdjs.Count < _playerAdjWindow)
			{
				addedPlayerAdjs = adjustments.Where(o => o.Action.Contains(_addPlayerActionText) && o.AddTimestamp.Year >= mostRecentYear-1)
					.OrderByDescending(o => o.AddTimestamp).ToList();
			}
			return GetAdjustedPlayers(addedPlayerAdjs);
		}

		private List<AdjustedPlayer> GetOtherAdjPlayers(List<PlayerAdjustment> adjustments, int mostRecentYear)
		{
			var otherPlayerAdjs = adjustments.Where(o => !o.Action.Contains(_addPlayerActionText) && o.AddTimestamp.Year == mostRecentYear)
				.OrderByDescending(o => o.AddTimestamp).ToList();
			if (otherPlayerAdjs.Count < _playerAdjWindow)
			{
				otherPlayerAdjs = adjustments.Where(o => !o.Action.Contains(_addPlayerActionText) && o.AddTimestamp.Year >= mostRecentYear - 1)
					.OrderByDescending(o => o.AddTimestamp).ToList();
			}
			return GetAdjustedPlayers(otherPlayerAdjs);
		}

		private List<AdjustedPlayer> GetAdjustedPlayers(List<PlayerAdjustment> playerAdjs)
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			players.AddRange(from ap in playerAdjs
							 join p in HomeEntity.Players on
							 (ap.NewPlayerId != null) ? ap.NewPlayerId : ap.OldPlayerId equals p.PlayerId
							 join t in HomeEntity.NFLTeams on ap.NewNFLTeam equals t.TeamAbbr
							 join u in HomeEntity.Users on ((ap.UserId != null) ? ap.UserId : -1) equals u.UserId into uLeft  //Left Outer Join
							 from u in uLeft.DefaultIfEmpty()
							 select GetAdjustedPlayer(ap, p, t, u, GetMatchingDrafts(p), GetMatchingRanks(p)));
			return players;
		}
		
		private AdjustedPlayer GetAdjustedPlayer(PlayerAdjustment ap, Player p, NFLTeam t, User u, List<Draft> drafts, List<Rank> ranks)
		{
			return new AdjustedPlayer
			{
				PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId.Value,
				PlayerName = ap.NewPlayerName,
				NFLTeam = ap.NewNFLTeam,
				NFLTeamDisplay = t.AbbrDisplay,
				Position = ap.NewPosition,
				Action = ap.Action,
				UserId = (u != null) ? u.UserId.ToString() : null,
				UserFullName = (u != null) ? u.FullName : null,
				DraftsRanksText = GetDraftsRanksText(p, drafts, ranks),
				IsActive = p.IsActive,
				IsDrafted = p.IsDrafted,
				AddTimestamp = ap.AddTimestamp
			};
		}

		private string GetDraftsRanksText(Player p, List<Draft> drafts, List<Rank> ranks)
		{
			string draftsRanksText = "---";
			var latestDraft = drafts.OrderByDescending(o => o.DraftDate).Select(o => o.DraftDate).FirstOrDefault();
			var latestRank = ranks.OrderByDescending(o => o.AddTimestamp).Select(o => o.AddTimestamp).FirstOrDefault();
			if (drafts.Count > 0 && latestDraft > latestRank)
			{
				draftsRanksText = string.Format("{0} {1} Draft", drafts[0].DraftYear, drafts[0].LeagueName);
				if (drafts.Count > 1 || ranks.Count > 0)
				{
					draftsRanksText += " (& more...)";
				}
            }
			else if (ranks.Count > 0)
			{
				var rankName = ranks[0].RankName;
				if (rankName.EndsWith(" Ranks"))
				{
					draftsRanksText = string.Format("{0} {1}", ranks[0].Year, rankName);
				}
				else
				{
					draftsRanksText = string.Format("{0} {1} Ranks", ranks[0].Year, rankName);
				}
				if (ranks.Count > 1)
				{
					draftsRanksText += " (& more...)";
				}
			}
			return draftsRanksText;
		}

		private List<Draft> GetMatchingDrafts(Player p)
		{
			return (from d in HomeEntity.Drafts
					join dp in HomeEntity.DraftPicks on d.DraftId equals dp.DraftId
					where dp.PlayerId == p.PlayerId
					orderby d.DraftDate descending
					select d).ToList();
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
