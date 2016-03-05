using System.Collections.Generic;
using System.Linq;
using DodgeDynasty.Models.Highlights;

namespace DodgeDynasty.Mappers.Highlights
{
	public class HighlightsMapper : MapperBase<HighlightsModel>
	{
		protected override void PopulateModel()
		{
			Model.Highlights = new List<HighlightModel>();
			HomeEntity.Highlights.ToList().ForEach(o => Model.Highlights.Add(new HighlightModel
			{
				HighlightId = o.HighlightId,
				HighlightName = o.HighlightName,
				HighlightClass = o.HighlightClass,
				HighlightValue = o.HighlightValue
			}));
		}
	}
}
