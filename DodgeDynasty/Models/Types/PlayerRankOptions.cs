using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DodgeDynasty.Models.Types
{
	public class PlayerRankOptions
	{
		[JsonProperty()]
		public string RankId { get; set; }
		[JsonProperty()]
		public string DraftId { get; set; }
		[JsonProperty()]
		public bool ExpandOverall { get; set; }
		[JsonProperty()]
		public bool ExpandQB { get; set; }
		[JsonProperty()]
		public bool ExpandRB { get; set; }
		[JsonProperty()]
		public bool ExpandWRTE { get; set; }
		[JsonProperty()]
		public bool ExpandDEF { get; set; }
		[JsonProperty()]
		public bool ExpandK { get; set; }
	}
}
