using System.Linq;
using DodgeDynasty.Entities;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Drafts;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Drafts
{
	//3/5/16 Rename and add to this as needed
	public class SingleDraftMapper : MapperBase<SingleDraftModel>
	{
		//private DraftModel _internalInstance;
		public int? DraftId { get; set; }

		protected override void PopulateModel()
		{
			Draft draft = null;
			if (DraftId != null)
			{
				draft = HomeEntity.Drafts.FirstOrDefault(o => o.DraftId == DraftId.Value);
            }
			else
			{
				var userId = HomeEntity.Users.GetLoggedInUserId();
                draft = Utilities.GetLatestUserDraft(userId, HomeEntity.Drafts.ToList(), 
					HomeEntity.DraftOwners.ToList(), HomeEntity.UserRoles.Where(o => o.UserId == userId).ToList());
			}
			Model.DraftId = draft.DraftId;
			Model.LeagueId = draft.LeagueId;
			Model.LeagueName = draft.LeagueName;
			Model.DraftYear = draft.DraftYear.Value;
        }
	}
}
