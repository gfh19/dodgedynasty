using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public abstract class FprosParser : RankParser
	{
		public override string RankTableSelect => "//table";
		public override string RankRowSelect => $"{RankTableSelect}//tr[contains(@class, 'player-row')]";
		public override string RankColSelect => "./td";

		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return columns[0].InnerText;
		}

		public override string GetPlayerName(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[2];
			var player = playerTeamNode?.InnerText;
			if (PlayerNameContainsTeam(player))
			{
				player = player.Split('(')[0];
			}
			return player?.Trim();
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[2];
			var nflTeam = "";
			var player = playerTeamNode.InnerText;
			if (PlayerNameContainsTeam(player))
			{
				var teamEnd = player.LastIndexOf(")");
				if (teamEnd > 0 && player.Length > player.Substring(0, teamEnd).Length)
				{
					player = player.Substring(0, teamEnd);
				}
				nflTeam = player.Split('(')[1].Replace(")", "");
			}

			return nflTeam;
		}

		public override string GetPlayerPos(List<HtmlNode> columns)
		{
			var posAndRank = columns[3].InnerText;
			return Regex.Replace(posAndRank, @"[\d-]", string.Empty);
		}

		private bool PlayerNameContainsTeam(string player)
		{
			return !string.IsNullOrEmpty(player) && player.Length > 1 && player.Contains("(");
        }
	}
}
