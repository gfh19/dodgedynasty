using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebGrease.Css.Extensions;

namespace DodgeDynasty.Models.Schedule
{
	public class RequiredCompleteMatchupsListAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var valid = false;
			var matchups = value as List<Matchup>;
			if (value is List<Matchup>)
			{
				if (matchups != null && !matchups.Any(m => (m == null || m.IsEmpty())))
				{
					valid = ScheduleValidator.ValidateMatchupsComplete(matchups);
				}
			}

			if (!valid)
			{
				return new ValidationResult($"{validationContext.DisplayName} must be completely filled in.");
			}

			var dupTeamNames = ScheduleValidator.ValidateTeamNamesDistinct(matchups);
			if (dupTeamNames.Length > 0)
			{
				return new ValidationResult($"{validationContext.DisplayName} has duplicate team names ({dupTeamNames})");
			}

			return ValidationResult.Success;
		}
	}

	public class OptionalCompleteMatchupsListAttribute : ValidationAttribute
	{
		private readonly string _matchupsPropertyToCompare;

		public OptionalCompleteMatchupsListAttribute(string matchupsPropertyToCompare)
		{
			_matchupsPropertyToCompare = matchupsPropertyToCompare;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var valid = false;
			var matchups = value as List<Matchup>;
			if (value is List<Matchup>)
			{
				if (matchups != null)
				{
					valid = ScheduleValidator.ValidateMatchupsComplete(matchups);
				}
			}

			if (!valid)
			{
				return new ValidationResult($"{validationContext.DisplayName} must either be left blank or completely filled in.");
			}

			var dupTeamNames = ScheduleValidator.ValidateTeamNamesDistinct(matchups);
			if (dupTeamNames.Length > 0)
			{
				return new ValidationResult($"{validationContext.DisplayName} has duplicate team names ({dupTeamNames})");
			}

			var property = validationContext.ObjectType.GetProperty(_matchupsPropertyToCompare);

			if (property == null)
				throw new ArgumentException($"Property {property} not found");

			var divisionMatchups = (List<Matchup>)property.GetValue(validationContext.ObjectInstance);
			var unmatchedTeamNames = ScheduleValidator.ValidateTeamNamesFoundInDivisions(matchups, divisionMatchups);
			if (unmatchedTeamNames.Length > 0)
			{
				return new ValidationResult($"Check spelling - Team names in {validationContext.DisplayName} not found in Divisions ({unmatchedTeamNames})");
			}

			return ValidationResult.Success;
		}
	}

	public class OptionalCompleteSingleMatchupAttribute : ValidationAttribute
	{
		private readonly string _matchupsPropertyToCompare;

		public OptionalCompleteSingleMatchupAttribute(string matchupsPropertyToCompare)
		{
			_matchupsPropertyToCompare = matchupsPropertyToCompare;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var valid = false;
			var matchup = value as Matchup;
			if (value is Matchup)
			{
				if (matchup != null)
				{
					valid = ScheduleValidator.ValidateMatchupsComplete(new[] { matchup });
				}
			}

			if (!valid)
			{
				return new ValidationResult($"{validationContext.DisplayName} must either be left blank or completely filled in.");
			}

			var dupTeamNames = ScheduleValidator.ValidateTeamNamesDistinct(new[] { matchup });
			if (dupTeamNames.Length > 0)
			{
				return new ValidationResult($"{validationContext.DisplayName} has duplicate team names ({dupTeamNames})");
			}

			var property = validationContext.ObjectType.GetProperty(_matchupsPropertyToCompare);

			if (property == null)
				throw new ArgumentException($"Property {property} not found");

			var divisionMatchups = (List<Matchup>)property.GetValue(validationContext.ObjectInstance);
			var unmatchedTeamNames = ScheduleValidator.ValidateTeamNamesFoundInDivisions(new[] { matchup }, divisionMatchups);
			if (unmatchedTeamNames.Length > 0)
			{
				return new ValidationResult($"Check spelling - Team names in {validationContext.DisplayName} not found in Divisions ({unmatchedTeamNames})");
			}

			return ValidationResult.Success;
		}
	}

	public static class ScheduleValidator
	{
		public static bool ValidateMatchupsComplete(IEnumerable<Matchup> matchups)
		{
			var valid = true;
			foreach (var matchup in matchups)
			{
				if (matchup != null && !matchup.IsEmpty())
				{
					valid &= !string.IsNullOrEmpty(matchup.AwayTeam?.Name) && !string.IsNullOrEmpty(matchup.HomeTeam?.Name);
				}
			}

			return valid;
		}

		public static string ValidateTeamNamesDistinct(IEnumerable<Matchup> matchups)
		{
			string dupTeamNames = "";
			var distinctTeams = matchups.Select(m => m.AwayTeam.Name).Where(n=>!string.IsNullOrEmpty(n))
				.Concat(matchups.Select(m => m.HomeTeam.Name).Where(n => !string.IsNullOrEmpty(n))).Distinct();
			if (distinctTeams.Count() < (matchups.Count() * 2))
			{
				var duplicateTeams = matchups.Where(m => !string.IsNullOrEmpty(m.AwayTeam.Name)).
					GroupBy(m => m.AwayTeam.Name).Where(group => group.Count() > 1).Select(group => group.Key);
				duplicateTeams.Concat(matchups.Where(m => !string.IsNullOrEmpty(m.AwayTeam.Name))
					.GroupBy(m => m.HomeTeam.Name).Where(group => group.Count() > 1).Select(group => group.Key));

				duplicateTeams.ForEach(t => dupTeamNames += $"{t}; ");
			}
			return dupTeamNames;
		}

		public static string ValidateTeamNamesFoundInDivisions(IEnumerable<Matchup> matchups, IEnumerable<Matchup> divisionMatchups)
		{
			string missingTeamNames = "";
			var divisionTeamNames = divisionMatchups.Select(m => m.AwayTeam.Name).Where(n => !string.IsNullOrEmpty(n))
				.Concat(divisionMatchups.Select(m => m.HomeTeam.Name).Where(n => !string.IsNullOrEmpty(n)));
			var missingTeams = matchups.Select(m => m.AwayTeam.Name).Where(n => !string.IsNullOrEmpty(n) && !divisionTeamNames.Contains(n))
				.Concat(matchups.Select(m => m.HomeTeam.Name).Where(n => !string.IsNullOrEmpty(n) && !divisionTeamNames.Contains(n)));
			if (missingTeams.Any())
			{
				missingTeams.ForEach(t => missingTeamNames += $"{t}; ");
			}
			return missingTeamNames;
		}
	}
}