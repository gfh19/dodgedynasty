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

		protected override void PopulateModel()
		{
			Draft draft = Utilities.GetLatestUserDraft(HomeEntity.Users.GetLoggedInUser(), 
				HomeEntity.Drafts.ToList(), HomeEntity.DraftOwners.ToList());
			Model.DraftId = draft.DraftId;
			Model.LeagueName = draft.LeagueName;
			Model.DraftYear = draft.DraftYear.Value;
        }
	}
}
