using System.Linq;
using DodgeDynasty.Models;
using DodgeDynasty.Models.Drafts;
using DodgeDynasty.Shared;

namespace DodgeDynasty.Mappers.Drafts
{
	public class SingleDraftMapper : MapperBase<SingleDraftModel>
	{
		//private DraftModel _internalInstance;

		protected override void PopulateModel()
		{
			Model.DraftId = Utilities.GetLatestUserDraftId(HomeEntity.Users.GetLoggedInUser(), 
				HomeEntity.Drafts.ToList(), HomeEntity.DraftOwners.ToList());
        }
	}
}
