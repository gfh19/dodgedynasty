using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DodgeDynasty.Models.Types
{
	public class PickInfoJson
	{
		public bool turn { get; set; }
		public int num { get; set; }
		public bool hasPrev { get; set; }
		public string prevName { get; set; }
	}
}