using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public abstract class FprosParser : RankParser
	{
		public override string RankTableSelect() { return "//div[contains(@class, 'mobile-table')]//table"; }
		public override string RankRowSelect() { return "//div[contains(@class, 'mobile-table')]//table//tr[contains(@class, 'mpb-player')]"; }
		public override string RankColSelect() { return "./td"; }

		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return columns[0].InnerText;
		}

		public override string GetPlayerName(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[1];
			var anch = "./a";
			var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
			return player;
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[1];
			var small = "./small";
			var nflTeam = "";
			var nflTeamNodes = playerTeamNode.SelectNodes(small);
			if (nflTeamNodes != null && nflTeamNodes.Count > 0)
			{
				nflTeam = nflTeamNodes[0].InnerText;
			}

			return nflTeam;
		}

		public override string GetPlayerPos(List<HtmlNode> columns)
		{
			var posAndRank = columns[2].InnerText;
			return Regex.Replace(posAndRank, @"[\d-]", string.Empty);
		}
	}
}
