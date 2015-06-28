using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using DodgeDynasty.Models.Types;

namespace DodgeDynasty.Models.Shared
{
	public class DraftChatModel
	{
		public bool IsDraftActive { get; set; }
		public List<UserChatMessage> ChatMessages { get; set; }
	}
}