using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Shared.Extensions
{
	public static class SessionExtensions
	{
		public static void AddOrUpdate(this HttpSessionStateBase session, string name, object value)
		{
			if (session[name] != null)
			{
				session[name] = value;
			}
			else
			{
				session.Add(name, value);
			}
		}

		public static List<T> GetList<T>(this HttpSessionStateBase session, string name)
		{
			if (session[name] != null && session[name] is List<T>)
			{
				return (List<T>)session[name];
			}
			return new List<T>();
		}

		public static List<T> AddToList<T>(this HttpSessionStateBase session, string name, T listItem)
		{
			var list = session.GetList<T>(name);
			list.Add(listItem);
			session.AddOrUpdate(name, list);
			return list;
		}

		public static List<T> RemoveFromList<T>(this HttpSessionStateBase session, string name, T listItem)
		{
			var list = session.GetList<T>(name);
			list.Remove(listItem);
			session.AddOrUpdate(name, list);
			return list;
		}
	}
}