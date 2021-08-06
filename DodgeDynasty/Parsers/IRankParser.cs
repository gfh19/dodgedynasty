using System.Collections.Generic;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public interface IRankParser
	{
		int MaxPlayerCount { get; }
		int PlayerCount { get; set; }
		bool CheckPositions { get; }
		List<Position> Positions { get; set; }
		List<RankedPlayer> ParseRankHtml(HtmlNode rankHtml, bool confirmed, int? maxCount);
		List<RankedPlayer> ParseRankJson(string rankJson, bool confirmed, int? maxCount);
		List<RankedPlayer> ParseRankPdf(byte[] pdfByteArray, bool confirmed, int? maxCount);
	}
}
