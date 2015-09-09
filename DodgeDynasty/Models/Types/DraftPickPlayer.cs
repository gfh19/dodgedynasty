
namespace DodgeDynasty.Models.Types
{
	public class DraftPickPlayer
	{
		public int DraftPickId { get; set; }
		public int PickNum { get; set; }
		public int? PlayerId { get; set; }
		public string Position { get; set; }
	}
}
