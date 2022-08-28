using DodgeDynasty.Mappers.Schedule;
using DodgeDynasty.Models.Schedule;
using DodgeDynasty.Shared;
using System.Web.Mvc;

namespace DodgeDynasty.Controllers
{
    public class ScheduleController : Controller
    {
        public static int SuccessCtr = 0;
        public static int FailureCtr = 0;

        // GET: Schedule
        public ActionResult Index()
        {
            var mapper = new ScheduleMapper();
            var schedule = mapper.PopulateModel();
            return View(schedule);
        }


        [HttpPost]
        public ActionResult Index(ScheduleModel model)
        {
            if (ModelState.IsValid)
            {
                var mapper = new ScheduleMapper();
                var schedule = mapper.GenerateFullSchedule(model);
                return View(Constants.Views.ScheduleIndex, schedule);
            }
            return View(Constants.Views.ScheduleIndex, model);
        }
    }
}