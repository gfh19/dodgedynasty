using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Shared.Exceptions
{
	public class DuplicatePickException : Exception
	{
		public DraftPick DuplicatePick { get; set; }

		public DuplicatePickException(DraftPick duplicatePick)
		{
			DuplicatePick = duplicatePick;
		}
	}
}