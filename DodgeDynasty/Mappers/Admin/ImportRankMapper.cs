using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.RankAdjustments;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Parsers;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Mappers.Admin
{
	public class ImportRankMapper : MapperBase<ImportRankModel>
	{
		public string RankId { get; set; }
		public bool Confirmed { get; set; }
		public int? MaxCount { get; set; }
		public short Year { get; set; }

		public ImportRankMapper(string rankId, bool confirmed, int? maxCount)
		{
			RankId = rankId;
			Confirmed = confirmed;
			MaxCount = maxCount;
		}

		protected override void PopulateModel()
		{
			//TODO:  Year Overridable?  Used in Add Player sp in search for other rankings
			Year = Convert.ToInt16(Utilities.GetEasternTime().Year);
			var rankId = Convert.ToInt32(RankId);
            var rank = HomeEntity.Ranks.FirstOrDefault(o => o.RankId == rankId);
            var autoImport = HomeEntity.AutoImports.FirstOrDefault(o => o.AutoImportId == rank.AutoImportId);
			List<RankedPlayer> rankedPlayers = null;
			IRankParser parser = null;
			try
			{
				parser = ParserFactory.Create(rank.AutoImportId);
				if (autoImport.IsApi)
				{
					string rankJson = GetRankData(autoImport);
					rankedPlayers = parser.ParseRankJson(rankJson, Confirmed, MaxCount);
				}
				else
				{
					HtmlNode rankDoc = LoadRankHtmlDoc(autoImport);
					if (parser.CheckPositions)
					{
						parser.Positions = HomeEntity.Positions.ToList();
					}
					rankedPlayers = parser.ParseRankHtml(rankDoc, Confirmed, MaxCount);
				}
			}
			catch (Exception ex)
			{
				Model.ErrorMessage = ex.Message;
				Model.StackTrace = ex.StackTrace;
				var maxStackTrace = 250;
                if (Model.StackTrace.Length > maxStackTrace)
				{
					Model.StackTrace = Model.StackTrace.Substring(0, maxStackTrace) + "...";
                }
                return;
			}

			if (Confirmed)
			{
				DeleteExistingPlayerRanks();
				foreach (var rankedPlayer in rankedPlayers)
				{
					AddPlayerRank(rankedPlayer);
				}
				SetRankLastUpdate(rank);
				HideReportingForRedundantEntries();
				UpdateSucceeded = true;
			}
			else
			{
				if (rankedPlayers.Count > 1)
				{
					var firstPlayer = rankedPlayers[0];
					Model.FirstPlayerText = string.Format("{0} ({1}-{2})", firstPlayer.PlayerName, firstPlayer.NFLTeam, 
						firstPlayer.Position);
					var lastPlayer = rankedPlayers[rankedPlayers.Count-1];
					Model.LastPlayerText = string.Format("{0} ({1}-{2})", lastPlayer.PlayerName, lastPlayer.NFLTeam,
						lastPlayer.Position);
					Model.PlayerCount = parser.PlayerCount;
					Model.MaxPlayerCount = parser.MaxPlayerCount;
				}
			}
		}

		private void HideReportingForRedundantEntries()
		{
			var redundantEntries = HomeEntity.PlayerAdjustments.GroupBy(o => new PlayerAdjType
			{
				OldPlayerId = o.OldPlayerId,
				NewPlayerId = o.NewPlayerId,
				TruePlayerId = o.TruePlayerId,
				NewFirstName = o.NewFirstName,
				NewLastName = o.NewLastName,
				NewPlayerName = o.NewPlayerName,
				NewPosition = o.NewPosition,
				NewNFLTeam = o.NewNFLTeam,
				Action = o.Action,
				UserId = o.UserId,
				LastUpdateYear = o.LastUpdateTimestamp.Year
			})
			.Where(grp => grp.Count() > 2 && grp.Key.LastUpdateYear == DateTime.Now.Year)
			.Select(grp=>grp.Key).ToList();

			foreach (var re in redundantEntries)
			{
				var entries = HomeEntity.PlayerAdjustments.Where(o =>
				(o.OldPlayerId == re.OldPlayerId || (o.OldPlayerId == null && re.OldPlayerId == null)) &&
				(o.NewPlayerId == re.NewPlayerId || (o.NewPlayerId == null && re.NewPlayerId == null)) &&
				(o.TruePlayerId == re.TruePlayerId || (o.TruePlayerId == null && re.TruePlayerId == null)) &&
				o.NewFirstName == re.NewFirstName &&
				o.NewLastName == re.NewLastName &&
				o.NewPlayerName == re.NewPlayerName &&
				o.NewPosition == re.NewPosition &&
				o.NewNFLTeam == re.NewNFLTeam &&
				o.Action == re.Action &&
				(o.UserId == re.UserId || (o.UserId == null && re.UserId == null)) &&
				o.LastUpdateTimestamp.Year >= re.LastUpdateYear).OrderBy(o=>o.LastUpdateTimestamp);
				var entryCount = 0;
				foreach (var e in entries)
				{
					if (entryCount++ >= 2)
					{
						e.HideReporting = true;
					}
				}
			}
			HomeEntity.SaveChanges();
		}

		private HtmlNode LoadRankHtmlDoc(AutoImport autoImport)
		{
			string source = GetRankData(autoImport);
			source = WebUtility.HtmlDecode(source);
			HtmlDocument rankHtml = new HtmlDocument();
			rankHtml.LoadHtml(source);
			return rankHtml.DocumentNode;
		}

		private static string GetRankData(AutoImport autoImport)
		{
			HttpClient http = new HttpClient();
			var task = http.GetByteArrayAsync(autoImport.ImportUrl);
			task.Wait();
			var response = task.Result;
			return Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
		}

		private void AddPlayerRank(RankedPlayer rankedPlayer)
		{
			string firstName = "";
			string lastName = "";
			var playerName = rankedPlayer.PlayerName;
			var players = HomeEntity.Players.ToList();
			var rankedPlayerFormatted = Utilities.FormatNamePunctuation(rankedPlayer.PlayerName);
			var playerNameMatch = players.FirstOrDefault(o => 
				Utilities.FormatNamePunctuation(o.PlayerName).Equals(rankedPlayerFormatted, StringComparison.InvariantCultureIgnoreCase));

			if (playerNameMatch != null)
			{
				firstName = playerNameMatch.FirstName;
				lastName = playerNameMatch.LastName;
			}
			else
			{
				var rankedPlayerNoSuffix = Utilities.TrimSuffix(rankedPlayer.PlayerName);
                var playerNameSuffixMatch = players.FirstOrDefault(o =>
					Utilities.TrimSuffix(o.PlayerName).Equals(rankedPlayerNoSuffix, StringComparison.InvariantCultureIgnoreCase) &&
						o.Position == rankedPlayer.Position && o.NFLTeam == rankedPlayer.NFLTeam);

				if (playerNameSuffixMatch != null && playerNameSuffixMatch.IsActive)
				{
					firstName = playerNameSuffixMatch.FirstName;
					lastName = playerNameSuffixMatch.LastName;
				}
				else
				{
					var firstSpace = playerName.Trim().IndexOf(" ");
					if (firstSpace < 0)
					{
						Model.ErrorMessage = "Error - No space found in: " + playerName;
						return;
					}
					firstName = playerName.Substring(0, firstSpace);
					lastName = playerName.Substring(firstSpace + 1, playerName.Length - firstSpace - 1);
				}
			}
			AddPlayerRank(rankedPlayer, firstName, lastName);
		}

		private void AddPlayerRank(RankedPlayer rankedPlayer, string firstName, string lastName)
		{
			var now = Utilities.GetEasternTime();
			HomeEntity.usp_LoadPlayerRanks_V2(firstName.Replace("*", ""), lastName.Replace("*", ""), rankedPlayer.Position, 
				rankedPlayer.NFLTeam, null, Utilities.ToNullInt(RankId), rankedPlayer.RankNum, null, null, Year, now);
		}

		private void DeleteExistingPlayerRanks()
		{
			var rankId = Utilities.ToNullInt(RankId);
            var oldPlayerRanks = HomeEntity.PlayerRanks.Where(o => o.RankId == rankId);
			foreach (var playerRank in oldPlayerRanks)
			{
				HomeEntity.PlayerRanks.DeleteObject(playerRank);
			}
			HomeEntity.SaveChanges();
        }

		private void SetRankLastUpdate(Rank rank)
		{
			rank.RankDate = Utilities.GetEasternTime();
			rank.LastUpdateTimestamp = Utilities.GetEasternTime();
			HomeEntity.SaveChanges();
		}
	}
}
