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

		public static PlayerRankingsModel CreatePlayerRankingsModel(IPlayerRankModel model)
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

		public static void SetPlayerRanks(IPlayerRankModel model, HomeEntity homeEntity, int rankId)
		{
			model.RankId = rankId;
			model.CurrentRank = homeEntity.Ranks.First(r => r.RankId == rankId);
			model.PlayerRanks = homeEntity.PlayerRanks.Where(pr => pr.RankId == rankId).ToList();
		}
		
		public static List<RankedPlayer> GetRankedPlayersAll(List<PlayerRank> playerRanks, DraftModel currentDraftModel)
		{
			var rankedPlayers = (from pr in playerRanks
								 join p in currentDraftModel.AllPlayers on pr.PlayerId equals p.PlayerId
								 join t in currentDraftModel.NFLTeams on p.NFLTeam equals t.TeamAbbr
								 join ph in currentDraftModel.CurrentPlayerHighlights on pr.PlayerId equals ph.PlayerId into phLeft
								 from ph in phLeft.DefaultIfEmpty()
								 select GetRankedPlayer(pr, p, t, ph)).OrderBy(p => p.RankNum).ToList();
			//return GetDraftedTruePlayersFor(rankedPlayers, currentDraftModel);
			return rankedPlayers;
		}

		public static List<RankedPlayer> GetRankedPlayersAllWithDraftPickInfo(List<PlayerRank> playerRanks, DraftModel currentDraftModel,
            List<RankedPlayer> draftedTruePlayers = null)
		{
			var rankedPlayers = (from pr in playerRanks
								 join p in currentDraftModel.AllPlayers on pr.PlayerId equals p.PlayerId
								 join t in currentDraftModel.NFLTeams on p.NFLTeam equals t.TeamAbbr
								 join pick in currentDraftModel.DraftPicks on pr.PlayerId equals pick.PlayerId into dpLeft    //Left Outer Join
								 from pick in dpLeft.DefaultIfEmpty()
								 join u in currentDraftModel.Users on ((pick != null) ? pick.UserId : -1) equals u.UserId into uLeft  //Left Outer Join
								 from u in uLeft.DefaultIfEmpty()
								 join lo in currentDraftModel.CurrentLeagueOwners on ((pick != null) ? pick.UserId : -1) equals lo.UserId into loLeft
								 from lo in loLeft.DefaultIfEmpty()
								 join ph in currentDraftModel.CurrentPlayerHighlights on pr.PlayerId equals ph.PlayerId into phLeft
								 from ph in phLeft.DefaultIfEmpty()
								 select GetRankedPlayer(pr, p, t, ph, pick, u, lo)).OrderBy(p => p.RankNum).ToList();

			MergeWithDraftedTruePlayers(rankedPlayers, draftedTruePlayers);
			return rankedPlayers;
		}

		public static void MergeWithDraftedTruePlayers(List<RankedPlayer> rankedPlayers, List<RankedPlayer> draftedTruePlayers)
		{
			if (draftedTruePlayers != null && draftedTruePlayers.Count > 0)
			{
				foreach (var dtp in draftedTruePlayers)
				{
					var rankedPlayer = rankedPlayers.FirstOrDefault(o => o.TruePlayerId == dtp.TruePlayerId);
					if (rankedPlayer != null)
					{
						rankedPlayer.PickNum = dtp.PickNum;
						rankedPlayer.UserId = dtp.UserId;
						rankedPlayer.NickName = dtp.NickName;
						rankedPlayer.CssClass = dtp.CssClass;
						rankedPlayer.HighlightClass = dtp.HighlightClass ?? rankedPlayer.HighlightClass;
						rankedPlayer.HighlightRankNum = dtp.HighlightRankNum ?? rankedPlayer.HighlightRankNum;
					}
				}
			}
		}

		//Don't call on every player ranking; only once per page load
		public static List<RankedPlayer> GetDraftedTruePlayersFor(List<Player> inactiveDraftedPlayers, DraftModel currentDraftModel)
		{
			if (inactiveDraftedPlayers != null && inactiveDraftedPlayers.Count > 0)
			{
				return (from p in inactiveDraftedPlayers
                        join t in currentDraftModel.NFLTeams on p.NFLTeam equals t.TeamAbbr
						join pick in currentDraftModel.DraftPicks on p.PlayerId equals pick.PlayerId into dpLeft    //Left Outer Join
						from pick in dpLeft.DefaultIfEmpty()
						join u in currentDraftModel.Users on ((pick != null) ? pick.UserId : -1) equals u.UserId into uLeft  //Left Outer Join
						from u in uLeft.DefaultIfEmpty()
						join lo in currentDraftModel.CurrentLeagueOwners on ((pick != null) ? pick.UserId : -1) equals lo.UserId into loLeft
						from lo in loLeft.DefaultIfEmpty()
						join ph in currentDraftModel.CurrentPlayerHighlights on p.PlayerId equals ph.PlayerId into phLeft
						from ph in phLeft.DefaultIfEmpty()
						select GetRankedPlayer(null, p, t, ph, pick, u, lo)).OrderBy(p => p.RankNum).ToList();
			}
			return null;
		}

		//TODO:  Unused, cleanup
		//public static List<RankedPlayer> GetDraftedTruePlayersFor(List<RankedPlayer> players, DraftModel currentDraftModel)
		//{
		//	foreach (var player in players.Where(o => o.PickNum == null))
		//	{
		//		var truePlayer = currentDraftModel.AllPlayers.FirstOrDefault(o => o.TruePlayerId == player.TruePlayerId &&
		//			o.PlayerId != player.PlayerId && currentDraftModel.DraftPicks.Any(dp => dp.PlayerId == o.PlayerId));
		//		if (truePlayer != null)
		//		{
		//			var draftPick = currentDraftModel.DraftPicks.First(dp => dp.PlayerId == truePlayer.PlayerId);
		//			player.PickNum = draftPick.PickNum.ToString();
		//			player.UserId = draftPick.UserId.ToString();
		//			player.NickName = currentDraftModel.Users.FirstOrDefault(lo => lo.UserId == draftPick.UserId).NickName;
		//			var leagueOwner = currentDraftModel.CurrentLeagueOwners.FirstOrDefault(lo => lo.UserId == draftPick.UserId);
		//			player.CssClass = (leagueOwner != null) ? leagueOwner.CssClass : null;
		//		}
		//	}
		//	return players;
		//}

		public static List<RankedPlayer> GetBestAvailOverallCompRanks(List<RankedPlayer> rankedPlayers, List<Player> draftedPlayers)
		{
			return rankedPlayers.Where(rp => !draftedPlayers.Any(dp => rp.TruePlayerId == dp.TruePlayerId)).ToList();
		}

		public static List<RankedPlayer> GetAllPlayersOverallCompRanks(List<RankedPlayer> rankedPlayers)
		{
			return rankedPlayers.ToList();
		}

		public static RankedPlayer GetRankedPlayer(PlayerRank pr, Player p, NFLTeam t, PlayerHighlight ph = null, DraftPick pick = null,
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

		public static RankedPlayer CopyRankedPlayer(RankedPlayer rp)
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

		public static List<DraftRankModel> GetDraftPublicRankings(int draftId, List<DraftRankModel> currentDraftRanks)
		{
			List<DraftRankModel> publicRankings = new List<DraftRankModel>();
			var allLeaguesRanks = currentDraftRanks.Where(dr => dr.DraftId == null && dr.UserId == null);
			var thisLeagueRanks = currentDraftRanks.Where(dr => dr.DraftId == draftId && dr.UserId == null);
			var primaryCurrentLeagueRanks = currentDraftRanks.Where(dr => dr.UserId == null &&
				(dr.PrimaryDraftRanking.HasValue && dr.PrimaryDraftRanking.Value));

			publicRankings.AddRange(primaryCurrentLeagueRanks
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));
			publicRankings.AddRange(thisLeagueRanks.Where(r => !primaryCurrentLeagueRanks.Contains(r))
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));
			publicRankings.AddRange(allLeaguesRanks.Where(r => !primaryCurrentLeagueRanks.Contains(r))
				.OrderByDescending(r => r.RankDate).ThenByDescending(r => r.LastUpdateTimestamp));

			return publicRankings;
		}
	}
}