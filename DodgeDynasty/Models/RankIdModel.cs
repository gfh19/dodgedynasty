using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models
{
	public class RankIdModel : DraftModel
	{
		public RankIdModel() : base(null) { }

		public RankIdModel(int? draftId) : base(draftId) { }

		public int RankId { get; set; }
	}
}