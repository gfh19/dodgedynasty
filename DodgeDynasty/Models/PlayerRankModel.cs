using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using System.Configuration;
using System.Web.Mvc;
using DodgeDynasty.Shared;
using DodgeDynasty.Models.ViewTypes;
using DodgeDynasty.Mappers.Highlights;
using DodgeDynasty.Models.Highlights;
using DodgeDynasty.Models.Drafts;
using DodgeDynasty.Mappers.Ranks;
using DodgeDynasty.Mappers;

namespace DodgeDynasty.Models
{
	public class PlayerRankModel : RankIdModel, IPlayerRankModel
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
		public RankCategoryModel CurrentRankCategory { get; set; }
		public List<RankedPlayer> HighlightedPlayers { get; set; }

		/* Compare Ranks */
		public List<PlayerRankingsModel> CompareRankModels { get; set; }
		public IPlayerRankModel CompareRank { get; set; }
		public IPlayerRankModel AveragePlayerRank { get; set; }
		public string CategoryRankHeader { get; set; }
		public string CompRankPosition { get; set; }

		public PlayerRankModel(int rankId, int? draftId = null)
			: this(draftId)
		{}

		public PlayerRankModel(int? draftId = null)
		{
			base.GetCurrentDraft(draftId);
			int window;
			RanksWindow = int.TryParse(ConfigurationManager.AppSettings["RanksWindow"], out window) ? window : 12;
		}

		public void SetPlayerRanks(int rankId, IPlayerRankModel playerRankModel = null)
		{
			playerRankModel = playerRankModel ?? this;
			playerRankModel.RankId = rankId;
			using (HomeEntity = new HomeEntity())
			{
				playerRankModel.CurrentRank = HomeEntity.Ranks.First(r => r.RankId == rankId);
				playerRankModel.PlayerRanks = HomeEntity.PlayerRanks.Where(pr => pr.RankId == rankId).ToList();
            }
		}

		public void GetBestAvailPlayerRanks()
		{
			GetRankedPlayersAll();

			OverallRankedPlayers = RankedPlayers.Where(rp => !DraftedPlayers.Any(dp => rp.TruePlayerId == dp.TruePlayerId)).ToList();
			QBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "QB").ToList();
			RBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "RB").ToList();
			WRTERankedPlayers = OverallRankedPlayers.Where(p => p.Position == "WR" || p.Position == "TE").ToList();
			DEFRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "DEF").ToList();
			KRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "K").ToList();
			HighlightedPlayers = GetBestAvailHighlightedPlayers();
		}

		public void GetAllPlayerRanksByPosition()
		{
			GetRankedPlayersAllWithDraftPickInfo();

			OverallRankedPlayers = RankedPlayers.ToList();
			QBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "QB").ToList();
			RBRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "RB").ToList();
			WRTERankedPlayers = OverallRankedPlayers.Where(p => p.Position == "WR" || p.Position == "TE").ToList();
			DEFRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "DEF").ToList();
			KRankedPlayers = OverallRankedPlayers.Where(p => p.Position == "K").ToList();
			HighlightedPlayers = GetAllHighlightedPlayers();
        }

		//TODO:  Move/use migrated PlayerRankModelHelper version
		public List<RankedPlayer> GetRankedPlayersAll()
		{
			RankedPlayers = (from pr in PlayerRanks
							 join p in AllPlayers on pr.PlayerId equals p.PlayerId
							 join t in NFLTeams on p.NFLTeam equals t.TeamAbbr
							 join ph in CurrentPlayerHighlights on pr.PlayerId equals ph.PlayerId into phLeft
							 from ph in phLeft.DefaultIfEmpty()
							 select PlayerRankModelHelper.GetRankedPlayer(pr, p, t, ph)).OrderBy(p => p.RankNum).ToList();
			return RankedPlayers;
		}

		//TODO:  Move/use migrated PlayerRankModelHelper version
		public List<RankedPlayer> GetRankedPlayersAllWithDraftPickInfo()
		{
			RankedPlayers = (from pr in PlayerRanks
							 join p in AllPlayers on pr.PlayerId equals p.PlayerId
							 join t in NFLTeams on p.NFLTeam equals t.TeamAbbr
							 join pick in DraftPicks on pr.PlayerId equals pick.PlayerId into dpLeft    //Left Outer Join
							 from pick in dpLeft.DefaultIfEmpty()
							 join u in Users on ((pick != null) ? pick.UserId : -1) equals u.UserId into uLeft  //Left Outer Join
							 from u in uLeft.DefaultIfEmpty()
							 join lo in CurrentLeagueOwners on ((pick != null) ? pick.UserId : -1) equals lo.UserId into loLeft
							 from lo in loLeft.DefaultIfEmpty()
							 join ph in CurrentPlayerHighlights on pr.PlayerId equals ph.PlayerId into phLeft
							 from ph in phLeft.DefaultIfEmpty()
							 select PlayerRankModelHelper.GetRankedPlayer(pr, p, t, ph, pick, u, lo)).OrderBy(p => p.RankNum).ToList();

			return GetDraftedTruePlayersFor(RankedPlayers);
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

		public List<RankedPlayer> GetAllHighlightedPlayers()
		{
			List<PlayerRank> playerRanks = PlayerRanks;
			if (Options.ShowAvgCompRanks && AveragePlayerRank != null)
			{
				playerRanks = AveragePlayerRank.RankedPlayers.Select(rp => new PlayerRank
				{
					RankId = -1,
					PlayerId = rp.PlayerId,
					RankNum = rp.RankNum
				}).ToList();
            }

			var highlightedPlayers = (from ph in CurrentPlayerHighlights
									  join pr in playerRanks on ph.PlayerId equals pr.PlayerId into prLeft      //Left Outer Join
									  from pr in prLeft.DefaultIfEmpty()
									  join pick in DraftPicks on ph.PlayerId equals pick.PlayerId into dpLeft    //Left Outer Join
									  from pick in dpLeft.DefaultIfEmpty()
									  join p in AllPlayers on ph.PlayerId equals p.PlayerId
									  join t in NFLTeams on p.NFLTeam equals t.TeamAbbr
									  join u in Users on ((pick != null) ? pick.UserId : -1) equals u.UserId into uLeft  //Left Outer Join
									  from u in uLeft.DefaultIfEmpty()
									  join lo in CurrentLeagueOwners on ((pick != null) ? pick.UserId : -1) equals lo.UserId into loLeft
									  from lo in loLeft.DefaultIfEmpty()
									  select PlayerRankModelHelper.GetRankedPlayer(pr, p, t, ph, pick, u, lo)).OrderBy(p => Convert.ToInt32(p.HighlightRankNum)).ToList();

            return GetDraftedTruePlayersFor(highlightedPlayers);
		}

		public List<RankedPlayer> GetBestAvailHighlightedPlayers()
		{
			return GetAllHighlightedPlayers().Where(o => o.PickNum == null).ToList();
		}

		//TODO:  Move/use migrated PlayerRankModelHelper version
		public List<RankedPlayer> GetDraftedTruePlayersFor(List<RankedPlayer> players)
		{
			foreach (var player in players.Where(o => o.PickNum == null))
			{
				var truePlayer = AllPlayers.FirstOrDefault(o => o.TruePlayerId == player.TruePlayerId &&
					o.PlayerId != player.PlayerId && DraftPicks.Any(dp => dp.PlayerId == o.PlayerId));
				if (truePlayer != null)
				{
					var draftPick = DraftPicks.First(dp => dp.PlayerId == truePlayer.PlayerId);
					player.PickNum = draftPick.PickNum.ToString();
					player.UserId = draftPick.UserId.ToString();
					player.NickName = Users.FirstOrDefault(lo => lo.UserId == draftPick.UserId).NickName;
					var leagueOwner = CurrentLeagueOwners.FirstOrDefault(lo => lo.UserId == draftPick.UserId);
					player.CssClass = (leagueOwner != null) ? leagueOwner.CssClass : null;
				}
			}
			return players;
		}

		public PlayerRankModel SetCategory(RankCategory category)
		{
			CurrentRankCategory = RankCategoryFactory.RankCatDict[category](this);
			return this;
		}
		
		public IPlayerRankModel SetCompareRanksCategory(IPlayerRankModel compareRankModel)
		{
			CurrentRankCategory = RankCategoryFactory.RankCatDict[RankCategory.CompRank](compareRankModel);
			CompareRank = compareRankModel;
			CompareRank.CategoryRankHeader = string.Format("{0} ({1})", compareRankModel.CurrentRank.RankName, 
				compareRankModel.CurrentRank.RankDate.ToString("M/d"));
			AveragePlayerRank = null;
            return this;
		}

		public IPlayerRankModel SetAverageRankCategory(IPlayerRankModel rankModel)
		{
			CurrentRankCategory = RankCategoryFactory.RankCatDict[RankCategory.Avg](rankModel);
			AveragePlayerRank = rankModel;
			CompareRank = null;
			return this;
		}

		public List<SelectListItem> GetHighlightColors()
		{
			var mapper = new HighlightsMapper();
			var model = mapper.GetModel();
			var highlightColors = model.Highlights;
			var maxHighlightId = model.Highlights.Select(o => o.HighlightId).DefaultIfEmpty().Max();
			var maxHighlightOrder = model.Highlights.Select(o => o.HighlightOrder).DefaultIfEmpty().Max();
			highlightColors.Add(
				new HighlightModel { HighlightId = (maxHighlightId+100),
					HighlightOrder = (short)(maxHighlightOrder.GetValueOrDefault() + 100),
					HighlightName="<Remove>", HighlightClass = "<remove>", HighlightValue = "<remove>" });
            return Utilities.GetListItems<HighlightModel>(highlightColors.OrderBy(o => o.HighlightOrder).ToList(),
				o => o.HighlightName, o => o.HighlightClass.ToString(), false, GetSelectedHighlightColor());
		}

		public string GetSelectedHighlightColor()
		{
			return !string.IsNullOrEmpty(Options.HighlightColor) ? Options.HighlightColor : null;
        }

		public SingleDraftModel GetLastHighlightsDraft()
		{
			CopyLastDraftHighlightsMapper mapper = new CopyLastDraftHighlightsMapper();
			return mapper.GetModel();
        }

		public List<SelectListItem> GetCurrentAvailableDraftRankItems(string rankId)
		{
			return Utilities.GetListItems(GetCurrentAvailableDraftRanks(), r => r.RankName, r => r.RankId.ToString(), false, rankId);
        }

		public List<SelectListItem> GetDraftPublicRankItems(string rankId)
		{
			return Utilities.GetListItems(PlayerRankModelHelper.GetDraftPublicRankings(DraftId.Value, GetCurrentAvailableDraftRanks()), 
				r => r.RankName, r => r.RankId.ToString(), false, rankId);
		}

		public IPlayerRankModel SetUnrankedCompareList()
		{
			var currentDraftRanks = GetCurrentAvailableDraftRanks();
			var publicRanks = PlayerRankModelHelper.GetDraftPublicRankings(DraftId.Value, currentDraftRanks);
			DraftRankModel selectedCompareRank = null;
			if (Options.BUPId != null && publicRanks.Any(o => o.RankId == Options.BUPId.ToNullInt()))
			{
				selectedCompareRank = publicRanks.First(o => o.RankId == Options.BUPId.ToNullInt());
			}
			else
			{
				selectedCompareRank = publicRanks.FirstOrDefault();
				Options.BUPId = selectedCompareRank.RankId.ToString();
				PlayerRankOptionsMapper mapper = MapperFactory.CreatePlayerRankOptionsMapper(Options.Id);
				mapper.UpdateEntity(Options);
			}
			if (selectedCompareRank != null)
			{
				var compareRank = PlayerRankModelHelper.CreatePlayerRankingsModel(this);
				compareRank.CategoryRankHeader = selectedCompareRank.RankName;
				SetPlayerRanks(selectedCompareRank.RankId, compareRank);
				compareRank.RankedPlayers = PlayerRankModelHelper.GetRankedPlayersAllWithDraftPickInfo(compareRank.PlayerRanks, this);
				CompareRank = compareRank;
			}
			return CompareRank;
		}

		public List<SelectListItem> GetPositions(string pos)
		{
			//TODO:  Read positions from League value (create SQL column, add to lg Add/Edit, and modify rank category logic)
			List<KeyValuePair<string, string>> positions = new List<KeyValuePair<string, string>>();
			positions.Add(new KeyValuePair<string, string>("OVERALL", "Overall"));
			positions.Add(new KeyValuePair<string, string>("QB", "QB"));
			positions.Add(new KeyValuePair<string, string>("RB", "RB"));
			positions.Add(new KeyValuePair<string, string>("WR/TE", "WR/TE"));
			positions.Add(new KeyValuePair<string, string>("DEF", "DEF"));
			positions.Add(new KeyValuePair<string, string>("K", "K"));

			return Utilities.GetListItems(positions, r => r.Value, r => r.Key, false, pos);
		}
	}
}