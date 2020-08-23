using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class EspnTop300Parser : RankParser
	{
		private bool _isHtmlTable = false;
		private string _htmlTablePath = "(//div[contains(@class, 'article-body')]//table)[2]";
		private string _htmlListPath = "(//div[contains(@class, 'article-body')]//p)[3]";

		public override string RankTableSelect() { return _isHtmlTable ? _htmlTablePath : _htmlListPath; }
		public override string RankRowSelect() { return _isHtmlTable ? $"{_htmlTablePath}//tbody//tr" : $"{_htmlListPath}//br"; }
		public override string RankColSelect() { return _isHtmlTable ? "./td" : "span"; }

		public override void PreRankParse(HtmlNode rankHtml)
		{
			_isHtmlTable = (rankHtml.SelectNodes(_htmlTablePath) != null);
            base.PreRankParse(rankHtml);
		}

		public override List<HtmlNode> GetRankRows(HtmlNode rankTable)
		{
			if (_isHtmlTable)
			{
				return base.GetRankRows(rankTable);
			}

			var brTag = rankTable.InnerHtml.IndexOf("<br/>") > 0 ? "<br/>" : "<br>";
            var rankRows = rankTable.InnerHtml.Replace("\n", "").Split(new string[] { brTag }, StringSplitOptions.RemoveEmptyEntries);

			List<HtmlNode> rankHtmlRows = new List<HtmlNode>();
			
			foreach (var rank in rankRows)
			{
				HtmlDocument rankRowsDoc = new HtmlDocument();
				rankRowsDoc.LoadHtml($"<span>{rank}</span>");
				rankHtmlRows.Add(rankRowsDoc.DocumentNode);
            }

			return rankHtmlRows;
		}

		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return _isHtmlTable ? GetTablePlayerRankNum(columns) : GetListPlayerRankNum(columns);
        }

		public override string GetPlayerName(List<HtmlNode> columns)
		{
			return _isHtmlTable ? GetTablePlayerName(columns) : GetListPlayerName(columns);
		}

		public override string GetPlayerPos(List<HtmlNode> columns)
		{
			return _isHtmlTable ? GetTablePlayerPos(columns) : GetListPlayerPos(columns);
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			return _isHtmlTable ? GetTablePlayerNFLTeam(columns) : GetListPlayerNFLTeam(columns);
		}


		#region Html P list
		private string GetListPlayerRankNum(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerHtml;
			var firstDot = rkNumPlayerPosTeam.IndexOf(". ");
			var rankNum = rkNumPlayerPosTeam.Substring(0, firstDot).Trim();
			return rankNum;
        }

		private string GetListPlayerName(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerHtml;
			var playerLink = columns[0].SelectNodes("a")[0];
			var playerName = playerLink.InnerText;
			return playerName;
		}

		private string GetListPlayerPos(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerHtml;
			var trimmedPosRowText = rkNumPlayerPosTeam.Substring(rkNumPlayerPosTeam.IndexOf("--"));
			var nextComma = trimmedPosRowText.IndexOf(",");
			var pos = Regex.Replace(trimmedPosRowText.Substring(2, nextComma - 2).Trim(), @"[\d-]", string.Empty);
			return pos;
		}

		private string GetListPlayerNFLTeam(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerHtml;
			var firstComma = rkNumPlayerPosTeam.IndexOf(",");
			var dashDash = rkNumPlayerPosTeam.IndexOf("--");
			var nflTeam = rkNumPlayerPosTeam.Substring(firstComma + 1, dashDash - firstComma - 1).Trim();
			return nflTeam;
		}
		#endregion Html P list

		#region Html Table
		private string GetTablePlayerRankNum(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerText;
			var firstDot = rkNumPlayerPosTeam.IndexOf(". ");
			return rkNumPlayerPosTeam.Substring(0, firstDot).Trim();
		}

		private string GetTablePlayerName(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerText;
			var firstDot = rkNumPlayerPosTeam.IndexOf(".");
			var firstComma = rkNumPlayerPosTeam.Contains(",") ? rkNumPlayerPosTeam.IndexOf(",") : rkNumPlayerPosTeam.Length;
			return rkNumPlayerPosTeam.Substring(firstDot + 2, firstComma - (firstDot + 2)).Trim();
		}

		private string GetTablePlayerPos(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[1].InnerText;
			return rkNumPlayerPosTeam.Trim();
		}

		private string GetTablePlayerNFLTeam(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[2].InnerText;
			return rkNumPlayerPosTeam.Trim();
		}
		#endregion Html Table
	}
}
