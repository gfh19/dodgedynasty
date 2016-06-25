using System;
using System.Collections.Generic;

namespace DodgeDynasty.Parsers
{
	public static class ParserFactory
	{
		public static Dictionary<int, IRankParser> RankParserDict = new Dictionary<int, IRankParser>
		{
			{ 2, new FprosStandardParser() }
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
