using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public abstract class RankParser : IRankParser
	{
		public virtual bool CheckPositions { get { return false; } }
		public List<Position> Positions { get; set; }

		public virtual string RankTableSelect() { return "//div[contains(@class, 'mobile-table')]//table"; }
		public virtual string RankRowSelect() { return "//div[contains(@class, 'mobile-table')]//table//tr[contains(@class, 'mpb-player')]"; }
		public virtual string RankColSelect() { return "./td"; }

		public virtual List<RankedPlayer> ParseRankHtml(HtmlNode rankHtml)
		{
			List<RankedPlayer> rankedPlayers = new List<RankedPlayer>();
			HtmlNode rankTable = GetRankTable(rankHtml);
			if (rankTable != null)
			{
				List<HtmlNode> rows = GetRankRows(rankTable);
				if (rows != null)
				{
					foreach (var row in rows)
					{
						List<HtmlNode> columns = GetRankColumns(row);
						if (columns != null)
						{
							StartParsingPlayer(columns);
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

		public virtual HtmlNode GetRankTable(HtmlNode rankHtml)
		{
			var tables = rankHtml.SelectNodes(RankTableSelect());
			if (tables != null && tables.Count > 0)
			{
				return tables[0];
			}
			return null;
		}

		public virtual List<HtmlNode> GetRankRows(HtmlNode rankTable)
		{
			return rankTable.SelectNodes(RankRowSelect()).Where(o=>!string.IsNullOrWhiteSpace(o.InnerText)).ToList();
		}

		public virtual List<HtmlNode> GetRankColumns(HtmlNode rankRow)
		{
			return rankRow.SelectNodes(RankColSelect()).ToList();
		}

		public virtual void StartParsingPlayer(List<HtmlNode> columns)
		{ }

		public virtual string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return columns[0].InnerText;
		}

		public virtual string GetPlayerName(List<HtmlNode> columns)
		{
			var playerTeamNode = columns[1];
			var anch = "./a";
			var player = playerTeamNode.SelectNodes(anch)[0].InnerText;
			return player;
		}

		public virtual string GetPlayerNFLTeam(List<HtmlNode> columns)
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

		public virtual string GetPlayerPos(List<HtmlNode> columns)
		{
			var posAndRank = columns[2].InnerText;
            return Regex.Replace(posAndRank, @"[\d-]", string.Empty);
		}

		public virtual void AddRankedPlayer(List<RankedPlayer> rankedPlayers, string rank, string player, string nflTeam, string pos)
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
