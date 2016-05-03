using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Mappers.Commish;
using DodgeDynasty.Mappers.Ranks;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class MapperFactory
	{
		public static PlayerRankOptionsMapper CreatePlayerRankOptionsMapper(string playerRankOptionId)
		{
			return new PlayerRankOptionsMapper(playerRankOptionId);
		}

		public static CommishManageDraftsMapper<ManageDraftsModel> CreateCommishManageDraftsMapper(string leagueId)
		{
			return new CommishManageDraftsMapper<ManageDraftsModel> { LeagueId = leagueId };
		}

		public static CompareRanksMapper CreateCompareRanksMapper(PlayerRankModel playerRankModel)
		{
			return new CompareRanksMapper(playerRankModel); ;
		}
	}
}
