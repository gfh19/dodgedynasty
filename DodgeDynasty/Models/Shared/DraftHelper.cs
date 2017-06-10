using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using static DodgeDynasty.Shared.Constants;

namespace DodgeDynasty.Models.Shared
{
	//Can rename to DraftPickHelper or DraftModelHelper or something if this gets too large
	public static class DraftHelper
	{
		public static void SetFirstPickStartTime(HomeEntity homeEntity, Draft currentDraft)
		{
			var firstDraftPick = homeEntity.DraftPicks.Where(p => p.DraftId == currentDraft.DraftId && p.PlayerId == null)
				.OrderBy(p => p.PickNum).FirstOrDefault();
			if (firstDraftPick != null)
			{
				var currentDateTime = Utilities.GetEasternTime();
				//Past Draft - Set PickStart time to current time
				if (currentDateTime > currentDraft.DraftDate)
				{
					if (!firstDraftPick.PickStartDateTime.HasValue || firstDraftPick.PickStartDateTime > currentDateTime)
					{
						firstDraftPick.PickStartDateTime = currentDateTime;
					}
				}
				//Future Draft - Set PickStart to draft time
				else
				{
					firstDraftPick.PickStartDateTime = currentDraft.DraftDate;
				}
				homeEntity.SaveChanges();
			}
		}

		public static List<SelectListItem> GetDraftFormatItems()
		{
			List<SelectListItem> items = new List<SelectListItem>();
			Dictionary<string, string> draftFormats = Utilities.GetStringProperties(new DraftFormats());
			draftFormats.Keys.ToList().ForEach(prop =>
				items.Add(new SelectListItem() { Text = draftFormats[prop], Value = prop }));
			return items;
		}
	}
}
