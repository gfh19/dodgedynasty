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
		public DraftStatusMapper(string draftId, string isActive, string isComplete, string isPaused)
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
			if (!string.IsNullOrEmpty(isPaused))
			{
				Model.IsPaused = Convert.ToBoolean(isPaused);
			}
		}

		protected override void DoUpdate(DraftStatusModel model)
		{
			bool broadcastDisconnect = false;
			bool broadcastDraftRefresh = false;
			var draft = HomeEntity.Drafts.Where(d => d.DraftId == model.DraftId).FirstOrDefault();
			if (model.IsActive != null)
			{
				draft.IsActive = model.IsActive.Value;
				draft.IsPaused = false;
				broadcastDisconnect = true;
			}
			if (model.IsComplete != null)
			{
				draft.IsComplete = model.IsComplete.Value;
				draft.IsPaused = false;
			}
			if (model.IsPaused != null)
			{
				draft.IsPaused = model.IsPaused.Value;
				broadcastDraftRefresh = true;
			}
			draft.LastUpdateTimestamp = DateTime.Now;
			HomeEntity.SaveChanges();

			if (broadcastDisconnect)
			{
				DraftHubHelper.BroadcastDisconnectToClients();
			}
			if (broadcastDraftRefresh)
			{
				DraftHubHelper.BroadcastDraftToClients(null);
			}
		}
	}
}