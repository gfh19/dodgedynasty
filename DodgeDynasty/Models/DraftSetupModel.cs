using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;
using System.Text;
using DodgeDynasty.Shared;
using System.Web.Mvc;

namespace DodgeDynasty.Models
{
	public class DraftSetupModel : DraftModel
	{
		public string AdminMode { get; set; }

		public DraftSetupModel() { }
		public DraftSetupModel(int draftId) : base(draftId) { }

		public List<DraftPick> GetDraftSetupPicks()
		{
			var draftPicks = DraftPicks;
			if (draftPicks.Count() == 0)
			{
				draftPicks = new List<DraftPick> { new DraftPick() { DraftId = DraftId.Value, RoundNum = 1, PickNum=1} };
			}
			return draftPicks;
		}

		public bool IsNewRound(int roundNum)
		{
			return CurrentRoundNum != roundNum;
		}

		public List<SelectListItem> GetDraftOwnerListItems()
		{
			return Utilities.GetListItems<User>(DraftUsers, u => u.NickName, u => u.UserId.ToString());
		}

		public bool IsSnakeDraft()
		{
			return CurrentDraft.Format == Constants.DraftFormats.Snake;
		}
	}
}