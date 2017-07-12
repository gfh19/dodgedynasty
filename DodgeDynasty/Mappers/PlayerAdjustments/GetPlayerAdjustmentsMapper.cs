using System;
using System.Collections.Generic;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.PlayerAdjustments;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

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
			Model.NonUniquePlayers = GetNonUniquePlayers();
			Model.DuplicateActivePlayers = GetDuplicateActivePlayers();
			Model.NFLTeams = HomeEntity.NFLTeams.ToList();
			Model.Positions = HomeEntity.Positions.ToList();
			Model.AllPlayers = HomeEntity.Players.OrderBy(o=>o.PlayerName).ToList();
			Model.ActivePlayerCount = Model.AllPlayers.Count(o => o.IsActive);
			Model.InactivePlayerCount = Model.AllPlayers.Count(o => !o.IsActive);

			var userId = HomeEntity.Users.GetLoggedInUserId();
			var adminStatus = HomeEntity.AdminStatus.FirstOrDefault(o => o.UserId == userId);
			var currentDateTime = Utilities.GetEasternTime();
			if (adminStatus != null)
			{
				var lastPlayerAdjView = adminStatus.LastPlayerAdjView.HasValue ? adminStatus.LastPlayerAdjView : DateTime.MinValue;
                Model.NewAdjustmentIds = adjustments.Where(o => o.AddTimestamp.CompareTo(adminStatus.LastPlayerAdjView) > 0)
					.Select(o => o.AdjustmentId).ToList();
				adminStatus.LastPlayerAdjView = currentDateTime;
			}
			else
			{
				Model.NewAdjustmentIds = adjustments.Select(o => o.AdjustmentId).ToList();
				HomeEntity.AdminStatus.AddObject(new AdminStatu { UserId = userId, LastPlayerAdjView = currentDateTime });
            }
			HomeEntity.SaveChanges();
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
			var otherPlayerAdjs = adjustments.Where(o => !o.Action.Contains(_addPlayerActionText) && o.AddTimestamp.Year == mostRecentYear && !o.HideReporting)
				.OrderByDescending(o => o.AddTimestamp).ToList();
			if (otherPlayerAdjs.Count < _playerAdjWindow)
			{
				otherPlayerAdjs = adjustments.Where(o => !o.Action.Contains(_addPlayerActionText) && o.AddTimestamp.Year >= mostRecentYear - 1 && !o.HideReporting)
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
							 select GetAdjustedPlayer(ap, p, t, u, GetMatchingDrafts(p), GetMatchingRanks(p), HomeEntity.DraftRanks));
			return players;
		}
		
		private AdjustedPlayer GetAdjustedPlayer(PlayerAdjustment ap, Player p, NFLTeam t, User u, List<Draft> drafts, List<Rank> ranks, 
			IEnumerable<DraftRank> draftRanks)
		{
			return new AdjustedPlayer
			{
				AdjustmentId = ap.AdjustmentId,
                PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId.Value,
				PlayerName = ap.NewPlayerName,
				NFLTeam = ap.NewNFLTeam,
				NFLTeamDisplay = t.AbbrDisplay,
				Position = ap.NewPosition,
				Action = ap.Action,
				UserId = (u != null) ? u.UserId.ToString() : null,
				UserFullName = (u != null) ? u.FullName : "---",
				DraftsRanks = AuditPlayerHelper.GetDraftsRanks(p, drafts, ranks, draftRanks),
				IsActive = p.IsActive,
				IsDrafted = p.IsDrafted,
				AddTimestamp = ap.AddTimestamp
			};
		}

		private List<AdjustedPlayer> GetNonUniquePlayers()
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var auditPlayers = from p1 in HomeEntity.Players
							   from p2 in HomeEntity.Players.Where(p2 =>
								  p1.FirstName == p2.FirstName && p1.LastName == p2.LastName && p1.Position == p2.Position &&
								  p1.NFLTeam == p2.NFLTeam && p1.PlayerId != p2.PlayerId && p1.TruePlayerId != p2.TruePlayerId)
							   orderby p2.PlayerName, p2.AddTimestamp descending
							   select p2;
			foreach (var auditPlayer in auditPlayers)
			{
				players.Add(AuditPlayerHelper.GetAuditedPlayer(auditPlayer,
					GetMatchingDrafts(auditPlayer), GetMatchingRanks(auditPlayer), HomeEntity.DraftRanks));
			}
			return players;
		}

		private List<AdjustedPlayer> GetDuplicateActivePlayers()
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var auditPlayerTpids = from p in HomeEntity.Players
								   where p.IsActive
								   group p by p.TruePlayerId into grp
								   where grp.Count() > 1
								   select grp.Key;
			var auditPlayers = from p in HomeEntity.Players
							   where auditPlayerTpids.Contains(p.TruePlayerId)
							   orderby p.TruePlayerId, p.AddTimestamp descending
							   select p;
			foreach (var auditPlayer in auditPlayers)
			{
				players.Add(AuditPlayerHelper.GetAuditedPlayer(auditPlayer,
					GetMatchingDrafts(auditPlayer), GetMatchingRanks(auditPlayer), HomeEntity.DraftRanks));
			}
			return players;
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
