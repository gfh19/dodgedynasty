using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty
{
	/// <summary>
	/// Summary description for OwnerCss
	/// </summary>
	public class OwnerCss : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/css";
			context.Response.Write("");
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}