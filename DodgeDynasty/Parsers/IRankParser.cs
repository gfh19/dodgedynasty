using System.Collections.Generic;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public interface IRankParser
	{
		bool CheckPositions { get; }
		List<Position> Positions { get; set; }
		List<RankedPlayer> ParseRankHtml(HtmlNode rankHtml);
    }
}
