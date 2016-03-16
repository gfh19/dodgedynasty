using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DodgeDynasty.Entities;
using DodgeDynasty.Shared;
using System.Web.Mvc;
using DodgeDynasty.Models.Types;
using DraftFormats = DodgeDynasty.Shared.Constants.DraftFormats;
using DodgeDynasty.Models.Admin;

namespace DodgeDynasty.Models
{
	public class AddEditDraftModel : AdminModeModel, ILeagueIdModel, IDraftIdModel
	{
		[Display(Name = "Draft Date/Time")]
		[Required]
		public string DraftDate { get; set; }
		[Display(Name = "Draft Time")]
		[Required]
		public string DraftTime { get; set; }
		[Display(Name = "Location")]
		[Required]
		public string DraftLocation { get; set; }
		[Display(Name = "Draft Year")]
		[Required]
		public short DraftYear { get; set; }
		[Display(Name = "Owners")]
		[Required]
		public int NumOwners { get; set; }
		[Display(Name = "Rounds")]
		[Required]
		public int NumRounds { get; set; }
		[Display(Name = "Keepers")]
		[Required]
		public int NumKeepers { get; set; }
		[Display(Name = "Format")]
		[Required]
		public string Format { get; set; }
		[Display(Name = "Active?")]
		[Required]
		public bool IsActive { get; set; }
		[Display(Name = "Complete?")]
		[Required]
		public bool IsComplete { get; set; }
		[Display(Name = "Winner")]
		public int? WinnerId { get; set; }
		[Display(Name = "Runner Up")]
		public int? RunnerUpId { get; set; }
		[Display(Name = "Co Winners?")]
		public bool HasCoWinners { get; set; }

		public List<OwnerUser> DraftOwnerUsers { get; set; }

		public int LeagueId { get; set; }
		public string LeagueName { get; set; }
		public List<OwnerUser> LeagueOwnerUsers { get; set; }
		public int DraftId { get; set; }

		public List<SelectListItem> GetDraftFormatItems()
		{
			List<SelectListItem> items = new List<SelectListItem>();
			Dictionary<string, string> draftFormats = Utilities.GetStringProperties(new DraftFormats());
			draftFormats.Keys.ToList().ForEach(prop=>
				items.Add(new SelectListItem() { Text=draftFormats[prop], Value=prop }));
			return items;
		}

		public List<SelectListItem> GetLeagueOwnerUserItems(OwnerUser ownerUser=null)
		{
			var selectedUserId = (ownerUser == null) ? string.Empty : ownerUser.UserId.ToString();
			return GetOwnerUserItems(LeagueOwnerUsers, selectedUserId);
		}

		public List<SelectListItem> GetDraftOwnerUserItems(int? userId = null)
		{
			var selectedUserId = (userId == null) ? string.Empty : userId.ToString();
			return GetOwnerUserItems(DraftOwnerUsers, selectedUserId);
		}

		public List<SelectListItem> GetOwnerUserItems(List<OwnerUser> ownerUsers, string selectedUserId=null)
		{
			return Utilities.GetListItems<OwnerUser>(ownerUsers.OrderBy(u => u.FirstName).ToList(),
				u => u.FullName, u => u.UserId.ToString(), true, selectedUserId);
		}
	}
}