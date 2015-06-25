using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models.Site
{
	public class MessagesCountModel
	{
		public string UserName { get; set; }
		public List<Message> NewMessages { get; set; }
		public DateTime LatestMessageTime { get; set; }
	}
}