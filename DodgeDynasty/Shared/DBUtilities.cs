using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Account;
using DodgeDynasty.Mappers.Drafts;
using DodgeDynasty.Mappers.Shared;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Account;
using DodgeDynasty.Models.Shared;
using DodgeDynasty.Models.Site;

namespace DodgeDynasty.Shared
{
	public static class DBUtilities
	{
		/// <summary>
		/// Find matching player, checking active players first and then all players
		/// </summary>
		public static Player FindMatchingPlayer(string mode,
			HomeEntity homeEntity, List<Player> activePlayers,
			string firstName, string lastName, string position, string nflTeam,
			out int? inactiveTruePlayerId, out bool justActivated)
		{
			Player player = null;
			inactiveTruePlayerId = null;
			justActivated = false;

			//Check for matching active player
			player = FindMatchingActivePlayer(activePlayers, firstName, lastName, position, nflTeam, player);

			//If player null after active check, then look in Inactive players
			if (player == null)
			{
				player = FindMatchingInactivePlayer(mode, homeEntity, activePlayers, firstName, lastName, position, nflTeam, 
					player, ref inactiveTruePlayerId, ref justActivated);
			}
			return player;
		}

		private static Player FindMatchingActivePlayer(List<Player> activePlayers, 
			string firstName, string lastName, string position, string nflTeam, Player player)
		{
			//Start with all ACTIVE players matching at least FN/LN/Pos
			List<Player> matchingPlayers = activePlayers.Where(
				Utilities.FindPlayerMatch(firstName, lastName, position)).ToList();
			if (matchingPlayers != null && matchingPlayers.Count() > 0)
			{
				//Check if Exact ACTIVE match exists; if so, select
				player = matchingPlayers.FirstOrDefault(
					Utilities.FindPlayerMatch(firstName, lastName, position, nflTeam));
				if (player == null)
				{
					//Else select first close matching ACTIVE player (by LastUpdtTmstmp Desc)
					player = matchingPlayers.OrderByDescending(p => p.LastUpdateTimestamp).FirstOrDefault();
				}
			}
			return player;
		}

		private static Player FindMatchingInactivePlayer(string mode, HomeEntity homeEntity, List<Player> activePlayers,
			string firstName, string lastName, string position, string nflTeam, Player player,
			ref int? inactiveTruePlayerId, ref bool justActivated)
		{
			//If not in active players, check all players matching FN/LN/Pos
			List<Player> matchingPlayers = homeEntity.Players.Where(
				Utilities.FindPlayerMatch(firstName, lastName, position)).ToList();
			if (matchingPlayers != null && matchingPlayers.Count() > 0)
			{
				//Check if Exact Inactive match exists; 
				player = matchingPlayers.FirstOrDefault(
					Utilities.FindPlayerMatch(firstName, lastName, position, nflTeam));
				//if so, check if active TruePlayerId exists and select him
				if (player != null)
				{
					Player activeTruePlayer = activePlayers.FirstOrDefault(
						p => p.TruePlayerId == player.TruePlayerId);
					if (activeTruePlayer != null)
					{
						player = activeTruePlayer;	//Select matching active TruePlayer
					}
					else
					{
						player.IsActive = true;	//Activate exact matching inactive player
						homeEntity.SaveChanges();
						AddPlayerAdjustment(homeEntity, player, mode, "Activate Player");
						justActivated = true;
					}
				}
				else	//if no Exact Inactive, examine first close matching inactive
				{
					player = matchingPlayers.OrderByDescending(p => p.LastUpdateTimestamp).FirstOrDefault();
					Player activeTruePlayer = activePlayers.FirstOrDefault(
						p => p.TruePlayerId == player.TruePlayerId);
					if (activeTruePlayer != null)
					{
						player = activeTruePlayer;	//Select matching active TruePlayer
					}
					else if (player.IsDrafted)
					{
						//if inactive player is drafted, return inactiveTruePlayerId to add new player
						inactiveTruePlayerId = player.TruePlayerId;
						player = null;
					}
					else if (!player.IsDrafted)
					{
						//Update NFL team & Activate close matching inactive player
						player.NFLTeam = nflTeam.ToUpper();
						player.IsActive = true;
						homeEntity.SaveChanges();
						AddPlayerAdjustment(homeEntity, player, mode, "Update NFL Team, Activate");
						justActivated = true;
					}
				}
			}
			return player;
		}

		public static void AddPlayerAdjustment(HomeEntity homeEntity, Player player, string mode, string action)
		{
			var playerAdj = new Entities.PlayerAdjustment
			{
				NewPlayerId = player.PlayerId,
				TruePlayerId = player.TruePlayerId,
				NewFirstName = player.FirstName,
				NewLastName = player.LastName,
				NewPosition = player.Position,
				NewNFLTeam = player.NFLTeam.ToUpper(),
				Action = string.Format("{0} {1}", mode, action),
				UserId = homeEntity.Users.GetLoggedInUserId(),
				AddTimestamp = DateTime.Now,
				LastUpdateTimestamp = DateTime.Now
			};
			homeEntity.PlayerAdjustments.AddObject(playerAdj);
			homeEntity.SaveChanges();
		}

		public static MessagesCountModel GetMessageCountModel()
		{
			var messagesCountMapper = new MessagesCountMapper();
			return messagesCountMapper.GetModel();
		}

		public static DraftChatModel GetCurrentDraftChatModel()
		{
			var draftChatMapper = new DraftChatMapper();
			return draftChatMapper.GetModel();
		}


		/* Access Methods */

		public static RoleAccessModel GetCurrentUserRoleAccess()
		{
			RoleAccessMapper mapper = Factory.Create<RoleAccessMapper>();
			return mapper.GetModel();
		}

		public static List<int> GetCommishLeagueIds()
		{
			var userRoles = Factory.Create<RoleAccessMapper>().GetModel().UserRoles;
			return userRoles.Where(o => o.RoleId == Constants.Roles.Commish && o.LeagueId != null)
				.Select(o => o.LeagueId.Value).ToList();
		}

		public static bool IsUserAdmin(int? userId = null)
		{
			RoleAccessMapper mapper = Factory.Create<RoleAccessMapper>();
			mapper.Model = new RoleAccessModel { UserId = userId };
			return mapper.GetModel().IsUserAdmin;
		}

		public static bool IsUserCommish(int? leagueId = null, int? userId = null)
		{
			RoleAccessMapper mapper = Factory.Create<RoleAccessMapper>();
			mapper.Model = new RoleAccessModel { UserId = userId, LeagueId = leagueId };
			return mapper.GetModel().IsUserCommish;
		}

		public static bool IsUserAdminOrCommish(int? leagueId = null, int? userId = null)
		{
			RoleAccessMapper mapper = Factory.Create<RoleAccessMapper>();
			mapper.Model = new RoleAccessModel { UserId = userId, LeagueId = leagueId };
			var model = mapper.GetModel();
			return model.IsUserAdmin || model.IsUserCommish;
		}

		public static bool IsUserAdminOrCommishForDraft(int? draftId = null)
		{
			SingleDraftMapper draftMapper = Factory.Create<SingleDraftMapper>();
			draftMapper.DraftId = draftId;
			var draftModel = draftMapper.GetModel();
			if (draftModel != null)
			{
				RoleAccessMapper roleAccessMapper = Factory.Create<RoleAccessMapper>();
				roleAccessMapper.Model = new RoleAccessModel { LeagueId = draftModel.LeagueId };
				var model = roleAccessMapper.GetModel();
				return model.IsUserAdmin || model.IsUserCommish;
			}
			return false;
		}
	}
}