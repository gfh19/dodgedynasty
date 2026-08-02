using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class FproRank
	{
		public FproPlayer[] players { get; set; }
	}

	public class FproPlayer
	{
		public string name { get; set; }
		public string team { get; set; }
		public string position { get; set; }
		public int rank { get; set; }
	}

	public class YahooParser : RankParser
	{
		public override List<RankedPlayer> ConvertJsonRankRows(string rankJson)
        {
			var innerRankJson = (rankJson[0] == '{') ? rankJson : rankJson.Substring(15, rankJson.Length - 16);
            var fproRank = Newtonsoft.Json.JsonConvert.DeserializeObject<FproRank>(innerRankJson);
			return GetRankedPlayers(fproRank.players).ToList();
		}

		private IEnumerable<RankedPlayer> GetRankedPlayers(FproPlayer[] players)
		{
			foreach (var player in players)
			{
				yield return new RankedPlayer
				{
					RankNum = player.rank,
					PlayerName = player.name?.Trim(),
					NFLTeam = player.team,
					Position = player.position
				};
			}
		}
	}
}
