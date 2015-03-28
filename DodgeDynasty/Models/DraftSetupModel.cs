﻿using System;
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

		public string GetOwnerHints()
		{
			StringBuilder ownerHints = new StringBuilder("[");
			var ownerIds = DraftOwners.Select(o => o.OwnerId).ToList();
			var draftOwners = Owners.OrderBy(o => o.NickName).Where(o => ownerIds.Contains(o.OwnerId)).ToList();
			foreach (var owner in draftOwners)
			{
				ownerHints.Append(string.Format(
						"{{value:\"{0}\",ownerId:\"{1}\"}},",
						Utilities.JsonEncode(owner.NickName), owner.OwnerId.ToString()));
			}
			ownerHints.Append("]");
			return ownerHints.ToString();
		}

		public List<SelectListItem> GetDraftOwnerListItems()
		{
			return Utilities.GetListItems<Owner>(DraftOwners, o => o.NickName, o => o.OwnerId.ToString());
		}

		public bool IsSnakeDraft()
		{
			return CurrentDraft.Format == Constants.DraftFormats.Snake;
		}
	}
}