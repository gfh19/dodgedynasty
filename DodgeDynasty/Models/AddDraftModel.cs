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

namespace DodgeDynasty.Models
{
	public class AddDraftModel
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
		public int DraftYear { get; set; }
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
			var selectedOwnerId = (ownerUser==null) ? string.Empty : ownerUser.OwnerId.ToString();
			return Utilities.GetListItems<OwnerUser>(LeagueOwnerUsers.OrderBy(u => u.FirstName).ToList(),
				u => u.FullName, u => u.OwnerId.ToString(), true, selectedOwnerId);
		}
	}
}