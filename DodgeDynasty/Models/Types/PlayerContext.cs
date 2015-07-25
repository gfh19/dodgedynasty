using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.Types
{
	public class PlayerContext : Player
	{
		public PlayerContext(Player player)
		{
			if (player.PlayerId != 0)
			{
				this.PlayerId = player.PlayerId;
				this.TruePlayerId = player.TruePlayerId;
				this.DateOfBirth = player.DateOfBirth;
				this.FirstName = player.FirstName ?? this.FirstName;
				this.LastName = player.LastName;
				this.IsActive = player.IsActive;
				this.IsDrafted = player.IsDrafted;
				this.NFLTeam = player.NFLTeam;
				this.PlayerName = player.PlayerName;
				this.Position = player.Position;
				this.AddTimestamp = player.AddTimestamp;
				this.LastUpdateTimestamp = player.LastUpdateTimestamp;
			}
		}

		public int? ByeWeek { get; set; }
	}
}