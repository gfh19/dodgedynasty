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
using DodgeDynasty.Models.Shared;

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
		[Range(0, 9999)]
		public short DraftYear { get; set; }
		
		[Display(Name = "Rounds")]
		[Required]
		[Range(0, 99)]
		public int NumRounds { get; set; }

		[Display(Name = "Keepers")]
		[Required]
		[Range(0, 99)]
		public int NumKeepers { get; set; }

		[Display(Name = "Format")]
		[Required]
		public string Format { get; set; }

		[Display(Name = "Combine WR/TE?")]
		[Required]
		public bool CombineWRTE { get; set; }

		[Display(Name = "Seconds per Pick (0 for no timer)")]
		[Required]
		[Range(0, 9999)]
		public int PickTimeSeconds { get; set; }

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
			return DraftHelper.GetDraftFormatItems();
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