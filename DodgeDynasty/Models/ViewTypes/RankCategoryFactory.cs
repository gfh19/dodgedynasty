﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.ViewTypes
{
	public enum RankCategory
	{
		Overall,
		QB,
		RB,
		WRTE,
		DEF,
		K
	}

	public static class RankCategoryFactory
	{
		public static Dictionary<RankCategory, RankCatFn> RankCatDict = new Dictionary<RankCategory, RankCatFn>
		{
			{ RankCategory.Overall, new RankCatFn(CreateCategoryOverall) },
			{ RankCategory.QB, new RankCatFn(CreateCategoryQB) },
			{ RankCategory.RB, new RankCatFn(CreateCategoryRB) },
			{ RankCategory.WRTE, new RankCatFn(CreateCategoryWRTE) },
			{ RankCategory.DEF, new RankCatFn(CreateCategoryDEF) },
			{ RankCategory.K, new RankCatFn(CreateCategoryK) }
		};

		public static RankCategoryModel CreateCategoryOverall(PlayerRankModel playerRankModel)
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
		public static RankCategoryModel CreateCategoryQB(PlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandQB";
			result.HideId = "HideQB";
			result.Header = "QB";
			result.ShowPos = false;
			result.ShowByeWeek = false;
			result.ExpandValue = playerRankModel.Options.ExpandQB.ToString().ToLower();
			result.PlayerList = playerRankModel.QBRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryRB(PlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandRB";
			result.HideId = "HideRB";
			result.Header = "RB";
			result.ShowPos = false;
			result.ShowByeWeek = false;
			result.ExpandValue = playerRankModel.Options.ExpandRB.ToString().ToLower();
			result.PlayerList = playerRankModel.RBRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryWRTE(PlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandWRTE";
			result.HideId = "HideWRTE";
			result.Header = "WR/TE";
			result.ShowPos = true;
			result.ShowByeWeek = false;
			result.ExpandValue = playerRankModel.Options.ExpandWRTE.ToString().ToLower();
			result.PlayerList = playerRankModel.WRTERankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryDEF(PlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandDEF";
			result.HideId = "HideDEF";
			result.Header = "DEF";
			result.ShowPos = false;
			result.ShowByeWeek = false;
			result.ExpandValue = playerRankModel.Options.ExpandDEF.ToString().ToLower();
			result.PlayerList = playerRankModel.DEFRankedPlayers;
			return result;
		}
		public static RankCategoryModel CreateCategoryK(PlayerRankModel playerRankModel)
		{
			RankCategoryModel result = new RankCategoryModel();
			result.DataLink = result.ExpandId = "ExpandK";
			result.HideId = "HideK";
			result.Header = "K";
			result.ShowPos = false;
			result.ShowByeWeek = false;
			result.ExpandValue = playerRankModel.Options.ExpandK.ToString().ToLower();
			result.PlayerList = playerRankModel.KRankedPlayers;
			return result;
		}
	}

	public delegate RankCategoryModel RankCatFn(PlayerRankModel playerRankModel);
}
