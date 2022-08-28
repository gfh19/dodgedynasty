using DodgeDynasty.Models.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DodgeDynasty.Mappers.Schedule
{
	public class ScheduleMapper
	{
		public static readonly int _numWeeks = 14;
		public static readonly int _numWeeklyMatchups = 5;

		private static readonly int _maxAttempts = 400;
		private static readonly int _removeVenueRestrictThreshold = 200;
		private static readonly int _maxScheduleRestarts = 200;
		private bool _isScheduleAborted = false;
		private int _recursiveMatchupAttemptCtr = 0;

		public ScheduleModel PopulateModel()
		{
			var schedule = new ScheduleModel();
			schedule.InitializeSchedule(_numWeeklyMatchups);
			return schedule;
		}

		public ScheduleModel GenerateFullSchedule(ScheduleModel schedule)
		{
			_isScheduleAborted = false;
			_recursiveMatchupAttemptCtr = 0;
			schedule.ResetScheduleForRetry();
			var response = generateFullScheduleAttempt(schedule);
			if (_isScheduleAborted)
			{
				var retryCtr = 0;
				while (retryCtr++ < _maxScheduleRestarts && _isScheduleAborted)
				{
					_isScheduleAborted = false;
					_recursiveMatchupAttemptCtr = 0;
					schedule.ResetScheduleForRetry();
					response = generateFullScheduleAttempt(schedule);
				}
			}
			response.Success = !_isScheduleAborted;
			return response;
		}

		private ScheduleModel generateFullScheduleAttempt(ScheduleModel schedule)
		{
			schedule.Submitted = true;
			schedule.DivisionMatchups.ForEach(m => m.AwayTeam.Division = 1);
			schedule.DivisionMatchups.ForEach(m => m.HomeTeam.Division = 2);
			schedule.SetAllTeams();

			var currentWeekTeams = new List<SchedTeam>();

			//Week 14
			currentWeekTeams = new List<SchedTeam>();
			currentWeekTeams.AddRange(schedule.AllTeams);
			var finalWeekMatchups = new List<Matchup>();
			finalWeekMatchups.AddRange(schedule.FinalWeekRivalries?.Where(m => m != null && !m.IsEmpty()));
			var lastWeekSchedule = new WeekSchedule(_numWeeks);

			for (int matchupIx = 0; matchupIx < _numWeeklyMatchups; matchupIx++)
			{
				var lastWeekMatchup = new Matchup(_numWeeks);
				if (finalWeekMatchups.Count > 0 && !finalWeekMatchups.Any(m => m == null || m.IsEmpty()))
				{
					var selectedMatchup = finalWeekMatchups.First();
					lastWeekMatchup.AwayTeam = schedule.GetTeam(selectedMatchup.AwayTeam);
					lastWeekMatchup.HomeTeam = schedule.GetTeam(selectedMatchup.HomeTeam);
					currentWeekTeams.RemoveAll(isTeam(selectedMatchup.AwayTeam));
					currentWeekTeams.RemoveAll(isTeam(selectedMatchup.HomeTeam));
					finalWeekMatchups.RemoveAll(isMatchup(selectedMatchup));
					lastWeekSchedule.Matchups.Add(lastWeekMatchup);
					lastWeekMatchup.AwayTeam.FinalWeekMatchup = lastWeekMatchup;
					lastWeekMatchup.HomeTeam.FinalWeekMatchup = lastWeekMatchup;
				}
				else
				{
					//Can't call GenerateMatchup for Week 14 cuz it increments team counters!
					//var matchup = GenerateMatchup(_numWeeks, currentWeekTeams, schedule);
					var matchup = new Matchup(_numWeeks);
					var rand = new Random();
					matchup.AwayTeam = currentWeekTeams.ElementAt(rand.Next(0, currentWeekTeams.Count));
					currentWeekTeams.RemoveAll(isTeam(matchup.AwayTeam));
					matchup.HomeTeam = currentWeekTeams.ElementAt(rand.Next(0, currentWeekTeams.Count));
					currentWeekTeams.RemoveAll(isTeam(matchup.HomeTeam));
					lastWeekSchedule.Matchups.Add(matchup);
					matchup.AwayTeam.FinalWeekMatchup = matchup;
					matchup.HomeTeam.FinalWeekMatchup = matchup;
				}
			}

			//Week 1
			currentWeekTeams.AddRange(schedule.AllTeams);
			var firstWeekSchedule = new WeekSchedule(1);

			if (schedule.Week1TitleRematch != null && !schedule.Week1TitleRematch.IsEmpty())
			{
				var firstWeekMatchup = new Matchup(1);

				firstWeekMatchup.AwayTeam = schedule.GetTeam(schedule.Week1TitleRematch.AwayTeam);
				firstWeekMatchup.HomeTeam = schedule.GetTeam(schedule.Week1TitleRematch.HomeTeam);
				currentWeekTeams.RemoveAll(isTeam(firstWeekMatchup.AwayTeam));
				currentWeekTeams.RemoveAll(isTeam(firstWeekMatchup.HomeTeam));
				updateAwayTeamMatchup(firstWeekMatchup.AwayTeam, firstWeekMatchup);
				updateHomeTeamMatchup(firstWeekMatchup.HomeTeam, firstWeekMatchup);
				firstWeekSchedule.Matchups.Add(firstWeekMatchup);
			}
			while (currentWeekTeams.Any())
			{
				firstWeekSchedule.Matchups.Add(GenerateMatchup(1, currentWeekTeams, schedule));
			}
			schedule.FullSchedule.Add(firstWeekSchedule);

			for (int weekNum = 2; weekNum < _numWeeks; weekNum++)
			{
				//TODO: Primer logic?
				currentWeekTeams = new List<SchedTeam>();
				currentWeekTeams.AddRange(schedule.AllTeams);
				var currentWeekSchedule = new WeekSchedule(weekNum);
				_recursiveMatchupAttemptCtr = 0;

				var ineligibleWeeklyMatchups = new List<Matchup>();
				generateWeeklySchedule(schedule, currentWeekTeams, weekNum, currentWeekSchedule, ineligibleWeeklyMatchups);
				schedule.FullSchedule.Add(currentWeekSchedule);
				ineligibleWeeklyMatchups.Clear();
			}

			schedule.FullSchedule.Add(lastWeekSchedule);
			if (schedule.FinalWeekRivalries.Count > 0 && !schedule.FinalWeekRivalries.Any(m => m == null || m.IsEmpty()))
			{
				lastWeekSchedule.Matchups.ForEach(m =>
				{
					updateAwayTeamMatchup(m.AwayTeam, m);
					updateHomeTeamMatchup(m.HomeTeam, m);
				});
			}

			return schedule;
		}

		private void generateWeeklySchedule(ScheduleModel schedule, List<SchedTeam> currentWeekTeams, int weekNum, WeekSchedule currentWeekSchedule, List<Matchup> ineligibleWeeklyMatchups)
		{
			_recursiveMatchupAttemptCtr++;
			var restart = false;
			for (int matchupIx = 0; matchupIx < _numWeeklyMatchups; matchupIx++)
			{
				var matchup = GenerateMatchup(weekNum, currentWeekTeams, schedule, ineligibleWeeklyMatchups, _recursiveMatchupAttemptCtr);
				currentWeekSchedule.Matchups.Add(matchup);
				var lastMatchup = currentWeekSchedule.Matchups.LastOrDefault();
				if (lastMatchup == null || lastMatchup.HasNullMatchup())
				{
					currentWeekSchedule.Matchups.Remove(lastMatchup);
					restart = true;
					break;
				}
			}
			//If null abandon week & recursively restart
			if (restart && _recursiveMatchupAttemptCtr < _maxAttempts && !_isScheduleAborted)
			{
				foreach (var matchup in currentWeekSchedule.Matchups.Where(m => m != null))
				{
					removeAwayTeamMatchup(matchup.AwayTeam, matchup);
					removeHomeTeamMatchup(matchup.HomeTeam, matchup);
				}
				currentWeekSchedule.Matchups.Clear();
				currentWeekTeams.Clear();
				currentWeekTeams.AddRange(schedule.AllTeams);
				generateWeeklySchedule(schedule, currentWeekTeams, weekNum, currentWeekSchedule, ineligibleWeeklyMatchups);
			}
			else if (_recursiveMatchupAttemptCtr >= _maxAttempts)
			{
				_isScheduleAborted = true;
			}
		}

		private static Predicate<Matchup> isMatchup(Matchup matchup)
		{
			return m => m.AwayTeam.Name == matchup.AwayTeam.Name && m.HomeTeam.Name == matchup.HomeTeam.Name;
		}

		private static Predicate<SchedTeam> isTeam(SchedTeam team)
		{
			return t => t.Name == team?.Name;
		}

		private Matchup GenerateMatchup(int weekNum, List<SchedTeam> currentWeekTeams, ScheduleModel schedule, List<Matchup> ineligibleWeeklyMatchups = null, int? recCtr=null)
		{
			ineligibleWeeklyMatchups = ineligibleWeeklyMatchups ?? new List<Matchup>();
			var matchup = new Matchup(weekNum);
			var maxRandomAttempts = _maxAttempts;
			var eligibleAwayTeams = currentWeekTeams.Where(t => !isIneligibleAwayTeam(t)).ToList();
			var eligibleHomeTeams = currentWeekTeams.Where(t => !isIneligibleHomeTeam(t)).ToList();

			while (maxRandomAttempts-- > 0 && eligibleAwayTeams.Count > 0 && eligibleHomeTeams.Count > 0)
			{
				var rand = new Random(DateTime.Now.Millisecond - maxRandomAttempts + weekNum);
				var selectedAwayTeam = eligibleAwayTeams.ElementAt(rand.Next(0, eligibleAwayTeams.Count));
				if (isIneligibleAwayTeam(selectedAwayTeam))
				{
					continue;
				}
				matchup.AwayTeam = schedule.GetTeam(selectedAwayTeam);
				currentWeekTeams.RemoveAll(isTeam(selectedAwayTeam));
				eligibleAwayTeams.RemoveAll(isTeam(selectedAwayTeam));

				SchedTeam selectedHomeTeam = null;
				var fullyEligibleHomeTeams = eligibleHomeTeams.Where(t => !t.Equals(selectedAwayTeam) && !ineligibleWeeklyMatchups.Any(m => m.ContainsBothTeams(selectedAwayTeam, t))).ToList();
				if (fullyEligibleHomeTeams.Count > 0)
				{
					selectedHomeTeam = fullyEligibleHomeTeams.ElementAt(rand.Next(0, fullyEligibleHomeTeams.Count));
				}
				else
				{
					matchup = null;
					if (_recursiveMatchupAttemptCtr == _removeVenueRestrictThreshold)
					{
						ineligibleWeeklyMatchups.Clear();
					}
					break;
				}
				while (selectedHomeTeam != null && isIneligibleMatchup(weekNum, schedule, selectedAwayTeam, selectedHomeTeam) && currentWeekTeams.Count > 1 && maxRandomAttempts-- > 0)
				{
					if (!ineligibleWeeklyMatchups.Any(m => m.ContainsBothTeams(selectedAwayTeam, selectedHomeTeam)))
					{
						ineligibleWeeklyMatchups.Add(new Matchup(selectedAwayTeam.Name, selectedHomeTeam.Name));
					}
					selectedHomeTeam = currentWeekTeams.ElementAt(rand.Next(0, currentWeekTeams.Count));
				}
				matchup.HomeTeam = schedule.GetTeam(selectedHomeTeam);
				if (isIneligibleMatchup(weekNum, schedule, selectedAwayTeam, selectedHomeTeam) && (currentWeekTeams.Count <= 1 || maxRandomAttempts <= 0))
				{
					matchup = null;
					break;
				}
				currentWeekTeams.RemoveAll(isTeam(selectedHomeTeam));
				eligibleHomeTeams.RemoveAll(isTeam(selectedHomeTeam));

				updateAwayTeamMatchup(matchup.AwayTeam, matchup);
				updateHomeTeamMatchup(matchup.HomeTeam, matchup);

				break;
			}
			//If still null, remaining teams >2 still couldn't find valid matchups; restart week
			if (matchup != null && matchup.HasNullMatchup())
			{
				matchup = null;
			}
			return matchup;
		}

		private bool isIneligibleMatchup(int weekNum, ScheduleModel schedule, SchedTeam selectedAwayTeam, SchedTeam selectedHomeTeam)
		{
			return isIneligibleAwayTeam(selectedAwayTeam)
				|| (isIneligibleHomeTeam(selectedHomeTeam))
				|| didTeamsJustMatchup(selectedAwayTeam, selectedHomeTeam)
				|| isMaxedOutMatchups(selectedAwayTeam, selectedHomeTeam, schedule)
				|| onlyTwoMatchupsLeftAreTheSame(weekNum, selectedAwayTeam, selectedHomeTeam, schedule);
		}

		private bool isIneligibleHomeTeam(SchedTeam team)
		{
			if (_recursiveMatchupAttemptCtr > _removeVenueRestrictThreshold)
			{
				return false;
			}
			return team.HomeCtr >= (_numWeeks / 2) + 1 || team.HomeStreak >= 3;
		}

		private bool isIneligibleAwayTeam(SchedTeam team)
		{
			if (_recursiveMatchupAttemptCtr > _removeVenueRestrictThreshold)
			{
				return false;
			}
			return team.AwayCtr >= (_numWeeks / 2) + 1 || team.AwayStreak >= 3;
		}

		private static bool onlyTwoMatchupsLeftAreTheSame(int weekNum, SchedTeam awayTeam, SchedTeam homeTeam, ScheduleModel schedule)
		{
			var invalid = false;
			if (weekNum == _numWeeks - 3 || weekNum == _numWeeks - 2)
			{
				var awayDoubleOpponents = new List<SchedTeam>();
				awayDoubleOpponents.AddRange(schedule.AllTeams.Where(t => t.Division == awayTeam.Division));
				awayDoubleOpponents.AddRange(schedule.DivisionMatchups.Where(m => m.AwayTeam.Equals(awayTeam)).Select(m => m.HomeTeam));

				var homeDoubleOpponents = new List<SchedTeam>();
				homeDoubleOpponents.AddRange(schedule.AllTeams.Where(t => t.Division == homeTeam.Division));
				homeDoubleOpponents.AddRange(schedule.DivisionMatchups.Where(m => m.HomeTeam.Equals(homeTeam)).Select(m => m.AwayTeam));

				//If someone to play twice hasn't been played yet at all (and isn't current selected opponent, or final week opponent), invalid
				foreach (var awayOpp in awayDoubleOpponents)
				{
					if (awayTeam.TeamSchedule.Count(m => m.ContainsTeam(awayOpp)) == 0 && !awayOpp.Equals(homeTeam)
						&& (weekNum == _numWeeks - 2 || !awayTeam.FinalWeekMatchup.ContainsTeam(awayOpp)))
					{
						invalid = true;
						break;
					}
				}
				foreach (var homeOpp in homeDoubleOpponents)
				{
					if (homeTeam.TeamSchedule.Count(m => m.ContainsTeam(homeOpp)) == 0 && !homeOpp.Equals(awayTeam)
						&& (weekNum == _numWeeks - 2 || !homeTeam.FinalWeekMatchup.ContainsTeam(homeOpp)))
					{
						invalid = true;
						break;
					}
				}
			}
			return invalid;
		}

		private static bool didTeamsJustMatchup(SchedTeam awayTeam, SchedTeam homeTeam)
		{
			var awayLastMatchup = awayTeam.TeamSchedule.LastOrDefault();
			var homeLastMatchup = homeTeam.TeamSchedule.LastOrDefault();
			return (awayLastMatchup?.ContainsTeam(homeTeam) ?? false) || (homeLastMatchup?.ContainsTeam(awayTeam) ?? false);
		}

		private static bool isMaxedOutMatchups(SchedTeam team1, SchedTeam team2, ScheduleModel schedule)
		{
			var maxMatchups = 1;
			if (team1.Division == team2.Division || schedule.DivisionMatchups.Any(dm => dm.ContainsBothTeams(team1, team2)))
			{
				maxMatchups = 2;
			}
			var team1Count = team1.TeamSchedule.Count(m => m.ContainsTeam(team2));
			if (team1.FinalWeekMatchup?.ContainsTeam(team2) ?? false)
			{
				team1Count++;
			}

			var team2Count = team2.TeamSchedule.Count(m => m.ContainsTeam(team1));
			if (team2.FinalWeekMatchup?.ContainsTeam(team1) ?? false)
			{
				team2Count++;
			}

			return team1Count >= maxMatchups || team2Count >= maxMatchups;
		}

		private static void updateAwayTeamMatchup(SchedTeam team, Matchup matchup)
		{
			team.AwayCtr++;
			team.AwayStreak++;
			team.HomeStreak = 0;
			team.TeamSchedule.Add(matchup);
		}

		private static void updateHomeTeamMatchup(SchedTeam team, Matchup matchup)
		{
			team.HomeCtr++;
			team.HomeStreak++;
			team.AwayStreak = 0;
			team.TeamSchedule.Add(matchup);
		}

		private static void removeAwayTeamMatchup(SchedTeam team, Matchup matchup)
		{
			team.AwayCtr--;
			team.AwayStreak--;
			for (int i = team.TeamSchedule.Count - 1; i >= team.TeamSchedule.Count - 4; i--)
			{
				if (team.TeamSchedule[i].HomeTeam.Equals(team))
				{
					team.HomeStreak++;
				}
				else
				{
					break;
				}
			}
			team.TeamSchedule.Remove(matchup);
		}

		private static void removeHomeTeamMatchup(SchedTeam team, Matchup matchup)
		{
			team.HomeCtr--;
			team.HomeStreak--;
			for (int i=team.TeamSchedule.Count-1; i>= team.TeamSchedule.Count - 4; i--)
			{
				if (team.TeamSchedule[i].AwayTeam.Equals(team))
				{
					team.AwayStreak++;
				}
				else
				{
					break;
				}
			}
			team.TeamSchedule.Remove(matchup);
		}
	}
}