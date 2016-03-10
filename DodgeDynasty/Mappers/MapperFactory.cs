using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Mappers.Ranks;

namespace DodgeDynasty.Mappers
{
	public class MapperFactory
	{
		public static PlayerRankOptionsMapper CreatePlayerRankOptionsMapper(string playerRankOptionId)
		{
			return new PlayerRankOptionsMapper(playerRankOptionId);
		}
	}
}
