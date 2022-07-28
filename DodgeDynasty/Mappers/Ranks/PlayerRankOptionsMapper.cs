using System;
using System.Linq;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Ranks
{
	//TBD:  Probably should've taken the opportunity to call this "UserOptions"...
	public class PlayerRankOptionsMapper : MapperBase<PlayerRankOptions>
	{
		private string PlayerRankOptionId { get; set; }
		public string UpdatedPlayerRankOptionId { get; set; }

		public PlayerRankOptionsMapper(string playerRankOptionId)
		{
			PlayerRankOptionId = playerRankOptionId;
		}

		protected override void PopulateModel()
		{
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var playerRankOptionGuid = new Guid(PlayerRankOptionId);
			var options = HomeEntity.PlayerRankOptions.FirstOrDefault(o => o.PlayerRankOptionId == playerRankOptionGuid && o.UserId == userId);
			if (options == null)
			{
				options = AddNewPlayerRankOptions(Model, userId);
			}
			Model = new PlayerRankOptions
			{
				Id = PlayerRankOptionId,
				RankId = options.RankId.ToStringFromNullInt(),
				DraftId = options.DraftId.ToStringFromNullInt(),
				ExpandOverall = options.ExpandOverall,
				ExpandQB = options.ExpandQB,
				ExpandRB = options.ExpandRB,
				ExpandWRTE = options.ExpandWRTE,
				ExpandWR = options.ExpandWR,
				ExpandTE = options.ExpandTE,
				ExpandDEF = options.ExpandDEF,
				ExpandK = options.ExpandK,
				ExpandQueue = options.ExpandQueue,
				ExpandAvg = options.ExpandAvg,
				HideOverall = options.HideOverall,
				HideQB = options.HideQB,
				HideRB = options.HideRB,
				HideWRTE = options.HideWRTE,
				HideWR = options.HideWR,
				HideTE = options.HideTE,
				HideDEF = options.HideDEF,
				HideK = options.HideK,
				HideQueue = options.HideQueue,
				HideAvg = options.HideAvg,
                ShowHighlighting = options.ShowHighlighting,
				LockHighlighting = options.LockHighlighting,
				DraftHighlightId = options.DraftHighlightId.ToStringFromNullInt(),
				HighlightColor = options.HighlightColor,
				IsComparingRanks = options.IsComparingRanks,
				CompareRankIds = options.CompareRankIds,
				CompRankExpandIds = options.CompRankExpandIds,
				CompRanksExpandAll = options.CompRanksExpandAll,
				ShowAvgCompRanks = options.ShowAvgCompRanks,
				ExpandBUP = options.ExpandBUP,
				HideBUP = options.HideBUP,
				BUPId = options.BUPId.ToStringFromNullInt()
			};
		}

		protected override void DoUpdate(PlayerRankOptions model)
		{
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var playerRankOptionGuid = new Guid(PlayerRankOptionId);
			var options = HomeEntity.PlayerRankOptions.FirstOrDefault(o => o.PlayerRankOptionId == playerRankOptionGuid && o.UserId == userId);
			if (options == null)
			{
				AddNewPlayerRankOptions(model, userId);
			}
			else
			{
				SetPlayerRankOptions(options, model);
            }
			HomeEntity.SaveChanges();
		}

		private Entities.PlayerRankOption AddNewPlayerRankOptions(PlayerRankOptions model, int userId)
		{
			var now = Utilities.GetEasternTime();
			var playerRankOptionGuid = new Guid(PlayerRankOptionId);
			//If optionId exists in DB for a different user (i.e. new user logged into same browser)
			if (HomeEntity.PlayerRankOptions.Any(o => o.PlayerRankOptionId == playerRankOptionGuid))
			{
				UpdatedPlayerRankOptionId = PlayerRankOptionId = Guid.NewGuid().ToString();
			}
			Entities.PlayerRankOption options = new Entities.PlayerRankOption();
			options.PlayerRankOptionId = new Guid(PlayerRankOptionId);
			options.UserId = userId;
			options.AddTimestamp = now;
			HomeEntity.PlayerRankOptions.AddObject(SetPlayerRankOptions(options, model));
			return options;
		}

		private Entities.PlayerRankOption SetPlayerRankOptions(Entities.PlayerRankOption options, PlayerRankOptions model)
		{
			var now = Utilities.GetEasternTime();
			options.RankId = model.RankId.ToNullInt();
			options.DraftId = model.DraftId.ToNullInt();
			options.ExpandOverall = model.ExpandOverall;
			options.ExpandQB = model.ExpandQB;
			options.ExpandRB = model.ExpandRB;
			options.ExpandWRTE = model.ExpandWRTE;
			options.ExpandWR = model.ExpandWR;
			options.ExpandTE = model.ExpandTE;
			options.ExpandDEF = model.ExpandDEF;
			options.ExpandK = model.ExpandK;
			options.ExpandQueue = model.ExpandQueue;
			options.ExpandAvg = model.ExpandAvg;
			options.HideOverall = model.HideOverall;
			options.HideQB = model.HideQB;
			options.HideRB = model.HideRB;
			options.HideWRTE = model.HideWRTE;
			options.HideWR = model.HideWR;
			options.HideTE = model.HideTE;
			options.HideDEF = model.HideDEF;
			options.HideK = model.HideK;
			options.HideQueue = model.HideQueue;
			options.HideAvg = model.HideAvg;
			options.ShowHighlighting = model.ShowHighlighting;
			options.LockHighlighting = model.LockHighlighting;
			options.DraftHighlightId = model.DraftHighlightId.ToNullInt();
			options.HighlightColor = GetSafeHighlightColor(model.HighlightColor);
			options.IsComparingRanks = model.IsComparingRanks;
			options.CompareRankIds = model.CompareRankIds;
			options.CompRankExpandIds = model.CompRankExpandIds;
			options.CompRanksExpandAll = model.CompRanksExpandAll;
			options.ShowAvgCompRanks = model.ShowAvgCompRanks;
			options.ExpandBUP = model.ExpandBUP;
			options.HideBUP = model.HideBUP;
			options.BUPId = model.BUPId.ToNullInt();
			options.LastUpdateTimestamp = now;
			return options;
        }

		private string GetSafeHighlightColor(string highlightColor) {
			return (highlightColor == Constants.JS.RemoveColor 
				|| HomeEntity.Highlights.Any(o => o.HighlightClass == highlightColor)) ? highlightColor : null;
		}
	}
}
