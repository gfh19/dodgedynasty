﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class MapperBase<T> : ModelBase where T : new()
	{
		public T Model { get; set; }

		public virtual T GetModel()
		{
			Model = new T();
			using (HomeEntity = new Entities.HomeEntity())
			{
				UpdateModel();
			}
			return Model;
		}
		
		public virtual void UpdateModel() {}
	}
}