using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers;

namespace DodgeDynasty.Models
{
	public class DraftFactory
	{
		public static DraftInputModel GetCurrentDraftInputModel()
		{
			DraftInputModel model = new DraftInputModel();
			return model;
		}

		public static DraftDisplayModel GetDraftDisplayModel()
		{
			DraftDisplayModel model = new DraftDisplayModel();
			return model;
		}

		public static DraftTeamDisplayModel GetDraftTeamDisplayModel()
		{
			DraftTeamDisplayModel model = new DraftTeamDisplayModel();
			return model;
		}

		public static PlayerRankModel GetPlayerRankModel(int rankId)
		{
			PlayerRankModel model = new PlayerRankModel(rankId);
			return model;
		}

		public static DraftSetupModel GetDraftSetupModel()
		{
			DraftSetupModel model = new DraftSetupModel();
			return model;
		}

		public static DraftSetupMapper GetDraftSetupMapper()
		{
			DraftSetupMapper mapper = new DraftSetupMapper();
			return mapper;
		}

		public static RankingsListModel GetRankingsListModel()
		{
			RankingsListModel model = new RankingsListModel();
			return model;
		}

		public static RankSetupModel GetRankSetupModel()
		{
			RankSetupModel model = new RankSetupModel();
			return model;
		}

		public static RankSetupModel GetRankSetupModel(int rankId)
		{
			RankSetupModel model = new RankSetupModel(rankId);
			return model;
		}

		public static RankSetupMapper GetRankSetupMapper()
		{
			RankSetupMapper mapper = new RankSetupMapper();
			return mapper;
		}
	}
}