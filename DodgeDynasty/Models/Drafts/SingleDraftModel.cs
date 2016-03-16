namespace DodgeDynasty.Models.Drafts
{
	public class SingleDraftModel
	{
		public int DraftId { get; set; }
		public int LeagueId { get; set; }
		public string LeagueName { get; set; }
		public short DraftYear { get; set; }
	}
}
