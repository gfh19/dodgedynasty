using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class MapperBase<T> : ModelBase where T : class, new()
	{
		public T Model { get; set; }
		
		public T GetModel(T model)
		{
			Model = model ?? new T();
			using (HomeEntity = new Entities.HomeEntity())
			{
				PopulateModel();
			}
			return Model;
		}

		public T GetModel()
		{
			return GetModel((T)null);
		}

		protected virtual void PopulateModel() { }

		public void UpdateEntity(T model)
		{
			using (HomeEntity = new Entities.HomeEntity())
			{
				DoUpdate(model);
			}
		}
		protected virtual void DoUpdate(T model) { }
	}
}