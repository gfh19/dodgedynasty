using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models
{
	public class RankSetupModel : RankIdModel
	{
		public string RankName { get; set; }
		public short DraftYear { get; set; }
		public int OwnerId { get; set; }
		public List<RankedPlayer> RankedPlayers { get; set; }
		public PlayerModel Player { get; set; }

		public RankSetupModel()
		{ }

		public RankSetupModel(int rankId)
		{
			RankId = rankId;
			base.GetCurrentDraft();
		}
	}
}