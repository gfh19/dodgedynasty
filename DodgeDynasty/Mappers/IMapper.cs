using System.Web.Mvc;

namespace DodgeDynasty.Mappers
{
	public interface IMapper<T>
	{
		T Model { get; set; }
		ModelStateDictionary ModelState { get; set; }
		bool UpdateSucceeded { get; set; }

		T GetModel();
		T GetModel(T model);
		T GetUpdatedModel(T model);

		bool UpdateEntity(T model);
	}
}
