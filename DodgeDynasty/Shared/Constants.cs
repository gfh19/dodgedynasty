﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Shared
{
	public static class Constants
	{
		//TODO:  SQL-ize
		public const int NewPlayerId = -1;

		public static class AppSettings
		{
			public const string DefaultNumOwners = "DefaultNumOwners";
			public const string RefreshTimer = "RefreshTimer";
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

		public static class Messages
		{
			public const string StringLength = "The {0} must be between {2} and {1} characters long.";
		}

		public static class Roles
		{
			public const int Admin = 1;
		}

		public static class QS
		{
			public const string IsActive = "isActive";
			public const string IsComplete = "isComplete";
			public const string HistoryMode = "historyMode";
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
			public const string AddLeague = "AddLeague";
			public const string AddDraft = "AddDraft";
			public const string SetupDraft = "SetupDraft";
			public const string ActivateDraft = "ActivateDraft";
			public const string ColorStyles = "_ColorStyles";
			public const string UserInfoPartial = "UserInfoPartial";
			public const string ManageUsers = "ManageUsers";
		}
	}
}