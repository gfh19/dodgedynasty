using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class RankParser : IRankParser
	{
		public string RankTableSelect { get { return "//div[contains(@class, 'mobile-table')]//table"; } }
		public string RankRowSelect { get { return "//div[contains(@class, 'mobile-table')]//table//tr[contains(@class, 'mpb-player')]"; } }
		public string RankColSelect { get { return "./td"; } }

		public List<RankedPlayer> ParseRankHtml(HtmlNode rankHtml)
		{
			List<RankedPlayer> rankedPlayers = new List<RankedPlayer>();
			HtmlNode rankTable = GetRankTable(rankHtml);
			if (rankTable != null)
			{
				HtmlNodeCollection rows = GetRankRows(rankTable);
				if (rows != null)
				{
					foreach (var row in rows)
					{
						HtmlNodeCollection columns = GetRankColumns(row);
						if (columns != null)
						{
							string rank = GetPlayerRankNum(columns);
							string player = GetPlayerName(columns);
							string nflTeam = GetPlayerNFLTeam(columns);
							string pos = GetPlayerPos(columns);
							AddRankedPlayer(rankedPlayers, rank, player, nflTeam, pos);
						}
					}
				}
			}
			return rankedPlayers;
		}

		public HtmlNode GetRankTable(HtmlNode rankHtml)
		{
			var tables = rankHtml.SelectNodes(RankTableSelect);
			if (tables != null && tables.Count > 0)
			{
				return tables[0];
			}
			return null;
		}

		public HtmlNodeCollection GetRankRows(HtmlNode rankTable)
		{
			return rankTable.SelectNodes(RankRowSelect);
		}

		private HtmlNodeCollection GetRankColumns(HtmlNode rankRow)
		{
			return rankRow.SelectNodes(RankColSelect);
		}

		public string GetPlayerRankNum(HtmlNodeCollection columns)
		{
			return columns[0].InnerText;
		}

		public string GetPlayerName(HtmlNodeCollection columns)
		{
			var playerTeamNode = columns[1];
			var anch = "./a";
			var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
			return player;
		}

		public string GetPlayerNFLTeam(HtmlNodeCollection columns)
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

		public string GetPlayerPos(HtmlNodeCollection columns)
		{
			var posAndRank = columns[2].InnerText;
            return Regex.Replace(posAndRank, @"[\d-]", string.Empty);
		}

		public void AddRankedPlayer(List<RankedPlayer> rankedPlayers, string rank, string player, string nflTeam, string pos)
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
