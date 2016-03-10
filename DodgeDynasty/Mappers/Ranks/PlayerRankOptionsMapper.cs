using System;
using System.Linq;
using DodgeDynasty.Models.Types;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Ranks
{
	//TBD:  Probably should've taken the opportunity to call this "UserOptions"...
	public class PlayerRankOptionsMapper : MapperBase<PlayerRankOptions>
	{
		public string PlayerRankOptionId { get; set; }
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
				ExpandDEF = options.ExpandDEF,
				ExpandK = options.ExpandK,
				ExpandQueue = options.ExpandQueue,
				HideOverall = options.HideOverall,
				HideQB = options.HideQB,
				HideRB = options.HideRB,
				HideWRTE = options.HideWRTE,
				HideDEF = options.HideDEF,
				HideK = options.HideK,
				HideQueue = options.HideQueue,
				ShowHighlighting = options.ShowHighlighting,
				LockHighlighting = options.LockHighlighting,
				HighlightColor = options.HighlightColor
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
			var playerRankOptionGuid = new Guid(PlayerRankOptionId);
			if (HomeEntity.PlayerRankOptions.Any(o => o.PlayerRankOptionId == playerRankOptionGuid))
			{
				UpdatedPlayerRankOptionId = PlayerRankOptionId = Guid.NewGuid().ToString();
			}
			Entities.PlayerRankOption options = new Entities.PlayerRankOption();
			options.PlayerRankOptionId = new Guid(PlayerRankOptionId);
			options.UserId = userId;
			options.AddTimestamp = DateTime.Now;
			HomeEntity.PlayerRankOptions.AddObject(SetPlayerRankOptions(options, model));
			return options;
		}

		private Entities.PlayerRankOption SetPlayerRankOptions(Entities.PlayerRankOption options, PlayerRankOptions model)
		{
			options.RankId = model.RankId.ToNullableInt32();
			options.DraftId = model.DraftId.ToNullableInt32();
			options.ExpandOverall = model.ExpandOverall;
			options.ExpandQB = model.ExpandQB;
			options.ExpandRB = model.ExpandRB;
			options.ExpandWRTE = model.ExpandWRTE;
			options.ExpandDEF = model.ExpandDEF;
			options.ExpandK = model.ExpandK;
			options.ExpandQueue = model.ExpandQueue;
			options.HideOverall = model.HideOverall;
			options.HideQB = model.HideQB;
			options.HideRB = model.HideRB;
			options.HideWRTE = model.HideWRTE;
			options.HideDEF = model.HideDEF;
			options.HideK = model.HideK;
			options.HideQueue = model.HideQueue;
			options.ShowHighlighting = model.ShowHighlighting;
			options.LockHighlighting = model.LockHighlighting;
			options.HighlightColor = GetSafeHighlightColor(model.HighlightColor);
			options.LastUpdateTimestamp = DateTime.Now;
			return options;
        }

		private string GetSafeHighlightColor(string highlightColor) {
			return HomeEntity.Highlights.Any(o => o.HighlightClass == highlightColor) ? highlightColor : null;
		}
	}
}
