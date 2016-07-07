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
				var now = Utilities.GetEasternTime();
				var activateOnly = string.IsNullOrEmpty(playerModel.FirstName);
                var originalTruePlayerId = player.TruePlayerId;
				player.TruePlayerId = playerModel.TruePlayerId ?? player.TruePlayerId;
				player.FirstName = playerModel.FirstName ?? player.FirstName;
				player.LastName = playerModel.LastName ?? player.LastName;
				player.Position = (playerModel.Position != null) ? playerModel.Position.ToUpper() : player.Position;
				player.NFLTeam = (playerModel.NFLTeam != null) ? playerModel.NFLTeam.ToUpper() : player.NFLTeam;
				player.IsActive = playerModel.IsActive;
				player.IsDrafted = (!activateOnly) ? playerModel.IsDrafted : player.IsDrafted;
				player.LastUpdateTimestamp = now;
				HomeEntity.SaveChanges();

				var userId = HomeEntity.Users.GetLoggedInUserId();
				var action = "Admin Edit Player";
				if (activateOnly)
				{
					action = player.IsActive ? "Admin Activate Player" : "Admin Deactivate Player";
				}
				now = Utilities.GetEasternTime();
				var playerAdd = new Entities.PlayerAdjustment
				{
					NewPlayerId = player.PlayerId,
					TruePlayerId = player.TruePlayerId,
					NewFirstName = player.FirstName,
					NewLastName = player.LastName,
					NewPosition = player.Position.ToUpper(),
					NewNFLTeam = player.NFLTeam.ToUpper(),
					Action = action,
                    UserId = userId,
					AddTimestamp = now,
					LastUpdateTimestamp = now
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
