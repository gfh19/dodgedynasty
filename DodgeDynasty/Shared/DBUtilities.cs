using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Mappers.Shared;
using DodgeDynasty.Mappers.Site;
using DodgeDynasty.Models.Shared;
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

		public static DraftChatModel GetCurrentDraftChatModel()
		{
			var draftChatMapper = new DraftChatMapper();
			return draftChatMapper.GetModel();
		}
	}
}