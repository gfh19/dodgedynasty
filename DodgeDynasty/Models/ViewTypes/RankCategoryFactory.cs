﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Models.ViewTypes
{
	public enum RankCategory
	{
		Overall,
		QB,
		RB,
		WRTE,
		WR,
		TE,
		DEF,
		K,

		CompRank,
		Avg
	}

	public static class RankCategoryFactory
	{
		public static Dictionary<RankCategory, RankCatFn> RankCatDict = new Dictionary<RankCategory, RankCatFn>
		{
			{ RankCategory.Overall, new RankCatFn(CreateCategoryOverall) },
			{ RankCategory.QB, new RankCatFn(CreateCategoryQB) },
			{ RankCategory.RB, new RankCatFn(CreateCategoryRB) },
			{ RankCategory.WRTE, new RankCatFn(CreateCategoryWRTE) },
			{ RankCategory.WR, new RankCatFn(CreateCategoryWR) },
			{ RankCategory.TE, new RankCatFn(CreateCategoryTE) },
			{ RankCategory.DEF, new RankCatFn(CreateCategoryDEF) },
			{ RankCategory.K, new RankCatFn(CreateCategoryK) },
			{ RankCategory.CompRank, new RankCatFn(CreateCategoryCompRank) },
			{ RankCategory.Avg, new RankCatFn(CreateCategoryAvg) }
		};

		public static RankCategoryModel CreateCategoryOverall(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandOverall";
			result.HideId = "HideOverall";
			result.Header = "OVERALL";
			result.ShowPos = true;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandOverall.ToString().ToLower();
			result.PlayerList = playerRankModel.OverallRankedPlayers;
            return result;
		}
		public static RankCategoryModel CreateCategoryQB(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandQB";
			result.HideId = "HideQB";
			result.Header = "QB";
			result.ShowPos = false;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandQB.ToString().ToLower();
			result.PlayerList = playerRankModel.QBRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryRB(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandRB";
			result.HideId = "HideRB";
			result.Header = "RB";
			result.ShowPos = false;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandRB.ToString().ToLower();
			result.PlayerList = playerRankModel.RBRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryWRTE(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandWRTE";
			result.HideId = "HideWRTE";
			result.Header = "WR/TE";
			result.ShowPos = true;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandWRTE.ToString().ToLower();
			result.PlayerList = playerRankModel.WRTERankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryWR(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandWR";
			result.HideId = "HideWR";
			result.Header = "WR";
			result.ShowPos = false;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandWR.ToString().ToLower();
			result.PlayerList = playerRankModel.WRRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryTE(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandTE";
			result.HideId = "HideTE";
			result.Header = "TE";
			result.ShowPos = false;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandTE.ToString().ToLower();
			result.PlayerList = playerRankModel.TERankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryDEF(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandDEF";
			result.HideId = "HideDEF";
			result.Header = "DEF";
			result.ShowPos = false;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandDEF.ToString().ToLower();
			result.PlayerList = playerRankModel.DEFRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryK(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandK";
			result.HideId = "HideK";
			result.Header = "K";
			result.ShowPos = false;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandK.ToString().ToLower();
			result.PlayerList = playerRankModel.KRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryCompRank(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandCR-" + playerRankModel.RankId;
			result.ShowPos = true;
			result.ShowByeWeek = true;
			result.ExpandValue = GetCRExpandValue(playerRankModel.Options, playerRankModel.RankId);
			CheckCompRankPosition(playerRankModel, result);
			return result;
		}

		public static RankCategoryModel CreateCategoryAvg(IPlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandAvg";
			result.HideId = "HideAvg";
			result.ShowPos = true;
			result.ShowByeWeek = true;
			result.ExpandValue = playerRankModel.Options.ExpandAvg.ToString().ToLower();
			CheckCompRankPosition(playerRankModel, result);
			return result;
		}

		private static void CheckCompRankPosition(IPlayerRankModel playerRankModel, RankCategoryModel result)
		{
			if (string.IsNullOrEmpty(playerRankModel.CompRankPosition) || playerRankModel.CompRankPosition == Constants.Positions.Overall)
			{
				result.Header = "OVERALL";
				result.PlayerList = playerRankModel.OverallRankedPlayers;
			}
			else
			{
				result.Header = string.Format("***{0} ONLY***", playerRankModel.CompRankPosition);
				var positions = playerRankModel.CompRankPosition.Split('/');
				result.PlayerList = playerRankModel.OverallRankedPlayers.Where(o => positions.Contains(o.Position)).ToList();
			}
		}

		//Helpers
		public static string GetCRExpandValue(PlayerRankOptions options, int rankId)
		{
			if (!string.IsNullOrEmpty(options.CompRankExpandIds))
			{
				var compRankExpIds = options.CompRankExpandIds.Split(',');
				return compRankExpIds.Contains(rankId.ToString()).ToString().ToLower();
			}
			return "false";
		}
	}

	public delegate RankCategoryModel RankCatFn(IPlayerRankModel playerRankModel);
}
