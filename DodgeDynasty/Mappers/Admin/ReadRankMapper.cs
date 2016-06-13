using System;
using System.Net;
using System.Net.Http;
using System.Text;
using DodgeDynasty.Models.Admin;
using DodgeDynasty.Parsers;
using HtmlAgilityPack;

namespace DodgeDynasty.Mappers.Admin
{
	public class ReadRankMapper : MapperBase<ReadRankModel>
	{
		protected override void PopulateModel()
		{
			HttpClient http = new HttpClient();
			//var response = await http.GetByteArrayAsync("");
			var task = http.GetByteArrayAsync("https://www.fantasypros.com/nfl/rankings/consensus-cheatsheets.php");
			task.Wait();
			var response = task.Result;
			String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
			source = WebUtility.HtmlDecode(source);
			HtmlDocument rankHtml = new HtmlDocument();
			rankHtml.LoadHtml(source);
			var rankDoc = rankHtml.DocumentNode;

			var parser = ParserFactory.Create();
			var rankedPlayers = parser.ParseRankHtml(rankDoc);
		}
	}
}
