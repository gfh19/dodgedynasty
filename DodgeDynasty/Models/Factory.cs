using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models
{
	public class Factory
	{
		public static T GetClass<T>() where T : new()
		{
			T obj = new T();
			return obj;
		}
	}
}