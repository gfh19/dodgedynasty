using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers;

namespace DodgeDynasty.Models
{
	public class DraftFactory
	{
		public static DraftInputModel GetCurrentDraftInputModel(int? draftId = null)
		{
			DraftInputModel model = new DraftInputModel(draftId);
			return model;
		}

		public static DraftDisplayModel GetDraftDisplayModel(int? draftId = null)
		{
			DraftDisplayModel model = new DraftDisplayModel(draftId);
			return model;
		}

		public static DraftTeamDisplayModel GetDraftTeamDisplayModel(int? draftId = null, bool byPositions = false)
		{
			DraftTeamDisplayModel model = new DraftTeamDisplayModel(draftId, byPositions);
			return model;
		}

		public static PlayerRankModel GetEmptyPlayerRankModel(int? draftId = null)
		{
			PlayerRankModel model = new PlayerRankModel(draftId);
			return model;
		}

		public static PlayerRankModel GetPlayerRankModel(int rankId, int? draftId = null)
		{
			PlayerRankModel model = new PlayerRankModel(rankId, draftId);
			model.SetPlayerRanks(rankId);
			return model;
		}

		public static DraftSetupModel GetDraftSetupModel(int draftId)
		{
			DraftSetupModel model = new DraftSetupModel(draftId);
			return model;
		}

		public static DraftSetupMapper GetDraftSetupMapper()
		{
			DraftSetupMapper mapper = new DraftSetupMapper();
			return mapper;
		}

		//TODO:  Delete?
		public static RankingsListModel GetRankingsListModel(int? draftId = null)
		{
			RankingsListModel model = new RankingsListModel(draftId);
			return model;
		}

		public static RankingsListModel GetRankingsListModel(IDraftModel draftModel)
		{
			RankingsListModel model = new RankingsListModel(draftModel);
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



		public static IDraftModel GetDraftModel(int? draftId = null)
		{
			return new DraftModel(draftId);
		}
	}
}