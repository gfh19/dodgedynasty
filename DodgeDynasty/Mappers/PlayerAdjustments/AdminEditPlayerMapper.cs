using DodgeDynasty.Models.PlayerAdjustments;
using System.Linq;
using DodgeDynasty.Shared;
using System;

namespace DodgeDynasty.Mappers.PlayerAdjustments
{
	public class AdminEditPlayerMapper : MapperBase<AdminPlayerModel>
	{
		protected override void DoUpdate(AdminPlayerModel playerModel)
		{
			var player = HomeEntity.Players.FirstOrDefault(o => o.PlayerId == playerModel.PlayerId);
			if (player != null)
			{
				var originalTruePlayerId = player.TruePlayerId;
				if (playerModel.TruePlayerId != null)
				{
					player.TruePlayerId = playerModel.TruePlayerId;
				}
				player.FirstName = playerModel.FirstName;
				player.LastName = playerModel.LastName;
				player.Position = playerModel.Position.ToUpper();
				player.NFLTeam = playerModel.NFLTeam.ToUpper();
				player.IsActive = playerModel.IsActive;
				player.LastUpdateTimestamp = DateTime.Now;
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
					Action = "Admin Edit Player",
					UserId = userId,
					AddTimestamp = DateTime.Now,
					LastUpdateTimestamp = DateTime.Now
				};
				if (player.TruePlayerId != originalTruePlayerId)
				{
					playerAdd.Action = playerAdd.Action + ", Merge TruePlayerId";
				}
				HomeEntity.PlayerAdjustments.AddObject(playerAdd);
				HomeEntity.SaveChanges();
			}
		}
	}
}
