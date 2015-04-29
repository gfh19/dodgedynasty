using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models.Shared;

namespace DodgeDynasty.Mappers.Shared
{
	public class ColorStylesMapper : MapperBase<ColorStylesModel>
	{
		protected override void PopulateModel()
		{
			Model.Colors = HomeEntity.CssColors.ToList();
		}
	}
}