using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DodgeDynasty.Shared;
using HtmlAgilityPack;

namespace DodgeDynasty.Parsers
{
	public class YahooParser : RankParser
	{
		public override bool CheckPositions { get { return true; } }
		//[0]=Player, [1]=Pos, [2]=NFLTeam
		protected string[] PlayerPosTeam { get; set; }

		public override string RankTableSelect() { return "(//table[contains(@class, 'rankings')])[1]"; }
		public override string RankRowSelect() { return ".//tbody//tr"; }
		public override string RankColSelect() { return "./td"; }

		public override void StartParsingPlayer(List<HtmlNode> columns)
		{
			PlayerPosTeam = GetPlayerPosTeam(columns);
        }

		private string[] GetPlayerPosTeam(List<HtmlNode> columns)
		{
			//[0]=Player, [1]=Pos, [2]=NFLTeam
			string[] playerPosTeam = new string[3];
			var playerPosTeamText = Regex.Replace(columns[1].InnerText, @"[\d]", string.Empty);
			var textSplit = playerPosTeamText.Split(new string[] { " - " }, StringSplitOptions.None);
			if (textSplit.Length > 1 && !string.IsNullOrWhiteSpace(textSplit[1].Trim()))
			{
				//i.e. Antonio Brown WR - PIT,  
				bool posFound = false;
				foreach (var pos in Positions.Select(o=>o.PosCode))
				{
					var firstText = textSplit[0].Trim();
                    if (firstText.EndsWith(" " + pos))
					{
						var lastIx = firstText.LastIndexOf(" " + pos);
						playerPosTeam[0] = firstText.Substring(0, lastIx);
						playerPosTeam[1] = pos;
						posFound = true;
						break;
                    }
					if (!posFound)
					{
						playerPosTeam[0] = firstText;
                    }
				}
				//NFLTeam
				playerPosTeam[2] = textSplit[1].Trim().Replace(",", "");
            }
			else
			{
				//i.e. Seattle Seahawks  
				playerPosTeam[0] = textSplit[0].Trim();
				playerPosTeam[1] = "";
				//If digit found (bye week), then has team so must be Defense.  Else, is FA.
				if (Utilities.HasNumber(columns[1].InnerText))
				{
					playerPosTeam[2] = "";
				}
				else
				{
					playerPosTeam[2] = "FA";
				}
			}
			return playerPosTeam;
		}

		public override string GetPlayerRankNum(List<HtmlNode> columns)
		{
			return columns[0].InnerText;
		}
		
		public override string GetPlayerName(List<HtmlNode> columns)
		{
			return PlayerPosTeam[0];
		}

		public override string GetPlayerPos(List<HtmlNode> columns)
		{
			return PlayerPosTeam[1];
		}

		public override string GetPlayerNFLTeam(List<HtmlNode> columns)
		{
			return PlayerPosTeam[2];
		}
	}
}
