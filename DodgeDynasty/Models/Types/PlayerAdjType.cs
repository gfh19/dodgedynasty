using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodgeDynasty.Models.Types
{
	public class PlayerAdjType
	{
		public int? OldPlayerId { get; set; }
		public int? NewPlayerId { get; set; }
		public int? TruePlayerId { get; set; }
		public string NewFirstName { get; set; }
		public string NewLastName { get; set; }
		public string NewPlayerName { get; set; }
		public string NewPosition { get; set; }
		public string NewNFLTeam { get; set; }
		public string Action { get; set; }
		public int? UserId { get; set; }
        public int LastUpdateYear { get; set; }
    }
}
