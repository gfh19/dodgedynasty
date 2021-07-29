using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class FprosAdpParser : RankParser
	{
		public override string RankTableSelect => "//div[contains(@class, 'mobile-table')]//table";
		public override string RankRowSelect => ".//tbody//tr";
		public override string RankColSelect => "./td";

		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return columns[0].InnerText;
		}

		public override string GetPlayerName(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[1];
			var anch = "./a";
			var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
			player = player.TrimString(" Defense");
			player = player.TrimString(" DST");
			return player?.Trim();
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
			if (!string.IsNullOrEmpty(nflTeam) && nflTeam.IndexOf(',') > -1)
			{
				nflTeam = nflTeam.Substring(0, nflTeam.IndexOf(','));
			}
			if (!string.IsNullOrEmpty(nflTeam) && nflTeam.IndexOf('(') > -1)
			{
				nflTeam = nflTeam.Substring(0, nflTeam.IndexOf('(')).Trim();
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
