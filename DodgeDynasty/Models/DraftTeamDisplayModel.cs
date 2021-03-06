﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class DraftTeamDisplayModel : DraftDisplayModel
	{
		public List<DraftPickPlayer> TeamDraftPicks { get; set; }
		public List<Player> TeamPlayers { get; set; }
		public bool ByPositions { get; set; }

		public Dictionary<string, int> StandardPositionOrder = new Dictionary<string, int>()
		{
			{ "QB", 1 },
			{ "RB", 2 },
			{ "WR", 3 },
			{ "TE", 4 },
			{ "DEF", 5 },
			{ "K", 6 },
			{ "", 7 }
		};

		public Dictionary<string, int> CombineWRTEPositionOrder = new Dictionary<string, int>()
		{
			{ "QB", 1 },
			{ "RB", 2 },
			{ "WR", 3 },
			{ "TE", 3 },
			{ "DEF", 4 },
			{ "K", 5 },
			{ "", 6 }
		};

		public DraftTeamDisplayModel(int? draftId = null, bool byPositions = false) : base(draftId)
		{
			ByPositions = byPositions;
        }

		public List<DraftPickPlayer> GetTeamDraftPicks(int userId)
		{
			var posOrder = CurrentDraft.CombineWRTE ? CombineWRTEPositionOrder : StandardPositionOrder;
			if (ByPositions)
			{
				TeamDraftPicks = DraftPicks.Where(dp => dp.UserId == userId).GroupJoin(DraftedPlayers, tdp => tdp.PlayerId, plyr => plyr.PlayerId,
					(tdp, plyrGrp) => plyrGrp.Select(plyr => new DraftPickPlayer
					{
						DraftPickId = tdp.DraftPickId,
						PickNum = tdp.PickNum,
						PlayerId = tdp.PlayerId,
						Position = plyr.Position
					}).DefaultIfEmpty(new DraftPickPlayer
					{
						DraftPickId = tdp.DraftPickId,
						PickNum = tdp.PickNum,
						PlayerId = null,
						Position = ""
					})).SelectMany(g => g).OrderBy(dp=>dp.Position).OrderBy(dp => posOrder[dp.Position]).ThenBy(dp => dp.PickNum).ToList();
			}
			else
			{
				TeamDraftPicks = DraftPicks.Where(dp => dp.UserId == userId).Select(dp => new DraftPickPlayer
				{
					DraftPickId = dp.DraftPickId,
					PickNum = dp.PickNum,
					PlayerId = dp.PlayerId,
					Position = string.Empty
				}).OrderBy(dp => dp.PickNum).ToList();
			}
			var playerIds = TeamDraftPicks.Select(dp=>dp.PlayerId).ToList();
			TeamPlayers = DraftedPlayers.Where(p => playerIds.Contains(p.PlayerId)).ToList();

			return TeamDraftPicks;
		}

		public List<User> GetTeamDraftOwners()
		{
			return DraftUsers.OrderBy(o => o.UserId == CurrentLoggedInOwnerUser.UserId ? 1 : 2)
				.ThenBy(o => o.NickName).ToList();
		}
	}
}