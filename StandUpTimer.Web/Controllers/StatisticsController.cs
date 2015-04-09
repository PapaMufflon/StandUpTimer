using System;
using System.Collections.Generic;
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
            var statuses = _db.Statuses.ToList();
            statuses.AddRange(new List<Status>
            {
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 8, 15, 0),
                    Position = Position.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 9, 17, 0),
                    Position = Position.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 9, 37, 0),
                    Position = Position.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 11, 0, 0),
                    Position = Position.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 11, 20, 0),
                    Position = Position.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 12, 15, 0),
                    Position = Position.Inactive
                },

                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 12, 50, 0),
                    Position = Position.Sitting
                },

                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 14, 0, 0),
                    Position = Position.Standing
                },

                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 14, 20, 0),
                    Position = Position.Sitting
                },

                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 15, 25, 0),
                    Position = Position.Standing
                },

                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 15, 45, 0),
                    Position = Position.Sitting
                },

                new Status
                {
                    DateTime = new DateTime(2015, 4, 9, 17, 5, 0),
                    Position = Position.Inactive
                }
            });

            return View(statuses);
        }
    }
}