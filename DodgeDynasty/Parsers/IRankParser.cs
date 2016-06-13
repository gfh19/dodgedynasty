using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.Types;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public interface IRankParser
	{
		List<RankedPlayer> ParseRankHtml(HtmlNode rankHtml);
    }
}
