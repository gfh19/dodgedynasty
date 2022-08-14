using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Shared
{
	public class SiteConfigVarModel
	{
		[JsonProperty()]
		public string VarName { get; set; }

		[JsonProperty()]
		public string VarValue { get; set; }
	}
}