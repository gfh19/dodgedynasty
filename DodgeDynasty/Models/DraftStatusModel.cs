using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Models
{
	public class DraftStatusModel
	{
		public int DraftId { get; set; }
		public bool? IsActive { get; set; }
		public bool? IsComplete { get; set; }
		public bool? IsPaused { get; set; }
	}
}