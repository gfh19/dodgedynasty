using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Mappers.Shared
{
	public class PlayerSeasonHelper
	{
		//public static int GetOrCreateSeason(HomeEntity homeEntity, short draftYear)
		//{
		//	var season = homeEntity.Seasons.Where(s => s.SeasonYear == draftYear).FirstOrDefault();
		//	if (season == null)
		//	{
		//		season = new Season
		//		{
		//			SeasonYear = draftYear,
		//			AddTimestamp = DateTime.Now,
		//			LastUpdateTimestamp = DateTime.Now
		//		};
		//		homeEntity.Seasons.AddObject(season);
		//		homeEntity.SaveChanges();
		//	}
		//	return season.SeasonId;
		//}

		//public static void AddPlayerSeason(HomeEntity homeEntity, int playerId, int seasonId) {
		//	homeEntity.PlayerSeasons.AddObject(new PlayerSeason
		//	{
		//		PlayerId = playerId,
		//		SeasonId = seasonId,
		//		AddTimestamp = DateTime.Now,
		//		LastUpdateTimestamp = DateTime.Now
		//	});
		//	homeEntity.SaveChanges();
		//}
	}
}