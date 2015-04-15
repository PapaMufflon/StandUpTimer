using System.Linq;
using System.Web.Mvc;
using StandUpTimer.Web.Models;
using StandUpTimer.Web.Statistic;

namespace StandUpTimer.Web.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Index([Bind(Include = "DateTime,DeskState")] Contract.Status status)
        {
            db.Statuses.Add(status.ToModel(User.Identity.Name));
            db.SaveChanges();

            return null;
        }

        [HttpGet]
        // GET: Statistics
        public ActionResult Index()
        {
            var statuses = db.Statuses.Where(x => x.Username.Equals(User.Identity.Name)).ToList();
            var ganttStatuses = statuses.Ganttisize();

            return View(new StatisticModel
            {
                Statuses = ganttStatuses,
                Days = ganttStatuses.Select(x => x.Day).Distinct().ToList()
            });
        }
    }
}