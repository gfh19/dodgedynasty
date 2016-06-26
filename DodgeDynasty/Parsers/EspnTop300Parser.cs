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
		
		public override string GetPlayerRankNum(HtmlNodeCollection columns)
		{
			var rkNumAndPlayer = columns[0].InnerText;
			var firstDot = rkNumAndPlayer.IndexOf(". ");
			return rkNumAndPlayer.Substring(0, firstDot).Trim();
		}

		public override string GetPlayerName(HtmlNodeCollection columns)
		{
			var rkNumAndPlayer = columns[0].InnerText;
			var firstDot = rkNumAndPlayer.IndexOf(".");
			return rkNumAndPlayer.Substring(firstDot + 2, rkNumAndPlayer.Length - (firstDot + 2)).Trim();
		}

		public override string GetPlayerPos(HtmlNodeCollection columns)
		{
			return columns[1].InnerText;
		}

		public override string GetPlayerNFLTeam(HtmlNodeCollection columns)
		{
			return columns[2].InnerText;
		}
	}
}
