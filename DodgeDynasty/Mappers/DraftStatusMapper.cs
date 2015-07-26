using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;
using DodgeDynasty.WebSockets;

namespace DodgeDynasty.Mappers
{
	public class DraftStatusMapper : MapperBase<DraftStatusModel>
	{
		public DraftStatusMapper(string draftId, string isActive, string isComplete)
		{
			CreateModel();
			Model.DraftId = Int32.Parse(draftId);
			//Leave null if either value not provided
			if (!string.IsNullOrEmpty(isActive)) {
				Model.IsActive = Convert.ToBoolean(isActive);
			}
			if (!string.IsNullOrEmpty(isComplete))
			{
				Model.IsComplete = Convert.ToBoolean(isComplete);
			}
		}

		protected override void DoUpdate(DraftStatusModel model)
		{
			bool broadcastDisconnect=false;
			var draft = HomeEntity.Drafts.Where(d => d.DraftId == model.DraftId).FirstOrDefault();
			if (model.IsActive != null)
			{
				draft.IsActive = model.IsActive.Value;
				broadcastDisconnect = true;
			}
			if (model.IsComplete != null)
			{
				draft.IsComplete = model.IsComplete.Value;
			}
			draft.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();

			if (broadcastDisconnect)
			{
				DraftHubHelper.BroadcastDisconnectToClients();
			}
		}
	}
}