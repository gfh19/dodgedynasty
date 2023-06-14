using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DodgeDynasty.Models.Schedule
{
	public class ScheduleModel : ModelBase
	{
		[Display(Name = "Division Matchups")]
		[RequiredCompleteMatchupsList]
		public List<Matchup> DivisionMatchups { get; set; }

		[Display(Name = "Final Week Rivalries")]
		[OptionalCompleteMatchupsList("DivisionMatchups")]
		public List<Matchup> FinalWeekRivalries { get; set; }

		[Display(Name = "Week 1 Title Rematch")]
		[OptionalCompleteSingleMatchup("DivisionMatchups")]
		public Matchup Week1TitleRematch { get; set; }

		public List<WeekSchedule> FullSchedule { get; set; }
		public List<SchedTeam> AllTeams { get; set; }
		public bool Submitted { get; set; }
		public bool Success { get; set; }
		public DateTime AddTimestamp { get; set; }
		public void InitializeSchedule(int numWeeklyMatchups)
		{
			DivisionMatchups = new Matchup[numWeeklyMatchups].ToList();
			for (int i = 0; i < DivisionMatchups.Count; i++)
			{
				DivisionMatchups[i] = new Matchup();
			}
			FinalWeekRivalries = new Matchup[numWeeklyMatchups].ToList();
			for (int i = 0; i < FinalWeekRivalries.Count; i++)
			{
				FinalWeekRivalries[i] = new Matchup();
			}
			Week1TitleRematch = new Matchup();
			FullSchedule = new List<WeekSchedule>();
		}

		public void SetAllTeams()
		{
			AllTeams = new List<SchedTeam>();
			AllTeams.AddRange(DivisionMatchups.Select(m => m.AwayTeam));
			AllTeams.AddRange(DivisionMatchups.Select(m => m.HomeTeam));
		}

		public SchedTeam GetTeam(SchedTeam team)
		{
			return AllTeams.FirstOrDefault(t => t.Equals(team));
		}
		public void ResetScheduleForRetry()
		{
			FullSchedule = new List<WeekSchedule>();
			SetAllTeams();
			AllTeams.ForEach(t => t.ResetTeam());
		}
	}

	public class Matchup
	{
		public int WeekNum { get; set; }
		public SchedTeam AwayTeam { get; set; }
		public SchedTeam HomeTeam { get; set; }

		public Matchup ()
		{
			AwayTeam = new SchedTeam();
			HomeTeam = new SchedTeam();
		}
		public Matchup(int weekNum) : this()
		{
			WeekNum = weekNum;
		}
		public Matchup(string awayTeam, string homeTeam)
		{
			AwayTeam = new SchedTeam(awayTeam);
			HomeTeam = new SchedTeam(homeTeam);
		}
		public Matchup(int weekNum, string awayTeam, string homeTeam)
		{
			WeekNum = weekNum;
			AwayTeam = new SchedTeam(awayTeam);
			HomeTeam = new SchedTeam(homeTeam);
		}

		public bool ContainsTeam(SchedTeam team)
		{
			return AwayTeam.Equals(team) || HomeTeam.Equals(team);
		}
		public bool ContainsBothTeams(SchedTeam team1, SchedTeam team2)
		{
			return ContainsTeam(team1) && ContainsTeam(team2);
		}
		public bool IsEqual(Matchup matchup)
		{
			return ContainsTeam(matchup?.AwayTeam) && ContainsTeam(matchup?.HomeTeam);
		}
		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(AwayTeam.Name) && string.IsNullOrEmpty(HomeTeam.Name);
		}
		public bool HasNullMatchup()
		{
			return AwayTeam == null || HomeTeam == null || IsEmpty();
		}
		public string PrintMatchup()
		{
			return $"{AwayTeam.Name} v {HomeTeam.Name}  ";
		}
	}

	public class SchedTeam
	{
		public string Name { get; set; }
		public int Division { get; set; }
		public int AwayCtr { get; set; }
		public int HomeCtr { get; set; }
		public int AwayStreak { get; set; }
		public int HomeStreak { get; set; }
		public List<Matchup> TeamSchedule { get; set; }
		public Matchup FinalWeekMatchup { get; set; }

		public SchedTeam() { TeamSchedule = new List<Matchup>(); }
		
		public SchedTeam(string name) : this() { Name = name; }

		public void ResetTeam()
		{
			AwayCtr = 0;
			HomeCtr	= 0;
			AwayStreak = 0;
			HomeStreak = 0;
			TeamSchedule.Clear();
			FinalWeekMatchup = null;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is SchedTeam))
			{
				return false;
			}
			return this == (SchedTeam)obj || this.Name == ((SchedTeam)obj).Name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public string PrintMatchupCounts(List<SchedTeam> allTeams)
		{
			StringBuilder resp = new StringBuilder();
			resp.Append($"  {Name} matchups:");
			resp.Append(Environment.NewLine);
			foreach (var team in allTeams.Where(t => !t.Equals(this)))
			{
				resp.Append($"{team.Name}-{this.TeamSchedule.Count(m => m.ContainsTeam(team))};  ");
			}
			resp.Append(Environment.NewLine);
			return resp.ToString();
		}
		public string PrintTeamSchedule()
		{
			var resp = "";
			foreach (var matchup in TeamSchedule.Where(m => m != null && !m.IsEmpty()))
			{
				resp += $"Wk {matchup.WeekNum}-{matchup.AwayTeam.Name} v {matchup.HomeTeam.Name};  ";
			}
			return resp;
		}
	}

	public class WeekSchedule
	{
		public int WeekNum { get; set; }
		public List<Matchup> Matchups { get; set; }

		public WeekSchedule()
		{
			Matchups = new List<Matchup>();
		}
		public WeekSchedule(int weekNum) : this()
		{
			WeekNum = weekNum;
		}
		public string PrintWeekSchedule()
		{
			var resp = $"Week {WeekNum} Schedule:  ";
			foreach (var matchup in Matchups.Where(m => m != null && !m.IsEmpty()))
			{
				resp += matchup.PrintMatchup();
			}
			return resp;
		}
	}
}
