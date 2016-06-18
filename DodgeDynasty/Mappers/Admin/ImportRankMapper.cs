using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using DodgeDynasty.Models.Admin;
using DodgeDynasty.Models.RankAdjustments;
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
			HtmlNode rankDoc = null;
			try
			{
				rankDoc = LoadRankHtmlDoc();
			}
			catch (Exception ex)
			{
				Model.ErrorMessage = ex.Message;
			}

			var parser = ParserFactory.Create();
			var rankedPlayers = parser.ParseRankHtml(rankDoc);
			if (Confirmed)
			{
				foreach (var rankedPlayer in rankedPlayers)
				{
					AddPlayerRank(rankedPlayer);
				}
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

		private HtmlNode LoadRankHtmlDoc()
		{
			HttpClient http = new HttpClient();
			//var response = await http.GetByteArrayAsync("");
			var task = http.GetByteArrayAsync("https://www.fantasypros.com/nfl/rankings/consensus-cheatsheets.php");
			task.Wait();
			var response = task.Result;
			String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
			source = WebUtility.HtmlDecode(source);
			HtmlDocument rankHtml = new HtmlDocument();
			rankHtml.LoadHtml(source);
			var rankDoc = rankHtml.DocumentNode;
			return rankDoc;
		}

		private void AddPlayerRank(Models.Types.RankedPlayer rankedPlayer)
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
				firstName = playerName.Substring(0, firstSpace);
				lastName = playerName.Substring(firstSpace+1, playerName.Length-firstSpace);
			}
			AddPlayerRank(rankedPlayer, firstName, lastName);
		}

		private void AddPlayerRank(Models.Types.RankedPlayer rankedPlayer, string firstName, string lastName)
		{
			HomeEntity.usp_LoadPlayerRanks_V2(firstName, lastName, rankedPlayer.Position, rankedPlayer.NFLTeam, null,
				Utilities.ToNullInt(RankId), rankedPlayer.RankNum, null, null, Year);
		}
	}
}
