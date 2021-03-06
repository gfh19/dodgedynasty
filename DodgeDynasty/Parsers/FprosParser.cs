﻿using System.Collections.Generic;
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
			var playerTeamNode = columns[2];
			var anch = "./a//span[contains(@class, 'full-name')]";
			var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
			if (PlayerNameContainsTeam(player))
			{
				player = player.Split('(')[0];
			}
			return player;
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[2];
			var small = "./small";
			var nflTeam = "";
			var nflTeamNodes = playerTeamNode.SelectNodes(small);
			if (nflTeamNodes != null && nflTeamNodes.Count > 0)
			{
				nflTeam = nflTeamNodes[0].InnerText;
			}
			if (string.IsNullOrEmpty(nflTeam))
			{
				var anch = "./a//span[contains(@class, 'full-name')]";
				var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
				if (PlayerNameContainsTeam(player))
				{
					nflTeam = player.Split('(')[1].Replace(")", "");
				}
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
