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
		static PlayerRankModelHelper _instance = new PlayerRankModelHelper();

		public static PlayerRankModelHelper Instance
		{
			get { return _instance; }
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
			model.CurrentRank = homeEntity.Ranks.First(r => r.RankId == rankId);
			model.PlayerRanks = homeEntity.PlayerRanks.Where(pr => pr.RankId == rankId).ToList();
		}

		public List<RankedPlayer> GetBestAvailOverallCompRanks(List<RankedPlayer> rankedPlayers, List<Player> draftedPlayers)
		{
			return rankedPlayers.Where(rp => !draftedPlayers.Any(dp => rp.TruePlayerId == dp.TruePlayerId)).ToList();
		}

		public List<RankedPlayer> GetAllPlayersOverallCompRanks(List<RankedPlayer> rankedPlayers)
		{
			return rankedPlayers.ToList();
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

		public RankedPlayer CopyRankedPlayer(RankedPlayer rp)
		{
			return new RankedPlayer
			{
				PlayerId = rp.PlayerId,
				TruePlayerId = rp.TruePlayerId,
				RankId = rp.RankId,
				PlayerRankId = rp.PlayerRankId,
				FirstName = rp.FirstName,
				LastName = rp.LastName,
				PlayerName = rp.PlayerName,
				NFLTeam = rp.NFLTeam,
				NFLTeamDisplay = rp.NFLTeamDisplay,
				Position = rp.Position,
				RankNum = rp.RankNum,
				PosRankNum = rp.PosRankNum,
				AuctionValue = rp.AuctionValue,
				PickNum = rp.PickNum,
				UserId = rp.UserId,
				NickName = rp.NickName,
				CssClass = rp.CssClass,
				HighlightClass = rp.HighlightClass,
				HighlightRankNum = rp.HighlightRankNum
			};
		}
	}
}