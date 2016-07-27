using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;

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
	}
}
