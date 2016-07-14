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
		public short Year { get; set; }

		public ImportRankMapper(string rankId, bool confirmed)
		{
			RankId = rankId;
			Confirmed = confirmed;
		}

		protected override void PopulateModel()
		{
			Year = Convert.ToInt16(Utilities.GetEasternTime().Year);
			var rankId = Convert.ToInt32(RankId);
            var rank = HomeEntity.Ranks.FirstOrDefault(o => o.RankId == rankId);
            var autoImport = HomeEntity.AutoImports.FirstOrDefault(o => o.AutoImportId == rank.AutoImportId);
			HtmlNode rankDoc = null;
			List<RankedPlayer> rankedPlayers = null;
			try
			{
				rankDoc = LoadRankHtmlDoc(autoImport);
				var parser = ParserFactory.Create(rank.AutoImportId);
                if (parser.CheckPositions)
				{
					parser.Positions = HomeEntity.Positions.ToList();
				}
				rankedPlayers = parser.ParseRankHtml(rankDoc);
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
					Model.PlayerCount = rankedPlayers.Count;
				}
			}
		}

		private HtmlNode LoadRankHtmlDoc(AutoImport autoImport)
		{
			HttpClient http = new HttpClient();
			var task = http.GetByteArrayAsync(autoImport.ImportUrl);
			task.Wait();
			var response = task.Result;
			var source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
			source = WebUtility.HtmlDecode(source);
			HtmlDocument rankHtml = new HtmlDocument();
			rankHtml.LoadHtml(source);
			var rankDoc = rankHtml.DocumentNode;
			return rankDoc;
		}

		private void AddPlayerRank(RankedPlayer rankedPlayer)
		{
			string firstName = "";
			string lastName = "";
			var playerName = rankedPlayer.PlayerName;
			var players = HomeEntity.Players.ToList();
			var playerNameMatch = players.FirstOrDefault(o => 
				Utilities.FormatNamePunctuation(o.PlayerName).Equals(
					Utilities.FormatNamePunctuation(rankedPlayer.PlayerName), 
					StringComparison.InvariantCultureIgnoreCase));

			if (playerNameMatch != null)
			{
				firstName = playerNameMatch.FirstName;
				lastName = playerNameMatch.LastName;
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
				lastName = playerName.Substring(firstSpace+1, playerName.Length-firstSpace-1);
			}
			AddPlayerRank(rankedPlayer, firstName, lastName);
		}

		private void AddPlayerRank(RankedPlayer rankedPlayer, string firstName, string lastName)
		{
			var now = Utilities.GetEasternTime();
			HomeEntity.usp_LoadPlayerRanks_V2(firstName, lastName, rankedPlayer.Position, rankedPlayer.NFLTeam, null,
				Utilities.ToNullInt(RankId), rankedPlayer.RankNum, null, null, Year, now);
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
