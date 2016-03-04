using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models
{
	public class Factory
	{
		public static T Create<T>() where T : new()
		{
			T obj = new T();
			return obj;
		}
	}
}