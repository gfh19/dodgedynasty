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
		public string sport { get; set; }
		public FproPlayer[] players { get; set; }
	}

	public class FproPlayer
	{
		public string player_name { get; set; }
		public string player_team_id { get; set; }
		public string player_position_id { get; set; }
		public int rank_ecr { get; set; }
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
					RankNum = player.rank_ecr,
					PlayerName = player.player_name,
					NFLTeam = player.player_team_id,
					Position = player.player_position_id
				};
			}
		}
	}
}
