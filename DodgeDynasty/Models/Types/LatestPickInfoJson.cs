using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DodgeDynasty.Models.Types
{
	public class LatestPickInfoJson
	{
		//Player Id
		public int? pid { get; set; }
		//Pick number
		public int pnum { get; set; }
		//Is latest pick by user
		public bool yours { get; set; }
		//Is current user turn
		public bool uturn { get; set; }
		//Owner name
		public string oname { get; set; }
		//Owner css class
		public string ocss { get; set; }
		//Pick end time string
		public string ptime { get; set; }
		//Prev pick end time string
		public string prevtm { get; set; }
		//Latest pick get status
		public string status { get; set; }
	}
}