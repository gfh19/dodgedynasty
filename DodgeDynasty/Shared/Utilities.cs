using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

		public static Func<Entities.Player, bool> FindPlayerMatch(string firstName, string lastName, string position, string nflTeam)
		{
			return p => FormatName(p.FirstName) == FormatName(firstName)
									&& FormatName(p.LastName) == FormatName(lastName)
									&& p.Position.ToUpper() == position.ToUpper()
									&& p.NFLTeam.ToUpper() == nflTeam.ToUpper();
		}

		public static List<SelectListItem> GetListItems<T>(List<T> items,
			Func<T, string> textFn, Func<T, string> valueCodeFn, bool blankEntry = true, string selected = null)
		{
			var listItems = items.Select(s => new SelectListItem { Text = textFn(s), Value = valueCodeFn(s) }).ToList();
			if (blankEntry)
			{
				listItems.Insert(0, new SelectListItem());
			}
			if (selected != null)
			{
				listItems.ForEach(s => s.Selected = FormatName(s.Value) == FormatName(selected) ? true : false);
			}
			return listItems;
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

		public static int? ToNullInt(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				return Convert.ToInt32(id);
			}
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
			if (System.Web.HttpContext.Current.User == null)
			{
				return string.Empty;
			}
			return System.Web.HttpContext.Current.User.Identity.Name;
		}

		public static int GetLoggedInUserId(IEnumerable<User> users)
		{
			return users.GetLoggedInUser().UserId;
		}

		public static User GetLoggedInUser(this IEnumerable<User> users)
		{
			return users.FirstOrDefault(u => u.UserName == GetLoggedInUserName());
		}

		public static bool IsUserLoggedIn()
		{
			return System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
		}


		/* DB Helper Methods */

		public static bool IsUserAdmin()
		{
			LoginModel model = new LoginModel { UserName = Utilities.GetLoggedInUserName() };
			return model.IsUserAdmin();
		}
	}
}