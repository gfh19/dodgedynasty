using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Ranks
{
	public class PlayerRankModelHelper
	{
		public static PlayerRankModelHelper Instance
		{
			get { return new PlayerRankModelHelper(); }
		}

		public PlayerRankingsModel CreatePlayerRankingsModel(IPlayerRankModel model)
		{
			var response = new PlayerRankingsModel
			{
				DraftId = model.DraftId,
				NFLTeams = model.NFLTeams,
				ByeWeeks = model.ByeWeeks,
				AllPlayers = model.AllPlayers,
				ActivePlayers = model.ActivePlayers,
				DraftedPlayers = model.DraftedPlayers,
				Positions = model.Positions,
				Leagues = model.Leagues,
				CurrentPlayerHighlights = model.CurrentPlayerHighlights,
				Options = model.Options
			};

			return response;
		}

		public void SetPlayerRanks(IPlayerRankModel model, HomeEntity homeEntity, int rankId)
		{
			model.RankId = rankId;
			var userId = homeEntity.Users.GetLoggedInUserId();
			model.CurrentRank = homeEntity.Ranks.First(r => r.RankId == rankId);
			model.PlayerRanks = homeEntity.PlayerRanks.Where(pr => pr.RankId == rankId).ToList();
		}

		public void GetBestAvailOverallCompPlayerRanks(IPlayerRankModel model)
		{
			var rankedPlayerIds = model.PlayerRanks.Select(pr => pr.PlayerId).ToList();

			model.RankedPlayers = (from pr in model.PlayerRanks
								   join p in model.AllPlayers on pr.PlayerId equals p.PlayerId
								   join t in model.NFLTeams on p.NFLTeam equals t.TeamAbbr
								   join ph in model.CurrentPlayerHighlights on pr.PlayerId equals ph.PlayerId into phLeft
								   from ph in phLeft.DefaultIfEmpty()
								   select GetRankedPlayer(pr, p, t, ph)).OrderBy(p => p.RankNum).ToList();

			model.OverallRankedPlayers = model.RankedPlayers.Where(rp => !model.DraftedPlayers.Any(dp => rp.TruePlayerId == dp.TruePlayerId)).ToList();
			//model.QBRankedPlayers = model.OverallRankedPlayers.Where(p => p.Position == "QB").ToList();
			//model.RBRankedPlayers = model.OverallRankedPlayers.Where(p => p.Position == "RB").ToList();
			//model.WRTERankedPlayers = model.OverallRankedPlayers.Where(p => p.Position == "WR" || p.Position == "TE").ToList();
			//model.DEFRankedPlayers = model.OverallRankedPlayers.Where(p => p.Position == "DEF").ToList();
			//model.KRankedPlayers = model.OverallRankedPlayers.Where(p => p.Position == "K").ToList();
			//model.HighlightedPlayers = bestAvailHighlightedPlayers;
		}

		public RankedPlayer GetRankedPlayer(PlayerRank pr, Player p, NFLTeam t, PlayerHighlight ph = null, DraftPick pick = null,
			User u = null, LeagueOwner lo = null)
		{
			return new RankedPlayer
			{
				PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId.Value,
				RankId = (pr != null) ? pr.RankId : -1,
				PlayerRankId = (pr != null) ? pr.PlayerRankId : -1,
				FirstName = p.FirstName,
				LastName = p.LastName,
				PlayerName = p.PlayerName,
				NFLTeam = p.NFLTeam,
				NFLTeamDisplay = t.AbbrDisplay,
				Position = p.Position,
				RankNum = (pr != null) ? pr.RankNum : null,
				PosRankNum = (pr != null) ? pr.PosRankNum : null,
				AuctionValue = (pr != null) ? pr.AuctionValue : null,
				PickNum = (pick != null) ? pick.PickNum.ToString() : null,
				UserId = (u != null) ? u.UserId.ToString() : null,
				NickName = (u != null) ? u.NickName : null,
				CssClass = (u != null) ? lo.CssClass : null,
				HighlightClass = (ph != null) ? ph.HighlightClass : null,
				HighlightRankNum = (ph != null) ? ph.RankNum.ToString() : null
			};
		}
	}
}