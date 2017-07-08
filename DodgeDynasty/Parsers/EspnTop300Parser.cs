using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class EspnTop300Parser : RankParser
	{
		public override string RankTableSelect() { return "(//div[contains(@class, 'article-body')]//table)[2]"; }
		public override string RankRowSelect() { return RankTableSelect() + "//tbody//tr"; }
		public override string RankColSelect() { return "./td"; }
		
		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerText;
			var firstDot = rkNumPlayerPosTeam.IndexOf(". ");
			return rkNumPlayerPosTeam.Substring(0, firstDot).Trim();
		}

		public override string GetPlayerName(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerText;
			var firstDot = rkNumPlayerPosTeam.IndexOf(".");
			var firstComma = rkNumPlayerPosTeam.IndexOf(",");
			return rkNumPlayerPosTeam.Substring(firstDot + 2, firstComma - (firstDot + 2)).Trim();
		}

		public override string GetPlayerPos(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerText;
			var firstComma = rkNumPlayerPosTeam.IndexOf(",");
			var secondComma = rkNumPlayerPosTeam.IndexOf(",", firstComma + 1);
			return rkNumPlayerPosTeam.Substring(firstComma + 2, secondComma - (firstComma + 2)).Trim();
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			var rkNumPlayerPosTeam = columns[0].InnerText;
			var firstComma = rkNumPlayerPosTeam.IndexOf(",");
			var secondComma = rkNumPlayerPosTeam.IndexOf(",", firstComma + 1);
			return rkNumPlayerPosTeam.Substring(secondComma + 2, rkNumPlayerPosTeam.Length - (secondComma + 2)).Trim();
		}
	}
}
