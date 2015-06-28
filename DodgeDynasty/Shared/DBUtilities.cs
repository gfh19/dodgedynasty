using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models.Site;

namespace DodgeDynasty.Shared
{
	public static class DBUtilities
	{
		public static MessagesCountModel GetMessageCountModel()
		{
			var messagesCountMapper = new MessagesCountMapper();
			return messagesCountMapper.GetModel();
		}
	}
}