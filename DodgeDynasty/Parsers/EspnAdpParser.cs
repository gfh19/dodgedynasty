using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class EspnAdpParser : RankParser
	{
		public override string RankTableSelect() { return "//div[contains(@class, 'gamesmain')]//table"; }
		public override string RankRowSelect() { return ".//tr[not(contains(@class, 'tableHead')) and not(contains(@class, 'tableSubHead'))]"; }
		public override string RankColSelect() { return "./td"; }

		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return columns[0].InnerText;
		}

		private string[] GetPlayerNameAndTeam(List<HtmlNode> columns)
		{
			string[] playerNameAndTeam = new string[2];
			var playerNameAndTeamText = columns[1].InnerText;
			var firstComma = playerNameAndTeamText.IndexOf(",");
			if (firstComma > 0)
			{
				playerNameAndTeam[0] = playerNameAndTeamText.Substring(0, firstComma).Trim();
				playerNameAndTeam[1] = playerNameAndTeamText.Substring(firstComma + 2, playerNameAndTeamText.Length - (firstComma + 2)).Trim();
			}
			else
			{
				playerNameAndTeam[0] = playerNameAndTeamText.Trim();
				playerNameAndTeam[1] = "";
			}

			return playerNameAndTeam;
		}

		public override string GetPlayerName(List<HtmlNode> columns)
		{
			string[] playerNameAndTeam = GetPlayerNameAndTeam(columns);
			return playerNameAndTeam[0];
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			string[] playerNameAndTeam = GetPlayerNameAndTeam(columns);
			return playerNameAndTeam[1];
		}

		public override string GetPlayerPos(List<HtmlNode> columns)
		{
			return columns[2].InnerText;
		}
	}
}
