using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class FprosStandardParser : RankParser
	{
		public override string RankTableSelect() { return "//div[contains(@class, 'mobile-table')]//table"; }
		public override string RankRowSelect() { return "//div[contains(@class, 'mobile-table')]//table//tr[contains(@class, 'mpb-player')]"; }
		public override string RankColSelect() { return "./td"; }
		
		public override HtmlNode GetRankTable(HtmlNode rankHtml)
		{
			var tables = rankHtml.SelectNodes(RankTableSelect());
			if (tables != null && tables.Count > 0)
			{
				return tables[0];
			}
			return null;
		}

		public override HtmlNodeCollection GetRankRows(HtmlNode rankTable)
		{
			return rankTable.SelectNodes(RankRowSelect());
		}

		public override HtmlNodeCollection GetRankColumns(HtmlNode rankRow)
		{
			return rankRow.SelectNodes(RankColSelect());
		}

		public override string GetPlayerRankNum(HtmlNodeCollection columns)
		{
			return columns[0].InnerText;
		}

		public override string GetPlayerName(HtmlNodeCollection columns)
		{
			var playerTeamNode = columns[1];
			var anch = "./a";
			var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
			return player;
		}

		public override string GetPlayerNFLTeam(HtmlNodeCollection columns)
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

		public override string GetPlayerPos(HtmlNodeCollection columns)
		{
			var posAndRank = columns[2].InnerText;
            return Regex.Replace(posAndRank, @"[\d-]", string.Empty);
		}

		public override void AddRankedPlayer(List<RankedPlayer> rankedPlayers, string rank, string player, string nflTeam, string pos)
		{
			rankedPlayers.Add(new RankedPlayer
			{
				RankNum = Utilities.ToNullInt(rank),
				PlayerName = player,
				NFLTeam = nflTeam,
				Position = pos
			});
		}
	}
}
