﻿using System;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Drafts
{
	public class DraftPickAudioMapper : MapperBase<DraftPickAudio>
	{
		protected override void PopulateModel()
		{
			SingleDraftMapper mapper = new SingleDraftMapper();
			var currentDraftId = mapper.GetModel().DraftId;
			var lastDraftPick = HomeEntity.DraftPicks.Where(o => o.DraftId == currentDraftId && o.PlayerId != null)
				.OrderByDescending(o=>o.PickNum).FirstOrDefault();
			if (lastDraftPick != null)
			{
				var player = HomeEntity.Players.Where(o => o.PlayerId == lastDraftPick.PlayerId).First();
				var nflTeam = HomeEntity.NFLTeams.Where(o => o.TeamAbbr == player.NFLTeam).First();
				var position = HomeEntity.Positions.Where(o => o.PosCode == player.Position).First();
				Model = new DraftPickAudio
				{
					playerId = lastDraftPick.PlayerId.ToString(),
					name = GetPlayerName(player),
					pos = GetPositionAudio(position, nflTeam),
					team = GetTeamNameAudio(position, nflTeam)
				};
			}
		}

		private static string GetPlayerName(Player player)
		{
			if (player.PlayerName == "Ben Roethlisberger")
			{
				return "Alleged Sex Offender Ben Roethlisberger".ToUrlEncodedString();
            }
			return player.PlayerName.ToUrlEncodedString();
		}

		private static string GetPositionAudio(Position position, NFLTeam nflTeam)
		{
			string audio = position.PosDesc;
			switch (position.PosCode)
			{
				case "RB":
					audio = "Runningback";
					break;
				case "WR":
					audio = "Receiver";
					break;
				case "DEF":
					audio = (nflTeam.TeamAbbr == "FA") ? "" : "Deefence";
					break;
			}
			return audio.ToUrlEncodedString();
		}

		private static string GetTeamNameAudio(Position position, NFLTeam nflTeam)
		{
			string audio = nflTeam.TeamName;
			var random = new Random(DateTime.Now.Millisecond);
			if (position.PosCode == "DEF")
			{
				audio = "";
			}
			else
			{
				switch (nflTeam.TeamAbbr)
				{
					case "FA":
						audio = "Free Agent";
						break;
					case "SF":
						audio = "Forty-Niners";
						break;
				}
				if (random.Next(1, 3) % 2 == 0)
				{
					switch (nflTeam.TeamAbbr)
					{
						case "CIN":
							audio = "Who dey think gonna beat them bengals";
							break;
						case "CLE":
							audio = "Here we go BROWNIES";
							break;
						case "DAL":
							audio = "How bout them COWBOYS";
							break;
						case "DEN":
							audio = "World Champion Broncos";
							break;
						case "NYJ":
							audio = "J-E-T-S JETS JETS JETS";
							break;
						case "NYG":
							audio = "The Geeeee Men";
							break;
						case "OAK":
							audio = "The Rrrrrrrray Ders";
							break;
						case "SD":
							audio = "SAN DEE-YAY GO, SUPER CHARGERS!";
							break;
						case "PIT":
							audio = "Pittspuke";
							break;
					}
				}
			}
			return audio.ToUrlEncodedString();
		}
	}
}
 