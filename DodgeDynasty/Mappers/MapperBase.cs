using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DodgeDynasty.Models;

namespace DodgeDynasty.Mappers
{
	public class MapperBase<T> : ModelBase where T : class, new()
	{
		public T Model { get; set; }
		public ModelStateDictionary ModelState { get; set; }
		public bool UpdateSucceeded { get; set; }

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

		public T GetUpdatedModel(T model)
		{
			if (!UpdateSucceeded)
			{
				Model = GetModel();
				foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
				{
					if (propertyInfo.CanRead)
					{
						object propValue = propertyInfo.GetValue(Model, null);
						object updatedPropValue = propertyInfo.GetValue(model, null);
						if (updatedPropValue != null)
						{
							propertyInfo.SetValue(Model, updatedPropValue);
						}
					}
				}
			}
			return Model;
		}

		protected virtual void PopulateModel() { }

		public bool UpdateEntity(T model)
		{
			using (HomeEntity = new Entities.HomeEntity())
			{
				if (ValidateModel(model))
				{
					DoUpdate(model);
					UpdateSucceeded = true;
				}
			}
			return UpdateSucceeded;
		}

		protected virtual bool ValidateModel(T model)
		{
			return (ModelState != null) ? ModelState.IsValid : true;
		}

		protected virtual void DoUpdate(T model) { }
	}
}