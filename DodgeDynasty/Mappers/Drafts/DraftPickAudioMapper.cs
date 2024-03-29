﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Audio;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Drafts
{
	public class DraftPickAudioMapper : MapperBase<DraftPickAudio>
	{
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
			var currentDraft = mapper.GetModel();
            var currentDraftId = currentDraft.DraftId;
			var lastDraftPick = HomeEntity.DraftPicks.Where(o => o.DraftId == currentDraftId && o.PlayerId != null)
				.OrderByDescending(o=>o.PickNum).FirstOrDefault();
			if (lastDraftPick != null)
			{
				var player = HomeEntity.Players.Where(o => o.PlayerId == lastDraftPick.PlayerId).First();
				var nflTeam = HomeEntity.NFLTeams.Where(o => o.TeamAbbr == player.NFLTeam).First();
				var position = HomeEntity.Positions.Where(o => o.PosCode == player.Position).First();

				//TODO: Add AudioKillSwitch & TextToVoiceKillSwitch check 
				AudioApi selectedApi = null;
				bool isAudioUserSuccessful = false;
				bool isFinalDraftPick = false;
				bool access = false;
				string errorText = null;
				var userId = HomeEntity.Users.GetLoggedInUserId();
				var leagueOwner = HomeEntity.LeagueOwners.FirstOrDefault(o => o.LeagueId == currentDraft.LeagueId && o.UserId == userId);
				var currentUserRoles = HomeEntity.UserRoles.Where(o => o.UserId == userId).ToList();
				var isGuest = currentUserRoles.Select(o => o.RoleId).Contains(Constants.Roles.Guest);
				if (isGuest || leagueOwner != null)
				{
					access = ShouldPlayPickAudio(currentDraftId, isGuest, leagueOwner, currentUserRoles, out isFinalDraftPick);
					if (access)
					{
						isAudioUserSuccessful = ThrottleOneAudioUser(userId, currentDraftId, lastDraftPick, out errorText);
						if (isAudioUserSuccessful)
						{
							selectedApi = SelectAvailableAudioApi();
						}
					}
				}

				Model = new DraftPickAudio
				{
					playerId = lastDraftPick.PlayerId.ToString(),
					name = GetPlayerName(player, HomeEntity.PlayerAudios.ToList()),
					pos = GetPositionAudio(position, nflTeam),
					team = GetTeamNameAudio(position, nflTeam),
					apiCode = (selectedApi != null) ? selectedApi.AudioApiCode : "",
					url = (selectedApi != null) ? selectedApi.AudioApiUrl : "",
					access = access.ToString(),
					final = isFinalDraftPick.ToString(),
                    success = isAudioUserSuccessful.ToString(),
					error = errorText
				};
			}
		}

		private bool ShouldPlayPickAudio(int currentDraftId, bool isGuest, LeagueOwner leagueOwner, List<UserRole> currentUserRoles, out bool isFinalDraftPick)
		{
			isFinalDraftPick = false;
            bool access = false;

			if (isGuest || leagueOwner.AnnounceAllPicks || leagueOwner.AnnouncePrevPick)
			{
				var nextDraftPick = HomeEntity.DraftPicks.Where(o => o.DraftId == currentDraftId && o.PlayerId == null)
					.OrderBy(p => p.PickNum).FirstOrDefault();
				isFinalDraftPick = nextDraftPick == null;
                if (isGuest || leagueOwner.AnnounceAllPicks)
				{
					access = true;
				}
				else
				{
					if (nextDraftPick != null)
					{
						access = nextDraftPick.UserId == leagueOwner.UserId;
					}
					else
					{
						access = true;
					}
				}
			}
			return access;
		}

		private bool ThrottleOneAudioUser(int userId, int currentDraftId, DraftPick lastDraftPick, out string errorText)
		{
			//Throttle to only one audio call per user/draft/pick/playerId, in case of multiple tabs/devices per user
			var isAudioUserSuccessful = false;
			errorText = null;
			try
			{
				if (!HomeEntity.AudioUserCounts.Any(o => o.UserId == userId && o.DraftId == currentDraftId
					&& o.PickNum == lastDraftPick.PickNum && o.PlayerId == lastDraftPick.PlayerId.Value))
				{
					var now = Utilities.GetEasternTime();
					HomeEntity.AudioUserCounts.AddObject(new AudioUserCount
					{
						UserId = userId,
						DraftId = currentDraftId,
						PickNum = lastDraftPick.PickNum,
						PlayerId = lastDraftPick.PlayerId.Value,
						AddTimestamp = now,
						LastUpdateTimestamp = now
					});
					HomeEntity.SaveChanges();
					isAudioUserSuccessful = true;
				}
			}
			catch (SqlException ex)
			{
				isAudioUserSuccessful = false;
				errorText = ex.Message;
			}
			return isAudioUserSuccessful;
        }

		private AudioApi SelectAvailableAudioApi()
		{
			var now = Utilities.GetEasternTime();
			var exhaustedApiCodes = HomeEntity.AudioCounts.Where(o => o.CallDate == now.Date && o.CallCount >= MaxDailyAudioCalls).
				Select(o => o.AudioApiCode).ToList();
			var selectedApi = HomeEntity.AudioApis.Where(o => !exhaustedApiCodes.Contains(o.AudioApiCode) && o.AudioApiCode != Constants.Audio.Demo)
				.FirstOrDefault();
			if (selectedApi == null)
			{
				selectedApi = HomeEntity.AudioApis.Where(o => o.AudioApiCode == Constants.Audio.Demo).FirstOrDefault();
				if (selectedApi != null)
				{
					var random = new Random(DateTime.Now.Millisecond);
					selectedApi.AudioApiUrl = selectedApi.AudioApiUrl + "0." + random.Next(100000, Int32.MaxValue);
				}
			}

			return selectedApi;
		}

		private static string GetPlayerName(Player player, List<PlayerAudio> playerAudios)
		{
			string playerName = player.PlayerName.Replace("'", "");
			var playerAudio = playerAudios.FirstOrDefault(pa=>pa.TruePlayerId == player.TruePlayerId);
			if (playerAudio == null)
			{
				playerAudio = playerAudios.FirstOrDefault(pa => pa.TruePlayerId == null && pa.PlayerName.Replace("'", "") == playerName);
			}
			return (playerAudio != null)
				? playerAudio.PlayerNameAudio.ToLower().ToUrlEncodedString()
				: playerName.ToLower().ToUrlEncodedString();
		}

		private static string GetPositionAudio(Position position, NFLTeam nflTeam)
		{
			string audio = position.PosDesc;
			switch (position.PosCode)
			{
				case "WR":
					audio = "Receiver";
					break;
				case "DEF":
					audio = (nflTeam.TeamAbbr == "FA") ? "" : "Dee-fense";
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
 