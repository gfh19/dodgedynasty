using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Shared
{
	public static class Constants
	{
		public const int NewPlayerId = -1;
		public const string ChatDateTimeFormat = "M/d h:mmtt";
		public const string ChatTimeFormat = "h:mmtt";

		public static class AppSettings
		{
			public const string DefaultNumOwners = "DefaultNumOwners";
			public const string RefreshTimer = "RefreshTimer";
			public const string FastRefreshTimer = "FastRefreshTimer";
			public const string MaxCompareRanks = "MaxCompareRanks";
			public const string MaxDailyAudioCalls = "MaxDailyAudioCalls";
			public const string MaxTabConnections = "MaxTabConnections";
			public const string WebSocketsKillSwitch = "WebSocketsKillSwitch";
			public const string DraftChatKillSwitch = "DraftChatKillSwitch";
			public const string AudioKillSwitch = "AudioKillSwitch";
			public const string TextToVoiceKillSwitch = "TextToVoiceKillSwitch";
			public const string UserEditAudioKillSwitch = "UserEditAudioKillSwitch";
			public const string MajorVersion = "MajorVersion";
			public const string JSVersion = "JSVersion";
		}

		public static class Audio
		{
			public const string Demo = "demo";
			public const string None = "<none>";
			public const string All = "all";
			public const string Prev = "prev";

			public const string NoneText = "<none>";
			public const string AllText = "All Picks";
			public const string PrevText = "Before Their Turn";
		}
		
		public static class Cookies
		{
			public const string DodgeDynasty = "dodgeDynastyCk";
			public const string PlayerRankOptions = "playerRankOptions";
		}

		public static class CssClass
		{
			public const string None = "_none";
			public const string NoneText = "None (Black)";
		}

		public class Defaults
		{
			public const int PickTimeSeconds = 120;
		}

		public class DraftFormats
		{
			public const string Repeat = "repeat";
			public const string Snake = "snake";
		}

		public class JS
		{
			public const string RemoveColor = "<remove>";
		}

		public static class Messages
		{
			public const string StringLength = "The {0} must be between {2} and {1} characters long.";
		}

		public static class Modes
		{
			public const string Admin = "Admin";
			public const string Commish = "Commish";
		}
		
		public static class QS
		{
			public const string IsActive = "isActive";
			public const string IsComplete = "isComplete";
			public const string HistoryMode = "historyMode";
			public const string ByPositions = "byPositions";
			public const string IsComparingRanks = "compare";
			public const string Position = "position";
		}

		public static class Positions
		{
			public const string Overall = "OVERALL";
			public const string QB = "QB";
			public const string RB = "RB";
			public const string WRTE = "WR/TE";
			public const string DEF = "DEF";
            public const string K = "K";
		}

		public static class Roles
		{
			public const int Admin = 1;
			public const int Commish = 2;
		}

		public static class Session
		{
			public const string HubConnections = "HubConnections";
		}

		public static class TempData
		{
			public const string NextDraftInputModel = "NextDraftInputModel";
			public const string RankStatus = "RankStatus";
		}

		public static class Test
		{
			public const int ExampleLeagueId = 7;
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
			public const string ManageLeaguesPartial = "../AdminShared/_ManageLeaguesPartial";
			public const string ManageDraftsPartial = "../AdminShared/_ManageDraftsPartial";
			public const string AddEditLeaguePartial = "../AdminShared/_AddEditLeaguePartial";
			public const string AddEditDraftPartial = "../AdminShared/_AddEditDraftPartial";
			public const string SetupDraftPartial = "../AdminShared/_SetupDraftPartial";
			public const string ActivateDraftPartial = "../AdminShared/_ActivateDraftPartial";
			public const string InputPartial = "../AdminShared/_InputPartial";
			public const string Messages = "Messages";
			public const string DraftChatPartial = "_DraftChatPartial";
			public const string HighlightQueuePartial = "HighlightQueuePartial";
			public const string HighlightQueueInnerPartial = "_HighlightQueueInnerPartial";
			public const string TestAudioPartial = "_TestAudioPartial";
			public const string BupSectionPartial = "_BupSectionPartial";
		}
	}
}