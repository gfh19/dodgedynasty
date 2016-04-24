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
		public List<string> AddPlayerActions = new List<string> {
			"Add Player"
			,"Add Player, Match Active, IsDrafted"
			,"Add Player, Match Inactive, IsDrafted"
			,"Draft Add Player"
			,"Rank Add Player"
			,"Rank Add Player, Merge TruePlayerId"
			,"Add Player, Match Active, IsDrafted"
			,"Add Player, Match Active, IsDrafted (& Ranks)"
			,"Add Player, Match Both, IsDrafted"
			,"Add Player, Match Both, IsDrafted (& Ranks)"
			,"Add Player, Match Inactive, IsDrafted"
		};

		public List<string> OtherPlayerActions = new List<string> {
			"Draft Activate Player"
			,"Draft Update NFL Team, Activate"
			,"Rank Activate Player"
			,"Rank Update NFL Team, Activate"
			,"Update NFL Team"
			,"Update NFL Team, (Re)activate"
			,"Update NFL Team, Activate"
			,"Update NFL Team, Active Not Drafted"
			,"Update NFL Team, Active Was Drafted"	//Should be invalid
		};

		protected override void PopulateModel()
		{
			var adjustments = HomeEntity.PlayerAdjustments.OrderByDescending(o => o.AddTimestamp).ToList();

			Model.AddedPlayers = GetAddedPlayers(adjustments);
			Model.OtherAdjPlayers = GetOtherAdjPlayers(adjustments);
		}

		private List<AdjustedPlayer> GetAddedPlayers(List<PlayerAdjustment> adjustments)
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var addedPlayerAdjs = adjustments.Where(o => AddPlayerActions.Contains(o.Action)).OrderByDescending(o => o.AddTimestamp).ToList();
			players.AddRange(GetAdjustedPlayers(addedPlayerAdjs));
			return players;
		}

		private List<AdjustedPlayer> GetOtherAdjPlayers(List<PlayerAdjustment> adjustments)
		{
			List<AdjustedPlayer> players = new List<AdjustedPlayer>();
			var otherPlayerAdjs = adjustments.Where(o => !AddPlayerActions.Contains(o.Action)).OrderByDescending(o => o.AddTimestamp).ToList();
			players.AddRange(GetAdjustedPlayers(otherPlayerAdjs));
			return players;
		}

		private IEnumerable<AdjustedPlayer> GetAdjustedPlayers(List<PlayerAdjustment> addedPlayerAdjs)
		{
			return from ap in addedPlayerAdjs
				   join p in HomeEntity.Players on
				   (ap.NewPlayerId != null) ? ap.NewPlayerId : ap.OldPlayerId equals p.PlayerId
				   join t in HomeEntity.NFLTeams on ap.NewNFLTeam equals t.TeamAbbr
				   join u in HomeEntity.Users on ((ap.UserId != null) ? ap.UserId : -1) equals u.UserId into uLeft  //Left Outer Join
				   from u in uLeft.DefaultIfEmpty()
				   select GetAdjustedPlayer(ap, p, t, u, GetMatchingDrafts(p), GetMatchingRanks(p));
		}

		private AdjustedPlayer GetAdjustedPlayer(PlayerAdjustment ap, Player p, NFLTeam t, User u, List<Draft> drafts, List<Rank> ranks)
		{
			return new AdjustedPlayer
			{
				PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId.Value,
				PlayerName = p.PlayerName,
				NFLTeam = p.NFLTeam,
				NFLTeamDisplay = t.AbbrDisplay,
				Position = p.Position,
				Action = ap.Action,
				UserId = (u != null) ? u.UserId.ToString() : null,
				UserFullName = (u != null) ? u.FullName : null,
				DraftsRanksText = GetDraftsRanksText(p, drafts, ranks),
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
				draftsRanksText = string.Format("{0} {1} Ranks", ranks[0].Year, ranks[0].RankName);
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
