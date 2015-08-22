using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using System.Configuration;
using System.Web.Mvc;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models
{
	public class PlayerRankModel : RankIdModel
	{
		public int RanksWindow { get; set; }
		public Rank CurrentRank { get; set; }
		public List<PlayerRank> PlayerRanks { get; set; }
		public List<RankedPlayer> RankedPlayers { get; set; }
		public List<RankedPlayer> OverallRankedPlayers { get; set; }
		public List<RankedPlayer> QBRankedPlayers { get; set; }
		public List<RankedPlayer> RBRankedPlayers { get; set; }
		public List<RankedPlayer> WRTERankedPlayers { get; set; }
		public List<RankedPlayer> DEFRankedPlayers { get; set; }
		public List<RankedPlayer> KRankedPlayers { get; set; }
		public PlayerRankOptions Options { get; set; }
		public List<Player> SortedPlayers { get; set; }
		public string RankStatus { get; set; }
		public PlayerModel Player { get; set; }

		public PlayerRankModel(int rankId, int? draftId = null)
			: this(draftId)
		{}

		public PlayerRankModel(int? draftId = null)
		{
			base.GetCurrentDraft(draftId);
			int window;
			RanksWindow = int.TryParse(ConfigurationManager.AppSettings["RanksWindow"], out window) ? window : 12;
		}

		public void SetPlayerRanks(int rankId)
		{
			RankId = rankId;
			using (HomeEntity = new HomeEntity())
			{
				CurrentRank = HomeEntity.Ranks.First(r => r.RankId == rankId);
				PlayerRanks = HomeEntity.PlayerRanks.Where(pr => pr.RankId == rankId).ToList();
			}
		}

		public void GetBestAvailPlayerRanks()
		{
			var rankedPlayerIds = PlayerRanks.Select(pr=>pr.PlayerId).ToList();

			RankedPlayers = (from pr in PlayerRanks
							join p in AllPlayers on pr.PlayerId equals p.PlayerId
							join t in NFLTeams on p.NFLTeam equals t.TeamAbbr
							select GetRankedPlayer(pr, p, t)).OrderBy(p=>p.RankNum).ToList();

			OverallRankedPlayers = RankedPlayers.Where(rp=>!DraftedPlayers.Any(dp=>rp.TruePlayerId == dp.TruePlayerId)).ToList();
			QBRankedPlayers = OverallRankedPlayers.Where(p=>p.Position == "QB").ToList();
			RBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "RB").ToList();
			WRTERankedPlayers = OverallRankedPlayers.Where(p => p.Position == "WR" || p.Position == "TE").ToList();
			DEFRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "DEF").ToList();
			KRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "K").ToList();
		}

		public void GetAllPlayerRanksByPosition()
		{
			GetAllPlayerRanks();

			OverallRankedPlayers = RankedPlayers.ToList();
			QBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "QB").ToList();
			RBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "RB").ToList();
			WRTERankedPlayers = OverallRankedPlayers.Where(p => p.Position == "WR" || p.Position == "TE").ToList();
			DEFRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "DEF").ToList();
			KRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "K").ToList();
		}

		public List<RankedPlayer> GetAllPlayerRanks()
		{
			var draftPickTruePlayers = DraftPicks.Join(AllPlayers, dp => dp.PlayerId, p => p.PlayerId, (dp, p) => new
			{
				DraftPickId = dp.DraftPickId,
				PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId,
				PickNum = dp.PickNum,
				UserId = dp.UserId,
			});
			var rankedPlayerIds = PlayerRanks.Select(pr => pr.PlayerId).ToList();

			RankedPlayers = (from pr in PlayerRanks
							 join p in AllPlayers on pr.PlayerId equals p.PlayerId
							 join t in NFLTeams on p.NFLTeam equals t.TeamAbbr
							 join pick in DraftPicks on 
								pr.PlayerId equals pick.PlayerId into dpLeft	//Left Outer Join
							 from pick in dpLeft.DefaultIfEmpty()
							 join u in Users on ((pick != null) ? pick.UserId : -1) equals u.UserId into uLeft	//Left Outer Join
							 from u in uLeft.DefaultIfEmpty()
							 join lo in CurrentLeagueOwners on ((pick != null) ? pick.UserId : -1) equals lo.UserId into loLeft
							 from lo in loLeft.DefaultIfEmpty()
							 select GetRankedPlayer(pr, p, t, pick, u, lo)).OrderBy(p => p.RankNum).ToList();
			foreach (var rankedPlayer in RankedPlayers)
			{
				if (rankedPlayer.PickNum == null)
				{
					var draftPickTruePlayer = draftPickTruePlayers.FirstOrDefault(dp=>dp.TruePlayerId == rankedPlayer.TruePlayerId);
					if (draftPickTruePlayer != null)
					{
						rankedPlayer.PickNum = draftPickTruePlayer.PickNum.ToString();
						rankedPlayer.UserId = draftPickTruePlayer.UserId.ToString();
						rankedPlayer.NickName = Users.FirstOrDefault(lo => lo.UserId == draftPickTruePlayer.UserId).NickName;
						var leagueOwner = CurrentLeagueOwners.FirstOrDefault(lo=>lo.UserId == draftPickTruePlayer.UserId);
						rankedPlayer.CssClass = (leagueOwner != null) ? leagueOwner.CssClass : null;
					}
				}
			}
			return RankedPlayers;
		}

		private RankedPlayer GetRankedPlayer(PlayerRank pr, Player p, NFLTeam t, DraftPick pick=null, User u=null, LeagueOwner lo=null)
		{
			return new RankedPlayer
			{
				PlayerId = p.PlayerId,
				TruePlayerId = p.TruePlayerId.Value,
				RankId = pr.RankId,
				PlayerRankId = pr.PlayerRankId,
				FirstName = p.FirstName,
				LastName = p.LastName,
				PlayerName = p.PlayerName,
				NFLTeam = p.NFLTeam,
				NFLTeamDisplay = t.AbbrDisplay,
				Position = p.Position,
				RankNum = pr.RankNum,
				PosRankNum = pr.PosRankNum,
				AuctionValue = pr.AuctionValue,
				PickNum = (pick != null) ? pick.PickNum.ToString() : null,
				UserId = (u != null) ? u.UserId.ToString() : null,
				NickName = (u != null) ? u.NickName : null,
				CssClass = (u != null) ? lo.CssClass : null
			};
		}

		public List<SelectListItem> GetRankPlayersListItem(string playerId)
		{
			List<Player> players = GetSortedPlayers();
			var items = Utilities.GetListItems<Player>(players, p => GetPlayerDetails(p), p => p.PlayerId.ToString(), true, playerId);
			return items;
		}

		private List<Player> GetSortedPlayers()
		{
			if (SortedPlayers == null)
			{
				SortedPlayers = ActivePlayers.OrderBy(p => p.PlayerName).ToList();
			}
			return SortedPlayers;
		}

		public string GetPlayerDetails(Player player)
		{
			return GetPlayerDetails(player.PlayerName, player.NFLTeam, player.Position);
		}

		public string GetPlayerDetails(string playerName, string nflTeam, string position)
		{
			if (nflTeam != null)
			{
				var nflTeamDisplay = NFLTeams.First(t => t.TeamAbbr == nflTeam.ToUpper()).AbbrDisplay;
				return string.Format("{0} ({1}-{2})", playerName, nflTeamDisplay, position);
			}
			else
			{
				return playerName;
			}
		}
	}
}