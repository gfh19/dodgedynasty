using System;
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