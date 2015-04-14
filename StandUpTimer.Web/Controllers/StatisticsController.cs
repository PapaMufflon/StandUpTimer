using System.Linq;
using System.Web.Mvc;
using StandUpTimer.Web.Models;
using StandUpTimer.Web.Statistics;

namespace StandUpTimer.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Index([Bind(Include = "DateTime,DeskState")] Contract.Status status)
        {
            db.Statuses.Add(status.ToModel());
            db.SaveChanges();

            return null;
        }

        [HttpGet]
        // GET: Statistics
        public ActionResult Index()
        {
            var statuses = db.Statuses.ToList();
            var ganttStatuses = statuses.Ganttisize();

            return View(new StatisticModel
            {
                Statuses = ganttStatuses,
                Days = ganttStatuses.Select(x => x.Day).Distinct().ToList()
            });
        }
    }
}