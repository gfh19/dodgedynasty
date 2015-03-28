using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DodgeDynasty.Shared.Security
{
	public class PasswordInfo
	{
		public byte[] PasswordHash { get; set; }
		public byte[] Salt { get; set; }
	}
}