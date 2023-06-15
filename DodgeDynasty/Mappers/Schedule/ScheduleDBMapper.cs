using System;
using System.Data.SqlClient;
using System.Linq;
using DodgeDynasty.Models.Audio;
using DodgeDynasty.Models.Schedule;
using DodgeDynasty.Shared;
using WebGrease.Css.Extensions;

namespace DodgeDynasty.Mappers.Schedule
{
	public class ScheduleDBMapper : MapperBase<ScheduleModel>
	{
		protected override void DoUpdate(ScheduleModel model)
		{
			var userId = HomeEntity.Users.GetLoggedInUserId();

			HomeEntity.ExecuteStoreCommand($"DELETE FROM dbo.[ScheduleMatchup] WHERE [UserId]=@UserId AND [Year]=@Year",
				new SqlParameter("@UserId", userId), new SqlParameter("@Year", DateTime.Now.Year));

			HomeEntity.ExecuteStoreCommand($"DELETE FROM dbo.[Schedule] WHERE [UserId]=@UserId AND [Year]=@Year",
				new SqlParameter("@UserId", userId), new SqlParameter("@Year", DateTime.Now.Year));

			model.AddTimestamp = Utilities.GetEasternTime();

			var newSchedule = new Entities.Schedule
			{
				Year = DateTime.Now.Year,
				UserId = userId,
				AddTimestamp = model.AddTimestamp,
				LastUpdateTimestamp = model.AddTimestamp
			};
			HomeEntity.Schedules.AddObject(newSchedule);
			HomeEntity.SaveChanges();

			model.DivisionMatchups.Where(m => !m.HasNullMatchup()).ForEach(m =>
			{
				HomeEntity.ScheduleMatchups.AddObject(new Entities.ScheduleMatchup
				{
					ScheduleId = newSchedule.ScheduleId,
					Year = DateTime.Now.Year,
					UserId = userId,
					MatchupType = Constants.MatchupTypes.InputDivision,
					AwayTeam = m.AwayTeam.Name,
					HomeTeam = m.HomeTeam.Name,
					AddTimestamp = model.AddTimestamp,
					LastUpdateTimestamp = model.AddTimestamp
				});
			});

			model.FinalWeekRivalries.Where(m=>!m.HasNullMatchup()).ForEach(m =>
			{
				HomeEntity.ScheduleMatchups.AddObject(new Entities.ScheduleMatchup
				{
					ScheduleId = newSchedule.ScheduleId,
					Year = DateTime.Now.Year,
					UserId = userId,
					MatchupType = Constants.MatchupTypes.InputFinalWeek,
					AwayTeam = m.AwayTeam.Name,
					HomeTeam = m.HomeTeam.Name,
					AddTimestamp = model.AddTimestamp,
					LastUpdateTimestamp = model.AddTimestamp
				});
			});

			if (!model.Week1TitleRematch.HasNullMatchup())
			{
				HomeEntity.ScheduleMatchups.AddObject(new Entities.ScheduleMatchup
				{
					ScheduleId = newSchedule.ScheduleId,
					Year = DateTime.Now.Year,
					UserId = userId,
					MatchupType = Constants.MatchupTypes.InputWeek1,
					AwayTeam = model.Week1TitleRematch.AwayTeam.Name,
					HomeTeam = model.Week1TitleRematch.HomeTeam.Name,
					AddTimestamp = model.AddTimestamp,
					LastUpdateTimestamp = model.AddTimestamp
				});
			}

			model.FullSchedule.ForEach(ws => ws.Matchups.ForEach(m =>
			{
				HomeEntity.ScheduleMatchups.AddObject(new Entities.ScheduleMatchup
				{
					ScheduleId = newSchedule.ScheduleId,
					Year = DateTime.Now.Year,
					UserId = userId,
					MatchupType = Constants.MatchupTypes.Results,
					AwayTeam = m.AwayTeam.Name,
					HomeTeam = m.HomeTeam.Name,
					Week = m.WeekNum,
					AddTimestamp = model.AddTimestamp,
					LastUpdateTimestamp = model.AddTimestamp
				});
			}));
			HomeEntity.SaveChanges();
		}

		protected override void PopulateModel()
		{
			var userId = HomeEntity.Users.GetLoggedInUserId();
			var schedule = HomeEntity.Schedules.FirstOrDefault(s => s.UserId == userId && s.Year == DateTime.Now.Year);
			if (schedule != null)
			{
				Model.AddTimestamp = schedule.AddTimestamp;
				var matchups = HomeEntity.ScheduleMatchups.Where(sm => sm.ScheduleId == schedule.ScheduleId).ToList();
				
				if (matchups.Any())
				{
					matchups.Where(m=>m.MatchupType == Constants.MatchupTypes.InputDivision).ForEach(m =>
					{
						var matchup = Model.DivisionMatchups.FirstOrDefault(mm => mm != null && mm.HasNullMatchup());
						matchup.AwayTeam.Name = m.AwayTeam;
						matchup.HomeTeam.Name = m.HomeTeam;
					});
					matchups.Where(m => m.MatchupType == Constants.MatchupTypes.InputFinalWeek).ForEach(m =>
					{
						var matchup = Model.FinalWeekRivalries.FirstOrDefault(mm => mm != null && mm.HasNullMatchup());
						matchup.AwayTeam.Name = m.AwayTeam;
						matchup.HomeTeam.Name = m.HomeTeam;
					});
					matchups.Where(m => m.MatchupType == Constants.MatchupTypes.InputWeek1).ForEach(m =>
					{
						Model.Week1TitleRematch.AwayTeam.Name = m.AwayTeam;
						Model.Week1TitleRematch.HomeTeam.Name = m.HomeTeam;
					});
					
					var results = matchups.Where(m => m.MatchupType == Constants.MatchupTypes.Results).ToList();
					if (results.Any())
					{
						for (int i = (int)results.Min(r => r.Week); i <= results.Max(r => r.Week); i++)
						{
							var weekSchedule = new WeekSchedule(i);
							results.Where(r => r.Week == i).ForEach(r =>
							{
								weekSchedule.Matchups.Add(new Matchup(i, r.AwayTeam, r.HomeTeam));
							});
							Model.FullSchedule.Add(weekSchedule);
						}
					}
				}
			}
		}
	}
}
