﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Models.Audio
{
	public class DraftPickAudio
	{
		public string playerId { get; set; }
		public string name { get; set; }
		public string pos { get; set; }
		public string team { get; set; }
		public string apiCode { get; set; }
		public string url { get; set; }
		public string access { get; set; }
		public string final { get; set; }
		public string success { get; set; }
		public string error { get; set; }
	}
}