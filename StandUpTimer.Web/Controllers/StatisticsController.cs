using System.Linq;
using System.Web.Mvc;
using StandUpTimer.Web.Models;

namespace StandUpTimer.Web.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly StatusContext _db = new StatusContext();

        // GET: Statistics
        public ActionResult Index()
        {
            return View(_db.Statuses.ToList());
        }
    }
}