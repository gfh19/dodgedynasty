using System;
using System.Collections.Generic;
using System.Configuration;
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
		#region String Methods

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
				return name.Replace("-", "").Replace(".", "").Replace("'", "").Replace("*", "");
			}
			return name;
		}

		//May want to delete this in future if two truly different Jr/non-Jr players collide
		public static string TrimSuffix(string playerName)
		{
			var suffixes = new string[] { "Sr.", "Sr", "Jr.", "Jr", "II", "III", "IV", "V" };
			foreach (var suffix in suffixes)
			{
				if (playerName.Trim().EndsWith(" " + suffix))
				{
					playerName = playerName.Trim().Substring(0, playerName.Length - (suffix.Length + 1));
					break;
				}
			}
			return playerName;
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

		public static string Truncate(this string input, int maxLength)
		{
			if (string.IsNullOrEmpty(input))
			{
				return input;
			}
			return input.Substring(0, input.Length < maxLength ? input.Length : maxLength);
        }

		public static string TrimString(this string input, string strVal)
		{
			if (input.EndsWith(strVal))
			{
				input = input.Replace(strVal, "");
			}
			return input;
        }

		public static string ToDateTimeString(this DateTime? dateTime)
		{
			if (dateTime.HasValue)
			{
				return ToDateTimeString(dateTime.Value);
			}
			return string.Empty;
		}

		public static string ToDateTimeString(this DateTime dateTime)
		{
			return dateTime.ToString(Constants.Times.FullDateTimeFormat);
		}

		public static List<int> AllIndexesOf(this string str, string value)
		{
			List<int> indexes = new List<int>();
			if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(value))
			{
				for (int i = 0; i < str.Length; i += value.Length)
				{
					i = str.IndexOf(value, i);
					if (i == -1)
						return indexes;
					indexes.Add(i);
				}
			}
			return indexes;
		}

		public static void UniqueAdd(this List<string> strList, string value)
		{
			if (!strList.Contains(value))
			{
				strList.Add(value);
			}
		}
		
		#endregion String Methods

		/* Config access methods */
		#region Config Access Methods

		public static string GetConfigVal(string configKey)
		{
			return ConfigurationManager.AppSettings[configKey];
		}

		public static bool GetBoolConfigVal(string configKey)
		{
			var val = ConfigurationManager.AppSettings[configKey];
			if (!string.IsNullOrWhiteSpace(val))
			{
				return bool.Parse(val);
			}
			return false;
		}

		public static int GetMaxCompareRanks()
		{
			var maxCompareRanks = GetConfigVal(Constants.AppSettings.MaxCompareRanks);
			if (!string.IsNullOrWhiteSpace(maxCompareRanks))
			{
				return Int32.Parse(maxCompareRanks);
			}
            return 15;
		}

		public static string GetSiteConfigValue(DraftModel model, string configKey)
		{
			return model.SiteConfigVars.FirstOrDefault(v=>v.VarName == configKey)?.VarValue ?? GetConfigVal(configKey);
		}

		public static bool GetBoolSiteConfigValue(DraftModel model, string configKey)
		{
			var val = GetSiteConfigValue(model, configKey);
			if (!string.IsNullOrWhiteSpace(val))
			{
				return bool.Parse(val);
			}
			return false;
		}

		/* End Config access methods */
		#endregion Config Access Methods

		/* Conversion methods */
		#region Conversion Methods

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

		#endregion Conversion Methods

		/* Data Access/Parsing Methods */
		#region Data Access/Parsing Methods

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
		public static bool IsStartDraftingDomain(HttpRequestBase request)
		{
			return request.Url.Host.ToLower().Contains("startdrafting.com") || 
				(request.Url.Host == "george-pc" && request.Url.LocalPath.StartsWith("/StartDrafting"));
		}

		public static string GetMessageCountDisplay(int newMessageCount)
		{
			return (newMessageCount > 3) ? "3+" : newMessageCount.ToString();
		}

		public static int GetLatestUserDraftId(int userId, List<Draft> drafts, List<DraftOwner> draftOwners, List<UserRole> currentUserRoles)
		{
			return GetLatestUserDraft(userId, drafts, draftOwners, currentUserRoles).DraftId;
        }

		public static Draft GetLatestUserDraft(int userId, List<Draft> drafts, List<DraftOwner> allDraftOwners, List<UserRole> currentUserRoles)
		{
			List<Draft> ownerDrafts = GetAllUserDrafts(userId, drafts, allDraftOwners, currentUserRoles);
			if (ownerDrafts.Any(o => o.IsActive || !o.IsComplete))
			{
				return ownerDrafts.FirstOrDefault();
			}
			return ownerDrafts.Last();
		}

		public static List<Draft> GetAllUserDrafts(int userId, List<Draft> drafts, List<DraftOwner> draftOwners, List<UserRole> currentUserRoles)
		{
			if (HasGlobalDraftViewRole(currentUserRoles))
			{
				return drafts.OrderByDescending(o => o.IsActive).ThenBy(o => o.IsComplete).ThenBy(o => o.DraftDate).ToList();
			}
			var ownerDraftIds = draftOwners.Where(o => o.UserId == userId).Select(o => o.DraftId).ToList();
			var ownerDrafts = drafts.Where(d => ownerDraftIds.Contains(d.DraftId))
				.OrderByDescending(o => o.IsActive).ThenBy(o => o.IsComplete).ThenBy(o => o.DraftDate).ToList();
			return ownerDrafts;
		}

		public static bool ValidateUserDraftId(int? draftId, int userId, List<Draft> drafts, List<DraftOwner> allDraftOwners, 
			List<int> currentUserLeagueIds, List<UserRole> currentUserRoles)
		{
			if (!draftId.HasValue)
			{
				return false;
			}
			if (HasGlobalDraftViewRole(currentUserRoles))
			{
				return true;
			}
			var ownerDraftIds = allDraftOwners.Where(o => o.UserId == userId).Select(o => o.DraftId).ToList();
			var draftLeagueId = drafts.FirstOrDefault(o => o.DraftId == draftId.Value).LeagueId;
			if (!ownerDraftIds.Contains(draftId.Value) && !currentUserLeagueIds.Contains(draftLeagueId))
			{
				throw new UnauthorizedAccessException("User does not have access to this draft or league, draftId: " + draftId.Value);
			}
			return true;
		}

		public static bool HasGlobalDraftViewRole(List<UserRole> currentUserRoles)
		{
			if (currentUserRoles != null)
			{
				var currentUserRoleIds = currentUserRoles.Select(o => o.RoleId);
				if (currentUserRoleIds.Contains(Constants.Roles.Admin) || currentUserRoleIds.Contains(Constants.Roles.Guest))
				{
					return true;
				}
			}
			return false;
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
						"{{value:\"{0} {1} {3}-{4}\",firstName:\"{0}\",lastName:\"{1}\",nflTeam:\"{2}\",nflTeamDisplay:\"{3}\",pos:\"{4}\",id:\"{5}\"}},",
						Utilities.JsonEncode(player.FirstName), Utilities.JsonEncode(player.LastName), Utilities.JsonEncode(nflTeam.TeamAbbr),
							Utilities.JsonEncode(nflTeam.AbbrDisplay), Utilities.JsonEncode(player.Position), player.PlayerId));
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
		#endregion Data Access/Parsing Methods
	}
}