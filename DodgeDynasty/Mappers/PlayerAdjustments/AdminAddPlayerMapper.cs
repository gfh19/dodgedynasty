using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Models.PlayerAdjustments;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.PlayerAdjustments
{
	public class AdminAddPlayerMapper : MapperBase<AdminPlayerModel>
	{
		protected override void DoUpdate(AdminPlayerModel playerModel)
		{
			var player = new Entities.Player
			{
				FirstName = playerModel.FirstName,
				LastName = playerModel.LastName,
				Position = playerModel.Position.ToUpper(),
				NFLTeam = playerModel.NFLTeam.ToUpper(),
				IsActive = playerModel.IsActive,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			HomeEntity.Players.AddObject(player);
			HomeEntity.SaveChanges();
			player.TruePlayerId = playerModel.TruePlayerId ?? player.PlayerId;
			HomeEntity.SaveChanges();

			var userId = HomeEntity.Users.GetLoggedInUserId();
            var playerAdd = new Entities.PlayerAdjustment
			{
				NewPlayerId = player.PlayerId,
				TruePlayerId = player.TruePlayerId,
				NewFirstName = player.FirstName,
				NewLastName = player.LastName,
				NewPosition = player.Position.ToUpper(),
				NewNFLTeam = player.NFLTeam.ToUpper(),
				Action = "Admin Add Player",
				UserId = userId,
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			if (player.TruePlayerId != player.PlayerId)
			{
				playerAdd.Action = playerAdd.Action + ", Merge TruePlayerId";
			}
			HomeEntity.PlayerAdjustments.AddObject(playerAdd);
			HomeEntity.SaveChanges();
		}
	}
}
