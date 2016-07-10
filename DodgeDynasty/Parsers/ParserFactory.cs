using System;
using System.Collections.Generic;

namespace DodgeDynasty.Parsers
{
	public static class ParserFactory
	{
		public static Dictionary<int, IRankParser> RankParserDict = new Dictionary<int, IRankParser>
		{
			{ 1, new EspnTop300Parser() },		//ESPN Top 300
			{ 2, new FprosStandardParser() },	//Fantasypros - Standard
			{ 3, new EspnAdpParser() },			//ESPN ADP
			{ 4, new FprosAdpParser() },		//Fantasypros ADP
			{ 5, new YahooParser() },			//Yahoo!
			{ 5, new FprosDynasty() },			//Fantasypros - Dynasty
		};

		public static IRankParser Create(int? autoImportId)
		{
			if (autoImportId == null)
			{
				throw new Exception("AutoImportId is null!");
			}
			return RankParserDict[autoImportId.Value];
		}
	}
}
