using System;
using System.Configuration;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Audio;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Drafts
{
	public class DraftPickAudioMapper : MapperBase<DraftPickAudio>
	{
		private string _demoApiCode = "demo";
		private int? _maxDailyAudioCalls = null;
		public int MaxDailyAudioCalls
		{
			get
			{
				if (!_maxDailyAudioCalls.HasValue)
				{
					_maxDailyAudioCalls = Int32.Parse(ConfigurationManager.AppSettings[Constants.AppSettings.MaxDailyAudioCalls] ?? "340");
				}
				return _maxDailyAudioCalls.Value;
			}
		}

		protected override void PopulateModel()
		{
			SingleDraftMapper mapper = new SingleDraftMapper();
			var currentDraftId = mapper.GetModel().DraftId;
			var lastDraftPick = HomeEntity.DraftPicks.Where(o => o.DraftId == currentDraftId && o.PlayerId != null)
				.OrderByDescending(o=>o.PickNum).FirstOrDefault();
			if (lastDraftPick != null)
			{
				var now = Utilities.GetEasternTime();
				var player = HomeEntity.Players.Where(o => o.PlayerId == lastDraftPick.PlayerId).First();
				var nflTeam = HomeEntity.NFLTeams.Where(o => o.TeamAbbr == player.NFLTeam).First();
				var position = HomeEntity.Positions.Where(o => o.PosCode == player.Position).First();
				//TODO: Add AudioKillSwitch & TextToVoiceKillSwitch check 
				var exhaustedApiCodes = HomeEntity.AudioCounts.Where(o => o.CallDate == now.Date && o.CallCount >= MaxDailyAudioCalls).
					Select(o=>o.AudioApiCode).ToList();
				var selectedApi = HomeEntity.AudioApis.Where(o => !exhaustedApiCodes.Contains(o.AudioApiCode) && o.AudioApiCode != _demoApiCode)
					.FirstOrDefault();
				if (selectedApi == null)
				{
					selectedApi = HomeEntity.AudioApis.Where(o => o.AudioApiCode == _demoApiCode).FirstOrDefault();
					if (selectedApi != null)
					{
						var random = new Random(DateTime.Now.Millisecond);
						selectedApi.AudioApiUrl = selectedApi.AudioApiUrl + "0." + random.Next(10000, Int32.MaxValue);
					}
                }

				Model = new DraftPickAudio
				{
					playerId = lastDraftPick.PlayerId.ToString(),
					name = GetPlayerName(player),
					pos = GetPositionAudio(position, nflTeam),
					team = GetTeamNameAudio(position, nflTeam),
					apiCode = (selectedApi != null) ? selectedApi.AudioApiCode : "",
					url = (selectedApi != null) ? selectedApi.AudioApiUrl : ""
				};
			}
		}

		private static string GetPlayerName(Player player)
		{
			string audio = player.PlayerName;
			switch (audio)
			{
				case "Ben Roethlisberger":
					audio = "Alleged Sex Offender Ben Roethlisberger";
					break;
				case "San Francisco 49ers":
					audio = "San Francisco Forty-Niners";
					break;
			}
			return audio.ToUrlEncodedString();
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
					audio = (nflTeam.TeamAbbr == "FA") ? "" : "Dee-fence";
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
			}
			return audio.ToUrlEncodedString();
		}
	}
}
 