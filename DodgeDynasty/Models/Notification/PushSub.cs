using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Notification
{
	public class PushSub
	{
		public string UserId { get; set; }
		public string EndPoint { get; set; }
		public Dictionary<string, string> Keys { get; set; }
		public string JsonBody { get; set; }
	}
	public class PushSubJson
	{
		public string Request { get; set; }
	}
}