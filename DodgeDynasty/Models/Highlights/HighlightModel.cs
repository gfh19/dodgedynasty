using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Highlights
{
	public class HighlightModel
	{
		public int HighlightId { get; set; }
		public string HighlightName { get; set; }
		public string HighlightClass { get; set; }
		public string HighlightValue { get; set; }
		public short? HighlightOrder { get; set; }
	}
}