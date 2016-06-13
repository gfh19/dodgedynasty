namespace DodgeDynasty.Parsers
{
	public class ParserFactory
	{
		public static IRankParser Create() {
			return new RankParser();
		}
	}
}
