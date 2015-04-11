﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Shared
{
	public static class Constants
	{
		//TODO:  SQL-ize
		public static readonly int? DraftId = null;
		public const int NewPlayerId = -1;

		public static class AppSettings
		{
			public const string DefaultNumOwners = "DefaultNumOwners";
		}

		public static class Cookies
		{
			public const string PlayerRankOptions = "playerRankOptions";
		}

		public class DraftFormats
		{
			public const string Repeat = "repeat";
			public const string Snake = "snake";
		}

		public static class Roles
		{
			public const int Admin = 1;
		}

		public static class Views
		{
			public const string Display = "Display";
			public const string Input = "Input";
			public const string Pick = "Pick";
			public const string TeamDisplay = "TeamDisplay";
			public const string BestAvailable = "BestAvailable";
			public const string PlayerRanks = "PlayerRanks";
			public const string SetupRank = "SetupRank";
			public const string RankingsList = "RankingsList";
			public const string CurrentDraftPickPartial = "CurrentDraftPickPartial";
		}
	}
}