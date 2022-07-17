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
		//Player full name
		public string pname { get; set; }
		//NFL team display
		public string team { get; set; }
		//Position
		public string pos { get; set; }
		//Pick number
		public int pnum { get; set; }
		//Current Pick number
		public int curpnum { get; set; }
		//Latest pick user id
		public int? puid { get; set; }
		//Current turn user id
		public int? uturnid { get; set; }
		//Current turn owner name
		public string curoname { get; set; }
		//Owner name
		public string oname { get; set; }
		//Owner css class
		public string ocss { get; set; }
		//Pick end time string
		public string petime { get; set; }
		//Prev pick end time string
		public string prevtm { get; set; }
		//Current Pick start time string
		public string curpstime { get; set; }
		//Current server time string
		public string curtm { get; set; }
		//Pick counter until next turn
		public Dictionary<int, int> pctr { get; set; }
		//Potential audio eligible user ids
		public List<int> auduids { get; set; }
		//Latest pick get status
		public string status { get; set; }
		//Combine WR & TE
		public string combwrte { get; set; }
		//Draft Show Position Colors
		public string drshowposcol { get; set; }
		//Teams Show Position Colors
		public string tmshowposcol { get; set; }
	}
}