using Newtonsoft.Json;

namespace DodgeDynasty.Models.Types
{
	public class PlayerRankOptions
	{
		[JsonProperty()]
		public string Id { get; set; }
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
		[JsonProperty()]
		public bool ExpandQueue { get; set; }
		[JsonProperty()]
		public bool HideOverall { get; set; }
		[JsonProperty()]
		public bool HideQB { get; set; }
		[JsonProperty()]
		public bool HideRB { get; set; }
		[JsonProperty()]
		public bool HideWRTE { get; set; }
		[JsonProperty()]
		public bool HideDEF { get; set; }
		[JsonProperty()]
		public bool HideK { get; set; }
		[JsonProperty()]
		public bool HideQueue { get; set; }
		[JsonProperty()]
		public bool ShowHighlighting { get; set; }
		[JsonProperty()]
		public bool LockHighlighting { get; set; }
		[JsonProperty()]
		public string HighlightColor { get; set; }
	}
}
