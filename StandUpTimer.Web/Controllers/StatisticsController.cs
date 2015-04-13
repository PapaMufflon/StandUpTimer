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
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 8, 15, 0),
                    Position = Position.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 9, 17, 0),
                    Position = Position.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 9, 37, 0),
                    Position = Position.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 11, 0, 0),
                    Position = Position.Standing
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 11, 20, 0),
                    Position = Position.Sitting
                },
                new Status
                {
                    DateTime = new DateTime(2015, 4, 8, 12, 15, 0),
                    Position = Position.Inactive
                }
            });

            var ganttStatuses = statuses.Ganttisize();

            return View(new StatisticModel
            {
                Statuses = ganttStatuses,
                Days = ganttStatuses.Select(x => x.Day).Distinct().ToList()
            });
        }
    }

    public static class GanttificationExtension
    {
        public static List<GanttStatus> Ganttisize(this IList<Status> statuses)
        {
            var result = new List<GanttStatus>();

            for (int index = 1; index < statuses.Count; index++)
            {
                var status = statuses[index];
                var previousStatus = statuses[index - 1];

                result.Add(new GanttStatus
                {
                    Position = previousStatus.Position,
                    StartDate = DateTime.Today.Add(previousStatus.DateTime.TimeOfDay).ToString("yyyy, M, d, H, m, s, 0"),
                    EndDate = DateTime.Today.Add(status.DateTime.TimeOfDay).ToString("yyyy, M, d, H, m, s, 0"),
                    Day = ToReadableDay(previousStatus.DateTime)
                });
            }

            return result;
        }

        private static string ToReadableDay(DateTime dateTime)
        {
            if (dateTime.Date.Equals(DateTime.Today))
                return "Heute";

            if (dateTime.Date.Equals(DateTime.Today.AddDays(-1)))
                return "Gestern";

            return dateTime.Date.ToShortDateString();
        }
    }

    public class GanttStatus
    {
        public Position Position { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Day { get; set; }
    }

    public class StatisticModel
    {
        public List<GanttStatus> Statuses { get; set; }
        public List<string> Days { get; set; }
    }
}