using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class MapperBase<T> : ModelBase where T : class, new()
	{
		public T Model { get; set; }
		public ModelStateDictionary ModelState { get; set; }

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
				if (ValidateModel(model))
				{
					DoUpdate(model);
				}
			}
		}

		protected virtual bool ValidateModel(T model)
		{
			return (ModelState != null) ? ModelState.IsValid : true;
		}

		protected virtual void DoUpdate(T model) { }
	}
}