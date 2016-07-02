using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DodgeDynasty.Entities;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Site;

namespace DodgeDynasty.Shared
{
	public static class Utilities
	{
		/* Helper methods */

		public static string FormatName(string dbName)
		{
			if (!string.IsNullOrEmpty(dbName))
			{
				return dbName.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("'", "").ToUpper();
			}
			return dbName;
		}

		public static string FormatNamePunctuation(string name)
		{
			if (!string.IsNullOrEmpty(name))
			{
				return name.Replace("-", "").Replace(".", "").Replace("'", "");
			}
			return name;
		}

		public static bool CheckStartsWith(string dbName, string inputName)
		{
			if (string.IsNullOrEmpty(inputName))
			{
				return true;
			}
			dbName = FormatName(dbName);
			inputName = FormatName(inputName);
			return dbName.StartsWith(inputName);
		}

		public static string JsonEncode(string val)
		{
			string response = (!string.IsNullOrEmpty(val)) ? val.Replace("\"", "'") : val;
			return response;
		}

		public static Dictionary<string, string> GetStringProperties(object type)
		{
			var fields = type.GetType()
			  .GetFields(BindingFlags.Public | BindingFlags.Static)
			  .Where(f => f.FieldType == typeof(string))
			  .ToDictionary(f => (string)f.GetValue(null),
							f => f.Name);
			return fields;
		}

		public static bool IsTrimEmpty(string text)
		{
			return string.IsNullOrEmpty(text) || text.Trim().Length == 0;
		}

		public static DateTime GetEasternTime(DateTime? utcTime = null)
		{
			if (!utcTime.HasValue)
			{
				utcTime = DateTime.UtcNow;
			}
			TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(utcTime.Value, easternZone);
		}

		public static int? CheckActionParameterId(ActionExecutingContext filterContext, params string[] idNames)
		{
			foreach (string idName in idNames)
			{
				if (filterContext.ActionParameters.ContainsKey(idName))
				{
					if (filterContext.ActionParameters[idName] != null)
					{
						if (filterContext.ActionParameters[idName] is string)
						{
							return ((string)filterContext.ActionParameters[idName]).ToNullInt();
						}
						else
						{
							return (int)filterContext.ActionParameters[idName];
						}
					}
				}
			}

			return null;
		}

		public static T CheckActionParameterModel<T>(ActionExecutingContext filterContext, params string[] modelNames)
			where T : class
		{
			foreach (string modelName in modelNames)
			{
				if (filterContext.ActionParameters.ContainsKey(modelName))
				{
					var model = filterContext.ActionParameters[modelName] as T;
					if (model != null)
					{
						return model;
					}
				}
			}

			return null;
		}

		public static RedirectToRouteResult GetUnauthorizedRedirect()
		{
			return new RedirectToRouteResult(
				new RouteValueDictionary(new { controller = "Shared", action = "Unauthorized" }));
		}

		public static bool HasNumber(this string input)
		{
			return input.Where(x => Char.IsDigit(x)).Any();
		}


		/* Conversion methods */

		public static int? ToNullInt(this string s)
		{
			int i;
			if (Int32.TryParse(s, out i)) return i;
			return null;
		}

		public static bool ToBool(string val)
		{
			bool result;
			if (!string.IsNullOrEmpty(val))
			{
				Boolean.TryParse(val.ToLower(), out result);
				return result;
			}
			return false;
		}

		public static string ToStringFromNullInt(this int? i)
		{
			return (i.HasValue) ? i.ToString() : null;
		}

		public static string ToUrlEncodedString(this string str)
		{
			return (str == null) ? "" : HttpContext.Current.Server.UrlEncode(str);
		}


		/* Data Access/Parsing Methods */

		public static Func<Entities.Player, bool> FindPlayerMatch(string firstName, string lastName,
			string position, string nflTeam = null)
		{
			if (nflTeam != null)
			{
				return p => FormatName(p.FirstName) == FormatName(firstName)
										&& FormatName(p.LastName) == FormatName(lastName)
										&& p.Position.ToUpper() == position.ToUpper()
										&& p.NFLTeam.ToUpper() == nflTeam.ToUpper();
			}
			else
			{
				return p => FormatName(p.FirstName) == FormatName(firstName)
										&& FormatName(p.LastName) == FormatName(lastName)
										&& p.Position.ToUpper() == position.ToUpper();
			}
		}

		public static List<SelectListItem> GetListItems<T>(List<T> items,
			Func<T, string> textFn, Func<T, string> valueCodeFn, bool blankEntry = true, string selectedValue = null)
		{
			var listItems = items.Select(s => new SelectListItem { Text = textFn(s), Value = valueCodeFn(s) }).ToList();
			if (blankEntry)
			{
				listItems.Insert(0, new SelectListItem());
			}
			if (selectedValue != null)
			{
				listItems.ForEach(s => s.Selected = FormatName(s.Value) == FormatName(selectedValue) ? true : false);
			}
			return listItems;
		}

		public static string GetRouteDraftId(RouteData routeData)
		{
			int draftId;
			var routeId = routeData.Values["id"];
			if (routeId != null && Int32.TryParse(routeId.ToString(), out draftId))
			{
				return routeId.ToString();
			}
			return null;
		}
		public static string GetLoggedInUserName()
		{
			if (System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.User == null)
			{
				return string.Empty;
			}
			return System.Web.HttpContext.Current.User.Identity.Name;
		}

		public static int GetLoggedInUserId(this IEnumerable<User> users)
		{
			return users.GetLoggedInUser().UserId;
		}

		public static User GetLoggedInUser(this IEnumerable<User> users)
		{
			var userName = GetLoggedInUserName();
			return users.FirstOrDefault(u => u.UserName == userName);
		}

		public static bool IsUserLoggedIn()
		{
			return System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
		}

		public static string GetMessageCountDisplay(int newMessageCount)
		{
			return (newMessageCount > 3) ? "3+" : newMessageCount.ToString();
		}

		public static int GetLatestUserDraftId(User user, List<Draft> drafts, List<DraftOwner> draftOwners)
		{
			return GetLatestUserDraft(user, drafts, draftOwners).DraftId;
        }

		public static Draft GetLatestUserDraft(User user, List<Draft> drafts, List<DraftOwner> draftOwners)
		{
			var ownerDraftIds = draftOwners.Where(o => o.UserId == user.UserId).Select(o => o.DraftId).ToList();
			var ownerDrafts = drafts.Where(d => ownerDraftIds.Contains(d.DraftId))
				.OrderByDescending(o => o.IsActive).ThenBy(o => o.IsComplete).ThenBy(o => o.DraftDate).ToList();
			if (ownerDrafts.Any(o => o.IsActive || !o.IsComplete))
			{
				return ownerDrafts.FirstOrDefault();
			}
			return ownerDrafts.Last();
		}

		public static string GetAutoCompletePlayerHints(List<Player> playersSorted, List<NFLTeam> nflTeams, 
			bool excludeDrafted, List<int?> draftedPlayerIds)
		{
			StringBuilder playerHints = new StringBuilder("[");
			foreach (var player in playersSorted)
			{
				if (!excludeDrafted || !draftedPlayerIds.Contains(player.PlayerId))
				{
					var nflTeam = nflTeams.First(t => t.TeamAbbr == player.NFLTeam.ToUpper());
                    playerHints.Append(string.Format(
						"{{value:\"{0} {1} {3}-{4}\",firstName:\"{0}\",lastName:\"{1}\",nflTeam:\"{2}\",nflTeamDisplay:\"{3}\",pos:\"{4}\"}},",
						Utilities.JsonEncode(player.FirstName), Utilities.JsonEncode(player.LastName), Utilities.JsonEncode(nflTeam.TeamAbbr),
							Utilities.JsonEncode(nflTeam.AbbrDisplay), Utilities.JsonEncode(player.Position)));
				}
			}
			playerHints.Append("]");
			return playerHints.ToString();
		}

		public static string GetAutoCompleteTruePlayerHints(List<Player> playersSorted, List<NFLTeam> nflTeams)
		{
			StringBuilder playerHints = new StringBuilder("[");
			foreach (var player in playersSorted)
			{
				var nflTeam = nflTeams.First(t => t.TeamAbbr == player.NFLTeam.ToUpper());
				playerHints.Append(string.Format(
			"{{value:\"({6}) {0} {1} {3}-{4}\",firstName:\"{0}\",lastName:\"{1}\",nflTeam:\"{2}\",nflTeamDisplay:\"{3}\",pos:\"{4}\",id:\"{5}\",tpid:\"{6}\",active:\"{7}\",drafted:\"{8}\"}},",
					Utilities.JsonEncode(player.FirstName), Utilities.JsonEncode(player.LastName), Utilities.JsonEncode(nflTeam.TeamAbbr),
					Utilities.JsonEncode(nflTeam.AbbrDisplay), Utilities.JsonEncode(player.Position), player.PlayerId, player.TruePlayerId, player.IsActive, player.IsDrafted));
			}
			playerHints.Append("]");
			return playerHints.ToString();
		}


		/* End Data Access/Parsing Methods */
	}
}