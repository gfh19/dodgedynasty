using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DodgeDynasty.Models.Types
{
	public class PickInfoJson
	{
		[JsonProperty("turn")]
		public bool IsUserTurn { get; set; }
		[JsonProperty("num")]
		public int PickNum { get; set; }
		[JsonProperty("hasPrev")]
		public bool HasPreviousPick { get; set; }
		[JsonProperty("prevName")]
		public string PreviousPlayerName { get; set; }
	}
}