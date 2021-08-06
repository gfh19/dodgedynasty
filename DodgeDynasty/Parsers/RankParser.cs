using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;
using HtmlAgilityPack;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace DodgeDynasty.Parsers
{
	public abstract class RankParser : IRankParser
	{
		public int MaxPlayerCount { get { return 600; } }
		public virtual bool CheckPositions { get { return false; } }
		public List<Position> Positions { get; set; }
		public int PlayerCount { get; set; }

		public virtual string RankTableSelect => "//div[contains(@class, 'mobile-table')]//table";
		public virtual string RankRowSelect => "//div[contains(@class, 'mobile-table')]//table//tr[contains(@class, 'mpb-player')]";
		public virtual string RankColSelect => "./td";

		public virtual List<RankedPlayer> ParseRankHtml(HtmlNode rankHtml, bool confirmed, int? userMaxCount)
		{
			PreRankParse(rankHtml);
			List<RankedPlayer> rankedPlayers = new List<RankedPlayer>();
			HtmlNode rankTable = GetRankTable(rankHtml);
			int origPlayerCount = 0;
			if (rankTable != null)
			{
				List<HtmlNode> rows = GetRankRows(rankTable);
				if (rows != null)
				{
					origPlayerCount = rows.Count;
					if (RowsExceedMaxCount(rows.Count, confirmed, userMaxCount))
					{
						var max = userMaxCount ?? MaxPlayerCount;
						rows = rows.Take(max).ToList();
                    }
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
			PlayerCount = rankedPlayers.Count;
            if (!confirmed && origPlayerCount > rankedPlayers.Count && rankedPlayers.Count == MaxPlayerCount)
			{
				PlayerCount = origPlayerCount;
			}
			PostRankParse(rankHtml);
			return rankedPlayers;
		}

		public virtual void PreRankParse(HtmlNode rankHtml)
		{ }

		public virtual void PostRankParse(HtmlNode rankHtml)
		{ }

		public virtual List<RankedPlayer> ParseRankJson(string rankJson, bool confirmed, int? userMaxCount)
		{
			List<RankedPlayer> rankedPlayers = ConvertJsonRankRows(rankJson);
			int origPlayerCount = 0;
			if (rankedPlayers != null)
			{
				origPlayerCount = rankedPlayers.Count;
				if (RowsExceedMaxCount(rankedPlayers.Count, confirmed, userMaxCount))
				{
					var max = userMaxCount ?? MaxPlayerCount;
					rankedPlayers = rankedPlayers.Take(max).ToList();
				}
			}
			PlayerCount = rankedPlayers.Count;
			if (!confirmed && origPlayerCount > rankedPlayers.Count && rankedPlayers.Count == MaxPlayerCount)
			{
				PlayerCount = origPlayerCount;
			}
			return rankedPlayers;
		}

		public virtual List<RankedPlayer> ConvertJsonRankRows(string rankJson)
		{
			return new List<RankedPlayer>();
		}

		public List<RankedPlayer> ParseRankPdf(byte[] pdfByteArray, bool confirmed, int? maxCount)
		{
			List<RankedPlayer> rankedPlayers = new List<RankedPlayer>();
			if (pdfByteArray != null)
			{
				StringBuilder text = new StringBuilder();
				MemoryStream stream = new MemoryStream(pdfByteArray);
				using (var pdfDocument = new PdfDocument(new PdfReader(stream)))
				{
					for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
					{
						ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
						var page = pdfDocument.GetPage(i);
						string currentText = PdfTextExtractor.GetTextFromPage(page, strategy);

						currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
						text.Append(currentText);
					}
				}
				rankedPlayers = new List<RankedPlayer>();
				var numsAndPlayers = Regex.Split(text.ToString(), @"(\d+\.{1})").Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
				for (int i = 0; i < numsAndPlayers.Count() - 2; i += 2)
				{
					var rankNum = numsAndPlayers[i].Replace(".", "").Trim();
					var pnamePieces = numsAndPlayers[i + 1].Split(')');
					var pos = Regex.Replace(pnamePieces[0], @"[\d-]", string.Empty).Replace("(", "").Trim();
					pnamePieces = pnamePieces[1].Split(',');
					var playerName = pnamePieces[0].Trim();
					var team = pnamePieces[1].Substring(0, pnamePieces[1].LastIndexOf("$")).Trim();

					rankedPlayers.Add(new RankedPlayer
					{
						RankNum = Utilities.ToNullInt(rankNum),
						PlayerName = playerName,
						NFLTeam = team,
						Position = pos
					});
				}
				rankedPlayers = rankedPlayers.OrderBy(r => r.RankNum).ToList();
				PlayerCount = rankedPlayers.Count;
			}
			return rankedPlayers;
		}

		public virtual HtmlNode GetRankTable(HtmlNode rankHtml)
		{
			string xpath = RankTableSelect;
			var tables = rankHtml.SelectNodes(xpath);
			if (tables != null && tables.Count > 0)
			{
				return tables[0];
			}
			return null;
		}

		public virtual List<HtmlNode> GetRankRows(HtmlNode rankTable)
		{
			var select = RankRowSelect;
			return rankTable.SelectNodes(select).Where(o=>!string.IsNullOrWhiteSpace(o.InnerText)).ToList();
		}

		public virtual List<HtmlNode> GetRankColumns(HtmlNode rankRow)
		{
			return rankRow.SelectNodes(RankColSelect).ToList();
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

		public virtual bool RowsExceedMaxCount(int rowCount, bool confirmed, int? userMaxCount)
		{
			if (confirmed && userMaxCount.HasValue && rowCount > userMaxCount)
			{
				return true;
			}
			else if (!confirmed && rowCount > MaxPlayerCount)
			{
				return true;
			}
			return false;
		}
	}
}
