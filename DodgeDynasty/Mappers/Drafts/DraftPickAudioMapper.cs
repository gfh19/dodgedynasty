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
					name = player.PlayerName.ToUrlEncodedString(),
					pos = GetPositionAudio(position, nflTeam),
					team = GetTeamNameAudio(position, nflTeam)
				};
			}
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
					audio = (nflTeam.TeamAbbr == "FA") ? null : "Defense";
					break;
			}
			return audio.ToUrlEncodedString();
		}

		private static string GetTeamNameAudio(Position position, NFLTeam nflTeam)
		{
			string audio = nflTeam.TeamName;
			if (position.PosCode == "DEF")
			{
				audio = null;
			}
			switch (nflTeam.TeamAbbr)
			{
				case "FA":
					audio = "Free Agent";
					break;
				case "SF":
					audio = "Forty-Niners";
					break;
			}
			return audio.ToUrlEncodedString();
		}
	}
}
 